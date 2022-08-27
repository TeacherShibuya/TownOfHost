using System.Collections.Generic;
using Hazel;
using UnityEngine;

namespace TownOfHost
{
    public static class SPImpostor
    {
        static readonly int Id = 3500;
        static List<byte> playerIdList = new();
        public static CustomOption RoleType;
        private static CustomOption OverrideShapeShifterOptions;
        private static CustomOption ShapeShiftCoolDown;
        private static CustomOption ShapeShiftDuration;
        private static CustomOption LeaveShapeShiftingEvidence;
        private static CustomOption OverrideDefaultOptions;
        private static CustomOption ButtonCount;
        private static CustomOption EmergencyCooldown;
        private static CustomOption IgnoreAnonymousVotes;
        private static CustomOption Vision;
        private static CustomOption HasImpostorVision;
        private static CustomOption KillCooldown;
        private static CustomOption KillDistance;
        private static CustomOption AdvancedOptions;
        private static CustomOption CanSabotage;
        private static CustomOption CanUseVent;
        private static CustomOption CanReportDeadBody;

        private static readonly string[] RoleTypes =
        {
            CustomRoles.Impostor.ToString(), CustomRoles.Shapeshifter.ToString()
        };
        private static readonly string[] KillDistances =
        {
            "KillDistanceShort", "KillDistanceMedium", "KillDistanceLong"
        };

        public static void SetupCustomOption()
        {
            Options.SetupRoleOptions(Id, CustomRoles.SPImpostor);
            RoleType = CustomOption.Create(Id + 10, Color.white, "SPIRoleType", RoleTypes, RoleTypes[0], Options.CustomRoleSpawnChances[CustomRoles.SPImpostor]);
            OverrideShapeShifterOptions = CustomOption.Create(Id + 11, Color.white, "SPIOverrideShapeShifterOptions", false, RoleType);
            ShapeShiftCoolDown = CustomOption.Create(Id + 12, Color.white, "SPIShapeShiftCoolDown", 10f, 5f, 180f, 2.5f, OverrideShapeShifterOptions);
            ShapeShiftDuration = CustomOption.Create(Id + 13, Color.white, "SPIShapeShiftDuration", 30f, 0f, 180f, 2.5f, OverrideShapeShifterOptions);
            LeaveShapeShiftingEvidence = CustomOption.Create(Id + 14, Color.white, "SPILeaveShapeShiftingEvidence", false, OverrideShapeShifterOptions);
            OverrideDefaultOptions = CustomOption.Create(Id + 15, Color.white, "SPOverrideDefaultOptions", false, Options.CustomRoleSpawnChances[CustomRoles.SPImpostor]);
            ButtonCount = CustomOption.Create(Id + 16, Color.white, "SPButtonCount", 1, 0, 9, 1, OverrideDefaultOptions);
            EmergencyCooldown = CustomOption.Create(Id + 17, Color.white, "SPEmergencyCooldown", 20, 0, 60, 1, OverrideDefaultOptions);
            IgnoreAnonymousVotes = CustomOption.Create(Id + 18, Color.white, "SPIgnoreAnonymousVotes", false, OverrideDefaultOptions);
            Vision = CustomOption.Create(Id + 19, Color.white, "SPVision", 1.5f, 0f, 5f, 0.25f, OverrideDefaultOptions);
            HasImpostorVision = CustomOption.Create(Id + 20, Color.white, "SPHasImpostorVision", true, OverrideDefaultOptions);
            KillCooldown = CustomOption.Create(Id + 21, Color.white, "SPIKillCooldown", 30, 0, 180, 1, OverrideDefaultOptions);
            KillDistance = CustomOption.Create(Id + 22, Color.white, "SPIKillDistance", KillDistances, KillDistances[1], OverrideDefaultOptions);
            AdvancedOptions = CustomOption.Create(Id + 23, Color.white, "SPAdvancedOptions", false, Options.CustomRoleSpawnChances[CustomRoles.SPImpostor]);
            CanSabotage = CustomOption.Create(Id + 24, Color.white, "SPICanSabotage", true, AdvancedOptions);
            CanUseVent = CustomOption.Create(Id + 25, Color.white, "SPICanUseVent", true, AdvancedOptions);
            CanReportDeadBody = CustomOption.Create(Id + 26, Color.white, "SPCanReportDeadBody", true, AdvancedOptions);
        }
        public static void Init()
        {
            playerIdList = new();
        }
        public static void Add(PlayerControl pc)
        {
            playerIdList.Add(pc.PlayerId);
            if (OverrideDefaultOptions.GetBool())
                Main.AllPlayerNumEmergencyMeetings.Add(pc.PlayerId, ButtonCount.GetInt());
        }
        public static bool IsEnable() => playerIdList.Count > 0;
        public static void SetKillCooldown(byte id) => Main.AllPlayerKillCooldown[id] = KillCooldown.GetFloat();
        public static void ApplyGameOptions(GameOptionsData opt, PlayerControl player)
        {
            if (RoleType.GetSelection() == 1 && OverrideShapeShifterOptions.GetBool())
            {
                opt.RoleOptions.ShapeshifterCooldown = ShapeShiftCoolDown.GetFloat();
                opt.RoleOptions.ShapeshifterDuration = ShapeShiftDuration.GetFloat();
                opt.RoleOptions.ShapeshifterLeaveSkin = LeaveShapeShiftingEvidence.GetBool();
            }
            if (OverrideDefaultOptions.GetBool())
            {
                opt.EmergencyCooldown = EmergencyCooldown.GetInt();
                if (IgnoreAnonymousVotes.GetBool()) opt.AnonymousVotes = false;
                opt.ImpostorLightMod = Vision.GetFloat();
                opt.CrewLightMod = Vision.GetFloat();
                opt.SetVision(player, HasImpostorVision.GetBool());
                Main.AllPlayerKillDistance[player.PlayerId] = KillDistance.GetSelection();
            }
        }
        public static bool DisableSabotage(PlayerControl player) => player.Is(CustomRoles.SPImpostor) && AdvancedOptions.GetBool() && !CanSabotage.GetBool();
        public static bool CanVent(PlayerControl player) => player.Is(CustomRoles.SPImpostor) && (!AdvancedOptions.GetBool() || CanUseVent.GetBool());
        public static bool DisableReportDeadBody(PlayerControl player, GameData.PlayerInfo target) => target != null && player.Is(CustomRoles.SPImpostor) && AdvancedOptions.GetBool() && !CanReportDeadBody.GetBool();
    }
}