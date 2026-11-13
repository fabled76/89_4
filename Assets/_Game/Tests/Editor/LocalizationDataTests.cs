using NUnit.Framework;
using UnityEditor;
using UnityEditor.Localization;
using UnityEngine.Localization.Tables;
using TMPro;
using DeadAir.Editor;

namespace DeadAir.Tests
{
    public sealed class LocalizationDataTests
    {
        [Test]
        public void BothTablesContainAllKeysAndEvidenceIdsStayUnchanged()
        {
            var collection = LocalizationEditorSettings.GetStringTableCollection(PrototypeText.TableName);
            Assert.That(collection, Is.Not.Null);
            foreach (var shared in collection.SharedData.Entries)
            foreach (string locale in new[] { "en", "ru" })
            {
                var entry = ((StringTable)collection.GetTable(locale)).GetEntry(shared.Key);
                Assert.That(entry, Is.Not.Null, locale + ":" + shared.Key);
                Assert.That(entry.Value, Is.Not.Empty, locale + ":" + shared.Key);
            }
            Assert.That(collection.SharedData.Entries.Count, Is.GreaterThanOrEqualTo(39));
            string[] names = { "MotelLedger", "MotelPhoto", "WitnessTestimony" };
            string[] ids = { "motel.guest-ledger", "motel.corridor-photo", "witness.station-lead" };
            for (int i = 0; i < names.Length; i++)
            {
                var evidence = AssetDatabase.LoadAssetAtPath<EvidenceDefinition>("Assets/_Game/Data/FirstWitness/" + names[i] + ".asset");
                Assert.That(evidence.Id, Is.EqualTo(ids[i]));
                var data = new SerializedObject(evidence);
                Assert.That(collection.SharedData.GetEntry(data.FindProperty("title").stringValue), Is.Not.Null);
                foreach (string key in evidence.PageKeys)
                    Assert.That(collection.SharedData.GetEntry(key), Is.Not.Null, key);
            }
            var font = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(LocalizationSceneInstaller.FontPath);
            Assert.That(font, Is.Not.Null);
            foreach (var shared in collection.SharedData.Entries)
            {
                string value = ((StringTable)collection.GetTable("ru")).GetEntry(shared.Key).Value;
                foreach (char character in value)
                    if (!char.IsControl(character)) Assert.That(font.HasCharacter(character), Is.True, "Missing glyph: " + character);
            }
            Assert.That(font.HasCharacter('Ё') && font.HasCharacter('ё'), Is.True);
        }
    }
}
