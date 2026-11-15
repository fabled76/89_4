# Palette C ArtPilot — integrated v02 technical baseline

2026-11-15. **Actual isolated Unity integration of material_revision_v02.** This muted color version is a technical baseline; the owner requested a more saturated v03 separately. No final color/style, camera, protagonist or animation approval is implied. Delivery is available; the earlier artist usage-limit/readiness wait is historical.

## Launch and controls

Use **89.4 > Studies > Open Palette C ArtPilot**, then Play, focus Game view, choose16:9 and disable Gizmos. Scene: `Assets/_Game/Scenes/Studies/ArtPilot_PaletteC.unity`. A/D walk horizontally; E beside the desk receiver removes it and its shadow; E again restores both. No HUD/audio, inventory, evidence flag or scene travel is added. The receiver trigger stays active.

This is the existing pilot from preparation commit `406ab42`, updated in place. It is not in Build Settings. CurrentPrologue/CurrentMotel, game startup and saved VisualBlockout remain unchanged. Opening the pilot never regenerates it.

## Pinned source and runtime mappings

Designer source: `output/art/visual_direction_comparison/palette_C_pilot_v01/material_revision_v02/`. The versioned manifest SHA256 is **d12e12bafa4cf1c65772ed857fbe5fba4c7af8bd0faf3b9823caca519451b0dd**. The root HANDOFF was snapshotted at integration time with SHA256 **23258a4bc079eacea6195990768b186cc18811110dd90338a46a62f450b39a3d**. A later root/v03 revision cannot change the tested imported bytes.

All five files were verified against the pinned manifest and copied unchanged from its `material_revision_v02/runtime/` paths to `Assets/_Game/Art/Studies/PaletteC_v02/`:

| Source/runtime filename | Sorting order | Camera-forward depth | Behavior |
| --- | --- | --- | --- |
| base_clean.png | 0 | .004m | Fixed opaque room/context, no receiver/person or their removable shadows |
| receiver_shadow.png | 10 | .003m | Fixed removable receiver shadow only |
| receiver.png | 20 | .002m | Fixed removable receiver/antenna |
| character_contact_shadow.png | 30 | .001m | Follows actor horizontal delta |
| reference_person.png | 40 | 0 | Lighted neutral reference; follows actor horizontal delta |

`Assets/_Game/Data/Studies/PaletteC_v02/` retains exact manifest/HANDOFF snapshots, `source_mapping.json` with every imported hash/path, offline verification, material provenance and AI source prompts. Its local .gitattributes preserves source text bytes. Editable Blender scenes/maps/tools remain outside Assets. No optional old hero, unfinished AI whole-room candidate or v03 layer is imported.

The source uses CC0 Poly Haven floor/plaster/veneer plus declared retained synthetic paint/cloth/plastic; CC0 is not asserted for the entire composite. Original material/license/author/map records are retained in material_provenance.json; bounded synthetic inputs are retained in ai_source_prompts.json and HANDOFF. No new artwork or downloaded materials were produced by this integration.

## Projection, input and scene wiring

Source camera is unchanged: position(0,4.29017067,-9.2605648), Euler(18,0,0), orthographic size2.25, clipping .1/30. At16:9 it spans8x4.5 projected metres. All sprites are1920x1080 full-canvas, PPU240, centre pivot(.5,.5), Full Rect, unit scale. They face the camera with its exact rotation. Frame centre C=(0,1.2,.25); pixel(x,y) maps to `C + right*((x-960)/240) + up*((540-y)/240)`, plus the declared tiny forward offset. The raster's existing18-degree projection is not tilted a second time by a world-XY placement.

`PaletteC_v02_Integrated` owns fixed art and `FollowingReferenceAndContact`. The latter has ArtPilotHorizontalVisuals, with Inspector body=ReferencePerson, referenceBodyX=.9, frameCenter=(0,1.2,.25). LateUpdate copies only the existing body's horizontal delta. This follows Rigidbody interpolation while avoiding visual Y drift from contact settling. It does not control physics, input or camera. ReferencePerson keeps its original Rigidbody2D,1.80m BoxCollider2D, existing PlayerController2D and InteractionController; pathZ=.25 and physical floorY=0 remain. The semantic foot anchor projects to(1176,813.90436) in the initial pose, below the rear seamY=721.199; the nearer toe extends slightly below the semantic anchor.

All49 neutral MeshRenderers are disabled **only in ArtPilot**; meshes/transforms/assets are retained for comparison and the 2D floor/end walls/proxies remain active. Nothing is stretched or duplicated over the sprites. Furniture stays behind the fixed path, so the actual manifest explicitly requires no foreground/desk overlap layer. Original measured character/furniture clearance is preserved.

Receiver_ReviewProxy retains ArtPilotReceiverToggle and its BoxCollider2D trigger. Inspector receiverVisual references PaletteC_v02_Integrated/receiver, receiverShadow references PaletteC_v02_Integrated/receiver_shadow. Existing InteractionController handles E through IInteractable, using proximity queries; no OnTriggerEnter handler is needed. The toggle affects neither reference/contact shadow nor base/fixture. All references are serialized; no manual Inspector assignment is required to run the saved scene.

ArtPaletteCV02Installer.Install validates the exact pinned manifest/five PNG hashes, imports them, and assigns only this scene. It was actually executed. It is an explicit versioned setup utility, not a watcher or runtime dependency. Repeated execution opens/validates an already integrated scene and preserves manual edits. A ready v03 requires a separate explicit refresh; do not overwrite v02 source assets or delete/rebuild the pilot.

## Lighting, alpha and import

Prelit/pregraded source pixels are displayed with existing **Universal Render Pipeline/2D/Sprite-Unlit-Default**. Texture imports: sRGB enabled; straight alpha; Bilinear; Clamp; no mipmaps, compression or NPOT resizing; max size2048. alphaIsTransparency is false to preserve the delivered edge RGB; source supplies its own edge treatment. Standard source-over blends in the existing Linear project. No new URP light, volume, exposure, LUT, runtime lamp or custom shader is added. Source Cycles lighting/AgX/exposure is baked once, not re-applied in Unity.

## Actual checks and screenshots

Unity6000.3.8f1 batchmode: **2/2 EditMode and2/2 PlayMode passed**, exit0. Reports `TestResults/art-palette-c-v02-editmode.xml` and `art-palette-c-v02-playmode.xml`; matching logs under Logs. These are new integration runs, not the earlier preparation-only checks.

- Saved-scene check compares all mesh references/transforms and camera to VisualBlockout; verifies all49 neutral renderers disabled, exactly five versioned full-canvas unit-scale Unlit sprites, import settings, assigned receiver/shadow references, no runtime lights and no Build Settings entry.
- Actual Input System A/D traverses three stopping targets: doorX=-2.25, desk-adjacentX=.1, bedX=1.3, then returns beside the desk. Assertions check feet within physics tolerance, constantZ, visual/contact-following root and actual nearest E target. Desk standing offset deliberately leaves the small receiver visible beside the actor rather than hiding it behind the foreground figure.
- Actual E removes/restores both delivered layers. Original blockout's independent saved-dimensions/traversal/end-wall checks also pass. No unexpected runtime log is accepted by those tests.

**Real Unity camera captures**, no HUD/gizmos, under `TestResults/ArtPaletteCPilot/v02/`:

- position-1-{1280,960}.png — door stop.
- position-2-{1280,960}.png — desk-adjacent stop, receiver present.
- position-3-{1280,960}.png — bed stop.
- desk-receiver-removed-{1280,960}.png and desk-receiver-restored-{1280,960}.png — same stopped pose, actual E states.
- reference-present-1920.png and reference-removed-1920.png — initial registered pose for numerical comparison with the pinned offline assembly.

All12 image hashes, source hashes and comparison results are retained in `Data/Studies/PaletteC_v02/integration_verification.json`. Both1920 Unity states differ from the corresponding offline v02 assembly by at most **1/255 RGB**, with mean channel errors approximately.000193/.000182. This validates frame registration and sRGB/Linear blending, not aesthetics. Comparing E-removed versus restored captures changes only the receiver/shadow region:883 pixels at1280 and534 at960; the rest of the frame is identical.

The programmer inspected the three positions and removal captures: feet stay inside visible floor; character is in front of the desk/chair as specified; no neutral meshes or duplicate receiver appear; source edge/light response is retained. This is batchmode rendering plus image inspection, not a new interactive human Editor playthrough or final atmosphere approval. Generated test images/reports remain Git-ignored local review artifacts; the imported assets/tests/verification/setup are versioned.

## Limitations and preservation

The neutral reference translates as a static image: **no walking animation**. Character lighting is baked for the source position, and its contact shadow is a translated local profile, not dynamically recomputed lighting across the room. Existing bed/bag remain context in the base; no additional isolated photographic prop is supplied, so a second independent prop/material-reuse test is not claimed. Source furniture/anatomy remain simplified. The optional old painterly hero was not chosen as the default. v02's muted color is not the owner's final color choice; the stronger v03 stays a separate art review/refresh.

Preserved SHA256: VisualBlockout B0179DF416E1BA5644DFB5FBEA0E6B623CCA331F288E2EC03EEA70697AD853B0; CurrentPrologue 9714FA8E5C9F9829F6D25A5D90D2D35407275195C2EF57BAE2895274E02F5FC1; CurrentMotel 23C2595B98DCDE067A12B2FA6390B66F5112580AA56CFFD4E30377A5AC48DAFB; working EditorBuildSettings DDE43D1C2DA8E4791FC568274D20AE695E8974E777D462848E32CA4BCC35933C. No package/global renderer/source-art changes. Preparation406ab42 and blockoutccd36e9 are historical; get the integration commit with `git log -1 --oneline -- Docs/ART_PALETTE_C_PILOT_SETUP.md` or the delivery message. No push.
