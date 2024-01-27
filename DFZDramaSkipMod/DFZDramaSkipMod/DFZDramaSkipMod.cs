using MelonLoader;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using Game.Specials;
using Game;
using GameLog;

namespace DFZDramaSkipMod
{
    public class DFZDramaSkipMod : MelonMod
    {
        private static List<string> ignoreScript = new List<string> {
            "D00011.FreyDialog",
            "D00011.ApollonDialog",
            "D00011.ArtemisDialog",
            "D00011.LastBossDialog"
        };

        [HarmonyPatch(typeof(Game.Specials.RunLua), "Execute")]
        public static class Patch
        {
            public static bool Prefix(Game.Specials.RunLua __instance, Field f)
            {
                if (ignoreScript.Contains(__instance.T.Script))
                {
                    f.SendAndWait(new GameLog.WaitMsec
                    {
                        Msec = 300
                    });
                    return false;
                }
                return true;
            }
        }
    }
}
