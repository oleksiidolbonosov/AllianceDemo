using AllianceDemo.Domain.Entities;
using UnityEngine;

namespace AllianceDemo.Presentation.Gameplay
{
    /// <summary>
    /// MonoBehaviour responsible for visually representing a hero in the scene.
    /// Binds to a pure domain Hero instance.
    /// </summary>
    public class HeroView : MonoBehaviour
    {
        public Hero BoundHero { get; private set; }

        public void Bind(Hero hero)
        {
            BoundHero = hero;
        }
    }
}
