using System;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace AllianceDemo.Presentation.UI
{
    /// <summary>
    /// Simple dialog panel with a single line of text and a "Continue" button.
    /// 
    /// Responsibilities:
    /// - Display a message
    /// - Wait for player confirmation
    /// - Invoke a completion callback
    ///
    /// Presentation-only: no gameplay logic lives here.
    /// </summary>
    [DisallowMultipleComponent]
    public class DialogView : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private CanvasGroup _canvasGroup;
        [SerializeField] private TMP_Text _text;
        [SerializeField] private Button _continueButton;

        [Header("Animation")]
        [SerializeField] private float _fadeDuration = 0.2f;

        private Action _onComplete;

        private void Awake()
        {
            if (_continueButton != null)
            {
                _continueButton.onClick.AddListener(OnContinueClicked);
            }
            else
            {
                Debug.LogWarning("[DialogView] Continue button reference is missing.");
            }

            HideImmediate();
        }

        private void OnDestroy()
        {
            if (_continueButton != null)
            {
                _continueButton.onClick.RemoveListener(OnContinueClicked);
            }
        }

        /// <summary>
        /// Shows the dialog with the given message and completion callback.
        /// The callback is invoked when the player presses "Continue".
        /// </summary>
        public void Show(string message, Action onComplete)
        {
            _onComplete = onComplete;

            if (_text != null)
            {
                _text.text = message;
            }
            else
            {
                Debug.LogWarning("[DialogView] Text component is missing.");
            }

            if (_canvasGroup == null)
            {
                Debug.LogWarning("[DialogView] CanvasGroup is missing – showing without fade.");
                return;
            }

            _canvasGroup.DOKill();
            _canvasGroup.alpha = 0f;
            _canvasGroup.blocksRaycasts = true;
            _canvasGroup.interactable = true;

            _canvasGroup
                .DOFade(1f, _fadeDuration)
                .SetEase(Ease.OutQuad);
        }

        /// <summary>
        /// Immediately hides the dialog and disables interaction.
        /// </summary>
        public void HideImmediate()
        {
            if (_canvasGroup == null)
                return;

            _canvasGroup.DOKill();
            _canvasGroup.alpha = 0f;
            _canvasGroup.blocksRaycasts = false;
            _canvasGroup.interactable = false;
        }

        /// <summary>
        /// Internal handler for the Continue button click.
        /// Fades out, then invokes the callback.
        /// </summary>
        private void OnContinueClicked()
        {
            if (_canvasGroup == null)
            {
                InvokeAndClearCallback();
                return;
            }

            _canvasGroup.DOKill();
            _canvasGroup
                .DOFade(0f, _fadeDuration)
                .SetEase(Ease.OutQuad)
                .OnComplete(() =>
                {
                    _canvasGroup.blocksRaycasts = false;
                    _canvasGroup.interactable = false;
                    InvokeAndClearCallback();
                });
        }

        /// <summary>
        /// Invokes current completion callback (if any) and clears it.
        /// </summary>
        private void InvokeAndClearCallback()
        {
            var callback = _onComplete;
            _onComplete = null;
            callback?.Invoke();
        }
    }
}
