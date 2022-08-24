using System.Collections.Generic;
using System.Linq;
using Hazel;
using UnityEngine;

namespace TownOfHost
{
    public static class ToughGuy
    {
        private static readonly int Id = 21000;
        public static List<byte> playerIdList = new();

        private static Dictionary<byte, (PlayerControl, PlayerState.DeathReason)> WillDieAfterMeeting = new();

        public static void SetupCustomOption()
        {
            Options.SetupRoleOptions(Id, CustomRoles.ToughGuy);
        }
        public static void Init()
        {
            playerIdList = new();
            WillDieAfterMeeting = new();
        }
        public static void Add(byte playerId)
        {
            playerIdList.Add(playerId);
            WillDieAfterMeeting.Remove(playerId);
        }
        public static bool IsEnable() => playerIdList.Count > 0;
        public static bool CheckAndGuardKill(PlayerControl killer, PlayerControl target)
        {
            if (WillDieAfterMeeting.TryGetValue(target.PlayerId, out var result)) return false;
            var deathReason = PlayerState.DeathReason.Kill;
            switch (killer.GetCustomRole())
            {
                case CustomRoles.Puppeteer:
                case CustomRoles.Arsonist:
                    return false;
                case CustomRoles.Vampire:
                    deathReason = PlayerState.DeathReason.Bite;
                    break;
                case CustomRoles.Witch:
                    if (killer.IsSpellMode()) return false;
                    break;
                case CustomRoles.Warlock:
                    if (!Main.CheckShapeshift[killer.PlayerId]) return false;
                    break;
                case CustomRoles.Sheriff:
                    if (!Sheriff.MisfireKillsTarget.GetBool()) return false;
                    break;
                default:
                    break;
            }
            killer.RpcGuardAndKill(target);
            WillDieAfterMeeting.Add(target.PlayerId, (killer, deathReason));
            Logger.Info($"{Utils.GetNameWithRole(target.PlayerId)}が{Utils.GetNameWithRole(killer.PlayerId)}に{deathReason}されて負傷", "WillDieAfterMeeting");
            Utils.NotifyRoles();
            Utils.CustomSyncAllSettings();
            return true;
        }
        public static void AfterMeetingDeath(byte playerId)
        {
            if (WillDieAfterMeeting.ContainsKey(playerId))
            {
                var deathReason = WillDieAfterMeeting[playerId].Item2;
                Main.AfterMeetingDeathPlayers.TryAdd(playerId, deathReason);
                Logger.Info($"{Utils.GetNameWithRole(playerId)}が{deathReason}で死亡", "ToughGuy");
                WillDieAfterMeeting.Remove(playerId);
            }
        }
        public static string GetMark(PlayerControl seer, PlayerControl target)
        {
            return Helpers.ColorString(Utils.GetRoleColor(CustomRoles.ToughGuy),
                (WillDieAfterMeeting.ContainsKey(target.PlayerId) && (seer.Data.IsDead
                || seer == WillDieAfterMeeting[target.PlayerId].Item1)) ? "×" : "");
        }
    }
}