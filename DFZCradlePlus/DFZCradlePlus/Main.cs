using Game;
using Game.FieldAction;
using Game.MapGenerator;
using GameLog;
using HarmonyLib;
using Master;
using MelonLoader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using static Game.FieldLoaders.Util;

namespace DFZCradlePlus
{
    public static class BuildInfo
    {
        public const string Name = "DFZ Cradle Plus";
        public const string Description = null;
        public const string Author = "yagamuu";
        public const string Company = null;
        public const string Version = "1.1.1";
        public const string DownloadLink = null;
    }

    public class Main : MelonMod
    {
        // ダンジョンデータの追加
        [HarmonyPatch(typeof(G), "loadDungeon")]
        public static class PatchFixDungeon
        {
            public static void Postfix()
            {
                Dungeon baseDungeon = G.FindDungeonById(11);
                Dungeon newDungeon = baseDungeon;

                newDungeon.Name = "竜のゆりかご+";
                newDungeon.Desc = "高難易度化した竜のゆりかご\r\nアイテム持ち込み不可\r\n30F構成\r\n剣・盾・薬・書・杖・箱が未識別状態で出現\r\n剣・盾の初期呪い無し\n一部アイテム・ファングの性能調整";
                newDungeon.FangDropProb = 20;
                newDungeon.UnrevealItemTypes = new List<ItemType> { ItemType.Potion, ItemType.Staff, ItemType.Book, ItemType.Weapon, ItemType.Shield, ItemType.Box };
                newDungeon.CurseRate = 0;
                newDungeon.ItemBringable = false;

                G.Dungeons_.Remove(baseDungeon);
                G.Dungeons_.Add(newDungeon);

                foreach (Dungeon dungeon in G.Dungeons_)
                {
                    dungeon.OnLoaded();
                }

                Type typeG = typeof(G);
                System.Reflection.FieldInfo readOnlyDungeons_ = typeG.GetField("readOnlyDungeons_", BindingFlags.NonPublic | BindingFlags.Static);
                readOnlyDungeons_.SetValue(null, G.Dungeons_.AsReadOnly());

                System.Reflection.FieldInfo DungeonById_ = typeG.GetField("DungeonById_", BindingFlags.NonPublic | BindingFlags.Static);
                DungeonById_.SetValue(null, new Dictionary<int, Dungeon>());

                int count = G.Dungeons.Count;
                var newDungeons = new Dictionary<int, Dungeon>();
                for (int i = 0; i < count; i++)
                {
                    Dungeon dungeon2 = G.Dungeons[i];
                    if (dungeon2.Id != 0)
                    {
                        if (newDungeons.ContainsKey(dungeon2.Id))
                        {
                            Game.Logger.Error("Idがかぶっています, Type=Dungeons, ID=" + dungeon2.Id, new object[0]);
                        }
                        newDungeons.Add(dungeon2.Id, dungeon2);
                    }
                }

                DungeonById_.SetValue(null, newDungeons);
            }
        }

        // ダンジョン階層データの差し替え
        [HarmonyPatch(typeof(G), "loadDungeonStage")]
        public static class PatchFixDungeonStage
        {
            public static void Postfix()
            {
                List<DungeonStage> fixDungeonStage = PbFiles.LoadPbFilesFromAssembly<DungeonStage>(new Func<DungeonStage>(DungeonStage.CreateInstance), "DungeonStage");
                foreach (DungeonStage dungeonStage in fixDungeonStage)
                {
                    DungeonStage baseDungeonStage = G.FindDungeonStageById(dungeonStage.Id);
                    G.DungeonStages_.Remove(baseDungeonStage);
                    G.DungeonStages_.Add(dungeonStage);
                }

                Type typeG = typeof(G);
                System.Reflection.FieldInfo readOnlyDungeonStages_ = typeG.GetField("readOnlyDungeonStages_", BindingFlags.NonPublic | BindingFlags.Static);
                readOnlyDungeonStages_.SetValue(null, G.DungeonStages_.AsReadOnly());

                System.Reflection.FieldInfo DungeonStageById_ = typeG.GetField("DungeonStageById_", BindingFlags.NonPublic | BindingFlags.Static);
                DungeonStageById_.SetValue(null, new Dictionary<int, DungeonStage>());

                int count = G.DungeonStages.Count;
                var newDungeonStages = new Dictionary<int, DungeonStage>();
                for (int i = 0; i < count; i++)
                {
                    DungeonStage dungeonStage = G.DungeonStages[i];
                    if (dungeonStage.Id != 0)
                    {
                        if (newDungeonStages.ContainsKey(dungeonStage.Id))
                        {
                            Game.Logger.Error("Idがかぶっています, Type=DungeonStage, ID=" + dungeonStage.Id, new object[0]);
                        }
                        newDungeonStages.Add(dungeonStage.Id, dungeonStage);
                    }
                }
                DungeonStageById_.SetValue(null, newDungeonStages);
            }
        }

        // アイテムテーブルの追加
        [HarmonyPatch(typeof(G), "loadItemSet")]
        public static class PatchFixItemSet
        {
            public static void Postfix()
            {
                List<ItemSet> addItemSet = PbFiles.LoadPbFilesFromAssembly<ItemSet>(new Func<ItemSet>(ItemSet.CreateInstance), "ItemSet");
                foreach (ItemSet itemSet in addItemSet)
                {
                    if (itemSet.Id == 14)
                    {
                        ItemSet baseItemSet = G.FindItemSetById(itemSet.Id);
                        G.ItemSets_.Remove(baseItemSet);
                    }
                    G.ItemSets_.Add(itemSet);
                }

                Type typeG = typeof(G);
                System.Reflection.FieldInfo readOnlyItemSets_ = typeG.GetField("readOnlyItemSets_", BindingFlags.NonPublic | BindingFlags.Static);
                readOnlyItemSets_.SetValue(null, G.ItemSets_.AsReadOnly());

                System.Reflection.FieldInfo ItemSetById_ = typeG.GetField("ItemSetById_", BindingFlags.NonPublic | BindingFlags.Static);
                ItemSetById_.SetValue(null, new Dictionary<int, ItemSet>());

                int count = G.ItemSets.Count;
                var newItemSet = new Dictionary<int, ItemSet>();
                for (int i = 0; i < count; i++)
                {
                    ItemSet itemSet = G.ItemSets[i];
                    if (itemSet.Id != 0)
                    {
                        if (newItemSet.ContainsKey(itemSet.Id))
                        {
                            Game.Logger.Error("Idがかぶっています, Type=ItemSet, ID=" + itemSet.Id, new object[0]);
                        }
                        newItemSet.Add(itemSet.Id, itemSet);
                    }
                }
                ItemSetById_.SetValue(null, newItemSet);
            }
        }

        // ステージデータの変更
        [HarmonyPatch(typeof(G), "loadStage")]
        public static class PatchFixStage
        {
            public static void Postfix()
            {
                List<Stage> fixStage = PbFiles.LoadPbFilesFromAssembly<Stage>(new Func<Stage>(Stage.CreateInstance), "Stage");
                foreach (Stage stage in fixStage) {
                    Stage baseStage = G.FindStageById(stage.Id);
                    G.Stages_.Remove(baseStage);
                    G.Stages_.Add(stage);
                }

                Type typeG = typeof(G);
                System.Reflection.FieldInfo readOnlyStages_ = typeG.GetField("readOnlyStages_", BindingFlags.NonPublic | BindingFlags.Static);
                readOnlyStages_.SetValue(null, G.Stages_.AsReadOnly());

                System.Reflection.FieldInfo StageById_ = typeG.GetField("StageById_", BindingFlags.NonPublic | BindingFlags.Static);
                StageById_.SetValue(null, new Dictionary<int, Stage>());

                int count = G.Stages.Count;
                var newStages = new Dictionary<int, Stage>();
                for (int i = 0; i < count; i++)
                {
                    Stage stage2 = G.Stages[i];
                    if (stage2.Id != 0)
                    {
                        if (newStages.ContainsKey(stage2.Id))
                        {
                            Game.Logger.Error("Idがかぶっています, Type=Stage, ID=" + stage2.Id, new object[0]);
                        }
                        newStages.Add(stage2.Id, stage2);
                    }
                }
                StageById_.SetValue(null, newStages);

                System.Reflection.FieldInfo StageBySymbol_ = typeG.GetField("StageBySymbol_", BindingFlags.NonPublic | BindingFlags.Static);
                StageBySymbol_.SetValue(null, new Dictionary<string, Stage>());

                var newStagesBySymbol = new Dictionary<string, Stage>();
                for (int i = 0; i < count; i++)
                {
                    Stage stage2 = G.Stages[i];
                    if (stage2.Symbol != null)
                    {
                        if (newStagesBySymbol.ContainsKey(stage2.Symbol))
                        {
                            Game.Logger.Error("Symbolがかぶっています, Type=Stage, ID=" + stage2.Symbol, new object[0]);
                        }
                        newStagesBySymbol.Add(stage2.Symbol, stage2);
                    }
                }
                StageBySymbol_.SetValue(null, newStagesBySymbol);
            }
        }

        // モンスターテーブルデータの追加
        [HarmonyPatch(typeof(G), "FindTenantListByDungeonId")]
        public static class PatchFixTenant
        {
            public static bool Prefix(int key_, ref List<Tenant> __result)
            {
                if (key_ != 11) {
                    return true;
                }

                List<Tenant> addTenant = PbFiles.LoadPbFilesFromAssembly<Tenant>(new Func<Tenant>(Tenant.CreateInstance), "Tenant");
                G.TenantLists[key_] = addTenant;
                __result = addTenant;
                return false;
            }
        }

        // 武具テーブルデータの追加
        [HarmonyPatch(typeof(G), "FindArmLevelListByDungeonId")]
        public static class PatchFixArmLevel
        {
            public static bool Prefix(int key_, ref Dictionary<int, ArmLevel> __result)
            {
                if (key_ != 11)
                {
                    return true;
                }

                Dictionary<int, ArmLevel> addArmLevel = PbFiles.LoadPbFilesFromAssembly<ArmLevel>(new Func<ArmLevel>(ArmLevel.CreateInstance), "ArmLevel").ToDictionary((ArmLevel row) => row.Floor);
                G.ArmLevelLists[key_] = addArmLevel;
                __result = addArmLevel;
                return false;
            }
        }

        // ダンジョンへの持ち込みを弾く
        [HarmonyPatch(typeof(GameScene), "StartDungeon")]
        public static class PatchFixStartDungeon
        {
            public static bool Prefix(ref GameScene.StartDungeonParam param)
            {
                if (param.DungeonId != 11)
                {
                    return true;
                }
                param.SavedInterFloorInfo = null;
                return true;
            }
        }

        // ダンジョン開始時の処理あれこれ
        [HarmonyPatch(typeof(TurnProcessing), "DoPrelude")]
        public static class PatchForDoPrelude
        {
            public static void Postfix()
            {
                GameScene gameScene = GameObject.Find("GameScene").GetComponent<GameScene>();
                if (gameScene.Field.FieldInfo.DungeonId != 11)
                {
                    return;
                }

                foreach (Character character in gameScene.Field.Map.Characters)
                {
                    // オベリスクに永続無敵を付与
                    if (character.CharacterTemplateId == 2008)
                    {
                        character.SetStatus(CharacterStatus.Invincible,255);
                    }
                    // 支援魔導器の仮眠を解除
                    if (character.CharacterTemplateId == 25 && character.IsStatusActive(CharacterStatus.Snooze))
                    {
                        character.RemoveStatus(CharacterStatus.Snooze);
                    }
                    // 死神の仮眠を解除
                    if (character.CharacterTemplateId == 111 && character.IsStatusActive(CharacterStatus.Snooze))
                    {
                        character.RemoveStatus(CharacterStatus.Snooze);
                    }
                    // アルテミスの仮眠を解除
                    if (character.CharacterTemplateId == 907 && character.IsStatusActive(CharacterStatus.Snooze))
                    {
                        character.RemoveStatus(CharacterStatus.Snooze);
                    }
                    // アポロンの仮眠を解除
                    if (character.CharacterTemplateId == 908 && character.IsStatusActive(CharacterStatus.Snooze))
                    {
                        character.RemoveStatus(CharacterStatus.Snooze);
                    }
                }
                gameScene.RedrawAll();
            }
        }

        // 特定フロアでの開幕階段を回避
        [HarmonyPatch(typeof(Game.FieldLoaders.Util), "PutCharacter")]
        public static class PatchFixPutCharacter
        {
            public static bool Prefix(Field f, int enemyId, ref Game.Point pos, Direction dir, RandomBase rand, bool isPlayer)
            {
                if (f.FieldInfo.DungeonId != 11)
                {
                    return true;
                }
                if (!isPlayer)
                {
                    return true;
                }
                if (f.FieldInfo.FloorNum < 28 || f.FieldInfo.FloorNum == 30)
                {
                    return true;
                }

                int stairRoomId = 0;
                bool roop = true;
                int seed = f.RandRange(0, 9999999, Marker.Rand("DFZCradlePlus:avoidStairInTheSameRoomAsThePlayer"));
                RandomXS randomXS = new RandomXS(seed, 0);

                for (int i = 0; i < f.Map.Width; i++)
                {
                    for (int j = 0; j < f.Map.Height; j++)
                    {
                        Floor floor = f.Map[i, j];
                        if (floor.IsStair)
                        {
                            stairRoomId = floor.RoomIdOfRoom;
                        }
                    }
                }

                while (roop)
                {
                    Floor playerFloor = f.Map[pos.X, pos.Y];
                    if (playerFloor.RoomIdOfRoom == stairRoomId)
                    {
                        pos = Game.FieldLoaders.Util.FindRandomItemPos(f.Map, randomXS, new Game.MapGenerator.Rect(0, 0, f.Map.Width, f.Map.Height), null);
                    }
                    else
                    {
                        roop = false;
                    }
                }

                return true;
            }
        }

        // 28F~29Fにアルテミスアポロンを追加
        [HarmonyPatch(typeof(Game.FieldLoaders.MetaMapStageLoader), "InitField")]
        public static class PatchFixInitField
        {
            public static void Postfix()
            {
                GameScene gameScene = GameObject.Find("GameScene").GetComponent<GameScene>();
                Field field = gameScene.Field;

                if (field.FieldInfo.DungeonId != 11 || field.FieldInfo.FloorNum < 28 || field.FieldInfo.FloorNum > 29)
                {
                    return;
                }

                // アルテミスを設置
                int seed = field.RandRange(0, 9999999, Marker.Rand("DFZCradlePlus:addAltemisAndApollon"));
                RandomXS randomXS = new RandomXS(seed, 0);

                int num2 = randomXS.RangeInt(0, 8);
                Game.Point point = Game.FieldLoaders.Util.FindRandomPos(field.Map, randomXS, MoveType.Walk, default);
                Character character = Game.FieldLoaders.Util.PutCharacter(field, 907, point, Direction.North.Rotate(num2), randomXS, false);

                // アポロンを設置
                seed = field.RandRange(0, 9999999, Marker.Rand("DFZCradlePlus:addAltemisAndApollon"));
                randomXS = new RandomXS(seed, 0);

                num2 = randomXS.RangeInt(0, 8);
                point = Game.FieldLoaders.Util.FindRandomPos(field.Map, randomXS, MoveType.Walk, default);
                character = Game.FieldLoaders.Util.PutCharacter(field, 908, point, Direction.North.Rotate(num2), randomXS, false);
            }
        }

        // コンフィグデータの変更
        [HarmonyPatch(typeof(InitializeScene), "LoadAllData")]
        public static class PatchFixConfig
        {
            public static void Postfix()
            {
                // ボスへの毒ダメージを1％化
                // Config.SetValue("Game.PoisonDamageForBoss", "10");
                Config.PoisonDamageForBoss = 1;
                // BOSSへの猛毒ダメージを2%化
                // Config.SetValue("Game.VenomDamageForBoss", "20");
                Config.VenomDamageForBoss = 2;
            }
        }

       // ダンジョンの記録を行わない(1)
        [HarmonyPatch(typeof(HomeInfo), "UpdateDungeonRecord", new Type[] { typeof(DungeonRecord) })]
        public static class PatchDungeonRecord
        {
            public static bool Prefix(DungeonRecord record)
            {
                if (record.DungeonId == 11)
                {
                    return false;
                }
                return true;
            }
        }

        // ダンジョンの記録を行わない(2)
        [HarmonyPatch(typeof(HomeInfo), "UpdateHighScore")]
        public static class PatchUpdateHighScore
        {
            public static bool Prefix(GameResult gameResult)
            {
                if (gameResult.DungeonId == 11)
                {
                    return false;
                }
                return true;
            }
        }
    }
}
