using Game.FieldAction;
using Game;
using GameLog;
using UnityEngine;
using System.Collections.Generic;
using Dfz.Ui;
using System.Linq;
using System.Reflection;

namespace DFZForceFangEquipMod
{
    internal class FangManagement
    {
        public static int nowFangIndex = 0;
        public static List<Item> dropFangs = new List<Item>();

        public static bool executeOnDropItemPrefix(Item item)
        {
            if (item.IsFang)
            {
                dropFangs.Add(item);
            }
            return true;
        }

        public static void executeOnDoPlayPostfix()
        {
            equipFang();
        }

        public static void executeOnDoTurnEndPostfix()
        {
            equipFang();
        }

        public static bool executeOnEquipFangRequestProcessPrefix(int itemId)
        {
            if (!Settings.enableEquipFangAction.Value)
            {
                GameScene gameScene = GameObject.Find("GameScene").GetComponent<GameScene>();
                Field field = gameScene.Field;
                Item item = field.FindItem(itemId);
                gameScene.ShowMessage(Marker.T("{0}を装着することはできない！").Format(new object[] { item.DisplayName(field, false) }));
                return false;
            }
            return true;
        }

        public static void equipFang()
        {
            if (dropFangs.Count <= 0)
            {
                return;
            }

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

                // 設定した強制装備を無効化するファングだった場合は無視
                if (checkIgnoreFangs(gameScene.Field, item))
                {
                    continue;
                }

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
                if (Settings.enableBraveReset.Value)
                {
                    ItemEquiping.EquipFang(gameScene.Field, item, player, nowFangIndex % 3);
                }
                else
                {
                    player.PlayerInfo.SetFang(nowFangIndex % 3, item);
                    player.SetDirty();
                    gameScene.Field.SendAndWait(new EquipFang
                    {
                        CharacterId = player.Id,
                        ItemId = item.Id,
                        Index = nowFangIndex % 3
                    });
                    gameScene.Field.ShowMessage(Marker.T("{0}は{1}を装着した！"), new object[] { player, item });
                    gameScene.Field.ShowMessage(Marker.T("<color=#00ff00><{0}></color>の力を手に入れた！"), new object[] { item.T.DisplaySoulName });
                }
                nowFangIndex++;
            }
            dropFangs.Clear();
        }
        private static void putItem(Field field, Character player, Item item)
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

        private static bool checkIgnoreFangs(Field field, Item item)
        {
            string displayName = item.DisplayName(field);
            if (Settings.ignoreEquipFangs.Value == "")
            {
                return false;
            }
            string[] ignoreFangs = Settings.ignoreEquipFangs.Value.Split(',');
            foreach (string ignoreFang in ignoreFangs)
            {
                if (displayName.Contains(ignoreFang))
                {
                    return true;
                }
            }
            return false;
        }
    }
}
