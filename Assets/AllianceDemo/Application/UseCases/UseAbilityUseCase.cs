using AllianceDemo.Domain.Entities;

namespace AllianceDemo.Application.UseCases
{
    /// <summary>
    /// Executes the hero's ability on the enemy.
    /// Handles damage application and result evaluation.
    /// </summary>
    public class UseAbilityUseCase
    {
        /// <summary>
        /// Deals ability damage from the hero to the enemy.
        /// Returns true if the enemy dies from this action.
        /// </summary>
        public bool Execute(Hero hero, Enemy enemy)
        {
            if (hero == null)
                throw new System.ArgumentNullException(nameof(hero));
            if (enemy == null)
                throw new System.ArgumentNullException(nameof(enemy));
            if (!enemy.IsAlive)
                return false;

            // Future-friendly: crits, buffs, debuffs, resistances can be inserted here.
            int damage = hero.AbilityDamage;

            enemy.TakeDamage(damage);

            // Returns whether the enemy died after receiving damage.
            return !enemy.IsAlive;
        }
    }
}
