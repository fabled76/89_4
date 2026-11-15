# Palette C ArtPilot — isolated preparation

2026-11-15. **Preparation only; designer delivery and artistic integration pending.** Authority: `ART_PALETTE_C_PILOT_BRIEF.md` stage 2 and Art Bible v0.2. Palette C is selected for development; final style/camera/character approval is not implied.

## Single launch entry

Use **89.4 > Studies > Open Palette C ArtPilot**, then Play with Game view focused, 16:9, Gizmos off. Scene: `Assets/_Game/Scenes/Studies/ArtPilot_PaletteC.unity`. It is intentionally outside Build Settings. Current game/startup and the saved VisualBlockout remain unchanged.

The scene is a one-time copy of saved VisualBlockout, not regenerated geometry. `ArtPaletteCPilotSetup.Prepare` opens an existing pilot without overwriting it. Do not delete/rebuild the pilot to apply future art. No extra package is required: existing Unity6000.3.8f1/URP17.3.0, Input System and ProBuilder6.1.4 are reused.

## What exists now

- Same 49 mesh references, transforms, 1.80 m neutral figure, room and fixed horizontal path. Camera remains the source's 18-degree orthographic test, size2.25, position(0,4.29017067,-9.2605648), 8x4.5 projected metres at16:9.
- A/D uses existing PlayerController2D; E uses existing InteractionController on the reference person. Static mannequin translation is placement/traversal verification, **not walking animation**.
- `Receiver_ReviewProxy` retains its separate BoxCollider2D trigger but replaces its generic InspectInteractable with `ArtPilotReceiverToggle`. E hides the separate `MeasuredGeometry/Receiver` root, E again restores it. Its trigger stays active so the check is reversible. It grants no item, evidence or narrative flag.
- `ArtPilotReceiverToggle.receiverVisual` is serialized to that neutral receiver. `receiverShadow` is deliberately **unassigned** because no artist shadow is delivered. Once assigned to a separate removable contribution, the same method toggles both. It must never reference the trigger itself, character shadow, full room plate or global lighting.
- Marker `ArtPilot_PaletteC_PENDING_DESIGNER_LAYERS` makes the pending status explicit in Hierarchy. There is no study HUD, sprite substitution, new light, grade, shader or inventory/save framework.

The component has no input action/lifecycle subscriptions of its own: existing interaction Update invokes IInteractable.Interact. Serialized references avoid runtime searches. SetVisible changes only the two explicit visual roots and leaves interaction/physics available. No OnTriggerEnter handler is needed because InteractionController queries nearby trigger colliders.

## Source mapping and hashes

Current source: `Assets/_Game/Scenes/Studies/VisualBlockout.unity`, SHA256 **B0179DF416E1BA5644DFB5FBEA0E6B623CCA331F288E2EC03EEA70697AD853B0** (blockout commit `ccd36e9`). All 49 meshes and neutral materials remain shared under `Assets/_Game/Data/Studies/VisualBlockout/`. **Do not edit those shared assets to style the pilot**; create pilot-specific materials only if required by actual delivery.

Prepared ArtPilot scene SHA256: **ebef28751cac4374a2fc7386860680e676ac4b8832edb98a1c2cb58aae8f7cd2**.

Camera/geometry authority: `output/art/visual_direction_comparison/blockout_v01/camera_layout_manifest.json`, with its SHA256SUMS. No prior A/B/C composite has been assigned to ArtPilot.

Expected artist source: `output/art/visual_direction_comparison/palette_C_pilot_v01/`. It was absent at initial inspection; by the end of preparation it contained only `tools/PilotComposite.cs`, without a completed layer package, manifest or HANDOFF.md. **Imported artist files: none. Source-to-runtime layer mappings/hashes: pending.** Do not import a preview/comparison sheet in their place. Designer editable sources stay outside Assets; selected runtime copies must retain exact source mappings/provenance and file hashes.

## Delivery gate and intended wiring

After real files arrive, read their actual manifest/HANDOFF before importing. Do not assume filenames beyond the brief or invent pivots to compensate for a crop. Check full-canvas dimensions, offsets, image/mesh registration, ownership of baked light/grade and which pixels actually belong to removable objects.

1. Assign the clean base at original framing. It must contain neither receiver nor its removable shadow. Disable only superseded neutral presentation, retaining measured references for comparison and existing 2D physics/path. Preserve camera/figure dimensions; no nonuniform stretching.
2. Keep receiver and its shadow in separate visual roots referenced by ArtPilotReceiverToggle. Retain the proxy. Verify E leaves no baked receiver/reflection/shadow duplicate and can restore both.
3. Use the supplied foreground/desk overlap layer and documented layer order. Assign a character contact shadow/profile only if actually delivered; attach it to the foot anchor, not the torso. A baked static character shadow in the base cannot support moving traversal.
4. Treat already-lit photographic sprites as unlit. Apply each baked/masked light or grade once according to the handoff. No extra URP light/volume may silently double it.
5. Reuse a second existing prop through the same delivered recipe if possible. Currently there is no delivered photographic recipe to test: the neutral bag/bed are not evidence of material reuse.

If these requirements cannot be met by the actual layers, record the exact missing/corrected layer instead of manufacturing another art batch or silently editing designer sources.

## Verification and remaining work

Preparation tests:

- `ArtPaletteCPilotSceneTests`: compares every saved mesh reference/position/scale and camera to VisualBlockout, checks no sprites/lights were substituted, receiver reference assigned, shadow explicitly pending, and no Build Settings entry.
- `ArtPaletteCPilotTests`: drives A/D to three X positions inside the floor, returns to the receiver, invokes actual E proximity interaction twice, then verifies the optional shadow-reference toggle with a **temporary empty test object**. That object is not an imported/delivered shadow and is never saved.

Actual Unity6000.3.8f1 batchmode results: **1/1 EditMode passed, 1/1 PlayMode passed**, both exit0, no failures/skips. Reports: `TestResults/art-palette-c-editmode.xml` and `TestResults/art-palette-c-playmode.xml`; logs use matching names under `Logs/`. No new artistic screenshots or interactive human Editor pass are claimed at preparation stage.

After delivery, capture actual Unity frames at all three standing positions at1280x720/960x540, before/after receiver removal, under `TestResults/ArtPaletteCPilot/`. Record actual screenshot paths and source hashes here. Inspect pivots, floor contact, foreground overlaps, light/dark/teal alpha fringes, texture density, single shadow/grade ownership and readability. Do not label static translation as finished animation. These art checks, second-prop reuse, imported layer mappings, human visual review and final style approval remain **pending**.

Preserved SHA256: CurrentPrologue `9714FA8E5C9F9829F6D25A5D90D2D35407275195C2EF57BAE2895274E02F5FC1`; CurrentMotel `23C2595B98DCDE067A12B2FA6390B66F5112580AA56CFFD4E30377A5AC48DAFB`; working EditorBuildSettings `DDE43D1C2DA8E4791FC568274D20AE695E8974E777D462848E32CA4BCC35933C`. The preparation commit hash is provided in the delivery message and can be retrieved with `git log -1 --oneline -- Docs/ART_PALETTE_C_PILOT_SETUP.md`; no push is authorized.
