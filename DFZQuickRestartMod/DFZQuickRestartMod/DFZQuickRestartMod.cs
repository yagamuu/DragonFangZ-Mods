using MelonLoader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace DFZQuickRestartMod
{
    public class DFZQuickRestartMod : MelonMod
    {
        public override void OnUpdate()
        {
            GameScene gameScene = GameObject.Find("GameScene").GetComponent<GameScene>();
            if (Input.GetKeyDown(KeyCode.F5) && gameScene.Panel.CanControl()) {
                StateSerializer.DeleteState(0);
                Browser.GoTo(string.Format("Game?dungeon={0}", gameScene.CurrentStartDungeonParam.DungeonId), new PageOption
                {
                    Fade = true,
                    ClearHistory = true
                });
            }
        }
    }
}
