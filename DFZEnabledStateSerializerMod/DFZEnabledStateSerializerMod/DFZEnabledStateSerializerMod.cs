using Game;
using MelonLoader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace DFZEnabledStateSerializerMod
{
    public class DFZEnabledStateSerializerMod : MelonMod
    {
        public override void OnLateUpdate()
        {
            bool isUseShift = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
            int slot = 99;

            if (Input.GetKeyDown(KeyCode.F1))
            {
                slot = 1;
            }
            else if (Input.GetKeyDown(KeyCode.F2))
            {
                slot = 2;
            }
            else if (Input.GetKeyDown(KeyCode.F3))
            {
                slot = 3;
            }
            else if (Input.GetKeyDown(KeyCode.F4))
            {
                slot = 4;
            }
            else if (Input.GetKeyDown(KeyCode.F5))
            {
                slot = 5;
            }
            else if (Input.GetKeyDown(KeyCode.F6))
            {
                slot = 6;
            }
            else if (Input.GetKeyDown(KeyCode.F7))
            {
                slot = 7;
            }
            else if (Input.GetKeyDown(KeyCode.F8))
            {
                slot = 8;
            }
            else if (Input.GetKeyDown(KeyCode.F9))
            {
                slot = 9;
            }
            else if (Input.GetKeyDown(KeyCode.F10))
            {
                slot = 10;
            }

            if (slot != 99)
            {
                if (isUseShift)
                {
                    this.StateSave(slot);
                }
                else
                {
                    this.StateLoad(slot);
                }
            }
        }

        private void StateSave(int slot)
        {
            GameScene gameScene = GameObject.Find("GameScene").GetComponent<GameScene>();
            if (!gameScene.Panel.CanControl())
            {
                return;
            }
            StateSerializer.StateSave(slot);
            gameScene.ShowMessage(Marker.D("ステート{0}に保存しました").Format(new object[] { slot }));
        }

        private void StateLoad(int slot)
        {
            GameScene gameScene = GameObject.Find("GameScene").GetComponent<GameScene>();
            if (!gameScene.Panel.CanControl())
            {
                return;
            }
            if (StateSerializer.StateLoad(slot) != null)
            {
                gameScene.ShowMessage(Marker.D("ステート{0}を読み込みました").Format(new object[] { slot }));
            }
            else
            {
                gameScene.ShowMessage(Marker.D("ステート{0}の読み込みに失敗しました").Format(new object[] { slot }));
            }
        }
    }
}
