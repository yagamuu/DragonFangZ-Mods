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
	}
}
