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

namespace Laba2_
{
    public partial class MainWindow : Window
    {
        private Player player;
        private Enemy currentEnemy;
        private EnemyManager enemyManager;
        private BigNumber baseDamage;
        private CIconList iniicon;

        public MainWindow()
        {
            InitializeComponent();
            InitializeGame();
        }

        private void InitializeGame()
        {
            // Инициализация игрока
            player = new Player();
            baseDamage = new BigNumber("50");

            // Загрузка врагов из JSON
            enemyManager = new EnemyManager();
            LoadEnemies();

            // Обновление интерфейса
            UpdatePlayerUI();
            SpawnNewEnemy();
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
                    // Создаем тестовых врагов если файла нет
                    CreateTestEnemies();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки врагов: {ex.Message}");
                CreateTestEnemies();
            }
        }

        private void CreateTestEnemies()
        {
            var testEnemies = new CEnemyTemplateList();
            testEnemies.AddEnemy(new CEnemyTemplate("Goblin", "goblin_1.png", 100, 1.1, 50, 1.2, 0.4));
            testEnemies.AddEnemy(new CEnemyTemplate("Skeleton", "skeleton_1.png", 200, 1.2, 100, 1.3, 0.3));
            testEnemies.AddEnemy(new CEnemyTemplate("Troll", "troll_1.png", 500, 1.5, 250, 1.5, 0.2));
            testEnemies.AddEnemy(new CEnemyTemplate("Dragon", "goblin_5.png", 1000, 2.0, 1000, 2.0, 0.1));
            testEnemies.AddEnemy(new CEnemyTemplate("Dragon", "goblin_7.png", 1500, 2.0, 1000, 2.0, 0.3));
            testEnemies.AddEnemy(new CEnemyTemplate("Dragon", "goblin_8.png", 2000, 3.0, 1000, 3.0, 0.4));

            enemyManager.SetEnemies(testEnemies.GetEnemies());
        }

        private void SpawnNewEnemy()
        {
            currentEnemy = enemyManager.GetRandomEnemy();

            if (currentEnemy != null)
            {
                UpdateEnemyUI();

                // Загрузка изображения врага
                try
                {
                    string basePath = AppDomain.CurrentDomain.BaseDirectory;
                    string imagePath = System.IO.Path.Combine(basePath, @"icons\Monsters", currentEnemy.IconName);
                    if (File.Exists(imagePath))
                    {
                        enemyImage.Source = new BitmapImage(new Uri(imagePath));
                    }
                    else
                    {
                        // Если файл не найден, показываем сообщение
                        MessageBox.Show($"Изображение не найдено: {imagePath}");
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

            // Проверяем, может ли игрок улучшить урон
            upgradeButton.IsEnabled = player.CanUpgrade();

            // Отладочная информация
            Debug.WriteLine($"Player UI - Gold: {player.Gold}, Damage: {baseDamage}, Level: {player.Level}");
        }

        private void UpdateEnemyUI()
        {
            if (currentEnemy != null && baseDamage != null)
            {
                enemyNameText.Text = $"{currentEnemy.Name} (Ур. {currentEnemy.GetLevel()})";
                enemyHpText.Text = $"{currentEnemy.CurrentHealth}/{currentEnemy.MaxHealth}";

                // Используем CurrentGoldReward вместо GoldReward
                enemyGoldText.Text = $"{currentEnemy.CurrentGoldReward} (x{currentEnemy.GoldModifier:F2})";
                currentDamageText.Text = baseDamage.ToString();

                // Прогресс HP
                double currentHP = currentEnemy.CurrentHealth.ToDouble();
                double maxHP = currentEnemy.MaxHealth.ToDouble();
                double hpPercent = maxHP > 0 ? currentHP / maxHP : 0;

                enemyHpProgress.Text = $"HP: {currentEnemy.CurrentHealth}/{currentEnemy.MaxHealth} ({(hpPercent * 100):F1}%)";
            }
        }

        private void EnemyImage_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (currentEnemy != null)
            {
                BigNumber reward;
                bool isDefeated = currentEnemy.TakeDamage(baseDamage, out reward);

                if (isDefeated)
                {
                    player.AddGold(reward);
                    MessageBox.Show($"Победа! Получено {reward} золота!");
                    SpawnNewEnemy();
                }

                UpdateEnemyUI();
                UpdatePlayerUI();
            }
        }

        private void UpgradeDamage_Click(object sender, RoutedEventArgs e)
        {
            if (player.TryUpgrade())
            {
                // Увеличиваем урон игрока - используем умножение на double
                baseDamage = baseDamage.Multiply(1.2);

                // Восстанавливаем здоровье ВСЕХ противников
                enemyManager.RestoreAllEnemiesHealth();




                // Увеличиваем уровень ВСЕХ врагов
                enemyManager.LevelUpAllEnemies();

                // Обновляем интерфейс
                UpdatePlayerUI();
                UpdateEnemyUI();

                // Отладочная информация
                Debug.WriteLine($"After upgrade - BaseDamage: {baseDamage}, Player Level: {player.Level}");
            }
        }

        private void NextEnemy_Click(object sender, RoutedEventArgs e)
        {
            SpawnNewEnemy();
        }

        private void ResetGame_Click(object sender, RoutedEventArgs e)
        {
            InitializeGame();
        }

     
        private void LoadIcons()
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