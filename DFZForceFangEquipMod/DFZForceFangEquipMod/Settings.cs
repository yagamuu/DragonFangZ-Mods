using MelonLoader;

namespace DFZForceFangEquipMod
{
    internal class Settings
    {
        private static MelonPreferences_Category settingsCategory;
        private static MelonPreferences_Entry<bool> enableRemoveEquipFangAction;

        public static void init()
        {
            settingsCategory = MelonPreferences.CreateCategory("settingsCategory");
            enableRemoveEquipFangAction = settingsCategory.CreateEntry<bool>("enableRemoveEquipFangAction", true);
        }
    }
}
