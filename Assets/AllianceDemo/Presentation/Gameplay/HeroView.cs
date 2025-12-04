using AllianceDemo.Domain.Entities;
using UnityEngine;

namespace AllianceDemo.Presentation.Gameplay
{
    /// <summary>
    /// MonoBehaviour responsible for visually representing a hero in the scene.
    /// Binds to a pure domain Hero instance and plays Animator-driven animations.
    /// </summary>
    public class HeroView : MonoBehaviour
    {
        [Header("Components")]
        [SerializeField] private Animator _animator;

        public Hero BoundHero { get; private set; }

        public void Bind(Hero hero)
        {
            BoundHero = hero;
            PlayIdle();
        }

        public void PlayIdle()
        {
            if (_animator == null) return;
            _animator.ResetTrigger("Attack");
            _animator.SetTrigger("Idle");
        }

        public void PlayAttack()
        {
            if (_animator == null) return;
            _animator.ResetTrigger("Idle");
            _animator.SetTrigger("Attack");
        }
    }
}
