using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Linq;

namespace Laba2_
{
    public class EnemyManager
    {
        private List<CEnemyTemplate> enemyTemplates;
        private Random random;

        public EnemyManager()
        {
            enemyTemplates = new List<CEnemyTemplate>();
            random = new Random();
        }

        public void LoadEnemies(string filePath)
        {
            try
            {
                string json = File.ReadAllText(filePath);
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                enemyTemplates = JsonSerializer.Deserialize<List<CEnemyTemplate>>(json, options);
                NormalizeChances();
            }
            catch (Exception ex)
            {
                throw new Exception($"Ошибка загрузки врагов: {ex.Message}");
            }
        }

        public void SetEnemies(List<CEnemyTemplate> enemies)
        {
            enemyTemplates = enemies;
            NormalizeChances();
        }

        private void NormalizeChances()
        {
            double sum = enemyTemplates.Sum(e => e.SpawnChance);
            foreach (var enemy in enemyTemplates)
            {
                enemy.SpawnChance /= sum;
            }
        }

        public Enemy GetRandomEnemy()
        {
            if (enemyTemplates.Count == 0) return null;

            double chance = random.NextDouble();
            double sum = 0;

            foreach (var template in enemyTemplates)
            {
                sum += template.SpawnChance;
                if (sum >= chance)
                {
                    return new Enemy(template);
                }
            }

            return new Enemy(enemyTemplates[0]);
        }
    }
}