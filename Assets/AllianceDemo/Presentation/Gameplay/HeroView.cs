using AllianceDemo.Domain.Entities;
using UnityEngine;

namespace AllianceDemo.Presentation.Gameplay
{
    /// <summary>
    /// Presentation layer view for a <see cref="Hero"/>.
    /// Displays animations only and never contains gameplay logic.
    /// 
    /// ✔ Responsible solely for visuals (SRP)  
    /// ✔ Works with domain Hero without modifying it  
    /// ✔ Safe animator triggers (null-guard + unified helper)  
    /// ✔ Clean API for transitions: Idle() / Attack()
    /// </summary>
    [DisallowMultipleComponent]
    public class HeroView : MonoBehaviour
    {
        [Header("Animator & Visuals")]
        [SerializeField] private Animator _animator;

        /// <summary>
        /// Bound domain entity. View reads data but never mutates it.
        /// </summary>
        public Hero Hero { get; private set; }

        /// <summary>
        /// Bind domain model to view.
        /// Call after hero creation once per battle session.
        /// </summary>
        public void Bind(Hero hero)
        {
            if (hero == null)
            {
                Debug.LogWarning("[HeroView] Tried to bind null Hero.");
                return;
            }

            Hero = hero;
            PlayIdle();
        }

        /// <summary>
        /// Default animation state.
        /// </summary>
        public void PlayIdle() => SetTriggerSafe("Idle");

        /// <summary>
        /// Used when hero performs an attack.
        /// </summary>
        public void PlayAttack() => SetTriggerSafe("Attack");

        /// <summary>
        /// Single trigger utility to avoid code repetition (DRY).
        /// </summary>
        private void SetTriggerSafe(string trigger)
        {
            if (_animator == null)
            {
                Debug.LogWarning($"[HeroView] Animator missing — trigger '{trigger}' ignored.");
                return;
            }

            // Reset all relevant states (expandable later without modifying gameplay code)
            _animator.ResetTrigger("Idle");
            _animator.ResetTrigger("Attack");

            _animator.SetTrigger(trigger);
        }
    }
}
