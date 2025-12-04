namespace AllianceDemo.Domain.Enums
{
    /// <summary>
    /// High-level summary of a battle outcome.
    /// Used in battle flow, analytics, UI, and backend reports.
    /// </summary>
    public enum BattleResult
    {
        /// <summary>
        /// Battle not finished or result not evaluated yet.
        /// </summary>
        None = 0,

        /// <summary>
        /// Hero defeated the enemy.
        /// </summary>
        Win = 1,

        /// <summary>
        /// Hero was defeated by the enemy.
        /// </summary>
        Lose = 2,

        /// <summary>
        /// Optional future support for stalemate/timeout modes.
        /// </summary>
        //Draw = 3
    }
}
