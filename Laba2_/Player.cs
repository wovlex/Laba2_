namespace Laba2_
{
    public class Player
    {
        public BigNumber Gold { get; private set; }
        public int Level { get; private set; }
        private BigNumber upgradeCost;

        public Player()
        {
            Gold = new BigNumber("0");
            Level = 1;
            upgradeCost = new BigNumber("100");
        }

        public void AddGold(BigNumber amount)
        {
            Gold = Gold + amount;
        }

        public bool TryUpgrade()
        {
            if (CanUpgrade())
            {
                Gold = Gold - upgradeCost;
                Level++;

                // Улучшенная формула стоимости улучшения
                double newCost = upgradeCost.ToDouble() * 1.3;
                upgradeCost = new BigNumber(((int)newCost).ToString());

                return true;
            }
            return false;
        }

        public bool CanUpgrade()
        {
            return Gold.GreaterThanOrEqual(upgradeCost);
        }

        public BigNumber GetUpgradeCost()
        {
            return upgradeCost;
        }
    }
}