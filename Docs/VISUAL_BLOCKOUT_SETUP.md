# Measured VisualBlockout study — v01

2026-11-15. Separate comparison source for `VISUAL_DIRECTION_COMPARISON_BRIEF.md`, not a new game route or final camera/style decision.

## Open and export

1. Open this project with Unity **6000.3.8f1**. Package Manager resolves official **ProBuilder 6.1.4**. It was installed with `UnityEditor.PackageManager.Client.Add`, not copied from an external package. Unity's [6000.3 package listing](https://docs.unity.com/en-us/engine/6000.3/manual/packages-list/packages-all/pack-safe/com-unity-probuilder) lists 6.1.4 as released.
2. Open `Assets/_Game/Scenes/Studies/VisualBlockout.unity`, or use **89.4 > Studies > Open Measured Visual Blockout**. This opens the saved scene if present. Do not delete it or regenerate it to apply changes.
3. Use 16:9 Game view at 1920x1080, 1280x720 or 960x540, with Gizmos off. The scene has no HUD or audio. Play uses the existing PlayerController2D: A/D walk, E invokes a study-only interaction log. W/S/arrows/depth movement are absent.
4. Outside Play, save deliberate study edits, then use **89.4 > Studies > Export Measured Blockout Handoff**. The exporter opens the saved study; it fails if the scene is missing. It does not rebuild geometry or save temporary poses/camera experiments. It overwrites only the bounded `blockout_v01` output files, so version/copy them before making a new approved study.
5. Run `python output/art/visual_direction_comparison/blockout_v01/verify_handoff.py` to check the actual exported geometry, camera anchor projection and image sizes, and regenerate verification/hashes.

Batch export uses `-batchmode -projectPath <project> -executeMethod DeadAir.Editor.Studies.VisualBlockoutExport.BuildAndExport -quit -logFile <log>`. The package setup helper is **batch-only** and exits the Editor when its async request completes; do not invoke it in an interactive Editor. Normal project opening resolves the pinned manifest without that helper.

## Scene wiring and lifecycle

- `MeasuredGeometry`: named groups for room, door, desk, chair, bed, travel bag, receiver, calibration cube and reference person. Geometry uses persistent ProBuilder Mesh assets under `Data/Studies/VisualBlockout/Meshes`; no transient mesh cache is needed to reopen it. Unit transform scales are preserved. No prefabs or generated final art are required.
- `ReferencePerson`: neutral segmented 1.80 m mannequin, Rigidbody2D, BoxCollider2D, existing PlayerController2D and InteractionController. Rigidbody motion is on XY with Z=.25 fixed. Its visual meshes are children of the body; foot support is Y=0. Existing controller input actions follow Awake/OnEnable/OnDisable/OnDestroy; no second controller or animation rig was introduced.
- `HorizontalPathPhysics_2DOnly`: floor and two end walls, separate from 3D presentation meshes. Furniture is staged behind the path. Mesh geometry does not silently define walkable depth.
- `Door_TransitionProxy`, `Receiver_ReviewProxy`, `Bag_InspectProxy`: BoxCollider2D triggers and existing InspectInteractable components. Inspector messages describe study interactions; E logs them. They do not load rooms or grant evidence. Existing InteractionController uses overlap queries, not OnTriggerEnter, so no new trigger-event script is required. Its 1.8 m radius is the historical prototype default, not final literal hand reach.
- `BlockoutCamera`: one orthographic camera; no AudioListener/UI. URP Unlit neutral face-value materials use the existing Renderer2D and depth buffer. Gray values separate silhouettes; there are no production textures, custom shaders, light rigs or final palette choices. The one-time `RepairNeutralContrastAndExport` utility changes only study materials/renderer assignments and saves that scene; normal export never calls it.

All required references are serialized. No Inspector assignment is required to inspect or play the committed scene. If shapes are pink, check URP/package import; if meshes disappear after reopen, check the mesh asset references rather than rebuilding the scene. If the Game view crops horizontally, select 16:9: Camera does not serialize the Editor Game-view aspect. Export explicitly sets 16:9 for renders, guide and manifest, then restores the prior aspect.

## Selected test composition

World units are metres; Unity +X right, +Y up, +Z towards the rear wall. Room interior X=[-3.2,3.2], Z=[-1.5,1.5], Y=[0,2.6]. A/D uses the fixed path Z=.25. Person root=(.9,.9,.25), feet Y=0, head top Y=1.8. This is a dimensional reference without protagonist identity/costume.

Saved camera position approximately **(0,4.29017067,-9.2605648)**, Euler **(18,0,0)**, target **(0,1.2,.25)**, orthographic half-height **2.25**, near/far **.1/30**. At 16:9 it covers **8 x 4.5 projected metres**, or 240 pixels/projected unit at 1920x1080. Exact transform, quaternion, basis and row-major matrices are in the exported manifest. OBJ does not contain a camera.

Candidate 8 degrees was rendered in `diagnostics/camera_candidate_08_1920.png`; it gives about 42 px between rear seam and path. The selected 18-degree test gives about 93 px there, with about 130 px from path to nominal front opening. It remains provisional and does not replace the current game camera. The coplanar floor apron continues to the viewport bottom; about 266 px remain from path to bottom, an explicit framing exception to the Art Bible's initial front-band suggestion. No platform front is shown. The magenta line measures contact at path Z=.25; the nearer toe projects slightly below it, with Y still zero.

Door leaf .85x2.10x.04, handle Y=1; desk 1.35x.65 footprint/top .74; chair seat .45; bed mattress 1.95x.90/top .50. Bed headboard adds .04 to overall X footprint and reaches .90 high. Bag overall XYZ=.45x.30x.25 includes handle. Receiver body XYZ=.18x.11x.05, antenna separate. Calibration cube at (5,.5,0) is exactly 1 m on every axis and intentionally outside the frame. Floor slab 8.8x6.1 extends beyond the nominal room as a presentation apron, not traversable room depth.

Standing evidence moves only the person root in memory to X=-2.25 (door), -.4 (desk), 1.3 (bed), retaining Y=.9/Z=.25 and the same camera. Reference meshes remain unchanged. Door handle is 1.185 m behind the path: future gameplay needs a controlled transition/cut, not an implausible reach or a new movement axis. Receiver is .60 m behind the path; chair/bed leave the current walking lane clear. These are comparison constraints, not animation/contact approval.

## Actual verification and limits

- Real Unity URP exports: neutral 1920/1280/960, separate annotated 1920 guide, three standing poses at 1920/960, diagnostic 8-degree frame, named OBJ/MTL and camera/layout manifest. Output: `output/art/visual_direction_comparison/blockout_v01/`.
- Fresh scene-open EditMode check: **1/1 passed**, persistent 49 meshes, unit scales, dimensions/height, clear chair lane, separate camera/no HUD/not in Build Settings. Report `TestResults/visual-blockout-editmode.xml`.
- PlayMode check: **1/1 passed**, existing A/D input traverses all three X positions, constant Z/feet height, door/receiver nearest targets, both walls block movement, no unexpected runtime logs. Report `TestResults/visual-blockout-playmode.xml`.
- Independent Python report and SHA-256 list accompany the handoff. They check triangulation/winding, OBJ bounds against all per-mesh manifest entries, calibration, camera reprojection and PNG dimensions. Programmer visually inspected the resulting neutral/guide/standing frames. No Blender import/render or human art approval is claimed.
- `CurrentPrologue`, `CurrentMotel` and working Build Settings hashes stayed unchanged. No art/audio assignment or current-game input changed. Package diff adds ProBuilder 6.1.4 plus transitive settings-manager 2.1.1; existing URP/ShaderGraph remain 17.3.0. No third-party dependency.

The designer's A/B/C studies, final camera/style/PPU, character production, animation, lighting and gameplay migration remain separate future work. Use the actual output base/OBJ/manifest as their shared geometric source, not the older painted room.
