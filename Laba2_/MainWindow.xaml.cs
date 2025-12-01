using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml.Linq;
using System.Windows.Threading;

namespace Laba2_
{
    public partial class MainWindow : Window
    {
        private Player player;
        private Enemy currentEnemy;
        private EnemyManager enemyManager;
        private BigNumber baseDamage;
        private CIconList iniicon;
        private BonusController bonusController;
        private DispatcherTimer gameTimer;
        private DispatcherTimer bonusSpawnTimer;

        public MainWindow()
        {
            InitializeComponent();
            InitializeGame();
            InitializeTimers();
        }

        private void InitializeGame()
        {
            // Инициализация игрока
            player = new Player();
            baseDamage = new BigNumber("50");

            // Загрузка врагов из JSON
            enemyManager = new EnemyManager();
            LoadEnemies();

            // Инициализация контроллера бонусов
            bonusController = new BonusController(new System.Windows.Size(160, 160));

            // Обновление интерфейса
            UpdatePlayerUI();
            SpawnNewEnemy();
        }

        private void InitializeTimers()
        {
            // Таймер для обновления игры
            gameTimer = new DispatcherTimer();
            gameTimer.Interval = TimeSpan.FromMilliseconds(100);
            gameTimer.Tick += GameTimer_Tick;
            gameTimer.Start();

            // Таймер для спавна бонусов
            bonusSpawnTimer = new DispatcherTimer();
            bonusSpawnTimer.Interval = TimeSpan.FromSeconds(3);
            bonusSpawnTimer.Tick += BonusSpawnTimer_Tick;
            bonusSpawnTimer.Start();
        }

        private void GameTimer_Tick(object sender, EventArgs e)
        {
            // Обновление перезарядки игрока
            player.UpdateCooldown(0.1);

            // Обновление бонусов
            bonusController.Update(0.1);

            // Обновление UI
            UpdateCooldownUI();
            UpdateActiveEffectsUI();
            UpdateBonusUI();
        }

        private void BonusSpawnTimer_Tick(object sender, EventArgs e)
        {
            bonusController.SpawnRandomBonus();
            UpdateBonusUI();
        }

        private void LoadEnemies()
        {
            try
            {
                string basePath = AppDomain.CurrentDomain.BaseDirectory;
                string enemiesPath = System.IO.Path.Combine(basePath, "enemies.json");

                if (File.Exists(enemiesPath))
                {
                    enemyManager.LoadEnemies(enemiesPath);
                }
                else
                {
                    CreateTestEnemies();
                }
            }
            catch (Exception ex)
            {
                
                CreateTestEnemies();
            }
        }

        private void CreateTestEnemies()
        {
            var testEnemies = new CEnemyTemplateList();
            testEnemies.AddEnemy(new CEnemyTemplate("Goblin", "goblin_1.png", 100, 1.1, 50, 1.2, 0.4));
            testEnemies.AddEnemy(new CEnemyTemplate("Skeleton", "skeleton_1.png", 200, 1.2, 100, 1.3, 0.3));
            testEnemies.AddEnemy(new CEnemyTemplate("Troll", "troll_1.png", 500, 1.5, 250, 1.5, 0.2));
            testEnemies.AddEnemy(new CEnemyTemplate("Goblin", "goblin_5.png", 1000, 2.0, 1000, 2.0, 0.1));
            testEnemies.AddEnemy(new CEnemyTemplate("Goblin", "goblin_7.png", 1500, 2.0, 1000, 2.0, 0.3));
            testEnemies.AddEnemy(new CEnemyTemplate("Goblin", "goblin_8.png", 2000, 3.0, 1000, 3.0, 0.4));

            enemyManager.SetEnemies(testEnemies.GetEnemies());
        }

        private void SpawnNewEnemy()
        {
            currentEnemy = enemyManager.GetRandomEnemy();

            if (currentEnemy != null)
            {
                UpdateEnemyUI();

                try
                {
                    string basePath = AppDomain.CurrentDomain.BaseDirectory;
                    string imagePath = System.IO.Path.Combine(basePath, @"icons\Monsters", currentEnemy.IconName);
                    if (File.Exists(imagePath))
                    {
                        enemyImage.Source = new BitmapImage(new Uri(imagePath));
                    }
                   
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка загрузки изображения: {ex.Message}");
                }
            }
        }

        private void UpdatePlayerUI()
        {
            playerGoldText.Text = player.Gold.ToString();
            playerDamageText.Text = baseDamage.ToString();
            playerLevelText.Text = player.Level.ToString();
            upgradeCostText.Text = player.GetUpgradeCost().ToString();
            upgradeCooldownCostText.Text = player.GetCooldownUpgradeCost().ToString();

            upgradeButton.IsEnabled = player.CanUpgrade();
            upgradeCooldownButton.IsEnabled = player.CanUpgradeCooldown();
        }

        private void UpdateCooldownUI()
        {
            cooldownText.Text = $"{player.CurrentCooldown:F1}s / {player.BaseCooldown:F1}s";
        }

        private void UpdateActiveEffectsUI()
        {
            activeEffectsList.Items.Clear();
            foreach (var effect in player.GetActiveEffects())
            {
                activeEffectsList.Items.Add(effect);
            }
        }

        private void UpdateBonusUI()
        {
            // Очищаем canvas от старых бонусов
            var bonusesToRemove = new List<UIElement>();
            foreach (UIElement element in gameCanvas.Children)
            {
                if (element is Ellipse && element != enemyImage)
                {
                    bonusesToRemove.Add(element);
                }
            }

            foreach (var bonus in bonusesToRemove)
            {
                gameCanvas.Children.Remove(bonus);
            }

            // Добавляем текущие бонусы
            foreach (var bonus in bonusController.GetActiveBonuses())
            {
                var ellipse = bonus.GetSprite();
                if (ellipse != null)
                {
                    // Устанавливаем позицию
                    Canvas.SetLeft(ellipse, bonus.GetPosition().X);
                    Canvas.SetTop(ellipse, bonus.GetPosition().Y);

                    // Добавляем обработчик клика (через Tag)
                    ellipse.Tag = bonus;
                    ellipse.MouseDown += Bonus_MouseDown;

                    gameCanvas.Children.Add(ellipse);
                }
            }
        }

        private void Bonus_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is Ellipse ellipse && ellipse.Tag is Bonus bonus)
            {
                Point mousePosition = e.GetPosition(gameCanvas);
                if (bonus.IsMouseOver(mousePosition))
                {
                    bonus.ApplyEffect(player);
                    bonusController.RemoveBonus(bonus);
                    UpdateBonusUI();
                    UpdatePlayerUI();
                    UpdateActiveEffectsUI();
                    e.Handled = true;
                }
            }
        }


        private void UpdateEnemyUI()
        {
            if (currentEnemy != null && baseDamage != null)
            {
                enemyNameText.Text = $"{currentEnemy.Name} (Ур. {currentEnemy.GetLevel()})";
                enemyHpText.Text = $"{currentEnemy.CurrentHealth}/{currentEnemy.MaxHealth}";
                enemyGoldText.Text = $"{currentEnemy.CurrentGoldReward} (x{currentEnemy.GoldModifier:F2})";
                currentDamageText.Text = baseDamage.ToString();

                double currentHP = currentEnemy.CurrentHealth.ToDouble();
                double maxHP = currentEnemy.MaxHealth.ToDouble();
                double hpPercent = maxHP > 0 ? currentHP / maxHP : 0;

                enemyHpProgress.Text = $"HP: {currentEnemy.CurrentHealth}/{currentEnemy.MaxHealth} ({(hpPercent * 100):F1}%)";
            }
        }

        private void EnemyImage_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (currentEnemy != null && player.CanAttack())
            {
                // Проверяем клик по бонусам
                Point mousePosition = e.GetPosition(gameCanvas);

                // Проверяем клик по бонусам через контроллер
                if (bonusController.CheckBonusClick(mousePosition, player))
                {
                    UpdateBonusUI();
                    UpdatePlayerUI();
                    UpdateActiveEffectsUI();
                    e.Handled = true;
                    return;
                }

                // Атака врага
                BigNumber reward;
                bool isDefeated = currentEnemy.TakeDamage(baseDamage, out reward);

                if (isDefeated)
                {
                    player.AddGold(reward);
                   
                    SpawnNewEnemy();
                }

                player.StartCooldown();
                UpdateEnemyUI();
                UpdatePlayerUI();
                UpdateCooldownUI();
            }
            else if (!player.CanAttack())
            {
                
            }
        }

        private void UpgradeDamage_Click(object sender, RoutedEventArgs e)
        {
            if (player.TryUpgrade())
            {
                baseDamage = baseDamage.Multiply(1.2);
                enemyManager.RestoreAllEnemiesHealth();
                enemyManager.LevelUpAllEnemies();

                UpdatePlayerUI();
                UpdateEnemyUI();

                Debug.WriteLine($"After upgrade - BaseDamage: {baseDamage}, Player Level: {player.Level}");
            }
        }

        private void UpgradeCooldown_Click(object sender, RoutedEventArgs e)
        {
            if (player.TryUpgradeCooldown())
            {
                UpdatePlayerUI();
                UpdateCooldownUI();
                
            }
        }

        private void NextEnemy_Click(object sender, RoutedEventArgs e)
        {
            SpawnNewEnemy();
        }

        private void ResetGame_Click(object sender, RoutedEventArgs e)
        {
            gameTimer.Stop();
            bonusSpawnTimer.Stop();
            InitializeGame();
            gameTimer.Start();
            bonusSpawnTimer.Start();
        }

        // Остальные методы остаются без изменений...
        private void LoadIcons() 
        {
            {
                string[] iconNames = { "Sword", "Axe", "Bow", "Staff" };
                Color[] colors = { Colors.Red, Colors.Blue, Colors.Green, Colors.Orange };

                double x = 10;
                double y = 10;

                for (int i = 0; i < iconNames.Length; i++)
                {
                    Image img = new Image
                    {
                        Width = 50,
                        Height = 50,
                        Source = new BitmapImage(new Uri("icons/Monsters")),
                        Tag = iconNames[i]
                    };

                    Rectangle icon = new Rectangle
                    {
                        Width = 50,
                        Height = 50,
                        Fill = new SolidColorBrush(colors[i]),
                        Stroke = Brushes.Black,
                        StrokeThickness = 2,
                        Tag = iconNames[i]
                    };

                    iniicon = new CIconList(50, 50, 4, 2);
                    DisplayIcons();

                    TextBlock text = new TextBlock
                    {
                        Text = iconNames[i],
                        Foreground = Brushes.Black,
                        FontSize = 10,
                        Width = 50,
                        TextAlignment = TextAlignment.Center
                    };

                    Canvas.SetLeft(text, x);
                    Canvas.SetTop(text, y + 55);

                    x += 60;
                }
            }
        }
        public void DisplayIcons()
        {
            var icons = iniicon.GetIcons();
            foreach (var icon in icons)
            {
                Image image = new Image();
                image.Source = new BitmapImage(new Uri(icon.Path));
                image.Tag = icon.Name;
            }
        }
    }
}