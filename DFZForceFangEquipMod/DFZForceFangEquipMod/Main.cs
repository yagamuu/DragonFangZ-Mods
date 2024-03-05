using Game;
using MelonLoader;
using HarmonyLib;
using System.Collections.Generic;
using UnityEngine;
using Game.FieldAction;
using static MelonLoader.MelonLogger;

namespace DFZForceFangEquipMod
{
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
