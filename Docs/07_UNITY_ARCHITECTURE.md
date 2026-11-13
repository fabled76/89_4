# Unity Architecture — Suggested, Not Mandatory

## Engine recommendation
Unity **6.3 LTS**, Universal **2D URP** project, simple orthographic side-on camera, layered background/foreground for 2.5D *appearance*. Avoid mixing real 3D meshes and 2D lighting until prototype proves need. Code: C#.

## Why 2D layered first
Efficient painted backgrounds, accessible camera, parallax, 2D lights on sprites, spritesheets/rigs, easy collision. 2.5D is a presentation style, **not** a distinct Unity engine mode. Actual 3D street with sprite characters costs additional modelling, materials, depth sorting, lighting complexity.

## Scene structure
`Bootstrap` (services/save/init) → `MotelPrototype` (single cohesive location). Scenes later for diner/station if time.

## Suggested components
- `PlayerController2D`: input/movement (no radio logic).
- `PlayerInteractor`: detects and triggers interactions.
- `RadioController`: ON/OFF/current battery, immutable event emission.
- `BatteryInventory`: battery items, replacement rules.
- `WorldPhaseController`: stable/drift state and environmental transitions.
- `InstabilityController`: optional intensity and thresholds.
- `WorldVariant`: per-object stable/drift visuals/collision.
- `EncounterDirector`: figure availability and progression flags.
- `ClueDefinition` (ScriptableObject): ID, UI info, links.
- `EvidenceJournal`: collected clue IDs, observations, hypothesis.
- `DeductionController`: test of selected evidence IDs.
- `RecordingService`: in-world radio records.
- `SaveGameService`: disk persistence, independent of in-world recording.
- `AudioStateController`: mixes on/off/encounter layers.

## High-level events
`OnRadioStateChanged(bool)`; `OnBatteryDepleted()`; `OnWorldPhaseChanged(WorldPhase)`; `OnClueFound(string)`; `OnRecordingCreated(id)`; `OnDeductionResolved(id)`.

Favor loose coupling / events over every MonoBehaviour searching for unrelated singletons. A single small state coordinator may be fine for jam; don't overengineer.

## State flags for slice
`SeenFigure`, `ReadGuestLedger`, `ReadBroadcastTranscript`, `ContactedWitness`, `SolvedChronology`.

## Suggested folders
```
Assets/_DeadAir/
  Scenes/
  Scripts/Core/
  Scripts/Player/
  Scripts/Radio/
  Scripts/World/
  Scripts/Investigation/
  Scripts/Save/
  Prefabs/
  ScriptableObjects/
  Art/Placeholders/
  Audio/
  UI/
```

## Implementation cautions
- Make `worldPhase` explicit state, not effects deduced from renderer color.
- Record state changes, never `SetActive` every object with unrelated scripts.
- Use `Sorting Layer` and player sprite pivot to handle pass-behind objects.
- Use Cinemachine only if camera smoothing is needed; simple follow first.
- Create scenes/prefabs in Unity Editor; let Codex write small C# scripts with testable interfaces.
- Version `ProjectSettings/`, `Packages/`, `Assets/` and `.meta` files. Exclude `Library/`, `Temp/`, `Logs/`, `Obj/`, `Build/`.
- Prefer Unity Test Framework edit-mode tests for radio state transitions and evidence conditions.

## 89.4 changes — separation of world signal and protagonist cognition
Do **not** put all consequences behind `RadioController.isOn`. Separate concerns:
- `BroadcastField` (WORKING): citywide/stationary signal strength or region signal context, mostly fixed in first slice.
- `PortableReceiver`: held radio power/frequency/charge; exposed as player equipment, always available in normal prototype.
- `PerceptionController`: player-local sound, visual and dialogue modifications while exposed to silence.
- `ObservationJournal`: facts with `Fresh`, `Anchored`, `Obscured`, `Recovered` states; ensure key evidence recoverability.
- `ContactConsequenceResolver`: on entity contact, apply nonlethal, reversible consequences without modifying durable clue collection or technical save.
- `StationaryRadioZone` (stretch): area of strengthened stability from diner/motel radios.

These are **logical boundaries**, not an instruction to immediately create six MonoBehaviours. For first 5-minute slice, implement only what is needed. Prefer ScriptableObjects for configurable thresholds, event-driven UI, and deterministic tests.

### No-death test cases
- Entity contact never calls scene restart, reduces HP or causes Game Over.
- Losing an unanchored observation never deletes source interactable or progression flags.
- `Anchored` evidence remains present across world drift and entity contact.
- A key observation can be restored by replaying recording or interacting with a second source.
- Radio out of charge → safe persistence/exit/recovery is still possible.
- Receiver ON/OFF does not accidentally rewrite the independent citywide broadcast state.
