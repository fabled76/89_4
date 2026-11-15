# VisualBlockout v01 — designer handoff

Real neutral Unity 6000.3.8f1/URP camera renders of the saved `Assets/_Game/Scenes/Studies/VisualBlockout.unity`. ProBuilder 6.1.4 geometry. No AI imagery, Blender rendering, final art or runtime migration is implied.

## Start here

- `blockout_base_1920.png`: clean 1920x1080 reference, same fixed figure/layout for A/B/C. 1280 and 960 copies are camera renders at those sizes.
- `blockout_guide_1920.png`: separate annotations only; never import as background art. Cyan rear seam, magenta path contact anchor, yellow viewport foreground, green world-height reference.
- `blockout_geometry.obj` + `.mtl`: 49 named triangulated mesh objects, baked world transforms, neutral gray materials. Names follow `Group__Mesh`. No scripts, physics or camera transfer.
- `camera_layout_manifest.json`: exact Unity camera/basis/projection matrices, object and mesh dimensions, bounds, transforms, projected bounds and anchors. Pixel rectangles use top-left origin; they are unoccluded vertex envelopes, not segmentation masks.
- `standing_{door,desk,bed}_{1920,960}.png`: three actual Unity renders, same camera/geometry. Only person root X changes to -2.25, -.4, 1.3 respectively; Y=.9/Z=.25 remain fixed. Main frame root X=.9.
- `diagnostics/camera_candidate_08_1920.png`: rejected shallow floor candidate; do not substitute for the shared base.
- `verification.json`, `SHA256SUMS.txt`, `verify_handoff.py`: independent checks and exact source hashes. Run the Python script after any authorized re-export.

## Scale, axes and camera reconstruction

1 numeric unit = 1 metre. OBJ is right-handed Y-up: **OBJ=(Unity X, Unity Y, -Unity Z)**. Winding/normals are reflected consistently. Do not apply another Z reflection if using these OBJ coordinates directly. Calibration object `Calibration_OutsideFrame__Calibration_1m_Cube` is exactly 1x1x1 at OBJ (5,.5,0), outside the camera frame. Reference figure is 1.80 m high, foot Y=0.

Unity camera position=(0,4.29017067,-9.2605648), Euler=(18,0,0), target=(0,1.2,.25). Orthographic **full vertical span 4.5**, horizontal span 8, aspect16:9, image1920x1080, clipping .1/30. Exact values are in manifest; do not infer camera from OBJ import orientation or an Euler convention in another app.

For an offline right-handed Y-up scene importing OBJ without axis conversion, camera position is the Unity position with Z mirrored; look toward target with Z mirrored. Reflect Unity camera up from the manifest too. If your tool converts Y-up to Z-up, apply the same documented conversion to geometry, position, target and up. Match the foot/head/door/receiver pixel anchors before surface work; don't just eyeball the imported viewport. Orthographic scale conventions vary: match the full **4.5 vertical span**, not an ambiguous numeric field. Blender import/camera reconstruction has not been executed here.

## Deliberate exceptions and constraints

Room clear envelope6.4x3x2.6m; flat floor Y=0; fixed movement path Z=.25. Selected **18-degree comparison camera** is provisional. 8 degrees gave only about42px behind feet; selected angle gives about93px rear floor and130px to the nominal front opening. A coplanar apron extends to the viewport bottom (about266px below path): it prevents a visible platform cut and is not extra walkable depth. Toe silhouette projects a little below the magenta centre-path contact anchor.

Door leaf .85x2.10, desk footprint1.35x.65/top.74, chair seat.45, bed mattress1.95x.90/top.50. Headboard adds.04 to overall bed X and reaches.90 high. Bag XYZ=.45x.30x.25 including handle; receiver body XYZ=.18x.11x.05 plus antenna. All actual bounds are supplied. Do not stretch the person to hit a nominal pixel height.

Door at rear uses a 2D transition proxy, not literal hand contact: handle is1.185m behind the path. Furniture leaves the horizontal lane clear. Neutral mannequin, static poses, face-value materials and absence of contact shadows are intentional blockout limitations, not final protagonist/lighting choices. Preserve the figure silhouette, framing and object anchors across A/B/C.

Unity checks: 1/1 saved-scene dimension/reference test and 1/1 PlayMode traversal/wall/proximity test passed. Current gameplay scenes/startup remain unchanged. Full setup and reproducible Unity export command are in `Docs/VISUAL_BLOCKOUT_SETUP.md` in the project.
