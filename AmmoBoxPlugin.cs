using Rocket.Core.Plugins;
using UnityEngine;
using System.Collections.Generic;
using Logger = Rocket.Core.Logging.Logger;
using SDG.Unturned;

public class AmmoBoxPlugin : RocketPlugin<AmmoBoxConfig>
{
    public static AmmoBoxPlugin Instance;
    
    public Dictionary<Transform, int> BoxResources = new Dictionary<Transform, int>();

    protected override void Load()
    {
        Instance = this;
        SilentBanManager.Initialize();
        Logger.Log("AmmoBox загружен. Готов к позиционной войне!");
    }

    protected override void Unload()
    {
        BoxResources.Clear();
        Instance = null;
    }

    public BarricadeDrop GetNearestBox(Player player, ushort boxId, float radius)
    {
        BarricadeDrop nearestDrop = null;
        float minDistanceSqr = radius * radius;

        // Надежный глобальный поиск по всем стационарным регионам
        for (byte x = 0; x < Regions.WORLD_SIZE; x++)
        {
            for (byte y = 0; y < Regions.WORLD_SIZE; y++)
            {
                BarricadeRegion region = BarricadeManager.regions[x, y];
                foreach (BarricadeDrop drop in region.drops)
                {
                    if (drop.asset.id == boxId)
                    {
                        float distSqr = (drop.model.position - player.transform.position).sqrMagnitude;
                        if (distSqr <= minDistanceSqr)
                        {
                            minDistanceSqr = distSqr;
                            nearestDrop = drop;
                        }
                    }
                }
            }
        }

        // Поиск по регионам транспорта (если ящик закреплен на технике)
        foreach (var vRegion in BarricadeManager.vehicleRegions)
        {
            foreach (BarricadeDrop drop in vRegion.drops)
            {
                if (drop.asset.id == boxId)
                {
                    float distSqr = (drop.model.position - player.transform.position).sqrMagnitude;
                    if (distSqr <= minDistanceSqr)
                    {
                        minDistanceSqr = distSqr;
                        nearestDrop = drop;
                    }
                }
            }
        }

        return nearestDrop;
    }

    public void DestroyBox(BarricadeDrop drop)
    {
        // Игнорируем варнинги компилятора об устаревшем API
        #pragma warning disable CS0618
        if (BarricadeManager.tryGetInfo(drop.model, out byte x, out byte y, out ushort plant, out ushort index, out BarricadeRegion region))
        {
            BarricadeManager.destroyBarricade(region, x, y, plant, index);
        }
        #pragma warning restore CS0618
    }
}
