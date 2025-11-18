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

        public Enemy(CEnemyTemplate template)
        {
            Name = template.Name;
            IconName = template.IconName;
            MaxHealth = new BigNumber(template.BaseLife.ToString());
            CurrentHealth = new BigNumber(template.BaseLife.ToString());
            GoldReward = new BigNumber(template.BaseGold.ToString());
            HealthModifier = template.LifeModifier;
            GoldModifier = template.GoldModifier;
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
            // Увеличиваем характеристики при появлении нового противника
            BigNumber healthIncrease = new BigNumber(((int)(MaxHealth.ToDouble() * (HealthModifier - 1))).ToString());
            MaxHealth = MaxHealth + healthIncrease;
            CurrentHealth = MaxHealth;

            BigNumber goldIncrease = new BigNumber(((int)(GoldReward.ToDouble() * (GoldModifier - 1))).ToString());
            GoldReward = GoldReward + goldIncrease;
        }

        private double ToDouble()
        {
            return double.Parse(this.ToString());
        }
    }
}