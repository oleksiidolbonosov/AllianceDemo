using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace AllianceDemo.Presentation.UI
{
    /// <summary>
    /// Simple highlight overlay used during FTUE to attract user attention.
    /// A pulsing ring/highlight will be positioned over the target UI element.
    ///
    /// Responsibilities:
    /// - Position highlight over a UI RectTransform
    /// - Animate pulse loop to draw focus
    /// - Fade out and stop animation on hide
    ///
    /// Pure presentation class: contains no gameplay logic.
    /// </summary>
    [DisallowMultipleComponent]
    public class FtueHighlightView : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private CanvasGroup _canvasGroup;
        [SerializeField] private Image _highlightImage;

        [Header("Animation")]
        [SerializeField] private float _pulseScale = 1.1f;
        [SerializeField] private float _pulseDuration = 0.7f;
        [SerializeField] private float _fadeOutDuration = 0.15f;

        private RectTransform _rt;

        private void Awake()
        {
            _rt = _highlightImage != null ? _highlightImage.rectTransform : null;
            HideImmediate();
        }

        /// <summary>
        /// Shows highlight positioned over target UI element
        /// and starts pulsing to attract player attention.
        /// </summary>
        public void ShowOver(RectTransform target)
        {
            if (!ValidateReferences(target))
                return;

            gameObject.SetActive(true);
            PlaceOverTarget(target);
            EnableCanvas();
            PlayPulse();
        }

        /// <summary>
        /// Instantly hides highlight without animation.
        /// Use for reset/restart.
        /// </summary>
        public void HideImmediate()
        {
            if (_canvasGroup == null)
                return;

            StopAnimations();
            _canvasGroup.alpha = 0f;
            _canvasGroup.interactable = false;
            _canvasGroup.blocksRaycasts = false;

            if (_rt != null)
                _rt.localScale = Vector3.one;

            gameObject.SetActive(false);
        }

        /// <summary>
        /// Fades out and stops pulse animation.
        /// </summary>
        public void Hide()
        {
            if (_canvasGroup == null)
                return;

            _canvasGroup.DOKill();
            _canvasGroup
                .DOFade(0f, _fadeOutDuration)
                .OnComplete(HideImmediate);
        }

        #region Helpers

        private bool ValidateReferences(RectTransform target)
        {
            if (target == null)
            {
                Debug.LogWarning("[FtueHighlightView] Target is null.");
                return false;
            }

            if (_canvasGroup == null || _rt == null)
            {
                Debug.LogWarning("[FtueHighlightView] Missing CanvasGroup or Highlight Image.");
                return false;
            }

            return true;
        }

        private void PlaceOverTarget(RectTransform target)
        {
            _rt.position = target.position;
        }

        private void EnableCanvas()
        {
            _canvasGroup.DOKill();
            _canvasGroup.alpha = 1f;
            _canvasGroup.interactable = false;
            _canvasGroup.blocksRaycasts = false;
        }

        private void PlayPulse()
        {
            StopAnimations();
            _rt.localScale = Vector3.one;

            _rt.DOScale(_pulseScale, _pulseDuration)
               .SetEase(Ease.InOutSine)
               .SetLoops(-1, LoopType.Yoyo);
        }

        private void StopAnimations()
        {
            _canvasGroup?.DOKill();
            _rt?.DOKill();
        }

        #endregion
    }
}
