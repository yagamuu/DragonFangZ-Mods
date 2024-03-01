using Game;
using MelonLoader;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dfz.Ui;
using GameLog;
using UnityEngine;
using Game.FieldAction;
using static MelonLoader.MelonLogger;
using Master;

namespace DFZForceFangEquipMod
{
    public class DFZForceFangEquipMod : MelonMod
    {
        public static int nowFangIndex = 0;
        public static string drawText = "DFZ Force Equipment Fang Mod v1.0.0";
        public static List<Item> dropFangs = new List<Item>();

        private MelonPreferences_Category settingsCategory;
        private static MelonPreferences_Entry<bool> enableRemoveEquipFangAction;
        // public static DFZForceFangEquipMod instance;

        [HarmonyPatch(typeof(Game.FieldAction.ItemManagement), "DropItem")]
        public static class PatchForDropItem
        {
            public static bool Prefix(Item item)
            {
                if (item.IsFang)
                {
                    dropFangs.Add(item);
                }
                return true;
            }
        }

        [HarmonyPatch(typeof(Game.FieldAction.TurnProcessing), "DoPlay")]
        public static class PatchForDoPlay
        {
            public static void Postfix()
            {
                GameScene gameScene = GameObject.Find("GameScene").GetComponent<GameScene>();
                Character player = gameScene.Field.Player;

                foreach (Item item in dropFangs)
                {
                    // 存在チェック
                    if (!gameScene.Field.Map.Items.TryGetValue(item.Id, out Item checkFang) || checkFang.OwnerCharacterId != 0)
                    {
                        continue;
                    }
                    Item targetFang = player.PlayerInfo.Fangs[nowFangIndex % 3];
                    // アイテムを拾う
                    if (player.Items.Count < player.PlayerInfo.MaxItem)
                    {
                        gameScene.Field.SendAndWait(new PickupItem
                        {
                            CharacterId = player.Id,
                            ItemId = item.Id,
                            X = player.X,
                            Y = player.Y
                        });
                        gameScene.Field.Map.AddItem(item, player);
                    }
                    // アイテムがいっぱいだった場合、装備先にファングがあった場合はその場に置く
                    else if (targetFang != null)
                    {
                        putItem(gameScene.Field, player, targetFang);
                        gameScene.Field.SendAndWait(new PickupItem
                        {
                            CharacterId = player.Id,
                            ItemId = item.Id,
                            X = player.X,
                            Y = player.Y
                        });
                        gameScene.Field.Map.AddItem(item, player);
                    }
                    // 装備先にファングが無い場合は未装備のアイテムの中から直近拾ったアイテムをその場に置く
                    else
                    {
                        int i = player.PlayerInfo.MaxItem - 1;
                        while (i >= 0)
                        {
                            Item putItemTarget = player.Items[i];
                            if (!player.PlayerInfo.FangIds.Contains(putItemTarget.Id))
                            {
                                putItem(gameScene.Field, player, putItemTarget);
                                gameScene.Field.SendAndWait(new PickupItem
                                {
                                    CharacterId = player.Id,
                                    ItemId = item.Id,
                                    X = player.X,
                                    Y = player.Y
                                });
                                gameScene.Field.Map.AddItem(item, player);
                                break;
                            }
                            i--;
                        }
                    }
                    ItemEquiping.EquipFang(gameScene.Field, item, player, nowFangIndex % 3);
                    nowFangIndex++;
                }
            }
        }

        [HarmonyPatch(typeof(Game.FieldAction.TurnProcessing), "DoTurnEnd")]
        public static class PatchForDoTurnEnd
        {
            public static void Postfix()
            {
                dropFangs.Clear();
            }
        }

        [HarmonyPatch(typeof(Game.FieldAction.ItemUsing), "PossibleActions")]
        public static class removeEquipFangAction
        {
            public static void Postfix(ref List<ItemUsing.PossibleActionResult> __result)
            {
                // 装着するコマンドを削除
                if (enableRemoveEquipFangAction.Value && __result.Contains(ItemUsing.PossibleActionResult.EquipFang))
                {
                    __result.Remove(ItemUsing.PossibleActionResult.EquipFang);
                }
            }
        }

        public static void putItem(Field field, Character player, Item item)
        {
            if (!ItemEquiping.TryUnequip(field, item, player))
            {
                return;
            }
            field.ShowMessage(Marker.T("{0}は{1}を足元においた"), new object[]
            {
                player.DisplayName,
                item.DisplayName(field, false)
            });
            ItemManagement.PutItem(field, item, player.Position, PutItemOption.None, 2, default(Game.Point));
            CharacterAction.RedrawAllStatus(field, player);
        }
        public override void OnInitializeMelon()
        {
            MelonEvents.OnGUI.Subscribe(DrawMenu, 100);
            settingsCategory = MelonPreferences.CreateCategory("settingsCategory");
            settingsCategory.SetFilePath("./dfzffem_settings.cfg");
            enableRemoveEquipFangAction = settingsCategory.CreateEntry<bool>("enableRemoveEquipFangAction", true);
        }

        private void DrawMenu()
        {
            GUIStyle style = GUI.skin.box;
            style.alignment = TextAnchor.UpperLeft;
            GUI.Box(new Rect(10, Screen.height - 80, 300, 40), drawText, style);
        }

        public override void OnLateUpdate()
        {
            updateDrawText();
            if (Input.GetKeyDown(KeyCode.F1))
            {
                GameScene gameScene = GameObject.Find("GameScene").GetComponent<GameScene>();
                if (!gameScene.Panel.CanControl())
                {
                    return;
                }
                enableRemoveEquipFangAction.Value = !enableRemoveEquipFangAction.Value;
                gameScene.ShowMessage(Marker.T("Mod設定変更：装着コマンドを" + (enableRemoveEquipFangAction.Value ? "削除" : "解禁") + "しました"));
            }
        }

        public static void updateDrawText()
        {
            GameScene gameScene = GameObject.Find("GameScene")?.GetComponent<GameScene>();
            if (!gameScene || gameScene.Field == null || gameScene.Field.Player == null)
            {
                return;
            }
            Character player = gameScene.Field?.Player;
            Item nextFang = player.PlayerInfo.Fangs[nowFangIndex % 3];

            string displayFangName = nextFang == null ? "なし" : nextFang.DisplayName(gameScene.Field);
            drawText = string.Format("DFZ Force Equipment Fang Mod v1.0.0\n次回のファング装備先:{0}({1})", new object[]
                {
                    nowFangIndex % 3 + 1,
                    displayFangName
                }
            );
        }

        /*
        public override void OnEarlyInitializeMelon()
        {
            instance = this;
        }
        */
    }
}
