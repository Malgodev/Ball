using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Tilemaps;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Malgo.UI
{
    public class ToggleSwitch : MonoBehaviour, IPointerClickHandler
    {
        [Header("Slider setup")]
        [SerializeField, Range(0, 1f)] private float sliderValue;

        public bool CurrentValue { get; private set; }

        private Slider _slider;

        [Header("Animation")]
        [SerializeField, Range(0, 1f)] private float animationDuration = 0.5f;
        [SerializeField]
        private AnimationCurve slideEase =
            AnimationCurve.EaseInOut(0, 0, 1, 1);

        private Coroutine _animationSliderCoroutine;

        [Header("Events")]
        [SerializeField] private UnityEvent onToggleOn;
        [SerializeField] private UnityEvent onToggleOff;

        private ToggleSwitchGroupManager _toggleSwitchGroupManager;

        protected void OnValidate()
        {
            SetupToggleComponents();

            _slider.value = sliderValue;
        }

        private void SetupToggleComponents()
        {
            _slider = GetComponent<Slider>();

            if (_slider == null)
            {
                Debug.LogError("No slider flouind!");
                return;
            }

            _slider.interactable = false;
            ColorBlock sliderColors = _slider.colors;
            sliderColors.disabledColor = Color.white;
            _slider.colors = sliderColors;
            _slider.transition = Selectable.Transition.None;
        }

        public void SetupForManager(ToggleSwitchGroupManager manager)
        {
            _toggleSwitchGroupManager = manager;
        }

        private void Awake()
        {
            SetupToggleComponents();
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            Toggle();
        }

        private void Toggle()
        {
            if (_toggleSwitchGroupManager != null)
            {
                // _toggleSwitchGroupManager.
            }
            else
            {
                SetStateAndStartAnimation(!CurrentValue);
            }
        }

        public void ToggleByGroupManager(bool targetValue)
        {
            SetStateAndStartAnimation(targetValue);
        }

        private void SetStateAndStartAnimation(bool state)
        {
            CurrentValue = state;

            if (CurrentValue)
            {
                onToggleOn?.Invoke();
            }
            else
            {
                onToggleOff?.Invoke();
            }

            if (_animationSliderCoroutine != null)
            {
                StopCoroutine(_animationSliderCoroutine);
            }

            _animationSliderCoroutine = StartCoroutine(AnimateSlider());
        }

        private IEnumerator AnimateSlider()
        {
            float startValue = _slider.value;
            float endValue = CurrentValue ? 1 : 0;

            float time = 0;

            if (animationDuration > 0)
            {
                while (time < animationDuration)
                {
                    time += Time.deltaTime;

                    float lerpFactor = slideEase.Evaluate(time / animationDuration);
                    _slider.value = sliderValue = Mathf.Lerp(startValue, endValue, lerpFactor);
                    Debug.Log(_slider.value);
                    yield return null;
                }
            }

            endValue = _slider.value;
        }
    }
}
