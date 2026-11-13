# Codex — Incremental Prompts and Workflow

## Master instruction (paste at start)
"We are prototyping 89.4 (repo codename DEAD AIR) in Unity 6.3 LTS, Universal 2D URP, C#. Read all `DEAD_AIR_docs/*.md` before proposing code. Respect LOCKED vs OPTION/TBD labels. Do not invent final plot, a psychiatric diagnosis, multiple timelines, or production art. Build the smallest playable slice. Explain any required Unity Inspector/prefab setup step-by-step. Keep C# components small, testable, and decoupled. Do not silently modify ProjectSettings or install packages. Before each change, list files to touch and acceptance criteria."

## Task 1 — Baseline
"Implement a minimal side-view 2D character controller and E-to-interact using Unity Input System if already configured, otherwise document input choice. Provide clear instructions to bind references in Unity Editor. Deliver: player walks, reaches a placeholder object, presses E and receives a debug message."

## Task 2 — Radio
"Implement `RadioController` with switchable radio on/off, charge, optional battery replacement and event callbacks. Author world toggling as separate listeners. Hold tunable values in a ScriptableObject. Acceptance: Left Shift toggles radio, low charge never produces negative values, OFF can be chosen voluntarily, entity state responds."

## Task 3 — Two world presentations
"Implement stable and drift appearances for motel corridor with toggleable sprites/colors/colliders, safely preserving player location. Play crossfade sound between station broadcast and static/ambience. No random world generation."

## Task 4 — The encounter
"Implement the locked motel-corridor Rejected encounter. First contact appears threatening, radio ON suppresses, ledger sets witness flag, returning OFF unlocks approach testimony. No health bar or lethal fail states. Contact conveys recoverable memory/perception consequences only if explicitly requested. No lost critical items, no softlocks."

## Task 5 — Clues and deduction
"Add three `ClueDefinition` objects (guest ledger, official broadcast transcript, witness testimony), basic journal UI and a correct two-clue deduction. Wrong answers must be recoverable."

## Task 6 — Records & saves
"Implement independent durable `SaveGameService` and diegetic `RecordingService`. A recording stores observed facts for comparison after world shift. Disk save cannot require battery, so users can close and resume. Include a save-data version field."

## Task 7 — Test & harden
"Add automated edit-mode tests for radio transitions, battery boundary conditions, clues, encounter flags and serialization. Manual test checklist for full 5–10 min loop. Do not add features beyond acceptance criteria."

## Review checklist for every Codex PR
Compile? Unity Console clean? Editor references documented? Do changes support locked experience? All critical paths recoverable? No bloated architecture? Test physically in Unity Play mode before merging.

## Task 8 — Nonlethal cognitive contact (ONLY after core slice works)
"Implement a **nonlethal** encounter consequence that optionally obscures one recently discovered, **unanchored** observation. Anchored recordings remain safe. Show a reversible UI/audio distortion and offer at least one distinct evidence recovery method. Do not add health, death, scene restart, random erasure of permanent clues, or a full sanity bar. Include deterministic tests for the observation state machine."

## Task 9 — Local vs citywide radio signal (ONLY after prototype test)
"Separate the protagonist's portable receiver power from a world-level broadcast field. The receiver influences local perception/entity visibility; citywide signal acts as scenario context. Prototype one safe zone from a stationary radio. Avoid retroactive level rewrites; all state changes must be explicit and testable."

## Title policy
Use **89.4** as the visible working title. Leave exact station frequency as a configurable field and do not invent scientific explanations, town names or psychiatric diagnoses. Maintain old project paths until a planned rename; unnecessary refactors risk broken Unity GUID/meta references.
