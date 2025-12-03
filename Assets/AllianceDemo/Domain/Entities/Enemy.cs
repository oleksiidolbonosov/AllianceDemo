namespace AllianceDemo.Domain.Entities
{
    /// <summary>
    /// Domain model for a simple enemy entity.
    /// </summary>
    public class Enemy
    {
        public string Id { get; }
        public string Name { get; }

        public int Health { get; private set; }
        public int MaxHealth { get; private set; }

        public bool IsAlive => Health > 0;

        public Enemy(string id, string name, int maxHealth = 100)
        {
            Id = id;
            Name = name;
            MaxHealth = maxHealth;
            Health = maxHealth;
        }

        /// <summary>
        /// Apply damage to the enemy. Health will never go below zero.
        /// </summary>
        public void TakeDamage(int amount)
        {
            if (amount <= 0) return;

            Health -= amount;
            if (Health < 0)
                Health = 0;
        }
    }
}
