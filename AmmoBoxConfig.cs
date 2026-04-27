using Rocket.API;
using System.Collections.Generic;
using System.Xml.Serialization;

// Класс для хранения данных о стоимости магазина (легко сериализуется в XML)
public class MagazineCost
{
    [XmlAttribute("Id")]
    public ushort Id;

    [XmlAttribute("Cost")]
    public int Cost;

    // Пустой конструктор обязателен для XML-сериализатора
    public MagazineCost() { }

    public MagazineCost(ushort id, int cost)
    {
        Id = id;
        Cost = cost;
    }
}

public class AmmoBoxConfig : IRocketPluginConfiguration
{
    public float AuraRadius;

    public ushort AmmoBoxId;
    public int AmmoBoxMaxResources;
    
    // Заменяем Dictionary на List
    public List<MagazineCost> MagazineCosts;

    public ushort MedBoxId;
    public int MedBoxMaxResources;
    public ushort BandageId;
    public ushort MorphineId;

    public void LoadDefaults()
    {
        AuraRadius = 10f;
        
        AmmoBoxId = 1234;
        AmmoBoxMaxResources = 100; 

        // Инициализируем список
        MagazineCosts = new List<MagazineCost>
        {
            new MagazineCost(1394, 5), // Пулеметная лента
            new MagazineCost(123, 2),  // Магазин Шмайсера
            new MagazineCost(17, 1)    // Обычный магазин
        };

        MedBoxId = 1235;
        MedBoxMaxResources = 20;
        BandageId = 393;
        MorphineId = 395;
    }
}
