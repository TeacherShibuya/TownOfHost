using System;
using System.Linq;
using System.Collections.Generic;
using Hazel;
using UnityEngine;

namespace TownOfHost
{
    public class CustomRoleManager
    {
        #region singleton
        public static CustomRoleManager Instance
        {
            get
            {
                if (_instance == null) Logger.Error("Instance Is Not Exists", "CustomRoleManager");
                return _instance;
            }
        }
        public static bool InstanceExists => _instance != null;
        public static bool TryGetInstance(out CustomRoleManager Instance)
        {
            Instance = _instance;
            return InstanceExists;
        }
        private CustomRoleManager() { }
        public CustomRoleManager CreateInstance()
        {
            if (!InstanceExists) _instance = new CustomRoleManager();
            return _instance;
        }
        public void RemoveInstance()
        {
            _instance = null;
        }
        public static CustomRoleManager _instance;
        #endregion
        public List<RoleBase> RoleInstances;
        public List<RolePlayer> RolePlayers;
        public void InitAllInstance()
        {
        }
        // RoleInstances/Players共通の呼び出し
        public void OnFixedUpdate()
        {
            RoleInstances.ForEach(role => role.OnFixedUpdate());
            RolePlayers.ForEach(rp => rp.OnFixedUpdate());
        }
        // RoleInstances内の関数呼び出し
        public void OnStartGame() => RoleInstances.ForEach(role => role.OnStartGame());
        public void OnStartMeeting() => RoleInstances.ForEach(role => role.OnStartMeeting());
        public void OnEndMeeting() => RoleInstances.ForEach(role => role.OnEndMeeting());

        // RolePlayers内の関数呼び出し
        public void OnReportDeadBody(PlayerControl player, GameData.PlayerInfo target)
            => RolePlayers.Where(rp => rp.player == player).FirstOrDefault()
                    .OnReportDeadBody(target);
    }
}