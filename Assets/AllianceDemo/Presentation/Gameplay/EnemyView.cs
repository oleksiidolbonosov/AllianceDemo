using AllianceDemo.Domain.Entities;
using UnityEngine;

namespace AllianceDemo.Presentation.Gameplay
{
    /// <summary>
    /// MonoBehaviour responsible for visually representing an enemy in the scene
    /// and playing hit/death animations.
    /// </summary>
    public class EnemyView : MonoBehaviour
    {
        [Header("Components")]
        [SerializeField] private Animator _animator;

        public Enemy BoundEnemy { get; private set; }

        public void Bind(Enemy enemy)
        {
            BoundEnemy = enemy;
            PlayIdle();
        }

        public void PlayIdle()
        {
            if (_animator == null) return;
            _animator.ResetTrigger("Hit");
            _animator.ResetTrigger("Die");
            _animator.SetTrigger("Idle");
        }

        public void PlayHit()
        {
            if (_animator == null) return;
            _animator.SetTrigger("Hit");
        }

        public void PlayDie()
        {
            if (_animator == null) return;
            _animator.SetTrigger("Die");
        }
    }
}
