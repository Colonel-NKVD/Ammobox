using Rocket.Core.Plugins;
using UnityEngine;
using System.Collections.Generic;
using Logger = Rocket.Core.Logging.Logger;
using SDG.Unturned;

public class AmmoBoxPlugin : RocketPlugin<AmmoBoxConfig>
{
    public static AmmoBoxPlugin Instance;
    
    // Храним оставшийся ресурс для каждого активного ящика. 
    // Ключ - Transform баррикады, Значение - оставшийся ресурс
    public Dictionary<Transform, int> BoxResources = new Dictionary<Transform, int>();

    protected override void Load()
    {
        Instance = this;
        Logger.Log("AmmoBox загружен.");
    }

    protected override void Unload()
    {
        BoxResources.Clear();
        Instance = null;
    }

    // Метод для получения ближайшего ящика нужного типа
    public BarricadeDrop GetNearestBox(Player player, ushort boxId, float radius)
    {
        BarricadeDrop nearestDrop = null;
        float minDistanceSqr = radius * radius;

        List<BarricadeRegion> regions = new List<BarricadeRegion>();
        BarricadeManager.getRegionsInRadius(player.transform.position, radius, regions);

        foreach (var region in regions)
        {
            foreach (var drop in region.drops)
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

    // Уничтожение ящика, когда ресурс иссяк
    public void DestroyBox(BarricadeDrop drop)
    {
        if (BarricadeManager.tryGetInfo(drop.model, out byte x, out byte y, out ushort plant, out ushort index, out BarricadeRegion region))
        {
            BarricadeManager.destroyBarricade(region, x, y, plant, index);
        }
    }
}
