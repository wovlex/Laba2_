using System.Text.Json;
using System.Text.Json.Serialization;
using System.IO;

namespace Laba2_
{
    public class CEnemyTemplate
    {
        [JsonInclude]
        public string Name { get; set; } = string.Empty;

        [JsonInclude]
        public string IconName { get; set; } = string.Empty;

        [JsonInclude]
        public int BaseLife { get; set; }

        [JsonInclude]
        public double LifeModifier { get; set; }

        [JsonInclude]
        public int BaseGold { get; set; }

        [JsonInclude]
        public double GoldModifier { get; set; }

        [JsonInclude]
        public double SpawnChance { get; set; }

        public CEnemyTemplate() { }

        public CEnemyTemplate(string name, string iconName, int baseLife, double lifeModifier,
                            int baseGold, double goldModifier, double spawnChance)
        {
            Name = name;
            IconName = iconName;
            BaseLife = baseLife;
            LifeModifier = lifeModifier;
            BaseGold = baseGold;
            GoldModifier = goldModifier;
            SpawnChance = spawnChance;
        }
    }

    public class CEnemyTemplateList
    {
        private List<CEnemyTemplate> enemies;

        public CEnemyTemplateList()
        {
            enemies = new List<CEnemyTemplate>();
        }

        public void AddEnemy(CEnemyTemplate enemy)
        {
            enemies.Add(enemy);
        }

        public void RemoveEnemy(CEnemyTemplate enemy)
        {
            enemies.Remove(enemy);
        }

        public List<CEnemyTemplate> GetEnemies()
        {
            return enemies;
        }

        public void SaveToFile(string filePath)
        {
            try
            {
                string jsonString = JsonSerializer.Serialize(enemies, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(filePath, jsonString);
            }
            catch (Exception ex)
            {
                throw new Exception($"Ошибка сохранения: {ex.Message}");
            }
        }

        public void LoadFromFile(string filePath)
        {
            try
            {
                string jsonFromFile = File.ReadAllText(filePath);
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var loadedEnemies = JsonSerializer.Deserialize<List<CEnemyTemplate>>(jsonFromFile, options);
                enemies.Clear();
                if (loadedEnemies != null)
                {
                    enemies.AddRange(loadedEnemies);
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Ошибка загрузки: {ex.Message}");
            }
        }
    }
}