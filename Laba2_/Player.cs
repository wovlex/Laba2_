using System;
using System.Collections.Generic;
using System.Linq;

namespace Laba2_
{
    public class Player
    {
        public BigNumber Gold { get; private set; }
        public int Level { get; private set; }
        public double BaseCooldown { get; private set; }
        public double CurrentCooldown { get; private set; }

        private BigNumber upgradeCost;
        private BigNumber cooldownUpgradeCost;
        private List<PlayerEffect> activeEffects;

        public Player()
        {
            Gold = new BigNumber("0");
            Level = 1;
            BaseCooldown = 1.0;
            CurrentCooldown = 0;
            upgradeCost = new BigNumber("100");
            cooldownUpgradeCost = new BigNumber("200");
            activeEffects = new List<PlayerEffect>();
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

                double newCost = upgradeCost.ToDouble() * 1.3;
                upgradeCost = new BigNumber(((int)newCost).ToString());

                return true;
            }
            return false;
        }

        public bool TryUpgradeCooldown()
        {
            if (CanUpgradeCooldown())
            {
                Gold = Gold - cooldownUpgradeCost;
                BaseCooldown = Math.Max(0.1, BaseCooldown * 0.9); // Уменьшаем перезарядку на 10%

                double newCost = cooldownUpgradeCost.ToDouble() * 1.5;
                cooldownUpgradeCost = new BigNumber(((int)newCost).ToString());

                return true;
            }
            return false;
        }

        public bool CanUpgrade()
        {
            return Gold.GreaterThanOrEqual(upgradeCost);
        }

        public bool CanUpgradeCooldown()
        {
            return Gold.GreaterThanOrEqual(cooldownUpgradeCost);
        }

        public BigNumber GetUpgradeCost()
        {
            return upgradeCost;
        }

        public BigNumber GetCooldownUpgradeCost()
        {
            return cooldownUpgradeCost;
        }

        public void StartCooldown()
        {
            double actualCooldown = BaseCooldown;

            // Применяем эффекты уменьшения перезарядки
            var cooldownEffects = activeEffects.Where(e => e.Type == EffectType.CooldownReduction);
            foreach (var effect in cooldownEffects)
            {
                actualCooldown *= (1 - effect.Power);
            }

            CurrentCooldown = Math.Max(0.1, actualCooldown);
        }

        public void UpdateCooldown(double deltaTime)
        {
            if (CurrentCooldown > 0)
            {
                CurrentCooldown = Math.Max(0, CurrentCooldown - deltaTime);
            }

            // Обновляем эффекты
            for (int i = activeEffects.Count - 1; i >= 0; i--)
            {
                activeEffects[i].Duration -= deltaTime;
                if (activeEffects[i].Duration <= 0)
                {
                    activeEffects.RemoveAt(i);
                }
            }
        }

        public bool CanAttack()
        {
            return CurrentCooldown <= 0;
        }

        public void AddEffect(PlayerEffect effect)
        {
            activeEffects.Add(effect);
        }

        public List<string> GetActiveEffects()
        {
            return activeEffects.Select(e => e.ToString()).ToList();
        }

        public double GetDamageMultiplier()
        {
            double multiplier = 1.0;
            var damageEffects = activeEffects.Where(e => e.Type == EffectType.DamageBoost);
            foreach (var effect in damageEffects)
            {
                multiplier += effect.Power;
            }
            return multiplier;
        }
    }

    public class PlayerEffect
    {
        public EffectType Type { get; set; }
        public double Power { get; set; }
        public double Duration { get; set; }
        public string Name { get; set; }

        public PlayerEffect(EffectType type, double power, double duration, string name)
        {
            Type = type;
            Power = power;
            Duration = duration;
            Name = name;
        }

        public override string ToString()
        {
            return $"{Name}: {Power * 100}% ({Duration:F1}s)";
        }
    }

    public enum EffectType
    {
        DamageBoost,
        CooldownReduction,
        InstantGold
    }
}