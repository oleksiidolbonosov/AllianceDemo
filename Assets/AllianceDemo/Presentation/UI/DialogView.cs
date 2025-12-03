using System;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace AllianceDemo.Presentation.UI
{
    /// <summary>
    /// Simple dialog panel with a single line and a continue button.
    /// Uses DOTween for fades.
    /// </summary>
    public class DialogView : MonoBehaviour
    {
        [SerializeField] private CanvasGroup _canvasGroup;
        [SerializeField] private TMP_Text _text;
        [SerializeField] private Button _continueButton;
        [SerializeField] private float _fadeDuration = 0.2f;

        private Action _onComplete;

        private void Awake()
        {
            if (_continueButton != null)
                _continueButton.onClick.AddListener(OnContinueClicked);

            HideImmediate();
        }

        private void OnDestroy()
        {
            if (_continueButton != null)
                _continueButton.onClick.RemoveListener(OnContinueClicked);
        }

        public void Show(string message, Action onComplete)
        {
            _onComplete = onComplete;

            if (_text != null)
                _text.text = message;

            if (_canvasGroup != null)
            {
                _canvasGroup.DOKill();
                _canvasGroup.blocksRaycasts = true;
                _canvasGroup.interactable = true;
                _canvasGroup
                    .DOFade(1f, _fadeDuration)
                    .From(0f);
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

        private void OnContinueClicked()
        {
            if (_canvasGroup != null)
            {
                _canvasGroup
                    .DOFade(0f, _fadeDuration)
                    .OnComplete(() =>
                    {
                        _canvasGroup.blocksRaycasts = false;
                        _canvasGroup.interactable = false;

                        var cb = _onComplete;
                        _onComplete = null;
                        cb?.Invoke();
                    });
            }
            else
            {
                var cb = _onComplete;
                _onComplete = null;
                cb?.Invoke();
            }
        }
    }
}
