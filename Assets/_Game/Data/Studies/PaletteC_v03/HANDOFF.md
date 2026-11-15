# Palette C v03 — stronger material colors

2026-11-15. Owner selected stronger color, closer to the first C comparison. **This package is a separate WORKING color revision**, not final style/camera/character approval. v02 remains the pinned integration baseline; root-level v02 manifest/HANDOFF and every file under material_revision_v02/ were left unchanged. Use this directory's own manifest.json only for a deliberate later refresh; never mix versions.

## Delivered / changed

pilot_composite_1920.png / _960.png are the preferred v03 neutral-reference assembly. matched_v02_v03.png shows equal960×540 v02/v03 panels, same framing/pose/scale, labels outside. pilot_receiver_removed_1920/_960 and the clearly labelled detail crop are also provided. Diagnostics and the saved source are under diagnostics/ and source/.

Actual Blender5.3.0/Cycles/OptiX continuation of the saved refined v02 scene. No room regeneration, new geometry, camera, actor, atlas or material-scale change. Geometry/transforms/**all UV layers** hash identically before/after. Camera registration remains within0.001px of Unity. The same real Poly Haven maps, texture scale, roughness, normal strength, quiet plaster and wood finish remain. No whole-image saturation, LUT or exposure/darkening operation was added to the room.

Pigment changes in sRGB targets, retaining the exact texture/ramp variation:

| Material | v02 | v03 |
| --- | --- | --- |
| Salmon plaster | .80,.68,.64 | .85,.52,.46 |
| Smoked teal enamel | .38,.56,.55 | .23,.58,.57 |
| Burgundy cloth | .48,.29,.35 | .54,.20,.32 |

The broad cool ambient was changed from sRGB(.57,.61,.74)/strength1.3 to(.68,.71,.79)/strength1.05, reducing pervasive purple without reducing exposure. The practical source stays at Unity(-.65,2.02,1.2),55W,AreaDisk size.35m; its linear color changes only slightly to(1,.75,.50). AgX Medium High Contrast/exposure+.65 is unchanged. Wood, neutral laminate, plastic and reference material colors are retained. They respond to the new light normally; they were not palette-tinted.

Outside the rendered cutaway only, the presentation matte is now supporting indigo/plum RGB(70,66,89)→(76,72,95). This is not a vignette on the room, newly modeled architecture or a walkable space. Original framing and visible floor are retained.

## Same runtime contract

Required runtime files are in runtime/: **base_clean.png → receiver_shadow.png → receiver.png → character_contact_shadow.png → reference_person.png**, sorting0/10/20/30/40. All1920×1080, origin(0,0), PPU240, Full Rect, default pivot(.5,.5), common camera-facing8×4.5m image plane through C=(0,1.2,.25). Exact layer hashes and tiny depth offsets are in manifest. No incompatible layer/anchor contract change.

Use the exact18° measured camera; do not foreshorten preprojected PNGs again. Camera-space pixel mapping remains `C + right*((x-960)/240) + up*((540-y)/240)`. Controller root(.9,.9,.25), physical foot(.9,0,.25), projected foot(1176,813.90436), semantic full-canvas foot pivot(.6125,.24638485). Reference/contact visuals follow the same horizontal root delta. Physical body stays1.80m; no walking-in-depth axis or character stretch.

Hide/restore receiver **and receiver_shadow together**, leaving base/fixture/character shadow unchanged. Base was rendered with receiver/person hidden; no duplicate is painted in. Camera visibility mattes are reused byte-identically from v02 because geometry/camera/UV layout are identical. Separate shadow excludes the receiver matte. Necessary overlap remains unchanged: all furniture is behind the fixed path, so no extra foreground layer is required. The receiver's true desk occlusion is already reflected in its visible mask.

Sprites are prelit/pregraded **sRGB RGB + straight alpha**, compiled for ordinary **linear-light source-over** in the existing Linear Unity project. Unlit, Bilinear, no mipmaps/compression for the first check, max texture size≥2048, no resize. Do not add runtime URP relighting, exposure/LUT/tone map or a duplicate shadow. Only the PNG contents/hashes and disclosed color/light profile change from v02.

The character contact shadow is the same type of local following floor profile, not a full dynamic relighting solution. Exact physical shadow/light validity across three moving positions remains a Unity-stage check. No new animation is claimed.

## Source / reproduction / character scope

Editable: source/palette_C_color_v03.blend, packed image inputs. Recipe: open the pinned v02 source .blend and run tools/revise_color.py; then tools/assemble.ps1 uses the saved beauty/isolation passes and invariant v02 visible mattes. The new geometry/UV and color settings, source hashes and protected v02 baseline hashes are recorded in source/. Verification and final manifest are published after actual files assemble.

No new source texture downloads are needed. Original maps/provenance remain in ../material_revision_v02/source/polyhaven/; scales and material details are inherited, not reinterpreted. The old bounded door-front synthetic albedo patch remains inside the same mesh only; its image is not a runtime room background.

Required actor is the neutral lighted reference. No new legacy-hero grade/face/rig was made: the existing v02 painterly hero comparison is still optional and incompatible with the18° photographic viewpoint. CHARACTER_METHODS.md compares rigged offline3D→2D and photographed cutouts with actual available tools/missing inputs; neither is commissioned here.

Designer verified offline layers, masks/alpha, linear assembly, object+shadow removal, camera/UV invariants and v02 preservation. Unity and narrative canon were not changed. v02 integration continues independently; v03 needs an explicit version-pinned refresh and actual engine captures before artistic/integration acceptance.
