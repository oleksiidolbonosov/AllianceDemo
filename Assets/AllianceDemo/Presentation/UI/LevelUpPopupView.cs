using System;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace AllianceDemo.Presentation.UI
{
    /// <summary>
    /// Small level-up popup with a title and a close button.
    /// 
    /// Responsibilities:
    /// - Show a short "Level Up" message
    /// - Animate fade + pop-in
    /// - Notify listeners when closed
    ///
    /// Presentation-only, no game logic inside.
    /// </summary>
    [DisallowMultipleComponent]
    public class LevelUpPopupView : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private CanvasGroup _canvasGroup;
        [SerializeField] private TMP_Text _titleText;
        [SerializeField] private Button _closeButton;

        [Header("Animation")]
        [SerializeField] private float _fadeDuration = 0.2f;
        [SerializeField] private float _popScale = 1.1f;

        /// <summary>
        /// Fired when the popup is closed by the player.
        /// </summary>
        public event Action Closed;

        private Action _onClosedCallback;

        private void Awake()
        {
            if (_closeButton != null)
            {
                _closeButton.onClick.AddListener(OnCloseClicked);
            }
            else
            {
                Debug.LogWarning("[LevelUpPopupView] Close button reference is missing.");
            }

            HideImmediate();
        }

        private void OnDestroy()
        {
            if (_closeButton != null)
            {
                _closeButton.onClick.RemoveListener(OnCloseClicked);
            }
        }

        /// <summary>
        /// Shows the popup with a given title and optional callback.
        /// </summary>
        public void Show(string title, Action onClosed = null)
        {
            _onClosedCallback = onClosed;

            if (_titleText != null)
            {
                _titleText.text = title;
            }
            else
            {
                Debug.LogWarning("[LevelUpPopupView] Title text reference is missing.");
            }

            if (_canvasGroup == null)
            {
                Debug.LogWarning("[LevelUpPopupView] CanvasGroup is missing, showing without animation.");
                gameObject.SetActive(true);
                return;
            }

            gameObject.SetActive(true);

            _canvasGroup.DOKill();
            _canvasGroup.alpha = 0f;
            _canvasGroup.interactable = true;
            _canvasGroup.blocksRaycasts = true;

            var t = _canvasGroup.transform;
            t.DOKill();
            t.localScale = Vector3.one * _popScale;

            // Fade in, then ease the scale back to 1
            _canvasGroup
                .DOFade(1f, _fadeDuration)
                .OnComplete(() =>
                {
                    t.DOScale(1f, _fadeDuration)
                     .SetEase(Ease.OutBack);
                });
        }

        /// <summary>
        /// Hides the popup immediately without animation.
        /// </summary>
        public void HideImmediate()
        {
            if (_canvasGroup != null)
            {
                _canvasGroup.DOKill();
                _canvasGroup.alpha = 0f;
                _canvasGroup.interactable = false;
                _canvasGroup.blocksRaycasts = false;
            }

            gameObject.SetActive(false);
        }

        /// <summary>
        /// Internal close handler for the button.
        /// Performs fade-out and then invokes callbacks.
        /// </summary>
        private void OnCloseClicked()
        {
            if (_canvasGroup == null)
            {
                InvokeClosed();
                HideImmediate();
                return;
            }

            _canvasGroup.DOKill();
            _canvasGroup
                .DOFade(0f, _fadeDuration)
                .OnComplete(() =>
                {
                    _canvasGroup.interactable = false;
                    _canvasGroup.blocksRaycasts = false;
                    HideImmediate();
                    InvokeClosed();
                });
        }

        /// <summary>
        /// Invokes both the event and the optional per-call callback.
        /// </summary>
        private void InvokeClosed()
        {
            Closed?.Invoke();

            var callback = _onClosedCallback;
            _onClosedCallback = null;
            callback?.Invoke();
        }
    }
}
