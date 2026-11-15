# v05 motion slice — programmer delivery contract

2026-11-15. Read with ART_PILOT_SILVER_PINES_BENCHMARK_V05.md, current Art/Narrative Bibles and the published v04 HANDOFF. This is a bounded integration contract, not an animation framework or new cast/style approval.

## Inspected baseline

Live ArtPilot remains v03, commit f891631. v04 is now published offline and its HANDOFF was inspected; no v04 art was imported. Its sources/notices are preserved. Prioritize complete v05 rather than a disposable full v04 import under the newer assignment.

- PlayerController2D uses Rigidbody2D horizontal A/D; current speed4m/s is serialized per instance. OnDisable stops horizontal velocity. No root-motion integration exists. Match the study instance to the delivered gait without changing the shared controller.
- ArtPilotHorizontalVisuals follows world-X displacement in LateUpdate for static sprites/contact shadow. Its old frame centre/reference root must not be silently reused for a changed v05 camera/layout.
- No `.anim`, Animator Controller or Override Controller assets were found under Assets/_Game; no Animator component exists in this pilot. Walk/idle/listen frame playback is not implemented. SpriteRenderer can display the frames; a small study-only component can swap them after the real contract is known. No Animator graph/package is required in advance.
- InteractionController dispatches E to the nearest valid IInteractable. ArtPilotReceiverToggle currently consumes E at the receiver and exposes SetVisible for both layer roots. Replace that E role locally with listening; do not leave two active E targets on the same proxy. Keep removal as a separate debug check.
- Existing capture code exports actual Unity PNGs. ffmpeg was not found on PATH; Unity Recorder is not a direct dependency. Do not install a toolchain pre-emptively. A timed Unity frame sequence is an allowed fallback, distinct from Blender's offline preview.

## READY delivery requirements for the designer

Publish a complete versioned manifest/HANDOFF only after all referenced files exist. Pin every layer/frame/source/notice hash. Do not depend on a mutable parent manifest or half-written outputs. Carry the delivered human-source copyright/license notices with imported derivatives.

1. **Camera/layout:** exact transform, basis/quaternion, orthographic projection/span, canvas/aspect, axes/metres and image-plane centre; old/new camera and reason. Supply pathX limits, floorY/pathZ, initial root/foot, desk listening stop and valid interaction area. Human standing reference remains1.80m; measure the model, not its alpha crop. Disclose yaw/roll that changes projected horizontal movement. No v03/v04 PPU/coordinates will be assumed compatible.
2. **Raster registration:** actual filenames/hashes, canvas, crop offsets if any, PPU, common semantic root/foot pivot, sorting/depth and baked light/finish ownership. Prefer one common frame across poses; do not independently trim/recentre/stretch frames. Alpha bottom is not automatically the physical foot. Deliver clean background without fixed person/radio/removable shadows, separate radio/shadow, character frames/shadows and any required foreground. Publish actual roles; actor ID need not be reference_person and the package need not have exactly five layers.
3. **Walk timing/distance:** ordered frames, per-frame duration or FPS, loop boundaries, supported direction, cycle travel distance in metres and foot-contact phases/events. Explicitly declare **in-place versus baked root displacement**. In-place is preferred with existing physics. If displacement is baked, provide per-frame root offsets/cycle distance so translation is applied once. Do not infer stride from image width or held-key time. Supply a recommended speed and valid short route.
4. **Stop/listen:** settled standing frame, listening frames/pose, actual enter/exit transitions if delivered, durations and hold/loop behavior. Keep common foot/root registration and declare the intended planted stop. If transitions are absent, state that rather than claim animation. Natural gait bob must not become per-frame scale correction.
5. **Direction policy:** one authored direction is sufficient. Declare its movement sign and whether mirroring is permitted under asymmetric light/clothes. No blind RGB/shadow mirroring. An unsupported return direction needs a study restriction/reset or explicitly labelled debug fallback; it cannot be presented as a second validated gait. This does not commission another frame batch.
6. **Motion shadows/occlusion:** matching shadow frames/timing/offsets or explicitly bounded reusable profiles; lighting validity across the route. Test the whole walking/listening envelope against desk/chair/floor, not just idle. If foreground is unnecessary, explain clearance for all those poses; otherwise publish order/masks. No fixed duplicate character/shadow in the background.
7. **Reference assembly:** matched idle/walk-phase/listen/removal states with known root coordinates/frame indices for numerical engine comparison, plus offline motion preview. Retain source/finish recipe, provenance and explicit geometry/UV/layout changes. A translated contact profile is not dynamic relighting.

## Smallest planned behavior after delivery

Reuse existing body/input, proximity query and unlit prelit sprites. Match phase to actual **travelled distance**: blocked movement must not march in place, and baked motion must not be added twice. Stop in the declared settled pose rather than slide a still.

Near the authored desk stop, fresh E enters listening, stops/temporarily disables study movement and leaves radio on desk. Fresh E exits and restores the prior control state with a held-input release gate where needed. Keep state local to this review; add no dialogue/evidence/inventory/save/audio/game-radio changes. Keep receiver+shadow removal behind a separate documented debug method/test.

No new component is implemented speculatively. A short serialized clip/frame list in one study component should suffice unless real delivery demonstrates otherwise.

## Evidence after actual import

Export Unity1280x720/960x540 stills and move → stop → E listen → E exit → resume engine footage. If no encoder is available, provide a timed Unity PNG sequence with clip indices, time/root displacement and reproducible controls; offline previews are not engine evidence.

Check foot plants/phase versus distance, floor height, stops/blocked motion, E enter/exit/release, every frame's alpha over wall/door/floor, foreground overlaps, shadow ownership and debug radio+shadow removal. Compare matched source/engine states under the published color/alpha contract. Technical tests/stills alone do not approve motion or art.

Preserve v02–v04 sources, current game/startup/audio and neutral blockout. Update ART_PALETTE_C_PILOT_SETUP.md with exact imported version, controls, captures/checks/limits and scoped commit. No published visual_slice_v05 manifest/HANDOFF was found during this inspection; no v05 art/code was imported and no new Unity test result is claimed.
