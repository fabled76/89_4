# Motel Corridor foundation — Unity guide

This is a mechanics foundation for **89.4**, not the complete narrative vertical slice in the design pack. `Docs/00_README.md` through `12_TITLE_AND_SIGNAL_GEOGRAPHY.md` remain design context. No new town name, witness identity, diagnosis, frequency rules or plot solution is established. STABLE/DRIFT here describe **local corridor presentation**, not citywide history. There is no AI, harm, inventory, save/load or diegetic recording system.

## Open or recreate

The repository already contains Unity **6000.3.8f1** with Universal 2D/URP and Input System **1.18.0**, and Active Input Handling is already Input System. No package installation or settings change is needed.

1. Unity Hub → Projects → Add → Add project from disk → select this repository root. Open with 6000.3.8f1. Let import and compilation finish.
2. Open `Assets/_Game/Scenes/MotelCorridor.unity` if present. If it has not yet been generated, use **89.4 → Create Motel Corridor Prototype**. Save any open scene when asked. The command creates a new scene and assigns all references. It refuses to overwrite an existing corridor scene. To make another version, move/rename the original through the Project window first.
3. Open Window → General → Console. There should be no compilation errors. Select Game view, choose 16:9, press Play, then click in Game view to give keyboard focus.
4. Use A/D or left/right arrows, E, Left Shift. Walk right to the green battery station at X=-8; the inspection marker is at X=1; the silhouette is at X=7.
5. Exit Play before changing tuning or saving the scene. Changes made during Play usually revert.

If creating an unrelated fresh project: Hub → Installs → install Unity 6.3 LTS; Projects → New project → select that editor → **Universal 2D** template → choose name/location → Create. Do not use the Built-In 2D template. This repository does not need that creation step.

The builder uses an unlit white sprite with color tints; URP 2D lighting is deliberately not required yet. Doors are background decoration. Floor, side walls and ceiling have solid BoxCollider2D components. The silhouette has no collider, interaction or behavior. Camera size is 4.5; visible-world bounds are (-15,-2) to (15,7). An unusually wide/tall viewport may show beyond an axis smaller than the viewport; that axis is centered.

## Folder structure

```text
Assets/_Game/
  Scenes/                 MotelCorridor.unity
  Scripts/
    Player/               horizontal physics movement
    Interaction/          proximity selection and test objects
    Radio/                separate battery, power and keyboard components
    World/                local presentation listener
    Camera/               bounded orthographic follow
    UI/                   read-only HUD listener
  Editor/                 reproducible scene construction
  Tests/Editor/           scene wiring and shared-collision validation
  Tests/PlayMode/         battery/radio boundaries and corridor integration
  Prefabs/                reserved; scene objects first
  Art/Placeholders/       generated white square
  Audio/                  reserved
  Data/                   player friction material; future tuning assets
  UI/                     reserved for UI assets
```

Empty reserved folders do not imply implemented systems. Prefabs and ScriptableObjects are unnecessary for one corridor. Inspector fields keep this first pass easy to tune; create shared tuning assets when multiple scenes genuinely need them. Preserve Unity-generated `.meta` files in version control. `.gitignore` excludes caches and local logs.

## How the components fit together

`PlayerController2D` only moves. `InteractionController` finds objects implementing `IInteractable`. `RadioInput` requests a toggle from `RadioController`. The radio drains `BatteryComponent` while operating; depletion switches it off. `WorldStateController` and `PrototypeHUD` listen for changes and read initial state when enabled. No global manager or singleton is needed. Events unsubscribe on disable; input actions enable/disable with their component and dispose on destruction.

InputActions are created in code using the installed Input System. No PlayerInput component, input-action Inspector reference, generated C# wrapper or edits to the template's `InputSystem_Actions` asset are required. Bindings currently live in the scripts; remapping is a future task.

## Component setup, tests and debugging

The builder does all assignments below. These details also describe how to reconstruct the scene manually. Inspector fields appear with spaces (for example, `stableVisuals` → **Stable Visuals**). To connect a reference, drag the named object from Hierarchy into the field.

### Player movement — PlayerController2D

- Attach to **Player**. Requires Rigidbody2D and BoxCollider2D (automatically added if absent). Add a SpriteRenderer for visibility.
- Rigidbody: Dynamic, Gravity Scale 3, Freeze Rotation Z, Interpolate, Continuous collision detection. Box collider is solid. Assign `Data/CorridorPlayer` physics material (zero friction/bounce). Move Speed defaults to 4 units/second. No object references.
- Read input and set X velocity in FixedUpdate; preserve Y velocity so gravity and floor collision still work. Transform teleportation is not normal movement.
- Test both key pairs and hold against each wall; release and check movement stops. Watch Rigidbody velocity and position in Inspector.
- If falling through: check solid **2D** colliders, enabled simulation and collision layers. If rotating: Freeze Rotation Z. If keys fail: focus Game view and verify this component is enabled. No jumping is implemented.

### Interaction — IInteractable / InteractionController

- Attach InteractionController to **Player**. No required Inspector references; Radius defaults to 1.8 and Interaction Layers includes all layers. A selected player draws the radius in Scene view.
- Every interactable needs a Collider2D on itself or a child, and a MonoBehaviour implementing IInteractable on itself/parent. Detection explicitly includes triggers. Candidates must be enabled, active and return true from CanInteract. Nearest means distance to collider surface; equal distances use instance ID as a stable tie-break within the run.
- E queries again that frame and invokes one nearest valid candidate. The HUD polls its prompt in LateUpdate. This prototype has no line-of-sight filter; keep usable objects on the corridor's accessible side. Future rooms will need occlusion rules.
- Attach **InspectInteractable** to TestInspectionMarker, add a trigger BoxCollider2D; edit Prompt and Message if desired. E writes to Console; it does not collect evidence or establish a story fact.
- Test E at the marker, then move beyond the radius and confirm no prompt/message. To test overlapping candidates, duplicate a marker, put it closer, give each a distinct Message; only the nearer valid one should log.
- If detection fails: check interaction layer mask, collider activity and component placement. If solid object blocks walking, set its interaction collider to Is Trigger. Use Unity 2D components throughout.

### Battery — BatteryComponent

- Attach to **Player/PortableRadio**. No references. Capacity defaults to 100; Starting Fraction to 1. Charge is runtime state, not an editable stored asset.
- Consume clamps at zero, ignores negative/NaN input, and publishes depletion once per transition. ReplaceBattery restores capacity. The battery knows nothing about keyboard, world or UI.
- Test depletion by reducing Capacity to 5 outside Play, then playing (radio drain defaults to 5/second). Restore it afterward. Starting Fraction 0 tests an empty start.
- This is intentionally a short **20-second** full battery at default tuning; it is a test convenience, not agreed game balance. For atmosphere testing, set the radio drain to 0.2 for about 500 seconds.
- Runtime charge resets on a new scene instance; there is no save/load. Capacity changes should be made outside Play.

### Radio — RadioController / RadioInput

- Attach both to **Player/PortableRadio**, alongside BatteryComponent (required by RadioController and automatically added). No references: radio looks up its battery on the same object; RadioInput looks up its radio.
- RadioController exposes Start Powered On and Drain Per Second. It owns only receiver power and operating drain. RadioInput owns the Left Shift InputAction. Movement never references either.
- Test Shift twice at full charge, then wait until empty and press Shift again. Empty radio must stay off. Disabling radio switches it off; re-enabling after initial Start leaves it off until toggled.
- If it never starts: inspect Starting Fraction and component activity. Check that the battery is on the same object. A replacement does not automatically start playback/power.

### Replacement — BatteryReplacementInteractable

- Attach to **TestBatteryStation_Unlimited**, add trigger BoxCollider2D; assign **Battery** to Player/PortableRadio's BatteryComponent.
- E refills that battery and logs confirmation. The unlimited station is a prototype recovery tool, not an inventory or an in-world stationary radio/safe zone.
- Empty the radio, walk back to the green station, press E and then Shift. The charge should refill and power should turn on only after Shift.
- If nothing happens, check the Battery reference and interaction radius. Test while fully depleted to distinguish a refill from no visible percentage change.

### World presentation — WorldStateController

- Attach to **LocalCorridorPresentation**. Assign Radio = Player/PortableRadio; Stable Visuals = Environment/STABLE_Visuals; Drift Visuals = Environment/DRIFT_Visuals.
- It listens to radio state and immediately enables exactly the appropriate visual root. Put the silhouette under DRIFT_Visuals. Keep Player, radio, controller, HUD and SharedCollision outside both roots. Never put the controller under a root it disables.
- Toggle and watch warm/cool corridor colors and silhouette visibility. Depletion must have the same result as voluntarily switching off. Disabling/re-enabling the listener resynchronizes current state.
- Missing radio logs an explicit error. Missing visual references produce no corresponding appearance: inspect all three assignments. Scene view can show inactive selection outlines; judge visibility in Game view.
- This switch has no persistence, city signal or story consequences. It does not restore history or move the player.

### Camera — CameraFollow2D

- Attach to **Main Camera**, alongside Camera; assign Target = Player transform. Orthographic enabled, Size 4.5, Z=-10. Bounds defaults above, Follow Speed 8. One AudioListener is on the camera for future use.
- LateUpdate follows the player after movement, clamps the viewport within bounds, and preserves Z. It snaps at startup and smooths subsequent motion. No Cinemachine dependency.
- Walk both directions and watch the frame stop before the end walls. Test 16:9 and 4:3 Game aspect ratios.
- If blank: verify camera Z, orthographic mode, sprite sorting, camera culling mask and target assignment. Door/background order is negative, props 1, player 2. Later parallax can use separate environment children and sorting layers without changing physics.

### HUD — PrototypeHUD

- Attach to **PrototypeHUD** root Canvas. Canvas = Screen Space Overlay; CanvasScaler = Scale With Screen Size, reference 1280×720. Uses installed uGUI Text for the smallest placeholder (no TMP font import).
- Assign Radio and Battery from PortableRadio, Interactor from Player, Status Text = RadioAndControls, Prompt Text = InteractionPrompt. Text children use built-in LegacyRuntime.ttf. The HUD listens to battery/radio events and reads state initially; prompts update after interaction selection.
- Test initial ON/STABLE, Shift → OFF/DRIFT, battery percentage while on, no change while off, depletion and refill. An EventSystem is unnecessary for this read-only HUD.
- If text missing, check Canvas activity, text references/font/color and Game view size. A missing required reference logs an explicit error and disables the HUD. Small rounding can show 0% just before actual zero; power uses exact remaining charge.

## Manual acceptance pass

1. Clear Console, enter Play, confirm player rests on floor, initial radio ON/STABLE and battery decreasing.
2. Walk A/D and arrows to both ends, continue holding against walls; no penetration, falling, rotation or jitter that prevents play.
3. E at the inspection marker logs once per press; out of range does nothing. Duplicate/offset markers to check nearest selection, then remove the duplicate outside Play.
4. Toggle Shift: OFF shows blue corridor and silhouette; ON hides it. Confirm OFF conserves battery. Hold Shift: it should toggle once, not every frame.
5. Let charge reach zero: UI OFF/DRIFT immediately; Shift cannot start it.
6. Walk to the green station while empty. E refills; UI updates; Shift can start again. Repeat to ensure recovery is unlimited.
7. Move while toggling near a wall: collision remains identical in both variants.
8. Exit and re-enter Play twice with Console visible; verify reset, correct initial display and no exceptions. Test normal editor reload and, optionally, Enter Play Mode Options with domain reload disabled.
9. Use Window → General → Test Runner → EditMode → Run All for scene wiring validation, then PlayMode → Run All for battery/radio boundary and corridor integration tests. The PlayMode harness temporarily ignores input focus on a cloned InputSettings object, then restores the original settings; normal gameplay still requires Game view focus.

To try a standalone build later: File → Build Profiles → select Windows → Scene List → add the open MotelCorridor scene and make it the first enabled scene (remove/disable SampleScene for that build). Then Build and Run to a disposable `Builds/` subfolder. The prototype does not change your project's scene list automatically. A standalone player build has not been verified.

Do not interpret this as proof of a complete detective loop. Audio feedback, witness approach, ledger, testimony, clues, deduction, records and durable saves remain future work.

## Next three small tasks

1. Playtest/tune walking, framing, battery duration and prompt legibility; record feedback from one new player.
2. Add placeholder switch/static sounds and two ambient audio layers reacting to the existing radio event.
3. Build the confirmed witness reversal as a separate small slice: ledger identification, deliberate return in DRIFT and testimony; agree placeholder text before adding narrative facts. No combat or memory-loss system.

## API references

The prototype uses Unity's physics and Input System directly: [Unity 6.3 Rigidbody 2D](https://docs.unity.com/en-us/engine/6000.3/manual/unity2d/2d-physics/rigidbody-2d/introduction-to-rigidbody-2d) and [official Input System action callbacks](https://github.com/Unity-Technologies/InputSystem/blob/develop/Packages/com.unity.inputsystem/Documentation~/set-callbacks-on-actions.md). The package source in this repository's Library/PackageCache is the installed-version reference.
