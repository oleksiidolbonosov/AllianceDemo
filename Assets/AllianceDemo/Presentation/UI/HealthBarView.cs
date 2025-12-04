using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace AllianceDemo.Presentation.UI
{
    /// <summary>
    /// UI component representing a health bar.
    /// Displays HP using a Unity Slider and updates it either instantly or with animation.
    ///
    /// Responsibilities:
    /// - Bind to external value providers (current/max HP)
    /// - Update visual slider representation
    /// - Smooth tween animation on value change
    ///
    /// No game logic inside — pure UI presentation.
    /// </summary>
    [DisallowMultipleComponent]
    public class HealthBarView : MonoBehaviour
    {
        [Header("UI Reference")]
        [SerializeField] private Slider _slider;

        private Func<int> _getCurrent;
        private Func<int> _getMax;

        /// <summary>
        /// True if the view has valid delegate bindings.
        /// </summary>
        public bool IsBound => _slider != null && _getCurrent != null && _getMax != null;

        /// <summary>
        /// Binds the health bar to value providers (domain side).
        /// Must be called before updating.
        /// </summary>
        public void Bind(Func<int> getCurrent, Func<int> getMax)
        {
            _getCurrent = getCurrent;
            _getMax = getMax;

            if (!IsBound)
            {
                Debug.LogWarning("[HealthBarView] Bind failed — missing delegates or slider reference.");
                return;
            }

            SetValueInstant();
        }

        /// <summary>
        /// Unbinds delegates — used on object disposal/reset.
        /// </summary>
        public void Unbind()
        {
            _getCurrent = null;
            _getMax = null;
        }

        /// <summary>
        /// Sets slider values immediately (no tween).
        /// Used when initializing/resetting the view.
        /// </summary>
        public void SetValueInstant()
        {
            if (!IsBound) return;

            _slider.DOKill();
            _slider.maxValue = _getMax.Invoke();
            _slider.value = _getCurrent.Invoke();
        }

        /// <summary>
        /// Smooth animated slider update.
        /// Used for HP change feedback in battle.
        /// </summary>
        public void AnimateToValue(float duration = 0.25f)
        {
            if (!IsBound) return;

            float target = _getCurrent.Invoke();
            _slider.maxValue = _getMax.Invoke();

            _slider.DOKill();
            _slider.DOValue(target, duration).SetEase(Ease.OutQuad);
        }

        /// <summary>
        /// Quick helper used in code to perform immediate refresh.
        /// Naming kept for backward compatibility.
        /// </summary>
        public void RefreshImmediate() => SetValueInstant();
    }
}
