using AllianceDemo.Domain.Entities;

namespace AllianceDemo.Application.UseCases
{
    /// <summary>
    /// Use case responsible for preparing hero and enemy for a new battle.
    /// </summary>
    public class StartBattleUseCase
    {
        public void Initialize(Hero hero, Enemy enemy)
        {
            hero.HealFull();
            // Any additional pre-battle logic could go here (buffs, states, etc.).
        }
    }
}
