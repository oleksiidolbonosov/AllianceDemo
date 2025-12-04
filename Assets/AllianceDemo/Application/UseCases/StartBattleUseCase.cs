using AllianceDemo.Domain.Entities;

namespace AllianceDemo.Application.UseCases
{
    /// <summary>
    /// Prepares hero and enemy for a new battle session.
    /// Responsible ONLY for initial state reset (no gameplay logic here).
    /// </summary>
    public class StartBattleUseCase
    {
        /// <summary>
        /// Restores health and resets states before the fight begins.
        /// </summary>
        public void Initialize(Hero hero, Enemy enemy)
        {
            if (hero == null || enemy == null)
                throw new System.ArgumentNullException("Hero or Enemy reference is null in StartBattleUseCase.Initialize");

            // Reset hero
            hero.HealFull();
            hero.ResetStats();          // optional future method (buffs, debuffs, etc.)

            // Reset enemy
            enemy.ResetStats();             // if у Enemy есть ResetStats(), иначе можно HealFull

            // Here we can later add:
            // - apply pre-battle effects
            // - equip temporary modifiers
            // - sync UI or analytics start markers
        }
    }
}
