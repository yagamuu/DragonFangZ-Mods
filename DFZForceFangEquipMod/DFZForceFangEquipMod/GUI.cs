using Game;
using MelonLoader;
using UnityEngine;

namespace DFZForceFangEquipMod
{
    internal class GUI
    {
        private static string drawText = "DFZ Force Equipment Fang Mod v1.0.0";

        public static void init()
        {
            MelonEvents.OnGUI.Subscribe(DrawMenu, 100);
        }
        public static void executeOnLateUpdate()
        {
            updateDrawText();
        }

        private static void DrawMenu()
        {
            GUIStyle style = UnityEngine.GUI.skin.box;
            style.alignment = TextAnchor.UpperLeft;
            UnityEngine.GUI.Box(new Rect(10, Screen.height - 80, 300, 40), drawText, style);
        }

        private static void updateDrawText()
        {
            GameScene gameScene = GameObject.Find("GameScene")?.GetComponent<GameScene>();
            if (!gameScene || gameScene.Field == null || gameScene.Field.Player == null)
            {
                return;
            }
            Character player = gameScene.Field?.Player;
            Item nextFang = player.PlayerInfo.Fangs[FangManagement.nowFangIndex % 3];

            string displayFangName = nextFang == null ? "なし" : nextFang.DisplayName(gameScene.Field);
            drawText = string.Format("DFZ Force Equipment Fang Mod v1.0.0\n次回のファング装備先:{0}({1})", new object[]
                {
                    FangManagement.nowFangIndex % 3 + 1,
                    displayFangName
                }
            );
        }
    }
}
