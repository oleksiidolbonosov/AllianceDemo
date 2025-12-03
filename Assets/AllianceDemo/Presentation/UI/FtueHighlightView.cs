using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace AllianceDemo.Presentation.UI
{
    /// <summary>
    /// Simple highlight overlay used to guide player attention during FTUE.
    /// </summary>
    public class FtueHighlightView : MonoBehaviour
    {
        [SerializeField] private CanvasGroup _canvasGroup;
        [SerializeField] private Image _highlightImage;
        [SerializeField] private float _pulseScale = 1.1f;
        [SerializeField] private float _pulseDuration = 0.7f;

        public void ShowOver(RectTransform target)
        {
            if (target == null || _highlightImage == null || _canvasGroup == null)
                return;

            var rt = _highlightImage.rectTransform;
            rt.position = target.position;

            _canvasGroup.DOKill();
            _canvasGroup.alpha = 1f;
            _canvasGroup.blocksRaycasts = false;
            _canvasGroup.interactable = false;

            rt.DOKill();
            rt.localScale = Vector3.one;
            rt
                .DOScale(_pulseScale, _pulseDuration)
                .SetLoops(-1, LoopType.Yoyo)
                .SetEase(Ease.InOutSine);
        }

        public void Hide()
        {
            if (_canvasGroup != null)
            {
                _canvasGroup.DOKill();
                _canvasGroup
                    .DOFade(0f, 0.15f)
                    .OnComplete(() =>
                    {
                        _canvasGroup.blocksRaycasts = false;
                        _canvasGroup.interactable = false;
                    });
            }

            if (_highlightImage != null)
            {
                var rt = _highlightImage.rectTransform;
                rt.DOKill();
                rt.localScale = Vector3.one;
            }
        }
    }
}
