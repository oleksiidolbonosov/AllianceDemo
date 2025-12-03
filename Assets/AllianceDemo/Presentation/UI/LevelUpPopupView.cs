using System;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace AllianceDemo.Presentation.UI
{
    /// <summary>
    /// Small level-up popup with a title and close button.
    /// Uses DOTween for scale-in animation.
    /// </summary>
    public class LevelUpPopupView : MonoBehaviour
    {
        [SerializeField] private CanvasGroup _canvasGroup;
        [SerializeField] private TMP_Text _titleText;
        [SerializeField] private Button _closeButton;
        [SerializeField] private float _fadeDuration = 0.2f;
        [SerializeField] private float _popScale = 1.1f;

        private Action _onClosed;

        private void Awake()
        {
            if (_closeButton != null)
                _closeButton.onClick.AddListener(OnCloseClicked);

            HideImmediate();
        }

        private void OnDestroy()
        {
            if (_closeButton != null)
                _closeButton.onClick.RemoveListener(OnCloseClicked);
        }

        public void Show(string title, Action onClosed = null)
        {
            _onClosed = onClosed;

            if (_titleText != null)
                _titleText.text = title;

            if (_canvasGroup != null)
            {
                _canvasGroup.DOKill();
                _canvasGroup.blocksRaycasts = true;
                _canvasGroup.interactable = true;

                var t = _canvasGroup.transform;
                t.DOKill();
                t.localScale = Vector3.one * _popScale;

                _canvasGroup.alpha = 0f;

                _canvasGroup
                    .DOFade(1f, _fadeDuration)
                    .OnComplete(() =>
                    {
                        t.DOScale(1f, _fadeDuration)
                            .SetEase(Ease.OutBack);
                    });
            }
        }

        public void HideImmediate()
        {
            if (_canvasGroup != null)
            {
                _canvasGroup.DOKill();
                _canvasGroup.alpha = 0f;
                _canvasGroup.blocksRaycasts = false;
                _canvasGroup.interactable = false;
            }
        }

        private void OnCloseClicked()
        {
            if (_canvasGroup != null)
            {
                _canvasGroup
                    .DOFade(0f, _fadeDuration)
                    .OnComplete(() =>
                    {
                        _canvasGroup.blocksRaycasts = false;
                        _canvasGroup.interactable = false;

                        var cb = _onClosed;
                        _onClosed = null;
                        cb?.Invoke();
                    });
            }
            else
            {
                var cb = _onClosed;
                _onClosed = null;
                cb?.Invoke();
            }
        }
    }
}
