# 89.4 — The First Witness

Unity **6000.3.8f1**, Universal 2D/URP, installed Unity Input System. The existing motel corridor now includes a small investigation encounter. All art and new writing are provisional. Design context is in `Docs/00_README.md` onward.

Open this repository in Unity Hub, let compilation finish, then open `Assets/_Game/Scenes/MotelCorridor.unity`. The encounter is already wired. For an older copy of this scene, use **89.4 → Add First Witness to Existing Corridor**; do not recreate it. Press Play and click Game view.

- **A/D or arrows:** walk.
- **Left Shift:** toggle portable radio. ON → STABLE; OFF → DRIFT and visible silhouette.
- **E:** nearest valid interaction. Green station near start refills battery indefinitely; marker farther right logs a test message.
- **Tab:** open/close evidence journal.
- **Space/Enter:** next page or finish; **left/right arrows:** browse pages; **Esc:** leave panel.
- **F1:** language settings; **1:** English; **2:** Russian. The selected language is saved between sessions.

See the figure with radio OFF, suppress it with ON, then finish reading the cream ledger at X=-5. The optional photo is at X=-13. With charge remaining, deliberately switch ON then OFF and approach the figure at X=7. Finish its testimony to collect the radio-station lead. All sources remain available; interrupted testimony can be retried.

Battery drains only while ON; empty forces OFF. Scene tuning now gives about 8m20s at full charge for exploration and reading. Refilling does not turn the radio on automatically. Reading/journaling stops walking and E, while radio and battery continue. No AI, combat, inventory, disk saves or diegetic recordings are implemented.

See [First Witness setup and Unity teaching guide](Docs/FIRST_WITNESS_SETUP.md) and [verification checklist](Docs/FIRST_WITNESS_VALIDATION.md). Earlier foundation documentation remains in `Docs/PROTOTYPE_SETUP.md` and `Docs/PROTOTYPE_VALIDATION.md` as historical context. Run EditMode/PlayMode tests from Window → General → Test Runner; follow the new guide's manual acceptance route before judging the target 5–10 minute duration.

Current text uses Unity Localization String Tables and Cyrillic-capable TextMeshPro. See [EN/RU setup, tests and content workflow](Docs/LOCALIZATION.md). Existing encounter setup references to uGUI Text/raw English fields are superseded by this guide's TMP references and table keys.
