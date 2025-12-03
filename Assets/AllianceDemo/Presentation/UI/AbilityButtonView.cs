using System;
using UnityEngine;
using UnityEngine.UI;

namespace AllianceDemo.Presentation.UI
{
    /// <summary>
    /// Simple wrapper around a button for the hero's active ability.
    /// </summary>
    public class AbilityButtonView : MonoBehaviour
    {
        [SerializeField] private Button _button;

        public event Action OnClicked;

        private void Awake()
        {
            if (_button != null)
                _button.onClick.AddListener(HandleClick);
        }

        private void OnDestroy()
        {
            if (_button != null)
                _button.onClick.RemoveListener(HandleClick);
        }

        private void HandleClick()
        {
            OnClicked?.Invoke();
        }

        public void SetInteractable(bool value)
        {
            if (_button != null)
                _button.interactable = value;
        }

        public RectTransform RectTransform => _button != null ? _button.GetComponent<RectTransform>() : null;
    }
}
