using AllianceDemo.Application.Dtos;
using AllianceDemo.Domain.Entities;
using AllianceDemo.Domain.Enums;
using AllianceDemo.Domain.Interfaces;

namespace AllianceDemo.Application.UseCases
{
    /// <summary>
    /// Use case to finalize battle results, reward hero and notify backend.
    /// </summary>
    public class CompleteBattleUseCase
    {
        private readonly IAllianceApiClient _apiClient;
        private readonly ILogService _log;

        public CompleteBattleUseCase(IAllianceApiClient apiClient, ILogService log)
        {
            _apiClient = apiClient;
            _log = log;
        }

        public BattleResult Execute(Hero hero, Enemy enemy)
        {
            BattleResult result;

            if (enemy.IsAlive && hero.Health <= 0)
            {
                result = BattleResult.Lose;
            }
            else if (!enemy.IsAlive)
            {
                result = BattleResult.Win;
                hero.AddExperience(100); // simple reward for the demo
            }
            else
            {
                result = BattleResult.None;
            }

            if (result != BattleResult.None)
            {
                _log.Info($"Battle completed. Result: {result}");

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
            }

            return result;
        }
    }
}
