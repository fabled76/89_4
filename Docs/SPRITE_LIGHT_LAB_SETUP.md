# SpriteLightLab — runtime light and soft contact proof

2026-11-19. Task-owned technical study under SPRITE_LIGHT_LAB_PROGRAMMER_BRIEF.md. **Actual Unity runtime lighting, not offline recolor.** No final rendering/art/camera approval. Static selected revision_02 translates for a lighting test; this is not a walk animation.

## Launch and controls

Open **89.4 > Studies > Sprite Light Lab > Open Saved Lab**, then Play with Game view focused,16:9 and Gizmos off. Scene: `Assets/_Game/Scenes/Studies/SpriteLightLab.unity`. Opening never rebuilds it. All references are already serialized; no manual Inspector wiring is required.

| Control | Lab action |
| --- | --- |
| A / D | Horizontal movement,1.45m/s; physical end bounds |
| 1 | Toggle warm source |
| 2 | Toggle teal neon source |
| H | Switch ambient between1.0 neutral and.32 half-shadow |
| R | Reset body to(.35,.92), zero velocity; floor settles it |

There is no E/radio/inventory/narrative action in this silent lab. Default is ambient.32 with both sources on. For unchanged base RGB: turn both off, H to1.0. No bloom, tone map or extra grade was added; diagnostics do not depend on them.

## Isolation and geometry/sprite ownership

Derived from the saved SpriteRoom shell at **8d7c232**. SourceShell.unity.txt is a construction-only snapshot; runtime architecture meshes/materials/textures are private copies under `Assets/_Game/Data/Studies/SpriteLightLab/` and `Assets/_Game/Art/Studies/SpriteLightLab/`. No ProBuilder component remains on these lab copies. SpriteRoom_ProBuilder, its furniture slots/materials and the parallel furniture import are untouched. No global pipeline/quality/package/startup change.

Primitive meshes supply wall/floor/opening depth. The hero and brown/burgundy/teal furniture/item **proxies are SpriteRenderers**, not detailed3D furniture. These rectangles are deliberate light receivers, not the designer's delivered furniture or finished art. No whole-room image, new hero or rejected R03–R05 walk is imported. The selected transparent hero and quiet wood/stucco bytes are copied from8d7c232 unchanged; sources.json records paths/hashes, with existing texture provenance/license. The2x2 white PNG is a code-authored primitive for sprite rectangles, not a generated art batch.

Camera is the shell's orthographic12-degree frame, size2.05, target(0,1.25,0), about(0,3.3291,-9.7815);16:9 projected span about7.2889x4.10m. FloorY=0, fixed pathZ=-.15. Body is.42x1.80m, gravity3, no-friction2D material, limits at approximatelyX=±2.99 for its centre. Doors/windows are geometric openings behind the fixed path, not new depth navigation/exits.

URP2D sorts renderers by sorting order as well as queue/depth. Lab architecture is **-30**, floor contact **-20**, sprite proxies **-10**, hero **20**. This prevents opaque floor/walls painting over lower-ordered sprites. Architecture keeps real depth testing/writes. Sprite planes use ZTest Always/ZWrite Off with this explicit2D order; this does not implement arbitrary3D sprite occlusion. Source markers are disabled by default; source selection shows Editor Gizmos, not baked annotations.

## Lighting method and Inspector controls

This is an **authored shared light field**, not native Unity3D Light/Light2D, measured albedo/PBR or physically correct relighting. Existing URP2D remains active. There are no disabled native Lights masquerading as the solution and no shader globals/singletons.

`SpriteLightLab_Runtime / LabLightField` references Warm_Source, Neon_Source and21 explicit renderer targets. Inspector ambient defaults.32; minimumBrightness.16 supplies a readable floor. `[ExecuteAlways]`/LateUpdate refreshes per-renderer MaterialPropertyBlocks in Edit/Play; source changes do not mutate pigment textures or shared SpriteRoom materials. Cached property IDs/block avoid per-frame string allocation. Sources need no Update manager: their current Transform and settings are read each update.

`Warm_Source / LabLightSource`: position(-1.25,1.7,.1), sRGB color(1,.83,.63), intensity1.65, range2.7m. `Neon_Source`: (1.35,1.25,-.45), color(.22,.90,1), intensity1.35, range2.3m. Both expose isOn, color, intensity, range and directionality(default.8). Move either Transform in Editor/Play to move its influence. Change neon color toward coral if desired; no new system is needed.

`SharedLightField.shader` multiplies existing base RGB by ambient plus both contributions. Falloff is cubic smoothstep of `clamp01(1-distance/range)`, zero beyond range and continuous at its edge. Colors are explicitly converted to linear vectors. Meshes use their actual normals; sprites use a plane normal with a bounded UV-based rounded/cylindrical approximation. Hero curvature1.25 and occupied-UV bounds(.4,.704,.045,.958) strengthen the source-facing side. Wrap directional response avoids crushing the opposite side. No specular/plastic response is added.

This preserves artistic shading already in RGB; it cannot recover true albedo, anatomy normals, self-shadow or furniture occlusion. Light can pass through walls because no occlusion/cast-shadow system is commissioned. Keep sources within this disclosed study. Existing RGB may limit the credibility of stronger recolighting; do not call a glow/tint physically correct illumination.

Shader channels: `_MainTex` is base RGB/unchanged coverage alpha; `_Tint` is static material pigment tint, not the dynamic light control. SpriteRenderer.color is deliberately not used by this local shader; use the material tint if needed. FlipX/Y are explicitly sent by LabLightField, avoiding hidden sprite-instance assumptions; the test flip is a facing diagnostic, not an approved automatic mirror/animation policy. Private surface textures use Repeat/sRGB; hero uses uncompressed Single/FullRect, Bilinear, no mipmaps, PPU331.03447 and semantic pivot(.5,30/640). Custom `_UvTransform` handles mesh tiling without unsupported2D `_ST` uniforms.

## Soft floor contact

`Soft_Floor_Contact_Only / LabContactOval` references LabActor_Physics. It follows root X/Z at fixed floorY, independent of body bob or visual facing. Defaults: radius.22m, depthAspect.55, opacity.24, floorOffset(0,.006,.025). Expose these directly in Inspector. The y offset only avoids z-fighting; normal light toggles do not relocate the root.

ContactOval.shader feathers an analytic floor oval as `opacity * max(0,1-r²)²`; one quad, no texture blur/render pass. No cast shadow, silhouette extrusion, per-frame shadow batch or baked duplicate is present. The selected source has transparent feet without a room-size shadow. All lab renderers have cast shadows disabled. The oval is grounding, not a physical light-direction shadow, and should remain subtle.

## Components and replacement contract

- LabActor_Physics: existing PlayerController2D + Rigidbody2D + BoxCollider2D. Hero is a camera-facing child at localY=-.9; no controller changes or extra axis. LabReviewControls owns only four Input Actions, enables/disables them with lifecycle and disposes them on destroy.
- LabLightSource: two small data components. These are not native Light objects. Source switches update the next shared-field pass.
- LabLightField: explicit sources/targets; no per-frame hierarchy search, asset regeneration or project-wide material mutation.
- LabContactOval: own Renderer/MeshFilter/material and body reference. Its position/scale/opacity update in Edit/Play, never from sprite alpha-bottom or face rotation.

To replace the hero, swap the Sprite on **Hero_Selected_Static_REV02**, retaining its material and renderer target reference. Use soft-shaded RGB plus straight alpha, no baked floor/cast shadow or strong room-specific rim. Supply actual PPU, semantic root/foot pivot, calibrated physical height and camera assumptions; do not resize each frame independently. Use common canvas/registration across idle/walk poses. FullRect/no atlas is the initial contract; packed/trimmed sprites require explicit UV remapping. Update occupied `_ShapeUv` if canvas/opaque envelope changes, rather than treating the old bounds as anatomy. The approximation may need different curvature for profile versus3/4; it does not infer a face or a correct normal map. Actual accepted future animation/phase integration is separate work. A different SpriteRenderer must be assigned to targets; replacing the existing sprite needs no new framework.

## Actual verification/evidence

The primary Editor was already open on the owner's project. It was not closed or taken over. Tests ran in a task-owned temporary Unity6000.3.8f1 project with **identical package/URP/quality/build inputs**, compiling the exact lab sources and existing PlayerController2D. Main-project global inputs were compared with that copy; only lab scene/resources/meta were copied back. No new interactive primary-Editor playthrough or unrelated whole-project regression is claimed.

Final results: **1/1 EditMode,2/2 PlayMode passed**, exit0, no unexpected logs. Reports: TestResults/sprite-light-lab-editmode.xml and sprite-light-lab-playmode.xml. Reopen test opens the saved scene twice, checks private meshes/materials/scripts/shader errors, ordering, native-Light absence, camera and no build entry. Runtime tests drive real A/D and controls, both architectural bounds, stable stops/feet/path, floor oval through facing changes, source distance/side movement, and alpha coverage.

Engine captures under **TestResults/SpriteLightLab/**,1600x900 and960x540:

- 01-neutral,02-half-shadow,03-warm,04-neon,05-both,06-neon-off-again at the identical actor position.
- 07-neon-left,08-neon-right,09-neon-far;10-facing-diagnostic.
- alpha-neutral/alpha-neon at1600, transparent-background diagnostics only.
- traversal/frame-000..035-960.png with timing.csv:36 actual engine frames over about3.53 sampled game seconds, rootX=-2.83→2.303. preview.gif is an encoded convenience preview of those frames, not an offline art render/video proof. Original PNG/timestamps are authoritative. **Static translation, not gait.**

Independent image checks: warm versus neon-off-again is identical (max RGB difference0); half-shadow versus out-of-range neon is identical. Alpha is exactly unchanged, including4140 partially covered edge pixels. Moving neon left/right reverses the stronger normalized green response across the actor; median gain ratios are about2.33 and2.16. Warm and neon affect respectively424855 and206105 pixels outside the eroded actor mask at1600. These are technical checks, not artistic scores. Programmer inspected the actual states, moved-source frames and traversal.

Measured practical cost:21 field receivers, two fixed per-fragment source evaluations and one contact quad; no shadow maps/blur/bloom.250 CPU property-block updates averaged **.0147ms per Apply** in the isolated Editor run. This excludes GPU cost, captures/readback and whole-game frame rate; no production performance guarantee. Raw cost, alpha data, frame hashes and image checks are retained in Data/Studies/SpriteLightLab/verification.json. Generated58 PNGs/GIF/CSV and test reports remain local Git-ignored review evidence.

Recommendation: keep this bounded common field for the next corrected single-sprite light proof; tune shape/ambient at gameplay size before requesting animation-wide normals. Keep the oval restrained. Do not expand into cast shadows or a new pipeline on this result. Furniture art integration remains owned by the parallel SpriteRoom task. Optional bloom/grade is intentionally omitted.

## Reproduction and preservation

Normal opening/Play requires no builder. SpriteLightLabBuilder.Create is an explicit create-once utility using the saved shell snapshot; it does not overwrite an existing lab. Construction includes the final UV/import/sorting settings; no repair or regeneration runs on open/Play. Do not regenerate the owner's SpriteRoom to recreate this test.

protected_hashes.json records unchanged current-game/startup/pipeline inputs. SourceRoom/current scenes matched HEAD at copy-back; no furniture-slot, material, source-character or audio edits were made by this task. Scoped commit includes only lab shaders/code/resources/scene/meta/tests/setup, not unrelated working-tree changes. No push. The final commit is reported with delivery (`git log -1 --oneline -- Docs/SPRITE_LIGHT_LAB_SETUP.md`).
