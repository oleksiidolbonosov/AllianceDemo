using AllianceDemo.Domain.Entities;

namespace AllianceDemo.Application.UseCases
{
    /// <summary>
    /// Use case for using the hero's ability against an enemy.
    /// Returns true if the enemy was killed by this action.
    /// </summary>
    public class UseAbilityUseCase
    {
        public bool Execute(Hero hero, Enemy enemy)
        {
            if (!enemy.IsAlive)
                return false;

            enemy.TakeDamage(hero.AbilityDamage);
            return !enemy.IsAlive;
        }
    }
}
