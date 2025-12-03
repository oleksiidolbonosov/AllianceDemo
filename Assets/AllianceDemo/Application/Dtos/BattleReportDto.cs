using AllianceDemo.Domain.Enums;

namespace AllianceDemo.Application.Dtos
{
    /// <summary>
    /// DTO representing a battle outcome that can be transferred to a backend.
    /// </summary>
    public class BattleReportDto
    {
        public string HeroId;
        public int HeroLevel;
        public int HeroRemainingHealth;
        public string EnemyId;
        public int EnemyRemainingHealth;
        public BattleResult Result;
    }
}
