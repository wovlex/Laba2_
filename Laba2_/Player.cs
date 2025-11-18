using System;

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

        public bool TryUpgrade(BigNumber currentDamage)
        {
            if (CanUpgrade(currentDamage))
            {
                Gold = Gold - upgradeCost;
                Level++;

                // Увеличиваем стоимость улучшения
                BigNumber modifier = new BigNumber("1.2");
                upgradeCost = upgradeCost * modifier * new BigNumber(Level.ToString());

                return true;
            }
            return false;
        }

        public bool CanUpgrade(BigNumber currentDamage)
        {
            return Gold >= upgradeCost;
        }

        public BigNumber GetUpgradeCost()
        {
            return upgradeCost;
        }
    }
}