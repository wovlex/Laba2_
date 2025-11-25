namespace Laba2_
{
    public class Enemy
    {
        public string Name { get; private set; }
        public string IconName { get; private set; }
        public BigNumber MaxHealth { get; private set; }
        public BigNumber CurrentHealth { get; private set; }
        public BigNumber CurrentGoldReward { get; private set; }
        public double HealthModifier { get; private set; }
        public double GoldModifier { get; private set; }
        private int level;

        public Enemy(CEnemyTemplate template)
        {
            Name = template.Name;
            IconName = template.IconName;
            MaxHealth = new BigNumber(template.BaseLife.ToString());
            CurrentHealth = new BigNumber(template.BaseLife.ToString());
            CurrentGoldReward = new BigNumber(template.BaseGold.ToString());
            HealthModifier = template.LifeModifier;
            GoldModifier = template.GoldModifier;
            level = 1;
        }

        public bool TakeDamage(BigNumber damage, out BigNumber reward)
        {
            reward = new BigNumber("0");

            if (CurrentHealth.LessThanOrEqual(new BigNumber("0")))
                return false;

            if (damage.GreaterThanOrEqual(CurrentHealth))
            {
                reward = CurrentGoldReward;
                CurrentHealth = new BigNumber("0");
                return true;
            }
            else
            {
                CurrentHealth = CurrentHealth - damage;
                return false;
            }
        }

        public void LevelUp()
        {
            level++;

            // Увеличиваем максимальное здоровье
            double newMaxHealth = MaxHealth.ToDouble() * HealthModifier;
            MaxHealth = new BigNumber(((int)newMaxHealth).ToString());

            // Восстанавливаем здоровье
            RestoreHealth();

            // Увеличиваем награду за золото
            double newGoldReward = CurrentGoldReward.ToDouble() * GoldModifier;
            CurrentGoldReward = new BigNumber(((int)newGoldReward).ToString());
        }

        public void RestoreHealth()
        {
            CurrentHealth = MaxHealth;
        }

        public void IncreaseGoldReward(double multiplier)
        {
            double newReward = CurrentGoldReward.ToDouble() * multiplier;
            CurrentGoldReward = new BigNumber(((int)newReward).ToString());
        }

        public int GetLevel()
        {
            return level;
        }

        public string GetEnemyInfo()
        {
            return $"{Name} (Ур. {level}): HP {CurrentHealth}/{MaxHealth}, Gold: {CurrentGoldReward}";
        }
    }
}