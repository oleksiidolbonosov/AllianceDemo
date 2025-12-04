using AllianceDemo.Domain.Enums;

namespace AllianceDemo.Application.Dtos
{
    /// <summary>
    /// DTO used for sending battle results to the backend service.
    /// Contains only pure data required for reporting.
    /// </summary>
    public class BattleReportDto
    {
        /// <summary> Unique identifier of the hero. </summary>
        public string HeroId { get; set; }

        /// <summary> Final hero level after battle rewards. </summary>
        public int HeroLevel { get; set; }

        /// <summary> Remaining hero HP at the end of the fight. </summary>
        public int HeroRemainingHealth { get; set; }

        /// <summary> Unique identifier of the enemy type or instance. </summary>
        public string EnemyId { get; set; }

        /// <summary> Remaining enemy HP (usually zero on victory). </summary>
        public int EnemyRemainingHealth { get; set; }

        /// <summary> Win / Lose result of the battle. </summary>
        public BattleResult Result { get; set; }

        public override string ToString() =>
            $"BattleResult: {Result} | Hero:{HeroId},HP:{HeroRemainingHealth},Lvl:{HeroLevel} vs Enemy:{EnemyId},HP:{EnemyRemainingHealth}";
    }
}
