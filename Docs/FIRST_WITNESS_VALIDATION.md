# The First Witness — implementation and verification

Date: 2026-11-13. Unity actually used: **6000.3.8f1**. The two previously approved foundation commits were successfully pushed to the configured GitHub remote. This report describes the subsequent encounter implementation.

## Delivered

- [x] Existing `MotelCorridor.unity` upgraded in place, not recreated. Scene GUID remains `d83b67eb662872545801078c97e50907`. All **166 original serialized scene-object IDs remain present**; 37 new serialized objects/components were added.
- [x] Existing player, collision, camera, portable radio, battery, interaction and world presentation components reused without source changes.
- [x] Configurable evidence assets: ledger, optional photograph, fragmented testimony. Scenario asset holds references, encounter title, empty-journal text and objective text. All narrative writing is provisional and editable.
- [x] Session investigation tracker and deduplicating evidence journal; no save/load or diegetic recording system.
- [x] E-to-inspect ledger/photo, paged reading, Tab journal, page navigation and reliable close/reopen controls.
- [x] Trigger-driven Unaccepted states: Hidden, Visible, AwaitingApproach, TestimonyDelivered. No movement AI, harm, health or Game Over.
- [x] Testimony requires ledger plus a deliberate charged ON→OFF transition. Empty battery reveals the figure but does not satisfy that choice.
- [x] Interruptible/retryable testimony; finishing its final page adds the radio-station lead. Completed testimony remains readable in the journal.
- [x] Shared collision and existing DRIFT silhouette retained; trigger/controller remain active outside the visual roots.
- [x] Small scene tuning change only: Drain Per Second 5→0.2, giving about 8m20s per full battery for reading. Drain semantics and unlimited refill station unchanged.
- [x] Idempotent Editor installation command and detailed Unity placement/reference/trigger/lifecycle guide in `FIRST_WITNESS_SETUP.md`.

## Verified

- [x] New scripts compiled in the actual installed Unity Editor. The scene upgrade was executed from its menu in the user's open Editor; initial encounter HUD was observed in visible Game view.
- [x] **11/11 PlayMode tests passed**: seven existing regressions and four new investigation tests. **1/1 EditMode scene-reference test passed** after the upgrade.
- [x] Full synthetic-keyboard route: initial stable state → see/suppress figure → abandon/retry ledger → finish ledger → voluntary OFF → walk into trigger → complete testimony → station lead and journal entry.
- [x] Proximity before ledger does not deliver testimony; exhaustion cannot satisfy voluntary choice; refilling and deliberately toggling restores progression.
- [x] Reading ledger while already OFF requires a fresh choice, and remains completable. Optional photo is not mandatory for testimony.
- [x] Power ON cancels unfinished testimony and restores control without collecting it; retry works while already inside the trigger via ON/OFF.
- [x] Esc cancellation stays closed until exit/re-entry or a new toggle; no reopening loop while standing in the trigger.
- [x] Journal close/reopen, repeated source inspection, evidence retention through world switches, and no duplicate entries after completion.
- [x] Current reading/journal text pages fit the configured body rectangle using Unity's preferred-height calculation. A detected journal overflow was corrected by placing evidence titles in the heading instead of repeating them in body text.
- [x] Existing movement, both walls, interaction selection, world/HUD synchronization and radio/battery boundary tests still pass. No unexpected runtime exceptions were reported in the passing test run.
- [x] Scene wiring test checks required serialized object references/missing scripts, unchanged shared boundary colliders and absence of colliders under STABLE/DRIFT visual roots.

Final machine-readable results: `TestResults/first-witness-playmode.xml` and `TestResults/first-witness-editmode.xml`. Logs: `Logs/first-witness-playmode.log` and `Logs/first-witness-editmode.log`. These generated artifacts are ignored by Git; test sources and Unity assets are not.

## Still requires human testing

- [ ] A complete physical-keyboard run, every rendered dialogue page, panel readability at different Game sizes/aspect ratios and input-focus behavior.
- [ ] First-time player understands that the ledger reframes the figure as a witness and chooses silence voluntarily.
- [ ] Target **5–10 minute** duration. There is no artificial waiting gate; a fast reader familiar with the route can finish sooner. Time a new tester and revise pacing from that evidence.
- [ ] Repeated interactive Play/Stop, domain-reload-disabled mode and a standalone Windows build. Automated runs exercised normal scene reloads and Play-mode exit.
- [ ] Atmosphere/audio: no authored switch sound, witness voice, ambient mix or final art was added.

The full test route passed headlessly; that does not prove the encounter feels clear or atmospheric. The prior foundation report remains historical. Use the new guide's manual acceptance route before shipping.

## Scope and next work

No final narrative explanation, identity mechanics, sanity, memory erasure, inventory, procedural generation, deduction board or durable save was added. The radio station is a lead, not a playable destination yet. Next: first-time playtest and tune; add audio feedback; then build one small station evidence-comparison beat after agreeing its provisional content.
