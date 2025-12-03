namespace AllianceDemo.Domain.Entities
{
    /// <summary>
    /// Domain model representing a hero under the player's control.
    /// Pure data + behavior, no Unity dependencies.
    /// </summary>
    public class Hero
    {
        public string Id { get; }
        public string Name { get; }

        public int Level { get; private set; }
        public int Experience { get; private set; }
        public int Health { get; private set; }
        public int MaxHealth { get; private set; }
        public int AbilityDamage { get; private set; }

        public Hero(string id, string name, int level = 1, int maxHealth = 100, int abilityDamage = 30)
        {
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
            if (amount <= 0) return;

            Health -= amount;
            if (Health < 0)
                Health = 0;
        }

        /// <summary>
        /// Fully restore hero health to MaxHealth.
        /// </summary>
        public void HealFull()
        {
            Health = MaxHealth;
        }

        /// <summary>
        /// Adds experience and performs level up when threshold is reached.
        /// </summary>
        public void AddExperience(int amount)
        {
            if (amount <= 0) return;

            Experience += amount;

            while (Experience >= ExperienceToNextLevel())
            {
                Experience -= ExperienceToNextLevel();
                Level++;
                // Domain event could be raised here in a larger system.
            }
        }

        /// <summary>
        /// Simple linear experience curve: 100 * current level.
        /// </summary>
        private int ExperienceToNextLevel() => 100 * Level;
    }
}
