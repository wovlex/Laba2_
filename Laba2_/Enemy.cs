namespace Laba2_
{
    public class Enemy
    {
        public string Name { get; private set; }
        public string IconName { get; private set; }
        public BigNumber MaxHealth { get; private set; }
        public BigNumber CurrentHealth { get; private set; }
        public BigNumber GoldReward { get; private set; }
        public double HealthModifier { get; private set; }
        public double GoldModifier { get; private set; }
        private int level;

        public Enemy(CEnemyTemplate template)
        {
            Name = template.Name;
            IconName = template.IconName;
            MaxHealth = new BigNumber(template.BaseLife.ToString());
            CurrentHealth = new BigNumber(template.BaseLife.ToString());
            GoldReward = new BigNumber(template.BaseGold.ToString());
            HealthModifier = template.LifeModifier;
            GoldModifier = template.GoldModifier;
            level = 1;
        }

        public bool TakeDamage(BigNumber damage, out BigNumber reward)
        {
            reward = new BigNumber("0");

            if (damage >= CurrentHealth)
            {
                reward = GoldReward;
                return true; // Противник побежден
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
            BigNumber healthIncrease = new BigNumber(((int)(MaxHealth.ToDouble() * (HealthModifier - 1))).ToString());
            MaxHealth = MaxHealth + healthIncrease;

            // ВОССТАНАВЛИВАЕМ здоровье полностью
            CurrentHealth = MaxHealth;

            // Увеличиваем GoldModifier с каждым уровнем
            GoldModifier *= 1.1; // Увеличиваем на 10% каждый уровень

            // Увеличиваем награду за золото
            BigNumber goldIncrease = new BigNumber(((int)(GoldReward.ToDouble() * (GoldModifier - 1))).ToString());
            GoldReward = GoldReward + goldIncrease;
        }

        public void RestoreHealth()
        {
            // Полное восстановление здоровья
            CurrentHealth = MaxHealth;
        }

        public double ToDouble()
        {
            try
            {
                return double.Parse(this.ToString());
            }
            catch
            {
                return 0;
            }
        }

        public void IncreaseGoldReward(double multiplier)
        {
            BigNumber newReward = this.GoldReward.Multiply(new BigNumber(multiplier.ToString()));
            this.GoldReward = newReward;
        }

        public int GetLevel()
        {
            return level;
        }
    }
}