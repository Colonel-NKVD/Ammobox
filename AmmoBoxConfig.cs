using Rocket.API;
using System.Collections.Generic;

public class AmmoBoxConfig : IRocketPluginConfiguration
{
    public float AuraRadius;

    public ushort AmmoBoxId;
    public int AmmoBoxMaxResources;
    // Словарь: ID магазина -> Стоимость пополнения
    public Dictionary<ushort, int> MagazineCosts;

    public ushort MedBoxId;
    public int MedBoxMaxResources;
    public ushort BandageId;
    public ushort MorphineId;

    public void LoadDefaults()
    {
        AuraRadius = 10f;
        
        AmmoBoxId = 1234;
        AmmoBoxMaxResources = 100; 

        // Примеры цен по вашему запросу
        MagazineCosts = new Dictionary<ushort, int>
        {
            { 1394, 5 }, // Допустим, пулеметная лента
            { 123, 2 },  // Допустим, магазин Шмайсера
            { 17, 1 }    // Обычный магазин
        };

        MedBoxId = 1235;
        MedBoxMaxResources = 20;
        BandageId = 393;
        MorphineId = 395;
    }
}
