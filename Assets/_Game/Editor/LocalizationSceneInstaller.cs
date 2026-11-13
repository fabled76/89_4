using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro;
using UnityEditor;
using UnityEditor.Localization;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.Localization.Tables;
using UnityEngine.TextCore.LowLevel;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace DeadAir.Editor
{
    public static class LocalizationSceneInstaller
    {
        public const string Folder = "Assets/_Game/Localization";
        public const string FontPath = Folder + "/Fonts/PrototypeCyrillic SDF.asset";
        [Serializable] private sealed class Seed { public List<Entry> entries; }
        [Serializable] private sealed class Entry { public string key, en, ru; }

        [MenuItem("89.4/Reset Language Preference")]
        public static void ResetLanguagePreference()
        {
            if (EditorApplication.isPlayingOrWillChangePlaymode) return;
            PlayerPrefs.DeleteKey(PrototypeText.LocalePreferenceKey);
            PlayerPrefs.Save();
            Debug.Log("Language preference cleared; the next session will default to English.");
        }

        [MenuItem("89.4/Install EN-RU Localization in Existing Corridor")]
        public static void InstallFromMenu()
        {
            if (EditorApplication.isPlayingOrWillChangePlaymode) return;
            if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) Install();
        }

        public static void Install()
        {
            if (!File.Exists(MotelPrototypeBuilder.ScenePath)) throw new FileNotFoundException("Existing corridor required.");
            var seed = JsonUtility.FromJson<Seed>(File.ReadAllText(Folder + "/PrototypeSeed.json"));
            CreateTables(seed);
            var font = CreateFont(seed);
            var scene = EditorSceneManager.OpenScene(MotelPrototypeBuilder.ScenePath);
            var roots = scene.GetRootGameObjects();
            var firstWitness = Array.Find(roots, obj => obj.name == "FirstWitness");
            var hud = Array.Find(roots, obj => obj.name == "PrototypeHUD");
            if (firstWitness == null || hud == null) throw new InvalidOperationException("Install the existing First Witness encounter first.");
            MigrateEvidenceAndScenario(seed);
            ConvertTextComponents(roots, font);
            // A Text-to-TMP field type change clears Unity's incompatible serialized references.
            // Rewire the known scene UI explicitly, including on a repeated migration.
            var hudComponent = hud.GetComponent<PrototypeHUD>();
            Assign(hudComponent, "statusText", hud.transform.Find("RadioAndControls").GetComponent<TMP_Text>());
            Assign(hudComponent, "promptText", hud.transform.Find("InteractionPrompt").GetComponent<TMP_Text>());
            var reading = hud.transform.Find("InvestigationPanel");
            var ui = firstWitness.GetComponent<InvestigationUI>();
            Assign(ui, "headingText", reading.Find("Heading").GetComponent<TMP_Text>());
            Assign(ui, "bodyText", reading.Find("Body").GetComponent<TMP_Text>());
            Assign(ui, "footerText", reading.Find("Footer").GetComponent<TMP_Text>());
            Assign(ui, "objectiveText", hud.transform.Find("InvestigationObjective").GetComponent<TMP_Text>());
            foreach (var root in roots)
            foreach (var behaviour in root.GetComponentsInChildren<MonoBehaviour>(true))
            {
                if (behaviour is EvidenceInteractable || behaviour is InspectInteractable)
                    MigrateProperty(behaviour, "prompt", seed);
            }
            if (firstWitness.GetComponent<LanguageSettingsPanel>() == null)
            {
                var panel = new GameObject("LanguageSettingsPanel", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
                panel.transform.SetParent(hud.transform, false);
                var rect = panel.GetComponent<RectTransform>();
                rect.anchorMin = rect.anchorMax = rect.pivot = new Vector2(1, 1);
                rect.anchoredPosition = new Vector2(-15, -15);
                rect.sizeDelta = new Vector2(350, 170);
                panel.GetComponent<Image>().color = new Color(.025f, .035f, .05f, .98f);
                panel.GetComponent<Image>().raycastTarget = false;
                var heading = Label("LanguageHeading", panel.transform, font, new Vector2(15, -10), new Vector2(320, 30), 24);
                var options = Label("LanguageOptions", panel.transform, font, new Vector2(15, -45), new Vector2(320, 60), 23);
                var footer = Label("LanguageFooter", panel.transform, font, new Vector2(15, -110), new Vector2(320, 50), 19);
                var control = firstWitness.AddComponent<LanguageSettingsPanel>();
                Assign(control, "panel", panel); Assign(control, "headingText", heading);
                Assign(control, "optionsText", options); Assign(control, "footerText", footer);
                panel.SetActive(false);
            }
            EditorSceneManager.MarkSceneDirty(scene);
            if (!EditorSceneManager.SaveScene(scene)) throw new IOException("Could not save localized corridor.");
            AssetDatabase.SaveAssets();
            Debug.Log("EN/RU String Tables, saved-locale selector and Cyrillic TMP font installed in the existing corridor.");
        }

        private static void CreateTables(Seed seed)
        {
            Directory.CreateDirectory(Folder + "/Tables");
            Directory.CreateDirectory(Folder + "/Locales");
            AssetDatabase.Refresh();
            var settings = LocalizationEditorSettings.ActiveLocalizationSettings;
            if (settings == null)
            {
                settings = ScriptableObject.CreateInstance<LocalizationSettings>();
                AssetDatabase.CreateAsset(settings, Folder + "/LocalizationSettings.asset");
                LocalizationEditorSettings.ActiveLocalizationSettings = settings;
            }
            var locales = new List<Locale>();
            foreach (string code in new[] { "en", "ru" })
            {
                string path = Folder + "/Locales/" + code + ".asset";
                var locale = AssetDatabase.LoadAssetAtPath<Locale>(path);
                if (locale == null)
                {
                    locale = Locale.CreateLocale(code);
                    AssetDatabase.CreateAsset(locale, path);
                }
                LocalizationEditorSettings.AddLocale(locale);
                locales.Add(locale);
            }
            var selectors = settings.GetStartupLocaleSelectors();
            if (!selectors.OfType<PlayerPrefLocaleSelector>().Any(s => s.PlayerPreferenceKey == PrototypeText.LocalePreferenceKey))
            {
                selectors.Insert(0, new PlayerPrefLocaleSelector { PlayerPreferenceKey = PrototypeText.LocalePreferenceKey });
                selectors.Insert(1, new SpecificLocaleSelector { LocaleId = "en" });
            }
            var collection = LocalizationEditorSettings.GetStringTableCollection(PrototypeText.TableName)
                ?? LocalizationEditorSettings.CreateStringTableCollection(PrototypeText.TableName, Folder + "/Tables", locales);
            foreach (var locale in locales)
            {
                var table = collection.GetTable(locale.Identifier) as StringTable
                    ?? collection.AddNewTable(locale.Identifier) as StringTable;
                foreach (var entry in seed.entries)
                {
                    // Seed once; preserve subsequent translations edited in Unity's Tables window.
                    if (table.GetEntry(entry.key) == null)
                        table.AddEntry(entry.key, locale.Identifier.Code == "ru" ? entry.ru : entry.en);
                }
                LocalizationEditorSettings.SetPreloadTableFlag(table, true);
                EditorUtility.SetDirty(table);
            }
            EditorUtility.SetDirty(collection.SharedData);
            EditorUtility.SetDirty(collection);
            EditorUtility.SetDirty(settings);
            AssetDatabase.SaveAssets();
        }

        private static TMP_FontAsset CreateFont(Seed seed)
        {
            var existing = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(FontPath);
            if (existing != null) return PopulateFont(existing, seed);
            if (AssetDatabase.LoadAssetAtPath<TMP_Settings>("Assets/TextMesh Pro/Resources/TMP Settings.asset") == null)
                throw new InvalidOperationException("Use Window > TextMeshPro > Import TMP Essential Resources, wait for import, then run this installer again.");
            var source = AssetDatabase.LoadAssetAtPath<Font>("Assets/TextMesh Pro/Fonts/LiberationSans.ttf");
            if (source == null) throw new InvalidOperationException("Import TMP Essential Resources; LiberationSans.ttf was not found.");
            Directory.CreateDirectory(Folder + "/Fonts");
            AssetDatabase.Refresh();
            var font = TMP_FontAsset.CreateFontAsset(source, 64, 7, GlyphRenderMode.SDFAA, 2048, 2048, AtlasPopulationMode.Dynamic, true);
            font.name = "PrototypeCyrillic SDF";
            AssetDatabase.CreateAsset(font, FontPath);
            AssetDatabase.AddObjectToAsset(font.material, font);
            foreach (var texture in font.atlasTextures) AssetDatabase.AddObjectToAsset(texture, font);
            return PopulateFont(font, seed);
        }

        private static TMP_FontAsset PopulateFont(TMP_FontAsset font, Seed seed)
        {
            var fontData = new SerializedObject(font);
            fontData.FindProperty("m_ClearDynamicDataOnBuild").boolValue = false;
            fontData.ApplyModifiedPropertiesWithoutUndo();
            string characters = string.Concat(seed.entries.Select(e => e.en + e.ru));
            characters += "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789 Ёё";
            // Reserve the Russian alphabet for future text, not only today's dialogue.
            for (int code = 0x0410; code <= 0x044F; code++) characters += (char)code;
            characters = new string(characters.Where(c => !char.IsControl(c)).Distinct().ToArray());
            if (!font.TryAddCharacters(characters, out string missing))
                throw new InvalidOperationException("Source font lacks required glyphs: " + missing);
            EditorUtility.SetDirty(font);
            foreach (var texture in font.atlasTextures)
            {
                if (!AssetDatabase.Contains(texture)) AssetDatabase.AddObjectToAsset(texture, font);
                EditorUtility.SetDirty(texture);
            }
            AssetDatabase.SaveAssets();
            return font;
        }

        private static void MigrateEvidenceAndScenario(Seed seed)
        {
            foreach (string id in AssetDatabase.FindAssets("t:EvidenceDefinition", new[] { "Assets/_Game/Data" }))
            {
                var evidence = AssetDatabase.LoadAssetAtPath<EvidenceDefinition>(AssetDatabase.GUIDToAssetPath(id));
                MigrateProperty(evidence, "title", seed);
                var data = new SerializedObject(evidence);
                var pages = data.FindProperty("pages");
                for (int i = 0; i < pages.arraySize; i++)
                {
                    var page = pages.GetArrayElementAtIndex(i);
                    var entry = seed.entries.Find(e => e.en == page.stringValue);
                    if (entry != null) page.stringValue = entry.key;
                }
                data.ApplyModifiedPropertiesWithoutUndo();
            }
            var scenario = AssetDatabase.LoadAssetAtPath<FirstWitnessDefinition>("Assets/_Game/Data/FirstWitness/FirstWitnessScenario.asset");
            foreach (string field in new[] { "encounterTitle", "emptyJournalMessage", "exploreObjective", "figureObjective", "ledgerObjective", "returnObjective", "approachObjective", "completedObjective" })
                MigrateProperty(scenario, field, seed);
        }
        private static void MigrateProperty(Object target, string field, Seed seed)
        {
            var data = new SerializedObject(target);
            var property = data.FindProperty(field);
            var entry = seed.entries.Find(e => e.en == property.stringValue);
            if (entry != null) property.stringValue = entry.key;
            data.ApplyModifiedPropertiesWithoutUndo();
        }

        private static void ConvertTextComponents(GameObject[] roots, TMP_FontAsset font)
        {
            foreach (var root in roots)
            foreach (var old in root.GetComponentsInChildren<Text>(true))
            {
                var obj = old.gameObject;
                float size = old.fontSize;
                Color color = old.color;
                Object.DestroyImmediate(old);
                var text = obj.AddComponent<TextMeshProUGUI>();
                ConfigureText(text, font, size, color);
            }
        }

        private static TMP_Text Label(string name, Transform parent, TMP_FontAsset font, Vector2 position, Vector2 size, int fontSize)
        {
            var obj = new GameObject(name, typeof(RectTransform), typeof(TextMeshProUGUI));
            obj.transform.SetParent(parent, false);
            var rect = obj.GetComponent<RectTransform>();
            rect.anchorMin = rect.anchorMax = rect.pivot = new Vector2(0, 1);
            rect.anchoredPosition = position;
            rect.sizeDelta = size;
            var text = obj.GetComponent<TextMeshProUGUI>();
            ConfigureText(text, font, fontSize, new Color(.93f, .92f, .85f));
            return text;
        }
        private static void ConfigureText(TMP_Text text, TMP_FontAsset font, float size, Color color)
        {
            text.font = font;
            text.fontSize = size;
            text.enableAutoSizing = true;
            text.fontSizeMin = Mathf.Min(size, 18);
            text.fontSizeMax = size;
            text.color = color;
            text.richText = false;
            text.raycastTarget = false;
            text.text = string.Empty;
        }
        private static void Assign(Object target, string field, Object value)
        {
            var data = new SerializedObject(target);
            data.FindProperty(field).objectReferenceValue = value;
            data.ApplyModifiedPropertiesWithoutUndo();
        }
    }
}
