using UnityEngine;
using UnityEngine.Localization.Settings;

namespace DeadAir
{
    [CreateAssetMenu(menuName = "89.4/Prologue")]
    public sealed class PrologueDefinition : ScriptableObject
    {
        [SerializeField] private EvidenceDefinition call;
        [SerializeField] private EvidenceDefinition caseSummary;
        [SerializeField] private EvidenceDefinition article;
        [SerializeField] private EvidenceDefinition radioBeforeCase;
        [SerializeField] private EvidenceDefinition radioAfterCase;
        [Tooltip("Optional Prototype table keys; blank fields use neutral localized names.")]
        [SerializeField] private string townNameKey;
        [SerializeField] private string stationNameKey;
        [SerializeField] private string motelNameKey;
        [SerializeField] private string targetScenePath = "Assets/_Game/Scenes/MotelCorridor_ArtTrial.unity";
        [SerializeField, Min(.1f)] private float travelSeconds = 1.5f;
        public EvidenceDefinition Call => call;
        public EvidenceDefinition CaseSummary => caseSummary;
        public EvidenceDefinition Article => article;
        public EvidenceDefinition RadioBeforeCase => radioBeforeCase;
        public EvidenceDefinition RadioAfterCase => radioAfterCase;
        public string TargetScenePath => targetScenePath;
        public float TravelSeconds => travelSeconds;
        public string LeaveObjective => string.IsNullOrWhiteSpace(townNameKey)
            ? PrototypeText.Get("prologue.objective.leave_fallback") : Text("prologue.objective.leave");
        public string Text(string key) => Format(PrototypeText.Get(key));
        public string Format(string text) => text
            .Replace("[TOWN_NAME]", Name(townNameKey, "prologue.name.town"))
            .Replace("[STATION_NAME]", Name(stationNameKey, "prologue.name.station"))
            .Replace("[MOTEL_NAME]", Name(motelNameKey, "prologue.name.motel"));
        private static string Name(string key, string fallback)
        {
            if (string.IsNullOrWhiteSpace(key)) return PrototypeText.Get(fallback);
            var entry = LocalizationSettings.StringDatabase.GetTable(PrototypeText.TableName)?.GetEntry(key);
            return entry == null || string.IsNullOrWhiteSpace(entry.Value) ? PrototypeText.Get(fallback) : PrototypeText.Get(key);
        }
    }
}
