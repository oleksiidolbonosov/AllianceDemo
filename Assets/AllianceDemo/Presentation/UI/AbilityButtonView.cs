using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace AllianceDemo.Presentation.UI
{
    /// <summary>
    /// View wrapper for the hero ability button UI.
    /// 
    /// Responsibility:
    ///  - Handles button clicks
    ///  - Controls visual state (fade, visibility, interactability)
    ///
    /// No gameplay logic inside – strictly presentation layer (SRP, KISS).
    /// </summary>
    [DisallowMultipleComponent]
    public class AbilityButtonView : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private Button _button;
        [SerializeField] private CanvasGroup _canvasGroup;

        [Header("Animation")]
        [SerializeField] private float _fadeDuration = 0.25f;

        /// <summary>
        /// Fired when the player presses the ability button.
        /// </summary>
        public event Action Clicked;

        /// <summary>
        /// RectTransform reference for FTUE highlight systems.
        /// </summary>
        public RectTransform RectTransform =>
            _button != null ? _button.GetComponent<RectTransform>() : null;

        private void Awake()
        {
            if (_button != null)
            {
                _button.onClick.AddListener(OnClickInternal);
            }
            else
            {
                Debug.LogWarning("[AbilityButtonView] Button reference is missing.");
            }

            // Default hidden state (FTUE will reveal it).
            HideImmediate();
        }

        private void OnDestroy()
        {
            if (_button != null)
                _button.onClick.RemoveListener(OnClickInternal);
        }

        private void OnClickInternal() => Clicked?.Invoke();

        /// <summary>
        /// Enables/disables interaction. Updates CanvasGroup if present.
        /// </summary>
        public void SetInteractable(bool active)
        {
            if (_button != null)
                _button.interactable = active;

            if (_canvasGroup != null)
            {
                _canvasGroup.interactable = active;
                _canvasGroup.blocksRaycasts = active;
            }
        }

        /// <summary>
        /// Smooth fade-in reveal, typically used during FTUE.
        /// </summary>
        public void RevealWithFade()
        {
            if (_canvasGroup == null)
            {
                // Fall back to instant enable if CanvasGroup is absent.
                SetInteractable(true);
                return;
            }

            _canvasGroup.DOKill();
            _canvasGroup.alpha = 0f;
            _canvasGroup.interactable = true;
            _canvasGroup.blocksRaycasts = true;

            _canvasGroup
                .DOFade(1f, _fadeDuration)
                .SetEase(Ease.OutQuad);

            if (_button != null)
                _button.interactable = true;
        }

        /// <summary>
        /// Instantly hides the button and disables interaction.
        /// Used by battle reset or before FTUE.
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

            if (_button != null)
                _button.interactable = false;
        }

        /// <summary>
        /// Instantly shows the button without animation.
        /// Useful outside FTUE flow.
        /// </summary>
        public void ShowImmediate()
        {
            if (_canvasGroup != null)
            {
                _canvasGroup.DOKill();
                _canvasGroup.alpha = 1f;
                _canvasGroup.interactable = true;
                _canvasGroup.blocksRaycasts = true;
            }

            if (_button != null)
                _button.interactable = true;
        }
    }
}
