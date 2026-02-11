using Dfz.Ui;
using Game;
using HarmonyLib;
using Master;
using MelonLoader;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace DFZMochiInferno
{
    public static class BuildInfo
    {
        public const string Name = "DFZ Mochi Inferno";
        public const string Description = null;
        public const string Author = "yagamuu";
        public const string Company = null;
        public const string Version = "1.0.0";
        public const string DownloadLink = null;
    }

    public class Main : MelonMod
    {
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

        // クイックヘルプの表示を弾く
        [HarmonyPatch(typeof(MoveController), "DoQuickHelp")]
        public static class PatchFixDoQuickHelp
        {
            public static bool Prefix()
            {
                return false;
            }
        }

        // 商人に話した際のフリーズ対策
        [HarmonyPatch]
        public static class PatchFixOnLoadPage
        {
            // OnLoadPageはIEnumeratorなのでパッチ対象はイテレーター
            static MethodBase TargetMethod()
            {
                var iteratorType = AccessTools.Inner(typeof(ShopPage), "<OnLoadPage>c__Iterator0");
                return AccessTools.Method(iteratorType, "MoveNext");
            }

            static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
            {
                var code = new List<CodeInstruction>(instructions);
                var f_Image = AccessTools.Field(typeof(CharacterTemplate), "Image");
                var f_Id = AccessTools.Field(typeof(CharacterTemplate), "Id");
                var m_ReplaceImageId = AccessTools.Method(typeof(PatchFixOnLoadPage), "ReplaceImageId");
                var found = false;

                for (int i = 0; i < code.Count; i++)
                {
                    if (code[i].LoadsField(f_Image))
                    {
                        // ldfld Image → ldfld Id
                        code[i] = new CodeInstruction(OpCodes.Ldfld, f_Id);
                        // 直後に call ReplaceImageId(int) を挿入
                        code.Insert(i + 1, new CodeInstruction(OpCodes.Call, m_ReplaceImageId));
                        found = true;
                        break;
                    }
                }

                if (!found)
                {
                    Game.Logger.Error("MochiInferno: ILコードが見つかりません");
                }

                return code;
            }

            public static int ReplaceImageId(int characterId)
            {
                // マーガレット
                if (characterId == 4014)
                {
                    return 611;
                }
                // マルタポーロ
                if (characterId == 4016 || characterId == 4018 || characterId == 4019)
                {
                    return 687;
                }
                // ルルティエ
                return 566;
            }
        }
    }
}
