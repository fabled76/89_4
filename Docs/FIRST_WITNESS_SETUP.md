# The First Witness — playable encounter guide

Localization update: player text now uses EN/RU String Tables and TMP_Text. See `LOCALIZATION.md` for current text/font setup and F1 language controls; the gameplay references and route below remain applicable.

This continues the existing **89.4** motel corridor. The scene was upgraded in place, preserving its GUID and original GameObjects. No replacement scene, package, global singleton, navigation AI or save system was introduced. The latest request scopes this encounter to ledger recognition and a radio-station lead; the larger deduction puzzle from the design pack remains future work.

All new writing is **provisional playtest text**. It does not establish a guest name, town name, diagnosis, supernatural explanation, culprit or final chronology. Edit the assets rather than treating it as approved canon.

## Play now

1. Open `Assets/_Game/Scenes/MotelCorridor.unity` in Unity 6000.3.8f1. Wait for compilation; open Console and clear old messages.
2. Confirm **FirstWitness** exists in Hierarchy. It is already installed in the delivered scene. For an older copy without it, use **89.4 → Add First Witness to Existing Corridor**, outside Play mode. The command saves pending scene changes when accepted, opens the existing corridor, adds content and saves. It never creates/replaces the corridor, and running it again leaves an installed encounter intact. Do not use the original Create Motel Corridor command for this update.
3. Select Game view, choose 16:9 and press Play. Click in Game view. Increase its size or maximize the tab for reading.
4. Explore right with the receiver on. The dark figure at X=7 is suppressed. Switch off near the far doors to see it; switching on hides it. It cannot harm you.
5. Return left to the **cream ledger at X=-5**, press E, and use Space/Enter through all three pages. The final advance adds it to the journal. The **gray photograph at X=-13** is optional supporting evidence, available in both world states.
6. With charge remaining and the ledger collected, switch the receiver ON and then OFF. Approach X=7. Crossing the trigger opens fragmented testimony. Switching off while already within the trigger also works.
7. Finish the testimony's five pages. The journal gains a lead to the radio station's programme sheet, and the objective confirms completion. The station location is not built yet.
8. Tab opens/closes the journal. Arrows or Space/Enter browse its pages. Esc closes any panel. Every collected clue remains available during this scene session.

Controls: A/D or arrows walk; E inspects/refills; Left Shift toggles radio; Tab journal; Space/Enter next/finish; left arrow previous; Esc leave panel. Esc is not a global pause menu. Tab during testimony does not replace the unfinished reading panel; finish or leave it first.

Reading/journaling stops walking and E interaction, while **Left Shift and battery drain remain active**. Battery life is now about **500 seconds (8m20s)** at full charge: capacity 100, scene Drain Per Second 0.2. Only the scene's tuning changed; radio power, drain rules, depletion and refill mechanics are unchanged. The green station at X=-8 still provides unlimited test batteries. Exhaustion reveals the silhouette but does not count as voluntarily choosing silence. Refill, power ON, then OFF to recover the intended route.

The target is a **5–10 minute first-time exploration/read**, not an enforced timer. Actual duration and clarity need a human playtest; an experienced tester can complete it much faster.

## New files and responsibilities

`Scripts/Investigation/EvidenceDefinition.cs` is a ScriptableObject holding a stable evidence ID, title and pages. The three assets in `Data/FirstWitness/` are MotelLedger, MotelPhoto and WitnessTestimony. These hold content only. `FirstWitnessDefinition.cs` holds the three references plus objective strings in FirstWitnessScenario.asset. These assets are useful here because narrative text is shared by inspection, journal and encounter. Runtime collection never writes into the assets.

`EvidenceJournal.cs` lives on **FirstWitness**. It stores ordered session entries and a set of IDs, rejects duplicates, and emits Changed. It has no Inspector references and no disk persistence. New Play sessions start empty.

`InvestigationStateTracker.cs` also lives on **FirstWitness**. Assign **Journal** to its EvidenceJournal and **Scenario** to FirstWitnessScenario.asset. It exposes SeenUnaccepted, SuppressedUnaccepted, VoluntaryReturn, LedgerRead and TestimonyReceived. Evidence-derived flags query the journal; the other flags express this encounter's progress. It chooses objective text from the scenario, rather than putting story rules in player or radio scripts. Sightings use a local notice-distance check, not a visibility/occlusion simulation.

`EvidenceInteractable.cs` lives on **FirstWitness/MotelGuestLedger** and **FirstWitness/MotelCorridorPhoto**. It implements the existing IInteractable interface; movement and proximity selection are reused unchanged. Each has a SpriteRenderer and trigger BoxCollider2D. Assign Evidence to the matching asset, Journal to FirstWitness's journal, Presentation to FirstWitness's InvestigationUI, and Prompt to the appropriate E message. Finishing reading adds the entry; leaving early does not remove the source. Objects remain inspectable after collection and in either radio state.

`UnacceptedEncounter.cs` lives on **FirstWitness**, with the proximity BoxCollider2D on that same object. Assign Radio and Battery from Player/PortableRadio; Investigation and Journal from FirstWitness; Scenario = FirstWitnessScenario.asset; Presentation = FirstWitness's InvestigationUI; Player Body = Player's Rigidbody2D. Notice Distance is 8 units.

Its explicit state is Hidden when receiver is on, Visible when off without the required choice, AwaitingApproach when the ledger is collected and charged silence has been chosen, and TestimonyDelivered after completion while off. Powering on still produces Hidden after completion; the journal retains testimony. This component does not independently toggle silhouette visuals: the existing WorldStateController still owns STABLE/DRIFT roots.

`Scripts/UI/InvestigationUI.cs` lives on **FirstWitness**, which remains active when the panel is hidden. Assign Journal, Investigation, Movement = PlayerController2D on Player, Interactor = InteractionController on Player, Panel = PrototypeHUD/InvestigationPanel, Heading/Body/Footer to that panel's Text children, and Objective Text = PrototypeHUD/InvestigationObjective. It manages one reading/journal panel, input actions and the temporary movement/interaction lock. It emits DialogueClosed so the encounter can avoid reopening a canceled conversation every physics step.

`Editor/FirstWitnessSceneInstaller.cs` wires this once, using Unity serialization to assign private Inspector fields. It reuses the existing white sprite and silhouette; it does not delete the template marker or battery station. No prefabs existed initially, and none are necessary for these one-off scene objects. If reusing a clue later, create a prefab in Unity and assign its scene-dependent Journal/Presentation references on each instance; prefab assets cannot store references to arbitrary scene objects.

## Exact scene wiring and geometry

- **FirstWitness**: world position (7, 1.2, 0), scale (1,1,1). Components: EvidenceJournal, InvestigationStateTracker, InvestigationUI, UnacceptedEncounter, BoxCollider2D. Box size (3.2,3), offset (0,0), Is Trigger checked. It is outside Environment/DRIFT_Visuals and stays active in STABLE. It has no Rigidbody2D: the existing dynamic player body supplies the physics body required for trigger events.
- **MotelGuestLedger**: child of FirstWitness, world position (-5,0.65,0), scale (1,1.3,1), white sprite tinted cream, sorting order 1. BoxCollider2D size (1,1) before transform scale, Is Trigger checked; EvidenceInteractable references listed above.
- **MotelCorridorPhoto**: child, world position (-13,1,0), scale (0.9,1.5,1), gray sprite, sorting order 1. Same trigger/component pattern, Photo asset assigned.
- **Environment/DRIFT_Visuals/Silhouette_VisualOnly_NoAI**: existing visual at (7,0.95), unchanged, no collider/AI. Do not move the encounter controller under DRIFT_Visuals: deactivation would prevent it listening for the radio returning on.
- **PrototypeHUD**: existing Screen Space Overlay Canvas/CanvasScaler retained. InvestigationObjective is top-left anchored, position (20,-115), size (1230,100), font 23. Existing interaction prompt moved to (20,-225) to avoid overlap.
- **InvestigationPanel**: centered anchors, position (0,-80), size (1160,400), dark Image background. Heading top-left (25,-18), size (1110,45), font 28; Body (25,-75), size (1110,255), font 25; Footer (25,-352), size (1110,40), font 19. All Text components use the built-in LegacyRuntime font, rich text off. The panel starts inactive; the controller does not. Keyboard-only UI needs no buttons/EventSystem.
- **Player/PortableRadio**: existing components unchanged; RadioController's scene Drain Per Second = 0.2. Shared floor/walls/ceiling, player physics, camera and world references remain intact.

## Unity lifecycle and trigger behavior

Serialized private fields are stored by Unity in the scene or asset, so references survive reopening. They are not runtime discovery by name: the installer uses names only during setup. Renaming a wired object afterward does not break serialized references.

Awake creates local InputActions; OnEnable enables actions and subscribes events; OnDisable unsubscribes and closes/restores panel locks; OnDestroy disposes actions. Event listeners read current state when enabled, and the radio initializes power in Start after all Awake calls. UI scripts stay on an active object, so hiding a child panel does not disable their keyboard input.

OnTriggerEnter2D starts an eligible encounter when entering. OnTriggerStay2D also checks eligibility, because turning the radio off while already inside a trigger produces no new Enter event. Only colliders attached to the assigned Player Body qualify. OnTriggerExit2D clears the canceled-dialogue retry latch. Cancellation leaves progress intact; step out/back in or toggle ON/OFF to retry. Completing testimony commits its evidence once. No patrol, movement AI or damage is involved.

Panels temporarily disable just the PlayerController2D and InteractionController components, remembering their previous enabled states. This triggers their existing OnDisable input cleanup and stops horizontal movement. It leaves Rigidbody2D collision, RadioInput, battery, world events and camera running. It does not change Time.timeScale or pause the whole game.

## Debugging

- E missing: get closer, inspect collider Is Trigger, Interaction Layers, EvidenceInteractable activity and its three references. An open reading/journal panel intentionally disables E. Complete a source's final page to collect it.
- Testimony missing: check ledger in journal, battery charge, then ON → OFF. Reading the ledger while already off requires a new deliberate toggle. Exhaustion or turning off before the ledger does not qualify. Walk into the trigger at X=7.
- Esc seemed to suppress retries: leave the trigger and return, or use ON/OFF. This prevents a modal reopening loop while standing still.
- Trigger missing: use Collider2D, not Collider; player Rigidbody2D must be Dynamic/Simulated and assigned. Check 2D collision-layer interactions. Keep the trigger root active in both world states.
- UI missing: check every serialized reference, panel child hierarchy, font and Canvas. Keep FirstWitness active. Enlarge Game view; QHD at a tiny preview scale makes all text small.
- Evidence disappears after stopping Play: expected—session state is not a technical save. It must survive radio switches and panel reopening during the session.
- White rectangle overlay: try Game view Gizmos off; avoid mistaking editor overlays for world sprites. This encounter does not draw an outline.
- Content edits: stop Play, edit `Data/FirstWitness/*.asset` in Inspector. Keep IDs unique and nonempty, at least one page per evidence, and all scenario references assigned. Long pages need splitting; the UI does not scroll. Do not rerun the original scene builder to refresh content.

## Manual test route

1. Enter Play with Console visible: radio ON, STABLE, no silhouette, empty journal, introduction objective.
2. Near the far doors toggle OFF, observe figure, approach before ledger: no testimony. ON suppresses it and directs investigation toward ledger.
3. Inspect optional photo; finish both pages. Read/reopen it and confirm one entry only.
4. Start ledger, Esc, inspect again. Finish all pages; its entry and return objective appear. Tab close/reopen preserves both entries.
5. Switch ON then OFF while charged; walk toward witness. Reading opens automatically. Check movement/E disabled, Shift still works.
6. Partway through, switch ON: witness and panel disappear, control returns, no testimony recorded. OFF and return/retry remains possible.
7. Esc out of testimony; confirm it stays closed. Step away and return; finish all pages. Journal gets one testimony and objective displays the station lead.
8. Reopen journal, browse all evidence, toggle world states afterward and revisit: nothing is deleted or duplicated and testimony does not reopen endlessly.
9. Restart Play, read ledger before seeing figure, then do ON/OFF and approach: alternative exploration order still works.
10. Restart Play, temporarily lower battery capacity or use Inspector debugging to exhaust it: radio shuts off, figure visible, no voluntary testimony. Refill at green station, ON/OFF and return succeeds. Restore tuning outside Play.
11. Repeat Play/Stop and read the longest pages at 16:9 and your preferred resolution; check clipping, input focus and Console. Time one first-time tester without coaching.

Run Window → General → Test Runner → EditMode and PlayMode for scene wiring, existing regressions and encounter integration. Automated tests use synthetic keyboard input and fresh scene loads; they do not establish atmosphere or a 5–10 minute human duration. See FIRST_WITNESS_VALIDATION.md for actual results and limitations.

Next three tasks: first-time playtest and text/timing revision; switch/static and witness audio cues; then one small radio-station evidence comparison beat after agreeing its provisional content. No final save, memory or identity system is needed yet.
