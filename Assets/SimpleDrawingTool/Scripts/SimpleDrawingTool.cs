using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using SimpleDrawCanvas.Domain;
using RiptideNetworking;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
namespace SimpleDrawCanvas.Presentation
{
    public class SimpleDrawingTool : MonoBehaviour
    {
        [SerializeField] private BrushConfig Config = default;
        [SerializeField] private Shader _drawShader = null;
        [SerializeField] private RawImage _drawImage = null;
        [SerializeField] private Color _backgroundColor = Color.white;
        [SerializeField] private float _distanceThreshold = 1f;

        private List<DrawCommand> _drawCommands = new List<DrawCommand>();

        private bool _isDrawing = false;

        private float Width => _rectTrans.rect.width;
        private float Height => _rectTrans.rect.height;

        private Material _material = null;
        private RenderTexture _rt1 = null;
        private RenderTexture _rt2 = null;
        private RenderTexture _current = null;
        private RenderTexture _prev = null;

        private Camera _camera = null;

        private RectTransform _rectTrans = null;
        private Vector2 _prevPos = default;
        private Vector2 _startScreenPos = default;

        private DrawCommand _currentCommand = null;

        #region ### Shader property ids ###
        private int _posId = 0;
        private int _brashSizeId = 0;
        private int _brashTexId = 0;
        private int _aspectId = 0;
        #endregion ### Shader property ids ###

        #region Public methods ###
        public void SetColor(Color color)
        {
            Config.Color = color;
            ApplyConfig(Config);
        }

        public void SetBrushSize(float size)
        {
            Config.BrushSize = size;
            ApplyConfig(Config);
        }

        public void SetBrush(Texture2D brush)
        {
            Config.Brush = brush;
            ApplyConfig(Config);
        }

        public void Undo()
        {
            if (_drawCommands.Count == 0)
            {
                return;
            }

            ClearCanvas();

            PopLastSegment();

            foreach (var command in _drawCommands)
            {
                ApplyConfig(command.BrushConfig);

                foreach (var s in command.Segments)
                {
                    DrawSegment(s.Start, s.End);
                }
            }

            ApplyConfig(Config);
        }

        public RenderTexture GetCurrentBuffer()
        {
            return _current;
        }

        public void Clear()
        {
            ClearCanvas();

            _drawCommands.Clear();
        }
        #endregion Public methods ###

        #region ### MonoBehaviour ###
        private void Awake()
        {
            Initialize();

        }

        private void Update()
        {
#if UNITY_EDITOR
            if (Input.GetMouseButtonDown(0))
            {
                StartDraw(Input.mousePosition);
            }

            if (Input.GetMouseButton(0))
            {
                MoveDraw(Input.mousePosition);
            }

            if (Input.GetMouseButtonUp(0))
            {
                EndDraw();
            }


#else
            if (Input.touchCount > 0)
            {
                Touch touch = Input.GetTouch(0);

                if (touch.phase == TouchPhase.Began)
                {
                    StartDraw(touch.position);
                }

                if (touch.phase == TouchPhase.Moved)
                {
                    MoveDraw(touch.position);
                }

                if (touch.phase == TouchPhase.Ended)
                {
                    EndDraw();
                }
            }
#endif
        }

        private void OnDestroy()
        {
            _rt1.Release();
            _rt2.Release();
            _rt1 = null;
            _rt2 = null;

            Destroy(_material);
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (!UnityEngine.Application.isPlaying)
            {
                return;
            }

            if (_rectTrans == null)
            {
                return;
            }

            ApplyConfig(Config);
        }
#endif
        #endregion ### MonoBehaviour ###

        public void Initialize()
        {
            _rectTrans = transform as RectTransform;

            _camera = _drawImage.canvas.worldCamera;

            _rt1 = CreateRenderTexture((int)Width, (int)Height);
            _rt2 = CreateRenderTexture((int)Width, (int)Height);

            _current = _rt1;
            _prev = _rt2;

            GetPropertyIds();
            ApplyConfig(Config);
            ClearCanvas();

            _drawImage.texture = _current;
        }

        public bool IsInRectangle(Vector3 position)
        {
            return RectTransformUtility.RectangleContainsScreenPoint(_rectTrans, position);
        }

        public void ClearCanvas()
        {
            ClearRenderTexture(_current);
            ClearRenderTexture(_prev);
        }

        public void ClearRenderTexture(RenderTexture target)
        {
            RenderTexture tmp = RenderTexture.active;
            RenderTexture.active = target;
            GL.Clear(true, true, _backgroundColor);
            RenderTexture.active = tmp;
        }

        public void StartDraw(Vector3 startPos)
        {
            if (!IsInRectangle(Input.mousePosition))
            {
                return;
            }

            _startScreenPos = startPos;
            _prevPos = startPos;

            _currentCommand = new DrawCommand
            {
                BrushConfig = Config,
            };

            _isDrawing = true;
        }

        public void MoveDraw(Vector3 movedPosition)
        {
            if (!_isDrawing)
            {
                return;
            }

            float check = _prevPos.sqrMagnitude - movedPosition.sqrMagnitude;

            if (Mathf.Approximately(check, 0))
            {
                return;
            }

            _currentCommand.AddSegment(_prevPos, movedPosition);
            TryDraw(movedPosition);
        }

        public void EndDraw()
        {
            if (!_isDrawing)
            {
                return;
            }

            if (_currentCommand.Segments.Count != 0)
            {
                _drawCommands.Add(_currentCommand);
            }

            _currentCommand = null;

            _isDrawing = false;
        }

        public void PopLastSegment()
        {
            _drawCommands.RemoveAt(_drawCommands.Count - 1);
        }

        /// <summary>
        /// Get all propery ids of using in shader.
        /// </summary>
        public void GetPropertyIds()
        {
            _posId = Shader.PropertyToID("_Pos");
            _brashSizeId = Shader.PropertyToID("_BrashSize");
            _brashTexId = Shader.PropertyToID("_BrashTex");
            _aspectId = Shader.PropertyToID("_Aspect");
        }

        /// <summary>
        /// Setup the drawing material properties.
        /// </summary>
        public void ApplyConfig(BrushConfig config)
        {
            if (_material == null)
            {
                _material = new Material(_drawShader);
            }

            _material.SetFloat(_aspectId, Width / Height);

            _material.SetFloat(_brashSizeId, NormalizeBrushSize(config.BrushSize));
            _material.SetTexture(_brashTexId, config.Brush);
            _material.color = config.Color;
        }

        public RenderTexture CreateRenderTexture(int width, int height)
        {
            RenderTexture rt = new RenderTexture(width, height, 0);
            rt.Create();
            return rt;
        }

        public bool TryConvertToLocalPosition(Vector3 screenPos, out Vector2 pos)
        {
            Camera camera = null;
            if (_drawImage.canvas.renderMode != RenderMode.ScreenSpaceOverlay)
            {
                camera = _camera;
            }

            return RectTransformUtility.ScreenPointToLocalPointInRectangle(_rectTrans, screenPos, camera, out pos);
        }

        public void Draw(Vector2 screenPos)
        {
            Vector2 pos;
            TryConvertToLocalPosition(screenPos, out pos);

            Vector4 normalizedPos = NormalizePosition(screenPos);

            _material.SetVector(_posId, normalizedPos);

            Graphics.Blit(_current, _prev, _material);

            SwapBuffer();
        }

        public void TryDraw(Vector3 screenPos)
        {
            DrawSegment(_prevPos, screenPos);

            _prevPos = screenPos;

            _drawImage.texture = _current;
        }

        public void DrawSegment(Vector2 start, Vector2 end)
        {
            float distance = Vector3.Distance(start, end);

            if (Mathf.Approximately(distance, 0))
            {
                return;
            }

            for (float d = 0; d <= distance; d += _distanceThreshold)
            {
                float t = d / distance;
                Vector2 p = Vector2.Lerp(end, start, t);
                Draw(p);
            }
        }

        public Vector4 NormalizePosition(Vector2 pos)
        {
            float x = (pos.x + Width * 0.5f) / Width;
            float y = (pos.y + Height * 0.5f) / Height;
            return new Vector4(x, y, 0, 0);
        }

        public float NormalizeBrushSize(float brushSize)
        {
            return Height / brushSize;
        }

        /// <summary>
        /// Swap buffers for drawing.
        /// </summary>
        public void SwapBuffer()
        {
            RenderTexture tmp = _current;
            _current = _prev;
            _prev = tmp;
        }
    }
}
