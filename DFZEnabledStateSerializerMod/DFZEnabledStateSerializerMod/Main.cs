using Game;
using Game.FieldAction;
using Google.ProtocolBuffers;
using HarmonyLib;
using MelonLoader;
using System;
using UnityEngine;

namespace DFZEnabledStateSerializerMod
{
    public static class BuildInfo
    {
        public const string Name = "DFZ Enabled State Serializer Mod";
        public const string Description = null;
        public const string Author = "yagamuu";
        public const string Company = null;
        public const string Version = "1.0.0";
        public const string DownloadLink = null;
    }
    public class Main : MelonMod
    {
        private MelonPreferences_Category settingsCategory;
        private static MelonPreferences_Entry<bool> enableAutoStateSaveOnGoNextFloor;
        private static int nowSeed;
        private static bool stateLoading = false;

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
                    stateLoading = true;
                    this.StateLoad(slot);
                }
            }
        }


        [HarmonyPatch(typeof(GameScene), "startDungeon")]
        public static class PatchForStartDungeon
        {
            // シード値保存
            public static void Postfix(GameScene.StartDungeonParam param)
            {
                if (!enableAutoStateSaveOnGoNextFloor.Value || stateLoading)
                {
                    return;
                }

                nowSeed = param.Seed;
            }
        }

        [HarmonyPatch(typeof(TurnProcessing), "DoPrelude")]
        public static class PatchForDoPrelude
        {
            // 自動ステートセーブ
            public static void Postfix()
            {
                if (!enableAutoStateSaveOnGoNextFloor.Value || stateLoading)
                {
                    stateLoading = false;
                    return;
                }

                GameScene gameScene = GameObject.Find("GameScene").GetComponent<GameScene>();
                Field field = gameScene.Field;
                if (field.FieldInfo.Dungeon.NoStateSave)
                {
                    return;
                }

                byte[] dumpArray;
                dumpArray = field.SavedField;

                // 日付_ダンジョンID_階層_seed
                DateTime dt = DateTime.Now;
                string dateString = dt.ToString("yyyyMMdd_HHmmss");
                string fileName = string.Format("state{0}_{1}_{2}_{3}.dat", dateString, field.FieldInfo.DungeonId, field.FieldInfo.FloorNum, nowSeed);
                FileSystemWrapperBase<FileSystemWrapper>.Instance.SaveBinary(fileName, dumpArray);
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

        // 設定を用意
        public override void OnInitializeMelon()
        {
            settingsCategory = MelonPreferences.CreateCategory("DFZEnabledStateSerializer");
            enableAutoStateSaveOnGoNextFloor = settingsCategory.CreateEntry<bool>("enableAutoStateSaveOnGoNextFloor", false, null, "自動で階段を降りるたびにステートセーブを裏で行う(false:行わない、true:行う)");
        }
    }
}
