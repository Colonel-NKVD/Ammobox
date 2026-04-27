using Rocket.API;
using Rocket.Unturned.Player;
using SDG.Unturned;
using System.Collections.Generic;
using UnityEngine;

public class CommandGetMed : IRocketCommand
{
    public AllowedCaller AllowedCaller => AllowedCaller.Player;
    public string Name => "getmed";
    public string Help => "Получить медикаменты у мед. ящика";
    public string Syntax => "/getmed";
    public List<string> Aliases => new List<string>();
    public List<string> Permissions => new List<string> { "ammobox.getmed" };

    public void Execute(IRocketPlayer caller, string[] command)
    {
        UnturnedPlayer player = (UnturnedPlayer)caller;
        var plugin = AmmoBoxPlugin.Instance;
        var config = plugin.Configuration.Instance;

        BarricadeDrop nearestBox = plugin.GetNearestBox(player.Player, config.MedBoxId, config.AuraRadius);

        if (nearestBox == null)
        {
            Rocket.Unturned.Chat.UnturnedChat.Say(caller, "Рядом нет медицинского ящика!", Color.red);
            return;
        }

        int bandageCount = GetItemCount(player.Player, config.BandageId);
        int morphineCount = GetItemCount(player.Player, config.MorphineId);

        int bandagesToGive = 0;
        if (bandageCount == 0) bandagesToGive = 4;
        else if (bandageCount == 1) bandagesToGive = 3;
        else if (bandageCount == 2) bandagesToGive = 2;
        else if (bandageCount == 3) bandagesToGive = 1;

        int morphineToGive = (morphineCount == 0) ? 1 : 0;

        if (bandagesToGive == 0 && morphineToGive == 0)
        {
            Rocket.Unturned.Chat.UnturnedChat.Say(caller, "Ваша аптечка полна, пополнять нечего!", Color.yellow);
            return; 
        }

        if (!plugin.BoxResources.ContainsKey(nearestBox.model))
            plugin.BoxResources[nearestBox.model] = config.MedBoxMaxResources;

        int currentResources = plugin.BoxResources[nearestBox.model];
        bool inventoryFullWarning = false;

        for (int i = 0; i < bandagesToGive; i++)
            GiveItemSafely(player.Player, config.BandageId, ref inventoryFullWarning);

        if (morphineToGive > 0)
            GiveItemSafely(player.Player, config.MorphineId, ref inventoryFullWarning);

        currentResources--;
        plugin.BoxResources[nearestBox.model] = currentResources;

        Rocket.Unturned.Chat.UnturnedChat.Say(caller, $"Медикаменты получены. Ресурс ящика: {currentResources}", Color.green);

        if (inventoryFullWarning)
            Rocket.Unturned.Chat.UnturnedChat.Say(caller, "ВНИМАНИЕ: Некоторые медикаменты выпали на землю!", Color.red);

        if (currentResources <= 0)
        {
            plugin.DestroyBox(nearestBox);
            plugin.BoxResources.Remove(nearestBox.model);
            Rocket.Unturned.Chat.UnturnedChat.Say(caller, "Медицинский ящик исчерпан.", Color.yellow);
        }
    }

    private int GetItemCount(Player player, ushort itemId)
    {
        int count = 0;
        for (byte page = 0; page < PlayerInventory.PAGES - 2; page++)
        {
            if (player.inventory.items[page] == null) continue;
            for (byte i = 0; i < player.inventory.items[page].getItemCount(); i++)
            {
                if (player.inventory.items[page].getItem(i).item.id == itemId)
                    count++;
            }
        }
        return count;
    }

    private void GiveItemSafely(Player player, ushort itemId, ref bool warned)
    {
        Item item = new Item(itemId, EItemOrigin.ADMIN);
        if (!player.inventory.tryAddItem(item, true))
        {
            ItemManager.dropItem(item, player.transform.position, false, true, true);
            warned = true;
        }
    }
}
