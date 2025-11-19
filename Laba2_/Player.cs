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

                
                BigNumber baseCost = new BigNumber("100");
                BigNumber levelModifier = new BigNumber((1.2 + 0.05 * Level).ToString());
                upgradeCost = baseCost * levelModifier;

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