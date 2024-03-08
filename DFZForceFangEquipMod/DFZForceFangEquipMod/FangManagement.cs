using Game.FieldAction;
using Game;
using GameLog;
using UnityEngine;
using System.Collections.Generic;
using Dfz.Ui;
using System.Linq;
using System.Reflection;
using Harmony;

namespace DFZForceFangEquipMod
{
    internal class FangManagement
    {
        private static int _nowFangIndex = 0;
        public static List<Item> dropFangs = new List<Item>();
        private static List<int> equipFangsId = new List<int>();
        private static List<Item> boughtFangs = new List<Item>();

        public static int nowFangIndex
        {
            get
            {
                return _nowFangIndex;
            }
        }
        public static void updateNowFangIndex(int targetIndex)
        {
            if (Settings.enableEquipFangToRandom.Value)
            {
                _nowFangIndex = Random.Range(0, 3);
            }
            else if (targetIndex == _nowFangIndex)
            {
                _nowFangIndex++;
            }

            if (_nowFangIndex == 3)
            {
                _nowFangIndex = 0;
            }
        }

        public static int getTargetIndex(GameScene gameScene)
        {
            int targetIndex = nowFangIndex;
            if (Settings.enableEquipFangPriorityToEmptySlot.Value)
            {
                int index = 0;
                foreach (Item fang in gameScene.Field.Player.PlayerInfo.Fangs)
                {
                    if (fang == null)
                    {
                        targetIndex = index;
                        break;
                    }
                    index++;
                };
            }
            return targetIndex;
        }

        public static void executeOnDropItemPostfix(Item item)
        {
            if (!item.IsFang)
            {
                return;
            }

            if (!boughtFangs.Contains(item))
            {
                dropFangs.Add(item);
            }
            if (Settings.enableEquipFangOnExplosionDamage.Value)
            {
                equipFang();
            }
        }

        public static void executeOnDoPlayPostfix()
        {
            equipFang();
            boughtFangs.Clear();
        }

        public static void executeOnDoTurnEndPostfix()
        {
            equipFang();
        }

        public static bool executeOnBuyItemPrefix(Item item)
        {
            boughtFangs.Add(item);
            return true;
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

        public static bool executeOnPatchForPutItemExecutePrefix(Field field)
        {
            if (!Settings.enableEquipFangOnDropBySlip.Value)
            {
                return true;
            }
            foreach (Item fang in field.Player.PlayerInfo.Fangs)
            {
                if (fang != null)
                {
                    equipFangsId.Add(fang.Id);
                }
            };

            return true;
        }

        public static void executeOnPatchForPutItemExecutePostfix(Field field)
        {
            if (!Settings.enableEquipFangOnDropBySlip.Value)
            {
                return;
            }
            foreach (int fangId in equipFangsId)
            {
                if (!field.Map.Items.TryGetValue(fangId, out Item item))
                {
                    continue;
                }
                if (item.IsOnFloor)
                {
                    dropFangs.Add(item);
                }
            }
            equipFangsId.Clear();
        }

        public static void equipFang()
        {
            bool doExecuteEquipFangToBoughtFang = (Settings.enableEquipFangOnBoughtFang.Value && boughtFangs.Count > 0);
            if (dropFangs.Count <= 0 && !doExecuteEquipFangToBoughtFang)
            {
                return;
            }

            GameScene gameScene = GameObject.Find("GameScene").GetComponent<GameScene>();
            Character player = gameScene.Field.Player;

            executeEquipFang(gameScene, player, dropFangs, false);
            dropFangs.Clear();

            if (doExecuteEquipFangToBoughtFang)
            {
                executeEquipFang(gameScene, player, boughtFangs, true);
            }
        }

        private static void executeEquipFang(GameScene gameScene, Character player, List<Item>items, bool isBuy)
        {
            foreach (Item item in items)
            {
                // 存在チェック
                if (!gameScene.Field.Map.Items.TryGetValue(item.Id, out Item checkFang))
                {
                    continue;
                }
                if (!isBuy && checkFang.OwnerCharacterId != 0)
                {
                    continue;
                }

                int targetIndex = getTargetIndex(gameScene);
                Item targetFang = player.PlayerInfo.Fangs[targetIndex];

                // 設定した強制装備を無効化するファングだった場合は無視
                if (checkIgnoreFangs(gameScene.Field, item))
                {
                    continue;
                }

                // 店購入かつアイテム欄に入っているアイテムの場合そのまま装備
                if (isBuy && item.OwnerCharacterId != 0)
                {
                    // なにもしない
                }
                // アイテムを拾う
                else if (player.Items.Count < player.PlayerInfo.MaxItem)
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
                    ItemEquiping.EquipFang(gameScene.Field, item, player, targetIndex);
                }
                else
                {
                    player.PlayerInfo.SetFang(targetIndex, item);
                    player.SetDirty();
                    gameScene.Field.SendAndWait(new EquipFang
                    {
                        CharacterId = player.Id,
                        ItemId = item.Id,
                        Index = targetIndex
                    });
                    gameScene.Field.ShowMessage(Marker.T("{0}は{1}を装着した！"), new object[] { player, item });
                    gameScene.Field.ShowMessage(Marker.T("<color=#00ff00><{0}></color>の力を手に入れた！"), new object[] { item.T.DisplaySoulName });
                }
                updateNowFangIndex(targetIndex);
            }            
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
