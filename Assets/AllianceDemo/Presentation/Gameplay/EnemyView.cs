using AllianceDemo.Domain.Entities;
using UnityEngine;

namespace AllianceDemo.Presentation.Gameplay
{
    /// <summary>
    /// Presentation layer view for an <see cref="Enemy"/>.
    /// Responsible only for playing animator states (UI/Visual responsibilities only).
    /// </summary>
    [DisallowMultipleComponent]
    public class EnemyView : MonoBehaviour
    {
        [Header("Animator & Visuals")]
        [SerializeField] private Animator _animator;

        /// <summary>
        /// Reference to domain entity.
        /// View never mutates it — only reads (SRP).
        /// </summary>
        public Enemy Enemy { get; private set; }

        /// <summary>
        /// Binds domain model to view and switches to Idle animation.
        /// Call once on battle start.
        /// </summary>
        public void Bind(Enemy enemy)
        {
            if (enemy == null)
            {
                Debug.LogWarning("[EnemyView] Received null Enemy reference.");
                return;
            }

            Enemy = enemy;
            PlayIdle();
        }

        /// <summary>
        /// Plays default idle animation.
        /// </summary>
        public void PlayIdle() => SetTriggerSafe("Idle");

        /// <summary>
        /// Plays hit animation (short damage feedback).
        /// </summary>
        public void PlayHit() => SetTriggerSafe("Hit");

        /// <summary>
        /// Plays death animation (final state).
        /// </summary>
        public void PlayDie() => SetTriggerSafe("Die");

        /// <summary>
        /// Safe wrapper for working with Animator.
        /// Single exit point eliminates repetition (DRY).
        /// </summary>
        private void SetTriggerSafe(string trigger)
        {
            if (_animator == null)
            {
                Debug.LogWarning($"[EnemyView] Animator missing, trigger '{trigger}' skipped.");
                return;
            }

            _animator.ResetTrigger("Idle");
            _animator.ResetTrigger("Hit");
            _animator.ResetTrigger("Die");
            _animator.SetTrigger(trigger);
        }
    }
}
