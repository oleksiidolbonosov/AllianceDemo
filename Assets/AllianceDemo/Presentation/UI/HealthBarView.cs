using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace AllianceDemo.Presentation.UI
{
    /// <summary>
    /// Simple health bar UI wrapper that pulls values from delegates.
    /// </summary>
    public class HealthBarView : MonoBehaviour
    {
        [SerializeField] private Slider _slider;

        private Func<int> _getCurrent;
        private Func<int> _getMax;

        public void Bind(Func<int> getCurrent, Func<int> getMax)
        {
            _getCurrent = getCurrent;
            _getMax = getMax;
            RefreshImmediate();
        }

        public void RefreshImmediate()
        {
            if (!IsBound())
                return;

            _slider.maxValue = _getMax();
            _slider.value = _getCurrent();
        }

        public void AnimateRefresh(float duration = 0.25f)
        {
            if (!IsBound())
                return;

            _slider.maxValue = _getMax();
            var target = _getCurrent();

            _slider.DOKill();
            _slider.DOValue(target, duration);
        }

        private bool IsBound()
        {
            return _slider != null && _getCurrent != null && _getMax != null;
        }
    }
}
