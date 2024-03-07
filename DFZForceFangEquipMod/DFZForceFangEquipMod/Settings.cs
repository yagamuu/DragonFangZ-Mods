using MelonLoader;

namespace DFZForceFangEquipMod
{
    internal class Settings
    {
        private static MelonPreferences_Category settingsCategory;
        public static MelonPreferences_Entry<bool> enableEquipFangAction;
        public static MelonPreferences_Entry<bool> enableBraveReset;
        public static MelonPreferences_Entry<bool> disableFang100FromItem;
        public static MelonPreferences_Entry<bool> disableFang100FromObj;
        public static MelonPreferences_Entry<bool> disableFangDropRate;
        public static MelonPreferences_Entry<string> ignoreEquipFangs;

        public static void init()
        {
            settingsCategory = MelonPreferences.CreateCategory("dfzForceFangEquipMod");
            enableEquipFangAction = settingsCategory.CreateEntry<bool>("enableEquipFangAction", false, null, "ファングの装着コマンドを有効化するか(false:しない、true:する)");
            enableBraveReset = settingsCategory.CreateEntry<bool>("enableBraveReset", false, null, "強制装備時のブレイブ0化を有効化するか(false:しない、true:する)");
            disableFang100FromItem = settingsCategory.CreateEntry<bool>("disableFang100FromItem", true, null, "薬によるファングドロップ確定化を無効化する(false:しない、true:する)");
            disableFang100FromObj = settingsCategory.CreateEntry<bool>("disableFang100FromObj", false, null, "オブジェクトによるファングドロップ確定化を無効化する(false:しない、true:する)");
            disableFangDropRate = settingsCategory.CreateEntry<bool>("disableFangDropRate", false, null, "特定攻撃(戦乙女サクリなど)によるファングドロップ率増加を無効化する(false:しない、true:する)");
            ignoreEquipFangs = settingsCategory.CreateEntry<string>("ignoreEquipFangs", "", null, "強制装備を無効化するファングを指定(複数指定する場合は\",\"で区切ってください)");
        }
    }
}
