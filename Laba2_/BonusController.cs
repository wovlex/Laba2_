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

            double size = random.Next(20, 35); // Увеличил размер для лучшей видимости
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

        // Новый метод: удалить конкретный бонус
        public void RemoveBonus(Bonus bonus)
        {
            activeBonuses.Remove(bonus);
        }

        // Новый метод: получить все активные бонусы для отладки
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

            // Устанавливаем позицию через Canvas свойства
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
            // Проверяем попадание в круг
            double centerX = position.X + size / 2;
            double centerY = position.Y + size / 2;
            double radius = size / 2;

            double distance = Math.Sqrt(Math.Pow(mousePosition.X - centerX, 2) +
                                        Math.Pow(mousePosition.Y - centerY, 2));

            return distance <= radius;
        }

        public abstract void ApplyEffect(Player player);

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
            sprite.Fill = new SolidColorBrush(Color.FromArgb(255, 255, 100, 100)); // Красный
        }

        public override void ApplyEffect(Player player)
        {
            player.AddEffect(new PlayerEffect(
                EffectType.DamageBoost,
                0.5, // +50% урона
                10.0, // 10 секунд
                "Усиление урона"
            ));
           
        }
    }

    public class CooldownBonus : Bonus
    {
        public CooldownBonus(Point position, double size, double lifetime)
            : base(position, size, lifetime)
        {
            sprite.Fill = new SolidColorBrush(Color.FromArgb(255, 100, 100, 255)); // Синий
        }

        public override void ApplyEffect(Player player)
        {
            player.AddEffect(new PlayerEffect(
                EffectType.CooldownReduction,
                0.3, // -30% перезарядки
                8.0, // 8 секунд
                "Ускорение атаки"
            ));
           
        }
    }

    public class GoldBonus : Bonus
    {
        public GoldBonus(Point position, double size, double lifetime)
            : base(position, size, lifetime)
        {
            sprite.Fill = new SolidColorBrush(Color.FromArgb(255, 255, 215, 0)); // Золотой
        }

        public override void ApplyEffect(Player player)
        {
            player.AddEffect(new PlayerEffect(
                EffectType.InstantGold,
                0,
                0,
                "Бонусное золото"
            ));

            // Мгновенное добавление золота
            player.AddGold(new BigNumber("50"));
            
        }
    }
}