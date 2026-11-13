using UnityEngine.Localization.Settings;

namespace DeadAir
{
    // Thin access to Unity's preloaded String Table; no separate localization store or manager.
    public static class PrototypeText
    {
        public const string TableName = "Prototype";
        public const string LocalePreferenceKey = "89.4.locale";
        public static bool IsReady => LocalizationSettings.InitializationOperation.IsDone;

        public static string Get(string key, params object[] arguments)
        {
            if (string.IsNullOrEmpty(key) || !IsReady) return string.Empty;
            // Desktop prototype: tables are preloaded; synchronous lookup uses their cached entries.
            return LocalizationSettings.StringDatabase.GetLocalizedString(TableName, key, arguments);
        }
    }
}
