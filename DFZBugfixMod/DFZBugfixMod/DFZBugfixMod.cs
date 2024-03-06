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
using Master;
using Game.Thinkings;

namespace DFZBugrfixMod
{
	public class DFZBugrfixMod : MelonMod
	{
		[HarmonyPatch(typeof(Game.Thinkings.FindTarget), "Think")]
		public static class Patch
		{
			static MoveType savedMoveType;

			public static void Prefix(Field f, ThinkingCode code, Character c, Context ctx)
			{
				if (c.IsPlayer && code.Find == ThinkingCode.Types.FindType.Monster)
				{
					savedMoveType = c.T.MoveType;
					c.T.MoveType = MoveType.Fly;
					c.SetDirty();
				}

			}

			public static void Postfix(Field f, ThinkingCode code, Character c, Context ctx)
			{
				if (c.IsPlayer && code.Find == ThinkingCode.Types.FindType.Monster)
				{
					c.T.MoveType = savedMoveType;
					c.SetDirty();
				}
			}
		}


		[HarmonyPatch(typeof(Game.FieldAction.CharacterSoul), "CheckSoulFilter")]
		public static class PatchProtectSouls
        {
			public static bool Prefix(ref bool __result, Field f, Character c, SoulCondType cond, FilterOption opt)
            {
				__result = CheckSoulFilterPatched(f, c, cond, opt);
				return false;
			}

			public static bool CheckSoulFilterPatched(Field f, Character c, SoulCondType cond, FilterOption opt)
			{
				if (c.IsStatusActive(CharacterStatus.Seal))
				{
					return false;
				}

				var souls = c.Souls;
				var len = souls.Count;
				for (int i = 0; i < len; i++)
				{
					var soul = souls[i];
					if (soul.Type == cond && c.IsSoulActive(f, soul))
					{
						if (opt.Certain)
						{
							return true;
						}

						if (opt.Status != CharacterStatus.NoneCharacterStatus)
						{
							if (soul.Status.Contains(opt.Status))
							{
								return true;
							}
						}
						else
						{
							return false;
						}
					}
				}
				return false;
			}
		}

		[HarmonyPatch(typeof(GameSetting), "GetVersionText")]
		public static class OverwriteVersionText
		{
			public static void Postfix(ref string __result)
			{
				__result += "+Bugfix";
			}
		}
	}
}
