# EN/RU localization foundation

The existing MotelCorridor scene and First Witness encounter are retained. Player-facing text now comes from the official Unity Localization **Prototype** String Table collection. English (`en`) is the development/default language; Russian (`ru`) is provided for all 39 current keys, including the ten evidence/testimony pages. Translations preserve the provisional status of the existing story and use **Непринятый** for the witness. Evidence IDs and gameplay state are never translated.

## Try it in Unity

1. Open the project in Unity **6000.3.8f1**, wait for package resolution/compilation, then open `Assets/_Game/Scenes/MotelCorridor.unity`.
2. Press Play and focus Game view. **F1** opens a small language panel; **1** selects English, **2** selects Russian; **F1** closes it. The panel is intentionally keyboard-only and does not pause the game.
3. Change language while reading a ledger/testimony or viewing the journal. The visible text changes without advancing pages, dismissing the dialogue, collecting evidence, or altering encounter flags.
4. Stop and start Play again: the chosen locale should remain selected. The preference is `89.4.locale` in PlayerPrefs. Only language preference is persisted; this does not add a game save system.
5. To test first-launch English again, exit Play and choose **89.4 → Reset Language Preference**. This clears only that key, not all PlayerPrefs.

Movement, E interactions, radio toggle, battery drain/refill, STABLE/DRIFT, witness eligibility and evidence collection are unchanged. Gameplay keys remain physical Input System bindings; changing language does not replace key bindings or translate internal identifiers.

## Package installation and generated setup

Localization was absent at inspection. `Packages/manifest.json` now requests **com.unity.localization 1.5.9**, resolved by Unity. Addressables and associated official dependencies are installed automatically; no third-party localization framework was added. TextMeshPro comes from the installed uGUI package.

For a checkout where the package is missing:

1. **Window → Package Management → Package Manager**.
2. Select **Unity Registry**, search **Localization**, then **Install**. Alternatively use **+ → Install package by name**, name `com.unity.localization`, version `1.5.9` to match this project.
3. Wait for compilation. Keep the manifest and lock file in version control. See Unity's [official installation guide](https://docs.unity3d.com/Packages/com.unity.localization@1.5/manual/Installation.html).

The delivered checkout already contains the localization/font assets and wired scene. **No reinstall or scene recreation is needed.** For an older copy of the same encounter, import **Window → TextMeshPro → Import TMP Essential Resources**, wait for completion, then run **89.4 → Install EN-RU Localization in Existing Corridor** outside Play. Save pending scene changes when asked. The installer opens the existing scene and saves its additions; it never creates a replacement corridor. It seeds only missing table entries and does not overwrite translations subsequently edited in the tables.

Settings are in `Assets/_Game/Localization/LocalizationSettings.asset`: available locales en/ru; first startup selector Player Prefs (`89.4.locale`), then Specific Locale (en). An unavailable saved code falls through to English. Tables are marked for preload. Unity's generated Addressables configuration in `Assets/AddressableAssetsData/` is required source data, not a disposable cache. `ProjectSettings/EditorBuildSettings.asset` registers the localization and Addressables settings; the scene list was not replaced.

For a standalone player, include MotelCorridor in your Build Profile as before and build Addressables content using **Window → Asset Management → Addressables → Groups → Build → New Build → Default Build Script**, or use the installed Addressables build-with-player setting. Do not assume Play mode's Asset Database loading proves bundled content works in a standalone build. This task has not validated a Windows executable.

## Text keys and code boundaries

- `evidence.ledger.title`, `evidence.ledger.page.01` … `.03`
- `evidence.photo.title`, `evidence.photo.page.01` … `.02`
- `evidence.testimony.title`, `evidence.testimony.page.01` … `.05`
- `witness.*` — encounter title, empty journal and objective messages.
- `hud.*`, `radio.*`, `ui.*`, `prompt.*`, `settings.*`, `language.*` — interface and interaction text.

`EvidenceDefinition` still stores `motel.guest-ledger`, `motel.corridor-photo`, or `witness.station-lead` as a stable gameplay ID. Its **Title** and ordered **Pages** Inspector fields now store String Table keys, not prose. `FirstWitnessScenario` likewise stores keys for text fields and keeps the same evidence asset references. Encounter flags, class names, variable names and file names remain language-independent. There is no text baked into sprites.

`PrototypeText` is a small lookup helper around Unity's StringDatabase and the preloaded collection, not a second translation dictionary. Initial text rendering waits for LocalizationSettings initialization. HUD and InvestigationUI subscribe to SelectedLocaleChanged and redraw; prompts resolve the active locale during the existing prompt update. Journal pages cache evidence/page references rather than resolved English strings, preserving reading position across changes. The present desktop implementation uses synchronous cached StringDatabase lookups after initialization; a future WebGL port should use the package's asynchronous string APIs instead.

`LanguageSettingsPanel` is on **FirstWitness**. It owns F1/1/2 input, sets the official SelectedLocale, and writes/saves the language code to PlayerPrefs. The official PlayerPrefLocaleSelector reads that value at startup. Inputs and locale events enable/disable with the component and actions dispose on destruction. OnDisable tolerates Unity destroying the panel before its controller during scene unload.

`PrototypeHUD` and `InvestigationUI` now reference **TMP_Text**. The in-place installer explicitly reassigns their existing named Text GameObjects after replacing their uGUI Text components. It does not replace the Player, environment, camera or gameplay controllers. Language UI references: **Panel** = PrototypeHUD/LanguageSettingsPanel; Heading/Options/Footer = its matching TMP children. The controller is outside the hidden panel so F1 continues to work when the panel is inactive.

Console/debug messages, Inspector tooltips and Editor menu labels remain developer-facing English. They are not dialogue or player UI. Language names use autonyms **English** and **Русский** in either locale so players can recover from choosing an unfamiliar language.

## Cyrillic font setup

Every scene text uses `Assets/_Game/Localization/Fonts/PrototypeCyrillic SDF.asset`. It is generated from the Liberation Sans source font included in **Unity's TMP Essential Resources**, with its OFL license retained under `Assets/TextMesh Pro/Fonts/`. No Windows-installed font or machine-specific font path is required.

The asset contains the current English/Russian glyphs, Russian alphabet and Ё/ё, uses a dynamic SDF atlas with multi-atlas support, and retains the source font for future glyph additions. The generated atlas is retained across Editor exits. UI has bounded automatic font sizing to accommodate Russian expansion; this is not a substitute for splitting very long pages. Rich text is off so translation punctuation is treated literally.

To manually create an alternate font:

1. Import a licensed TTF/OTF with Cyrillic support into Assets and keep its redistribution license.
2. Use **Window → TextMeshPro → Font Asset Creator**, select it, and include Latin, Cyrillic (`0400–04FF`, including Ё/ё), punctuation and required arrow characters. Save the asset, material and atlas inside the project. A dynamic font asset must keep its source font available in the player; enable multi-atlas support for future text.
3. Assign the font to every TMP text under PrototypeHUD, including inactive InvestigationPanel and LanguageSettingsPanel children. Optionally configure TMP fallback assets for additional character sets. An ASCII-only default LiberationSans SDF is insufficient for this game's Russian text; use the generated project asset.
4. Test strings such as `Ёж, ещё, свидетель, батарея — «радиостанция»…` and check Console for missing-glyph warnings. A correct glyph test does not establish good typography at every resolution.

## Add or edit content without C# changes

1. Open **Window → Asset Management → Localization Tables** and select **Prototype**. The collection asset is `Assets/_Game/Localization/Tables/Prototype.asset`.
2. To edit existing text, change its English and Russian cells. The tables are the runtime source of truth; existing keys and IDs stay fixed. CSV export/import through Unity's collection tools can support translator review.
3. For a new evidence page, add a descriptive key, for example `evidence.reception-note.page.01`, and fill **both** locale columns. Select the EvidenceDefinition asset and append that key to **Pages**. Page order is the array order, identical across languages. Split a long translation across matching page keys rather than adding pages to only one locale.
4. For a new clue, **Assets → Create → 89.4 → Evidence**. Give it a unique language-independent Evidence Id, title key and ordered page keys. Add the existing EvidenceInteractable to a placeholder GameObject with a trigger BoxCollider2D; assign Evidence, the scene's EvidenceJournal and InvestigationUI. Set Prompt to a new `prompt.*` key with both translations. The existing E proximity system handles it.
5. To extend the current witness's testimony, add keys and append them to the testimony asset's page array. To change its title/objectives, select FirstWitnessScenario and assign other `witness.*` keys. No C# edit is required for this content work. A new gameplay condition or branching encounter is still code/design work; this is not a dialogue-tree framework.
6. Format strings such as `ui.reading.footer` use ordinary numbered `{0}`, `{1}` placeholders. Preserve their indexes in both languages; reorder them when grammar needs it. Smart Strings are not required. Do not rename a key just to change its wording.
7. Run the checklist below. Existing table edits survive rerunning the installer. `PrototypeSeed.json` is the initial bilingual migration seed/reference, not a runtime data source and not an automatic override of table edits.

## EN/RU test checklist

- [ ] First launch with no preference starts in English. F1 opens language options; 2 selects Russian; 1 returns English; F1 closes.
- [ ] HUD radio state, battery, controls, objective and all E prompts change. Radio/battery behavior remains unchanged.
- [ ] Inspect ledger and photo in both languages. Titles, every page and footer are translated, with no missing keys, tofu squares, clipped text or warnings.
- [ ] Switch on page 2 of a reading panel: same page and same evidence remain open, with no evidence granted early.
- [ ] Open an empty journal, then a populated one. Switch repeatedly on a later page, close/reopen, and verify evidence entries/IDs do not change.
- [ ] Play the witness route in Russian: ledger → charged ON/OFF → approach → complete testimony → station lead. Interruption/retry and exhaustion rules still work.
- [ ] Stop/re-enter Play and restart the Editor/standalone player: preference restores. Reset preference affects language only.
- [ ] Check fonts at 16:9, a smaller Game view and your target player resolution. Read the longest Russian pages manually.
- [ ] Run **Window → General → Test Runner → EditMode**, then **PlayMode**. Tests cover tables, IDs, glyphs, live page switching and the Russian encounter. Test fixtures restore the language preference they found.
- [ ] Build/load a Windows player with Addressables content; Editor Asset Database mode alone is not sufficient.

## Files changed or added for localization

- Packages manifest/lock and EditorBuildSettings registrations; generated Addressables source assets.
- New `Scripts/Localization/PrototypeText.cs`, `LanguageSettingsPanel.cs`, and `Editor/LocalizationSceneInstaller.cs`.
- Migrated EvidenceDefinition, FirstWitnessDefinition, EvidenceInteractable, InspectInteractable, BatteryReplacementInteractable, PrototypeHUD and InvestigationUI; runtime/editor/test assembly references.
- Existing scene, evidence/scenario assets and both existing scene installers updated to use keys/TMP. Core movement, physics, radio, battery, world and encounter rules were not rewritten.
- `Localization/` locales, settings, String Tables, font and bilingual seed; official TMP Essential Resources and license.
- New LocalizationDataTests/LocalizationFlowTests and updated existing UI tests; this guide and README.

## Actual validation

Verified with Unity **6000.3.8f1** on 2026-11-13:

- **3/3 EditMode tests passed**: scene references/collision boundaries, complete EN/RU entries, unchanged evidence IDs, and required Russian glyph coverage including Ё/ё.
- **13/13 PlayMode tests passed**: original movement/radio/battery/interaction/encounter regressions plus F1/2 selecting Russian, HUD/prompt updates, locale preference write/read through the startup selector, page-preserving switches in an open reading panel and journal, and a completed Russian witness route with unchanged evidence IDs.
- All current EN/RU evidence pages generated TMP meshes without overflow or missing required glyphs. Tests used bounded auto-sizing and the saved project font asset.
- Scene reloads and Play-mode exit completed without unexpected runtime exceptions. Locale refreshes wait for Unity's asynchronous initialization/table loading; the UI retains its current text during the transition rather than publishing blank strings.
- Localization installation and the scene/font conversion were actually executed by Unity. Scene GUID remains `d83b67eb662872545801078c97e50907`; existing gameplay objects and controller behavior were preserved.

Results: `TestResults/localization-editmode.xml`, `TestResults/localization-playmode.xml`. Logs: `Logs/localization-packages.log`, `Logs/localization-install.log`, `Logs/localization-editmode.log`, `Logs/localization-playmode.log`. Generated reports/logs are Git-ignored.

Not verified: a human visual/typographic review at target resolution, physical keyboard behavior in a visible Editor, preference restoration after a fresh Editor/player launch, and a standalone build with bundled Addressables content. The saved-preference startup selector was tested programmatically; that is not a claim of a separate application restart test. Native Russian editorial review remains useful before finalizing detective wording.
