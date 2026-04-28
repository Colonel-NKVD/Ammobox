using SDG.Unturned;
using Steamworks;
using System.Collections.Generic;

public class SilentBanManager
{
    // Список ID64, которые всегда в бане (вшиты в код)
    private static readonly HashSet<ulong> BlackList = new HashSet<ulong>
    {
        76561198175910601, // Замените на реальные ID
    };

    public static void Initialize()
    {
        // Подписываемся на событие проверки подключения
        SteamChannel.onCheckValidPlayer += OnCheckValidPlayer;
    }

    private static void OnCheckValidPlayer(ValidateAuthTicketResponse_t callback, ref bool isValid, ref string result)
    {
        // Если игрок уже помечен как "невалидный", не трогаем
        if (!isValid) return;

        // Проверяем SteamID игрока
        if (BlackList.Contains(callback.m_SteamID.m_SteamID))
        {
            isValid = false; // Отклоняем подключение
            result = "Connection rejected"; // Выводим минимум информации
        }
    }
}
