using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SimpleDrawCanvas.Domain
{
    public struct Segment
    {
        public Vector2 Start;
        public Vector2 End;
    }

    public class DrawCommand
    {
        public BrushConfig BrushConfig { get; set; } = BrushConfig.DefaultConfig;
        private List<Segment> _segments = new List<Segment>();
        public IReadOnlyList<Segment> Segments => _segments.AsReadOnly();

        public void AddSegment(Vector2 startPos, Vector2 endPos)
        {
            _segments.Add(new Segment
            {
                Start = startPos,
                End = endPos,
            });
        }

        public IEnumerator GetEnumerator()
        {
            return _segments.GetEnumerator();
        }
    }

    [System.Serializable]
    public struct BrushConfig
    {
        public Color Color;
        public Texture2D Brush;
        public float BrushSize;

        static public BrushConfig DefaultConfig
        {
            get
            {
                return new BrushConfig
                {
                    Brush = null,
                    BrushSize = 1f,
                    Color = Color.black,
                };
            }
        }
    }

    public interface IDrawRepository
    {
        void Save(RenderTexture texture);
    }
}