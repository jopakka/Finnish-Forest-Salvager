using System;
using UnityEngine;
using UnityEngine.UI;

namespace Ui
{
    public class HealthBar : MonoBehaviour
    {
        [SerializeField] private int maxValue = 100;
        [SerializeField] private int value = 0;
        [SerializeField] private Image sliderBar;
        [SerializeField] private Transform target;
        [SerializeField] private Vector3 offset;

        private Camera _camera;
        private bool _isInitialized = false;

        public Vector3 WantedPosition => target.position + offset;

        private void Awake()
        {
            SetActivateToChildrens(false);
        }

        public void SetValue(int newValue)
        {
            value = Math.Clamp(newValue, 0, maxValue);
            ScaleSliderBar();
        }

        public void SetMaxValue(int newValue)
        {
            maxValue = Math.Max(0, newValue);
            ScaleSliderBar();
        }

        public void Initialize(Camera camera)
        {
            _camera = camera;
            UpdatePosition();
            SetActivateToChildrens(true);
            _isInitialized = true;
        }

        private void LateUpdate()
        {
            if (!_isInitialized) return;
            UpdatePosition();
        }

        private void UpdatePosition()
        {
            Vector3 worldPos = _camera.WorldToScreenPoint(WantedPosition);
            transform.position = worldPos;
        }

        private void ScaleSliderBar()
        {
            float progress = Mathf.InverseLerp(0f, maxValue, value);
            sliderBar.rectTransform.localScale = new Vector3(progress, 1f, 1f);
        }

        private void SetActivateToChildrens(bool active)
        {
            foreach (Transform uiElement in transform)
            {
                uiElement.gameObject.SetActive(active);
            }
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            SetValue(value);
            SetMaxValue(maxValue);
        }
#endif
    }
}