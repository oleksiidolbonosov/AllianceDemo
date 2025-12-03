using AllianceDemo.Domain.Entities;
using UnityEngine;

namespace AllianceDemo.Presentation.Gameplay
{
    /// <summary>
    /// MonoBehaviour responsible for visually representing an enemy in the scene.
    /// </summary>
    public class EnemyView : MonoBehaviour
    {
        public Enemy BoundEnemy { get; private set; }

        public void Bind(Enemy enemy)
        {
            BoundEnemy = enemy;
        }
    }
}
