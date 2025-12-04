using AllianceDemo.Application.Dtos;
using AllianceDemo.Domain.Entities;
using AllianceDemo.Domain.Enums;
using AllianceDemo.Domain.Interfaces;

namespace AllianceDemo.Application.UseCases
{
    /// <summary>
    /// Finalizes battle result, applies rewards and reports outcome to backend.
    /// </summary>
    public class CompleteBattleUseCase
    {
        private const int DefaultWinExperienceReward = 100;

        private readonly IAllianceApiClient _apiClient;
        private readonly ILogService _log;

        public CompleteBattleUseCase(IAllianceApiClient apiClient, ILogService log)
        {
            _apiClient = apiClient;
            _log = log;
        }

        /// <summary>
        /// Evaluates battle outcome for the given hero and enemy, 
        /// applies rewards and sends a battle report to the backend.
        /// </summary>
        public BattleResult Execute(Hero hero, Enemy enemy)
        {
            if (hero == null || enemy == null)
            {
                _log.Error("CompleteBattleUseCase.Execute called with null hero or enemy.");
                return BattleResult.None;
            }

            // Decide outcome
            var result = ResolveResult(hero, enemy);

            if (result == BattleResult.None)
                return BattleResult.None;

            // Apply rewards (if any)
            if (result == BattleResult.Win)
            {
                hero.AddExperience(DefaultWinExperienceReward);
            }

            // Log + send report
            _log.Info($"Battle completed. Result={result}, Hero={hero.Id}, Enemy={enemy.Id}");

            var report = new BattleReportDto
            {
                HeroId = hero.Id,
                HeroLevel = hero.Level,
                HeroRemainingHealth = hero.Health,
                EnemyId = enemy.Id,
                EnemyRemainingHealth = enemy.Health,
                Result = result
            };

            _apiClient.SendBattleReport(report);

            return result;
        }

        private static BattleResult ResolveResult(Hero hero, Enemy enemy)
        {
            // If both are alive – battle is not finished yet.
            if (hero.Health > 0 && enemy.IsAlive)
                return BattleResult.None;

            if (hero.Health <= 0 && enemy.IsAlive)
                return BattleResult.Lose;

            if (!enemy.IsAlive)
                return BattleResult.Win;

            // Fallback, though по логике сюда попасть нельзя.
            return BattleResult.None;
        }
    }
}
