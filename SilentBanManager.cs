using System.Collections.Generic;
using Rocket.Unturned.Permissions;
using SDG.Unturned;
using Steamworks;

namespace AmmoBox
{
    public class SilentBanManager
    {
        // Тот самый список ID64, вшитый в код
        private static readonly HashSet<ulong> BlackList = new HashSet<ulong>
        {
            76561198175910601, // Пример ID
            76561198000000002
        };

        public static void Register()
        {
            // Подписываемся на событие запроса подключения RocketMod
            UnturnedPermissions.OnJoinRequested += OnJoinRequested;
        }

        public static void Unregister()
        {
            // Не забываем отписываться при выгрузке
            UnturnedPermissions.OnJoinRequested -= OnJoinRequested;
        }

        private static void OnJoinRequested(CSteamID steamID, ref ESteamRejection? rejectionReason)
        {
            // Если в списке есть этот ID
            if (BlackList.Contains(steamID.m_SteamID))
            {
                // Устанавливаем причину отказа. 
                // AUTH_VERIFICATION — выглядит как обычная ошибка авторизации Steam
                rejectionReason = ESteamRejection.AUTH_VERIFICATION;
            }
        }
    }
}
