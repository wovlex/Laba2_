using Microsoft.Win32;
using System;
using System.IO;
using System.Numerics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace Laba2_
{
    public partial class MainWindow : Window
    {
        private Player player;
        private Enemy currentEnemy;
        private EnemyManager enemyManager;
        private BigNumber baseDamage;

        public MainWindow()
        {
            InitializeComponent();
            InitializeGame();
        }

        private void InitializeGame()
        {
            // Инициализация игрока
            player = new Player();
            baseDamage = new BigNumber("10");

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
            testEnemies.AddEnemy(new CEnemyTemplate("Goblin", "goblin.png", 100, 1.1, 50, 1.2, 0.4));
            testEnemies.AddEnemy(new CEnemyTemplate("Skeleton", "skeleton.png", 200, 1.2, 100, 1.3, 0.3));
            testEnemies.AddEnemy(new CEnemyTemplate("Troll", "troll.png", 500, 1.5, 250, 1.5, 0.2));
            testEnemies.AddEnemy(new CEnemyTemplate("Dragon", "dragon.png", 1000, 2.0, 1000, 2.0, 0.1));

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
                    string imagePath = System.IO.Path.Combine(basePath, "icons/Monsters", currentEnemy.IconName);
                    if (File.Exists(imagePath))
                    {
                        enemyImage.Source = new BitmapImage(new Uri(imagePath));
                    }
                }
                catch
                {
                    // Если изображение не найдено, используем placeholder
                    enemyImage.Source = new BitmapImage(new Uri("pack://application:,,,/placeholder.png"));
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
            upgradeButton.IsEnabled = player.CanUpgrade(baseDamage);
        }

        private void UpdateEnemyUI()
        {
            if (currentEnemy != null)
            {
                enemyNameText.Text = currentEnemy.Name;
                enemyHpText.Text = $"{currentEnemy.CurrentHealth} / {currentEnemy.MaxHealth}";
                enemyGoldText.Text = currentEnemy.GoldReward.ToString();
                currentDamageText.Text = baseDamage.ToString();

                // Прогресс HP
                double hpPercent = (double) currentEnemy.CurrentHealth / currentEnemy.MaxHealth;
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
            if (player.TryUpgrade(baseDamage))
            {
                baseDamage = baseDamage.Multiply(new BigNumber("1.2"));
                UpdatePlayerUI();
                UpdateEnemyUI();
                MessageBox.Show("Урон улучшен!");
            }
            else
            {
                MessageBox.Show("Недостаточно золота для улучшения!");
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

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            // Не используется в новой версии
        }
    }
}