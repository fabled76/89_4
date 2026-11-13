# Unity Learning Path — Learn by Building 89.4 (DEAD AIR codename)

## Strategy
Do not learn the entire Unity engine before coding. Build a tiny vertical slice while learning each editor concept deliberately. AI/Codex can implement repetitive scripts, but you should understand scenes, prefabs, components, inspector bindings and debugging.

## Camera/art options
1. **True 2D side-view**: Sprites on an XY plane, orthographic camera, colliders, 2D lights. Fastest prototype.
2. **Layered 2D / pseudo-2.5D (recommended)**: Same as #1 plus parallax, scale/occlusion sorting, moving light/fog, foreground; depth *illusion*.
3. **True hybrid 2.5D**: 3D geometry with a fixed camera and 2D characters. Useful for camera movement, but harder materials, lighting and collisions.
4. **Full 3D**: Least suitable under jam deadline for a painted mystery adventure unless existing assets/expertise.

## Sessions
**Session 1 — Editor foundations (60–90 min):** Unity Hub; create Universal 2D; understand Scene / Game / Hierarchy / Inspector / Project / Console; transform, GameObject, component, prefab; camera view; press Play.

**Session 2 — Movement (60–90 min):** 2D character + Rigidbody2D + Collider2D + player movement script; understand `Update` versus `FixedUpdate`; sprite sorting, triggers, collisions.

**Session 3 — Interaction (60–90 min):** proximity + `E` prompt, inspect interactable, journal clue ID, Unity Events or interface.

**Session 4 — Radio SHIFT (90–120 min):** ON/OFF state machine, 2 variants of one object, audio crossover, visual tint, radio UI, battery.

**Session 5 — Investigation (90–120 min):** guest ledger clue, figure, revisit, testimony and simple evidence combination.

**Session 6 — Persistence (60–90 min):** technical save state and fictional recording separated; quit/reload test.

## Exercise zero
Create a 16:9 Unity scene with one rectangle character, background rectangle, one interactable doorway. Use only primitives. Press Play, walk, press E, show text. Next add radio ON/OFF to hide/unhide a ghost rectangle and change a light. This proves the engine pipeline without getting distracted by asset style.

## Useful official documentation
- Unity 6 releases: https://unity.com/releases/unity-6/support
- Unity 2D art guide: https://learn.unity.com/article/2d-in-unity-tech-art-essentials
- 2D lights: https://docs.unity.cn/Manual/urp/Lights-2D-intro.html

## Learn to debug (critical with Codex)
Read Console errors; distinguish script compile error vs missing reference vs physics trigger vs sorting issue; view inspector during Play mode; understand Play mode changes usually aren't persisted. Ask Codex for *why* a fix works, then inspect it yourself.
