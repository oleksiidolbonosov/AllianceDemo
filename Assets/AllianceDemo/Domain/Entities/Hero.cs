using System;

namespace AllianceDemo.Domain.Entities
{
    /// <summary>
    /// Domain model representing a hero under the player's control.
    /// Pure data + behavior, no Unity dependencies.
    /// </summary>
    public class Hero
    {
        /// <summary>Unique identifier of the hero (used for analytics / backend).</summary>
        public string Id { get; }

        /// <summary>Display name of the hero.</summary>
        public string Name { get; }

        /// <summary>Current hero level (progression is persistent across battles).</summary>
        public int Level { get; private set; }

        /// <summary>Current amount of experience towards the next level.</summary>
        public int Experience { get; private set; }

        /// <summary>Current health value. Never goes below zero.</summary>
        public int Health { get; private set; }

        /// <summary>Maximum health value at the current level.</summary>
        public int MaxHealth { get; private set; }

        /// <summary>Damage dealt by the hero's basic ability (can be scaled with level).</summary>
        public int AbilityDamage { get; private set; }

        public Hero(string id, string name, int level = 1, int maxHealth = 100, int abilityDamage = 30)
        {
            if (string.IsNullOrWhiteSpace(id))
                throw new ArgumentException("Hero ID cannot be null or empty.", nameof(id));

            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Hero name cannot be null or empty.", nameof(name));

            if (level <= 0)
                throw new ArgumentOutOfRangeException(nameof(level), "Level must be greater than zero.");

            if (maxHealth <= 0)
                throw new ArgumentOutOfRangeException(nameof(maxHealth), "MaxHealth must be greater than zero.");

            if (abilityDamage <= 0)
                throw new ArgumentOutOfRangeException(nameof(abilityDamage), "AbilityDamage must be greater than zero.");

            Id = id;
            Name = name;
            Level = level;
            MaxHealth = maxHealth;
            Health = maxHealth;
            AbilityDamage = abilityDamage;
            Experience = 0;
        }

        /// <summary>
        /// Apply damage to the hero. Health will never go below zero.
        /// </summary>
        public void TakeDamage(int amount)
        {
            if (amount <= 0)
                return;

            Health -= amount;
            if (Health < 0)
                Health = 0;
        }

        /// <summary>
        /// Fully restores hero health to MaxHealth.
        /// </summary>
        public void HealFull()
        {
            Health = MaxHealth;
        }

        /// <summary>
        /// Adds experience and performs level up when threshold is reached.
        /// Level up can later be extended to modify stats (MaxHealth, AbilityDamage, etc.).
        /// </summary>
        public void AddExperience(int amount)
        {
            if (amount <= 0)
                return;

            Experience += amount;

            while (Experience >= ExperienceToNextLevel())
            {
                Experience -= ExperienceToNextLevel();
                LevelUp();
            }
        }

        /// <summary>
        /// Simple linear experience curve: 100 * current level.
        /// </summary>
        private int ExperienceToNextLevel() => 100 * Level;

        /// <summary>
        /// Handles level-up logic. For the demo we slightly scale MaxHealth and AbilityDamage.
        /// In a real project this logic would live in a separate progression service.
        /// </summary>
        private void LevelUp()
        {
            Level++;

            // Very simple progression curve for the demo.
            MaxHealth = (int)(MaxHealth * 1.1f);
            AbilityDamage = (int)(AbilityDamage * 1.05f);

            // Make sure hero is not left at very low HP after level-up in a demo scenario.
            HealFull();

            // In a larger system, a domain event could be raised here.
            // e.g. HeroLeveledUpDomainEvent
        }

        /// <summary>
        /// Resets battle-related stats while keeping persistent progression (level/experience).
        /// Intended to be used when starting a new battle session.
        /// </summary>
        public void ResetStats()
        {
            HealFull();
        }
    }
}
