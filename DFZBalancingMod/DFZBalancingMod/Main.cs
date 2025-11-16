using Game;
using MelonLoader;
using HarmonyLib;
using System;
using System.Collections.Generic;
using Master;
using System.Reflection;
using DFZCradlePlus;

namespace DFZBalancingMod
{    public static class BuildInfo
    {
        public const string Name = "DFZ Balancing Mod";
        public const string Description = null;
        public const string Author = "yagamuu";
        public const string Company = null;
        public const string Version = "1.0.2";
        public const string DownloadLink = null;
    }

    public class Main : MelonMod
    {
        /*
        // EXPデータの差し替え
        [HarmonyPatch(typeof(G), "loadExpOfLevel")]
        public static class PatchExpOfLevel
        {
            public static void Postfix()
            {
                List<ExpOfLevel> list = PbFiles.LoadPbFilesFromAssembly<ExpOfLevel>(new Func<ExpOfLevel>(ExpOfLevel.CreateInstance), "ExpOfLevel");

                G.ExpOfLevels_ = list;

                Type typeG = typeof(G);
                System.Reflection.FieldInfo readOnlyExpOfLevels_ = typeG.GetField("readOnlyExpOfLevels_", BindingFlags.NonPublic | BindingFlags.Static);
                readOnlyExpOfLevels_.SetValue(null, G.ExpOfLevels_.AsReadOnly());

                System.Reflection.FieldInfo ExpOfLevelById_ = typeG.GetField("ExpOfLevelById_", BindingFlags.NonPublic | BindingFlags.Static);
                ExpOfLevelById_.SetValue(null, new Dictionary<int, ExpOfLevel>());

                int count = G.ExpOfLevels.Count;
                var newExpOfLevels = new Dictionary<int, ExpOfLevel>();
                for (int i = 0; i < count; i++)
                {
                    ExpOfLevel expOfLevel = G.ExpOfLevels[i];
                    if (expOfLevel.Id != 0)
                    {
                        if (newExpOfLevels.ContainsKey(expOfLevel.Id))
                        {
                            Game.Logger.Error("Idがかぶっています, Type=ExpOfLevel, ID=" + expOfLevel.Id, new object[0]);
                        }
                        newExpOfLevels.Add(expOfLevel.Id, expOfLevel);
                    }
                }

                ExpOfLevelById_.SetValue(null, newExpOfLevels);
            }
        }
        */

        /*
        // スキルデータの差し替え
        [HarmonyPatch(typeof(G), "loadSkill")]
        public static class PatchSkill
        {
            public static void Postfix()
            {
                List<Skill> list = PbFiles.LoadPbFilesFromAssembly<Skill>(new Func<Skill>(Skill.CreateInstance), "Skill");

                foreach (Skill skill in list)
                {
                    G.Skills_.Add(skill);
                }

                Type typeG = typeof(G);
                System.Reflection.FieldInfo readOnlySkills_ = typeG.GetField("readOnlySkills_", BindingFlags.NonPublic | BindingFlags.Static);
                readOnlySkills_.SetValue(null, G.Skills_.AsReadOnly());

                System.Reflection.FieldInfo SkillById_ = typeG.GetField("SkillById_", BindingFlags.NonPublic | BindingFlags.Static);
                SkillById_.SetValue(null, new Dictionary<int, Skill>());

                int count = G.Skills.Count;
                var newSkills = new Dictionary<int, Skill>();
                for (int i = 0; i < count; i++)
                {
                    Skill skill = G.Skills[i];
                    if (skill.Id != 0)
                    {
                        if (newSkills.ContainsKey(skill.Id))
                        {
                            Game.Logger.Error("Idがかぶっています, Type=Skill, ID=" + skill.Id, new object[0]);
                        }
                        newSkills.Add(skill.Id, skill);
                    }
                }

                SkillById_.SetValue(null, newSkills);
            }
        }
        */

        // アイテムデータの差し替え
        [HarmonyPatch(typeof(G), "loadItem")]
        public static class PatchItemTemplate
        {
            public static void Postfix()
            {
                List<ItemTemplate> list = PbFiles.LoadPbFilesFromAssembly<ItemTemplate>(new Func<ItemTemplate>(ItemTemplate.CreateInstance), "ItemTemplate");

                foreach (ItemTemplate item in list)
                {
                    ItemTemplate baseItemTemplate = G.FindItemById(item.Id);
                    G.Items_.Remove(baseItemTemplate);
                    G.Items_.Add(item);
                }

                Type typeG = typeof(G);
                System.Reflection.FieldInfo readOnlyItems_ = typeG.GetField("readOnlyItems_", BindingFlags.NonPublic | BindingFlags.Static);
                readOnlyItems_.SetValue(null, G.Items_.AsReadOnly());

                foreach (ItemTemplate itemTemplate in G.Items_)
                {
                    itemTemplate.OnLoaded();
                }

                System.Reflection.FieldInfo ItemById_ = typeG.GetField("ItemById_", BindingFlags.NonPublic | BindingFlags.Static);
                ItemById_.SetValue(null, new Dictionary<int, ItemTemplate>());

                int count = G.Items.Count;
                var newItems = new Dictionary<int, ItemTemplate>();
                for (int i = 0; i < count; i++)
                {
                    ItemTemplate item = G.Items[i];
                    if (item.Id != 0)
                    {
                        if (newItems.ContainsKey(item.Id))
                        {
                            Game.Logger.Error("Idがかぶっています, Type=ItemTemplate, ID=" + item.Id, new object[0]);
                        }
                        newItems.Add(item.Id, item);
                    }
                }

                ItemById_.SetValue(null, newItems);

                System.Reflection.FieldInfo ItemByAocDescId_ = typeG.GetField("ItemByAocDescId_", BindingFlags.NonPublic | BindingFlags.Static);
                ItemByAocDescId_.SetValue(null, new Dictionary<int, ItemTemplate>());

                var newItems2 = new Dictionary<int, ItemTemplate>();
                for (int i = 0; i < count; i++)
                {
                    ItemTemplate item = G.Items[i];
                    if (item.Id != 0)
                    {
                        if (newItems2.ContainsKey(item.Id))
                        {
                            Game.Logger.Error("AocDescIdがかぶっています, Type=ItemTemplate, ID=" + item.Id, new object[0]);
                        }
                        newItems2.Add(item.Id, item);
                    }
                }

                ItemByAocDescId_.SetValue(null, newItems2);
            }
        }

        // キャラデータの差し替え
        [HarmonyPatch(typeof(G), "loadCharacter")]
        public static class PatchCharacterTemplate
        {
            public static void Postfix()
            {
                List<CharacterTemplate> list = PbFiles.LoadPbFilesFromAssembly<CharacterTemplate>(new Func<CharacterTemplate>(CharacterTemplate.CreateInstance), "CharacterTemplate");

                foreach (CharacterTemplate character in list)
                {
                    CharacterTemplate baseCharacterTemplate = G.FindCharacterById(character.Id);
                    G.Characters_.Remove(baseCharacterTemplate);
                    G.Characters_.Add(character);
                }

                Type typeG = typeof(G);
                System.Reflection.FieldInfo readOnlyCharacters_ = typeG.GetField("readOnlyCharacters_", BindingFlags.NonPublic | BindingFlags.Static);
                readOnlyCharacters_.SetValue(null, G.Characters_.AsReadOnly());

                foreach (CharacterTemplate character in G.Characters_)
                {
                    character.OnLoaded();
                }

                System.Reflection.FieldInfo CharacterById_ = typeG.GetField("CharacterById_", BindingFlags.NonPublic | BindingFlags.Static);
                CharacterById_.SetValue(null, new Dictionary<int, CharacterTemplate>());

                int count = G.Characters.Count;
                var newCharacter = new Dictionary<int, CharacterTemplate>();
                for (int i = 0; i < count; i++)
                {
                    CharacterTemplate character = G.Characters[i];
                    if (character.Id != 0)
                    {
                        if (newCharacter.ContainsKey(character.Id))
                        {
                            Game.Logger.Error("Idがかぶっています, Type=CharacterTemplate, ID=" + character.Id, new object[0]);
                        }
                        newCharacter.Add(character.Id, character);
                    }
                }

                CharacterById_.SetValue(null, newCharacter);
            }
        }

        // コンフィグデータの変更
        [HarmonyPatch(typeof(InitializeScene), "LoadAllData")]
        public static class PatchFixConfig
        {
            public static void Postfix()
            {
                // ボスへの毒ダメージを1％化
                Config.PoisonDamageForBoss = 1;
                // ボスへの猛毒ダメージを2%化
                Config.VenomDamageForBoss = 2;
            }
        }
    }
}
