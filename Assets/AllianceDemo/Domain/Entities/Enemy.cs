using System;

namespace AllianceDemo.Domain.Entities
{
    /// <summary>
    /// Domain model for a simple enemy entity used in the battle.
    /// Responsible only for holding stats and health manipulation.
    /// No Unity dependencies — pure domain logic (clean architecture).
    /// </summary>
    public class Enemy
    {
        public string Id { get; }
        public string Name { get; }

        public int Health { get; private set; }
        public int MaxHealth { get; private set; }

        /// <summary>True when enemy still has HP above zero.</summary>
        public bool IsAlive => Health > 0;

        public Enemy(string id, string name, int maxHealth = 100)
        {
            if (string.IsNullOrWhiteSpace(id))
                throw new ArgumentException("Enemy ID cannot be null or empty", nameof(id));

            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Enemy name cannot be null or empty", nameof(name));

            if (maxHealth <= 0)
                throw new ArgumentOutOfRangeException(nameof(maxHealth), "MaxHealth must be > 0");

            Id = id;
            Name = name;
            MaxHealth = maxHealth;
            Health = maxHealth;
        }

        /// <summary>
        /// Apply damage to the enemy (cannot go below zero).
        /// </summary>
        public void TakeDamage(int amount)
        {
            if (amount <= 0) return;

            Health -= amount;
            if (Health < 0) Health = 0;
        }

        /// <summary>
        /// Fully restores health and resets battle-related temporary status.
        /// </summary>
        public void ResetStats()
        {
            Health = MaxHealth;

            // <-- Hooks for future scaling:
            // Reset buffs, debuffs, states, shields, etc.
            // Example:
            // _statusEffects.Clear();
        }
    }
}
