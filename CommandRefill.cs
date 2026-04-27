using Rocket.API;
using Rocket.Unturned.Player;
using SDG.Unturned;
using System.Collections.Generic;
using UnityEngine;

public class CommandRefill : IRocketCommand
{
    public AllowedCaller AllowedCaller => AllowedCaller.Player;
    public string Name => "refill";
    public string Help => "Пополняет магазины у ящика с патронами";
    public string Syntax => "/refill";
    public List<string> Aliases => new List<string>();
    public List<string> Permissions => new List<string> { "ammobox.refill" };

    public void Execute(IRocketPlayer caller, string[] command)
    {
        UnturnedPlayer player = (UnturnedPlayer)caller;
        var plugin = AmmoBoxPlugin.Instance;
        var config = plugin.Configuration.Instance;

        BarricadeDrop nearestBox = plugin.GetNearestBox(player.Player, config.AmmoBoxId, config.AuraRadius);

        if (nearestBox == null)
        {
            Rocket.Unturned.Chat.UnturnedChat.Say(caller, "Рядом нет ящика с боеприпасами!", Color.red);
            return;
        }

        if (!plugin.BoxResources.ContainsKey(nearestBox.model))
            plugin.BoxResources[nearestBox.model] = config.AmmoBoxMaxResources;

        int currentResources = plugin.BoxResources[nearestBox.model];
        int refilledCount = 0;

        for (byte page = 0; page < PlayerInventory.PAGES - 2; page++)
        {
            var region = player.Player.inventory.items[page];
            if (region == null) continue;

            for (byte i = 0; i < region.getItemCount(); i++)
            {
                ItemJar jar = region.getItem(i);
                
                if (config.MagazineCosts.TryGetValue(jar.item.id, out int cost))
                {
                    ItemAsset asset = (ItemAsset)Assets.find(EAssetType.ITEM, jar.item.id);
                    if (asset != null && jar.item.amount < asset.amount)
                    {
                        if (currentResources >= cost)
                        {
                            jar.item.state = asset.getState(EItemOrigin.ADMIN);
                            jar.item.amount = asset.amount;
                            player.Player.inventory.sendUpdateState(page, jar.x, jar.y, jar.item.state);
                            player.Player.inventory.sendUpdateAmount(page, jar.x, jar.y, jar.item.amount);

                            currentResources -= cost;
                            refilledCount++;
                        }
                        else
                        {
                            Rocket.Unturned.Chat.UnturnedChat.Say(caller, "В ящике недостаточно ресурсов для этого БК!", Color.yellow);
                            goto EndRefill; 
                        }
                    }
                }
            }
        }

    EndRefill:
        if (refilledCount > 0)
        {
            plugin.BoxResources[nearestBox.model] = currentResources;
            Rocket.Unturned.Chat.UnturnedChat.Say(caller, $"Боезапас пополнен. Ресурс ящика: {currentResources}", Color.green);

            if (currentResources <= 0)
            {
                plugin.DestroyBox(nearestBox);
                plugin.BoxResources.Remove(nearestBox.model);
                Rocket.Unturned.Chat.UnturnedChat.Say(caller, "Ящик с боеприпасами полностью опустел.", Color.yellow);
            }
        }
        else
        {
            Rocket.Unturned.Chat.UnturnedChat.Say(caller, "Нет подходящих магазинов для пополнения.", Color.yellow);
        }
    }
}
