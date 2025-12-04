using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using AllianceDemo.Domain.Enums;
using TMPro;

namespace AllianceDemo.Presentation.UI
{
    /// <summary>
    /// Victory/Defeat popup shown after a battle is completed.
    ///
    /// Responsibilities:
    /// - Display battle result (Win / Lose)
    /// - Play appear animation (fade + pop)
    /// - Notify listeners when player presses "Continue"
    ///
    /// Presentation-only: no game logic inside.
    /// </summary>
    [DisallowMultipleComponent]
    public class VictoryPopupView : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private CanvasGroup _canvasGroup;
        [SerializeField] private RectTransform _root;
        [SerializeField] private Button _continueButton;
        [SerializeField] private TMP_Text _titleText;

        [Header("Animation Settings")]
        [SerializeField] private float _fadeDuration = 0.3f;
        [SerializeField] private float _popScale = 1.15f;
        [SerializeField] private float _popDuration = 0.35f;

        /// <summary>
        /// Fired when the player presses the Continue button.
        /// </summary>
        public event Action Continued;

        private void Awake()
        {
            if (_continueButton != null)
            {
                _continueButton.onClick.AddListener(OnContinueClicked);
            }
            else
            {
                Debug.LogWarning("[VictoryPopupView] Continue button reference is missing.");
            }

            InitializeState();
        }

        private void OnDestroy()
        {
            if (_continueButton != null)
            {
                _continueButton.onClick.RemoveListener(OnContinueClicked);
            }
        }

        /// <summary>
        /// Shows the popup with a default "Win" context.
        /// </summary>
        public void Show()
        {
            Show(BattleResult.Win);
        }

        /// <summary>
        /// Shows the popup with the given battle result.
        /// Updates title and plays appear animations.
        /// </summary>
        public void Show(BattleResult result)
        {
            if (_canvasGroup == null || _root == null)
            {
                Debug.LogWarning("[VictoryPopupView] Missing CanvasGroup or Root RectTransform.");
                gameObject.SetActive(true);
                return;
            }

            gameObject.SetActive(true);

            // Update title
            if (_titleText != null)
            {
                _titleText.text = result == BattleResult.Win
                    ? "VICTORY!"
                    : "DEFEAT";
            }

            // Reset and play animations
            _canvasGroup.DOKill();
            _root.DOKill();

            _canvasGroup.alpha = 0f;
            _canvasGroup.interactable = true;
            _canvasGroup.blocksRaycasts = true;

            _root.localScale = Vector3.one * 0.8f;

            _canvasGroup
                .DOFade(1f, _fadeDuration)
                .SetEase(Ease.OutQuad);

            _root
                .DOScale(_popScale, _popDuration)
                .SetEase(Ease.OutBack)
                .OnComplete(() =>
                {
                    _root
                        .DOScale(1f, 0.15f)
                        .SetEase(Ease.OutQuad);
                });
        }

        /// <summary>
        /// Hides the popup immediately (no animation).
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

            if (_root != null)
            {
                _root.DOKill();
                _root.localScale = Vector3.one;
            }

            gameObject.SetActive(false);
        }

        /// <summary>
        /// Hides the popup. If animated = true, plays fade-out;
        /// otherwise hides instantly.
        /// </summary>
        public void Hide(bool animated = false)
        {
            if (!animated || _canvasGroup == null)
            {
                HideImmediate();
                return;
            }

            _canvasGroup.DOKill();
            _canvasGroup
                .DOFade(0f, _fadeDuration * 0.7f)
                .SetEase(Ease.OutQuad)
                .OnComplete(HideImmediate);
        }

        /// <summary>
        /// Ensures the popup is fully hidden and non-interactive on startup.
        /// </summary>
        private void InitializeState()
        {
            if (_canvasGroup != null)
            {
                _canvasGroup.alpha = 0f;
                _canvasGroup.interactable = false;
                _canvasGroup.blocksRaycasts = false;
            }

            if (_root != null)
            {
                _root.localScale = Vector3.one;
            }

            gameObject.SetActive(false);
        }

        /// <summary>
        /// Internal handler for the Continue button.
        /// </summary>
        private void OnContinueClicked()
        {
            Continued?.Invoke();
        }
    }
}
