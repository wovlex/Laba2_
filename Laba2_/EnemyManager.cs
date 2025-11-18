using System;
using System.Collections.Generic;

namespace Laba2_
{
    public class EnemyManager
    {
        private List<Enemy>? enemies;
        private Random random;

        public EnemyManager()
        {
            random = new Random();
            enemies = new List<Enemy>();
        }

        public void SetEnemies(List<CEnemyTemplate> enemyTemplates)
        {
            if (enemyTemplates != null)
            {
                enemies = new List<Enemy>();
                foreach (var template in enemyTemplates)
                {
                    enemies.Add(new Enemy(template));
                }
            }
        }

        public void LoadEnemies(string filePath)
        {
            var templateList = new CEnemyTemplateList();
            templateList.LoadFromFile(filePath);
            SetEnemies(templateList.GetEnemies());
        }

        public Enemy? GetRandomEnemy()
        {
            if (enemies == null || enemies.Count == 0)
                return null;

            int randomIndex = random.Next(0, enemies.Count);
            return enemies[randomIndex];
        }
    }
}