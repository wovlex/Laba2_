using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Laba2_
{
    public class BonusController
    {
        private List<Bonus> activeBonuses;
        private Random random;
        private Size gameAreaSize;

        public BonusController(Size gameAreaSize)
        {
            activeBonuses = new List<Bonus>();
            random = new Random();
            this.gameAreaSize = gameAreaSize;
        }

        public void SpawnRandomBonus()
        {
            if (activeBonuses.Count >= 3)
                return;

            Point position = new Point(
                random.Next(10, (int)gameAreaSize.Width - 30),
                random.Next(10, (int)gameAreaSize.Height - 30)
            );

            double size = random.Next(20, 35);
            double lifetime = random.Next(3, 8);

            Bonus bonus = null;
            int bonusType = random.Next(0, 3);

            switch (bonusType)
            {
                case 0:
                    bonus = new DamageBonus(position, size, lifetime);
                    break;
                case 1:
                    bonus = new CooldownBonus(position, size, lifetime);
                    break;
                case 2:
                    bonus = new GoldBonus(position, size, lifetime);
                    break;
            }

            if (bonus != null)
            {
                activeBonuses.Add(bonus);
            }
        }

        public void Update(double deltaTime)
        {
            for (int i = activeBonuses.Count - 1; i >= 0; i--)
            {
                if (!activeBonuses[i].Update(deltaTime))
                {
                    activeBonuses.RemoveAt(i);
                }
            }
        }

        public bool CheckBonusClick(Point mousePosition, Player player)
        {
            for (int i = activeBonuses.Count - 1; i >= 0; i--)
            {
                if (activeBonuses[i].IsMouseOver(mousePosition))
                {
                    activeBonuses[i].ApplyEffect(player);
                    activeBonuses.RemoveAt(i);
                    return true;
                }
            }
            return false;
        }

        public List<Bonus> GetActiveBonuses()
        {
            return activeBonuses;
        }

        public void RemoveBonus(Bonus bonus)
        {
            activeBonuses.Remove(bonus);
        }

        public int GetBonusCount()
        {
            return activeBonuses.Count;
        }
    }

    public abstract class Bonus
    {
        protected Point position;
        protected double size;
        protected double lifetime;
        protected Ellipse sprite;

        public Bonus(Point position, double size, double lifetime)
        {
            this.position = position;
            this.size = size;
            this.lifetime = lifetime;
            CreateSprite();
        }

        protected virtual void CreateSprite()
        {
            sprite = new Ellipse();
            sprite.StrokeThickness = 2;
            sprite.Stroke = Brushes.Black;
            sprite.Width = size;
            sprite.Height = size;
            sprite.Cursor = System.Windows.Input.Cursors.Hand;

            Canvas.SetLeft(sprite, position.X);
            Canvas.SetTop(sprite, position.Y);
        }

        public virtual bool Update(double deltaTime)
        {
            lifetime -= deltaTime;
            return lifetime > 0;
        }

        public virtual bool IsMouseOver(Point mousePosition)
        {
            double centerX = position.X + size / 2;
            double centerY = position.Y + size / 2;
            double radius = size / 2;

            double distance = Math.Sqrt(Math.Pow(mousePosition.X - centerX, 2) +
                                      Math.Pow(mousePosition.Y - centerY, 2));

            return distance <= radius;
        }

        public abstract void ApplyEffect(Player player);

        public abstract string GetBonusInfo();

        public Ellipse GetSprite()
        {
            return sprite;
        }

        public Point GetPosition()
        {
            return position;
        }

        public double GetSize()
        {
            return size;
        }
    }

    public class DamageBonus : Bonus
    {
        public DamageBonus(Point position, double size, double lifetime)
            : base(position, size, lifetime)
        {
            sprite.Fill = new SolidColorBrush(Color.FromArgb(255, 255, 100, 100));
        }

        public override void ApplyEffect(Player player)
        {
            player.AddEffect(new PlayerEffect(
                EffectType.DamageBoost,
                0.5,
                10.0,
                "Усиление урона"
            ));
        }

        public override string GetBonusInfo()
        {
            return $"Бонус урона: +50% урона на 10 сек";
        }
    }

    public class CooldownBonus : Bonus
    {
        public CooldownBonus(Point position, double size, double lifetime)
            : base(position, size, lifetime)
        {
            sprite.Fill = new SolidColorBrush(Color.FromArgb(255, 100, 100, 255));
        }

        public override void ApplyEffect(Player player)
        {
            player.AddEffect(new PlayerEffect(
                EffectType.CooldownReduction,
                0.3,
                8.0,
                "Ускорение атаки"
            ));
        }

        public override string GetBonusInfo()
        {
            return $"Бонус перезарядки: -30% кд на 8 сек";
        }
    }

    public class GoldBonus : Bonus
    {
        public GoldBonus(Point position, double size, double lifetime)
            : base(position, size, lifetime)
        {
            sprite.Fill = new SolidColorBrush(Color.FromArgb(255, 255, 215, 0));
        }

        public override void ApplyEffect(Player player)
        {
            player.AddGold(new BigNumber("50"));

            player.AddEffect(new PlayerEffect(
                EffectType.InstantGold,
                0,
                0,
                "Бонусное золото: +50"
            ));
        }

        public override string GetBonusInfo()
        {
            return $"Бонус золота: +50 монет";
        }
    }
}