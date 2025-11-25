namespace Laba2_
{
    public class EnemyManager
    {
        private List<Enemy>? enemies;
        private List<CEnemyTemplate> enemyTemplates;
        private Random random;

        public EnemyManager()
        {
            random = new Random();
            enemies = new List<Enemy>();
            enemyTemplates = new List<CEnemyTemplate>();
        }

        public void SetEnemies(List<CEnemyTemplate> templates)
        {
            if (templates != null)
            {
                enemyTemplates = templates;
                CreateNewEnemies();
            }
        }

        private void CreateNewEnemies()
        {
            enemies = new List<Enemy>();
            foreach (var template in enemyTemplates)
            {
                enemies.Add(new Enemy(template));
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

        // Увеличение уровня всех врагов
        public void LevelUpAllEnemies()
        {
            if (enemies != null)
            {
                foreach (var enemy in enemies)
                {
                    enemy.LevelUp();
                }
            }
        }

        // Восстановление здоровья всех противников
        public void RestoreAllEnemiesHealth()
        {
            if (enemies != null)
            {
                foreach (var enemy in enemies)
                {
                    enemy.RestoreHealth();
                }
            }
        }

        // Создать новых врагов (сбросить уровни)
        public void CreateNewEnemiesFromTemplates()
        {
            CreateNewEnemies();
        }

        // Получить список всех врагов
        public List<Enemy>? GetAllEnemies()
        {
            return enemies;
        }
    }
}