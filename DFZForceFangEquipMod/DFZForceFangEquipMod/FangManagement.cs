using Game.FieldAction;
using Game;
using GameLog;
using UnityEngine;
using System.Collections.Generic;

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
    }
}
