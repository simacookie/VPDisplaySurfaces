using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using SimpleDrawCanvas.Domain;
using SimpleDrawCanvas.Presentation;
using SimpleDrawCanvas.Infrastructure;

namespace SimpleDrawCanvas.Application
{
    public class DrawController : MonoBehaviour
    {
        
        [SerializeField] private SimpleDrawingTool _drawCanvas = null;

        [SerializeField] private Slider _slider = null;
        [SerializeField] private Text _sliderText = null;

        [Header("==== Buttons ====")]
        [SerializeField] private Button _brushButton1 = null;
        [SerializeField] private Button _brushButton2 = null;
        [SerializeField] private Button _brushButton3 = null;
        [SerializeField] private Button _brushButton4 = null;

        [SerializeField] private Button _brackButton = null;
        [SerializeField] private Button _redButton = null;
        [SerializeField] private Button _greenButton = null;
        [SerializeField] private Button _blueButton = null;

        [SerializeField] private Button _saveButton = null;
        [SerializeField] private Button _undoButton = null;
        [SerializeField] private Button _clearButton = null;

        private IDrawRepository _drawRepository = new DrawRepository();

        private void Start()
        {
            _drawCanvas.SetBrushSize(_slider.value);
            _sliderText.text = _slider.value.ToString("f1");

            _brushButton1.onClick.AddListener(() =>
            {
                _drawCanvas.SetBrush(_brushButton1.GetComponentInChildren<RawImage>().texture as Texture2D);
            });

            _brushButton2.onClick.AddListener(() =>
            {
                _drawCanvas.SetBrush(_brushButton2.GetComponentInChildren<RawImage>().texture as Texture2D);
            });

            _brushButton3.onClick.AddListener(() =>
            {
                _drawCanvas.SetBrush(_brushButton3.GetComponentInChildren<RawImage>().texture as Texture2D);
            });
            _brushButton4.onClick.AddListener(() =>
            {
                _drawCanvas.SetBrush(_brushButton4.GetComponentInChildren<RawImage>().texture as Texture2D);
            });

            _brackButton.onClick.AddListener(() =>
            {
                _drawCanvas.SetColor(_brackButton.image.color);
            });

            _redButton.onClick.AddListener(() =>
            {
                _drawCanvas.SetColor(_redButton.image.color);
            });

            _greenButton.onClick.AddListener(() =>
            {
                _drawCanvas.SetColor(_greenButton.image.color);
            });

            _blueButton.onClick.AddListener(() =>
            {
                _drawCanvas.SetColor(_blueButton.image.color);
            });

            _slider.onValueChanged.AddListener((val) =>
            {
                _drawCanvas.SetBrushSize(val);
                _sliderText.text = val.ToString("f1");
            });

            _saveButton.onClick.AddListener(() =>
            {
                _drawRepository.Save(_drawCanvas.GetCurrentBuffer());
            });

            _undoButton.onClick.AddListener(() =>
            {
                _drawCanvas.Undo();
            });

            _clearButton.onClick.AddListener(() =>
            {
                _drawCanvas.Clear();
            });
        }
    }
}
