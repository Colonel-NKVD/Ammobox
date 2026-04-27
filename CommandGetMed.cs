public void Execute(IRocketPlayer caller, string[] command)
{
    UnturnedPlayer player = (UnturnedPlayer)caller;
    var plugin = AmmoBoxPlugin.Instance;
    var config = plugin.Configuration.Instance;

    BarricadeDrop nearestBox = plugin.GetNearestBox(player.Player, config.MedBoxId, config.AuraRadius);

    if (nearestBox == null)
    {
        Rocket.Unturned.Chat.UnturnedChat.Say(caller, "Рядом нет медицинского ящика!", UnityEngine.Color.red);
        return;
    }

    // Подсчет необходимых предметов
    int bandageCount = GetItemCount(player.Player, config.BandageId);
    int morphineCount = GetItemCount(player.Player, config.MorphineId);

    int bandagesToGive = 0;
    if (bandageCount == 0) bandagesToGive = 4;
    else if (bandageCount == 1) bandagesToGive = 3;
    else if (bandageCount == 2) bandagesToGive = 2;
    else if (bandageCount == 3) bandagesToGive = 1;

    int morphineToGive = (morphineCount == 0) ? 1 : 0;

    // КРИТИЧЕСКАЯ ПРОВЕРКА: Если пополнять нечего
    if (bandagesToGive == 0 && morphineToGive == 0)
    {
        Rocket.Unturned.Chat.UnturnedChat.Say(caller, "Ваша аптечка полностью укомплектована, пополнять нечего!", UnityEngine.Color.yellow);
        return; // Ресурс ящика НЕ ТРАТИТСЯ
    }

    // Если мы дошли сюда, значит хотя бы один предмет будет выдан
    if (!plugin.BoxResources.ContainsKey(nearestBox.model))
        plugin.BoxResources[nearestBox.model] = config.MedBoxMaxResources;

    int currentResources = plugin.BoxResources[nearestBox.model];
    bool inventoryFullWarning = false;

    // Выдача
    for (int i = 0; i < bandagesToGive; i++)
        GiveItemSafely(player.Player, config.BandageId, ref inventoryFullWarning);

    if (morphineToGive > 0)
        GiveItemSafely(player.Player, config.MorphineId, ref inventoryFullWarning);

    // Тратим 1 единицу ресурса ящика за использование
    currentResources--;
    plugin.BoxResources[nearestBox.model] = currentResources;

    Rocket.Unturned.Chat.UnturnedChat.Say(caller, $"Медикаменты получены. Ресурс ящика: {currentResources}", UnityEngine.Color.green);

    if (inventoryFullWarning)
        Rocket.Unturned.Chat.UnturnedChat.Say(caller, "ВНИМАНИЕ: Некоторые медикаменты выпали на землю!", UnityEngine.Color.red);

    if (currentResources <= 0)
    {
        plugin.DestroyBox(nearestBox);
        plugin.BoxResources.Remove(nearestBox.model);
        Rocket.Unturned.Chat.UnturnedChat.Say(caller, "Медицинский ящик исчерпан.", UnityEngine.Color.yellow);
    }
}
