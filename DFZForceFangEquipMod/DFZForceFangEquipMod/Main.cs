using Game;
using MelonLoader;
using HarmonyLib;
using System.Collections.Generic;
using UnityEngine;
using Game.FieldAction;
using static MelonLoader.MelonLogger;
using Dfz.Ui;
using GameLog;
using Game.Specials;

namespace DFZForceFangEquipMod
{
    public static class BuildInfo
    {
        public const string Name = "DragonFangZ Force Fang Equipment Mod";
        public const string Description = null;
        public const string Author = "yagamuu";
        public const string Company = null;
        public const string Version = "1.0.3";
        public const string DownloadLink = null;
    }

    public class Main : MelonMod
    {
        // public static DFZForceFangEquipMod instance;

        [HarmonyPatch(typeof(ItemManagement), "DropItem")]
        public static class PatchForDropItem
        {
            public static bool Prefix(Item item)
            {
                return FangManagement.executeOnDropItemPrefix(item);
            }
        }

        [HarmonyPatch(typeof(TurnProcessing), "DoPlay")]
        public static class PatchForDoPlay
        {
            public static void Postfix()
            {
                FangManagement.executeOnDoPlayPostfix();
            }
        }

        [HarmonyPatch(typeof(TurnProcessing), "DoTurnEnd")]
        public static class PatchForDoTurnEnd
        {
            public static void Postfix()
            {
                FangManagement.executeOnDoTurnEndPostfix();
            }
        }

        [HarmonyPatch(typeof(GameLog.EquipFangRequest), "Process")]
        public static class PatchForEquipFangRequestProcess
        {
            public static bool Prefix(EquipFangRequest __instance)
            {
                return FangManagement.executeOnEquipFangRequestProcessPrefix(__instance.ItemId);
            }
        }

        [HarmonyPatch(typeof(SetStatus), "Execute")]
        public static class PatchForSetStatusExecute
        {
            public static bool Prefix(SetStatus __instance, SpecialParam p)
            {
                return FangDropRate.executeOnSetStatusExecutePrefix(__instance, p);
            }
        }

        [HarmonyPatch(typeof(CharacterAction), "KillCharacter")]
        public static class PatchForCharacterActionKillCharacter
        {
            public static bool Prefix(CharacterAction.AddDamageOption opt)
            {
                return FangDropRate.executeOnPatchForCharacterActionKillCharacterPrefix(opt);
            }
        }

        public override void OnInitializeMelon()
        {
            GUI.init();
            Settings.init();
        }

        public override void OnLateUpdate()
        {
            GUI.executeOnLateUpdate();
        }

        /*
        public override void OnEarlyInitializeMelon()
        {
            instance = this;
        }
        */
    }
}
