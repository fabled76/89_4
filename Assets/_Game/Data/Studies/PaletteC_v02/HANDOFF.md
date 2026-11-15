# Palette C pilot — ready for isolated integration

2026-11-15. **READY art-layer delivery; visual style and character method remain WORKING.** This continues the existing pilot under ART_PILOT_MATERIALS_FOLLOWUP.md. It does not authorize replacing CurrentPrologue/CurrentMotel. Designer wrote no Unity files.

## Current delivery

Use root `manifest.json`, published last after checks. All actual layers are under **material_revision_v02/runtime/**. Root pilot_composite_1920.png / _960.png are viewing copies of that version, not substitutes for layers. Read `material_revision_v02/verification.json` for actual offline checks. Earlier `.blend`, images and composite experiments are preserved.

The actual refined source is **material_revision_v02/source/palette_C_material_revision.blend**, opened/refined from the existing source/palette_C_pilot.blend. Blender5.3.0/Cycles/OptiX was actually executed. No scene rebuild or whole-room image generation occurred during this follow-up. Geometry/transforms before and after material revision hash identically; camera anchors differ from Unity by less than0.001px. Original source meshes retain their measured dimensions. Earlier shallow2mm bevels and coplanar chair-material subdivisions remain; only one WORKING practical fixture was added in the earlier pilot stage.

## Assembly order

All required PNG layers are **1920×1080, original full canvas, offset(0,0), PPU240, Full Rect**. Do not trim/rescale individual objects or put their image origins at independent world positions.

| Order | File in material_revision_v02/runtime/ | Ownership |
| --- | --- | --- |
| 0 | base_clean.png | Opaque clean room/context, fixture, fixed furniture/its shadows; no receiver/person or their removable shadows |
| 10 | receiver_shadow.png | Only the removable receiver's dark shadow contribution |
| 20 | receiver.png | Occlusion-correct visible receiver and antenna; fixed placement |
| 30 | character_contact_shadow.png | Removable/following local floor-contact profile for reference foot position |
| 40 | reference_person.png | Neutral, actually lighted segmented1.80m reference, not final protagonist |

Hide **both receiver.png and receiver_shadow.png** on the study E toggle; restore both together. Leave contact shadow/reference/base/fixture untouched. Trigger/proxy stays active. Neither receiver nor its shadow is baked in base_clean: that pass was rendered with receiver meshes hidden.

No foreground/desk occlusion layer is required for this fixed horizontal path: furniture is behind pathZ=.25; chair frontZ=.395 stays behind source person's maximumZ=.37. The original scene depth is reflected in the receiver's visible mask. Do not add an invented foreground layer or draw a duplicate neutral mesh in front of these plates. If a changed path later crosses furniture depth, this contract must be revised.

## Camera, pivots and movement

Retain measured camera position(0,4.29017067,-9.2605648), Euler(18,0,0), orthographicSize2.25,16:9, projected8×4.5m. Exact basis/quaternion/anchors and source hashes are in manifest. The18° tilt is already represented by the projected images: **do not place a pre-tilted render on a world-XY plane and foreshorten it a second time**.

Default full-canvas pivot=(.5,.5). Place camera-facing image planes through **C=(0,1.2,.25)** with cameraRight/up as their axes. For image pixel(x,y): `C + right*((x-960)/240) + up*((540-y)/240)`. Use declared tiny depth offsets/sorting orders; use Unlit. Background is one8×4.5m camera-space image plane. Source z/depth does not become a new navigation axis.

Neutral reference controller root starts(.9,.9,.25); physical foot anchor(.9,0,.25) projects to(1176,813.90436). Keep the existing1.80m body/collider and fixedZ. Visual reference and contact-shadow quads start in full-canvas registration; parent them with the compensating initial offset to the existing root and translate by the root's horizontal delta. Each1m horizontal move is240px. Do not place the foot on the wall seamY≈721.199. Near toe reaches below the semantic foot anchor; do not replace the pivot with a guessed bottom-of-alpha point.

For custom foot-pivot imports, the semantic normalized pivot is(.6125,.24638485). The manifest supplies its camera-plane projected anchor as well as physical foot/root anchors. These are alternative placement representations, not two transforms to apply simultaneously. Receiver body-center anchor is original Unity(-.4,.795,.85), pixel(864,587.94434). Preserve the common canvas rather than compensating a crop by eye.

## Light/color/alpha contract

Every runtime pixel is **prelit and pregraded**. Practical AreaDisk: Unity(-.65,2.02,1.2),55W,size.35m, linear color(1,.78,.53), plus cool ambient/world1.3. Fixture mount is(-.65,2.13,1.48). Blender AgX Medium High Contrast/exposure+.65 is baked. These are WORKING art-study settings, not radio/electricity rules. No runtime lamp/URP relighting, second exposure, LUT or tone map should be applied on top.

Use **sRGB color textures, straight alpha, ordinary source-over blending in the existing Linear project**, Bilinear, no mipmaps, no texture compression for the initial check, max size at least2048/non-POT resize disabled. Layers were decomposed/assembled in linear-light math, with sRGB-encoded RGB and linear alpha. Standard Unlit blending is sufficient; no new shader suite is supplied or required. Keep artist RGB; do not color-tint sprites by default. Alpha edges are corrected against the actual visible-matte/beauty passes, not an unoccluded object cutout. No silhouette pixels were invented outside the Blender visible mask.

Receiver shadow is a separate localized dark contribution, excluding all receiver-covered pixels. Character shadow is deliberately a **local contact profile**, feathered around the reference foot position, not a static full-body shadow baked into the room. Moving it with the actor is an approximation; it is not a general dynamic relighting solution across the whole room. Floor contact/overlaps at other X positions need the programmer's actual traversal captures.

Transparent cutaway context was composited over a quiet plum matte(RGB about124,115,126 at top), preserving original framing/floor. It is presentation/context fill, not newly modeled architecture or a walkable extension. The source room remains a cutaway; the selected fragment is the door/desk area. No camera crop hides foot placement.

## Materials actually used

Nine original2K JPEG maps(color/roughness/OpenGL normal) were downloaded directly from official Poly Haven URLs. Originals, authors, URLs and hashes are under material_revision_v02/source/polyhaven/provenance.json. Poly Haven declares its assets [CC0](https://polyhaven.com/license).

- [Old Wooden Floor02](https://polyhaven.com/a/old_wooden_floor_02): used on the existing floor/apron.2×2m tile; horizontal boards along roomX. Natural brown retained with saturation.72; roughness=.28+.55×map, normal strength.25. No polished-lobby reflection or displacement.
- [White Stucco02](https://polyhaven.com/a/white_stucco_02): used as a quiet interior base, photographic variation only96–104% of the existing salmon tint; normal strength **.055**, roughness=.70+.15×map, no displacement. Artist test tile1.8×1.8m: the website texel-density metadata is implausible, so verified source scan dimensions are **not** claimed. At960 it reads as subdued interior texture rather than coarse exterior render.
- [Natural Walnut Veneer](https://polyhaven.com/a/natural_walnut_veneer): used for existing woodwork/legs/chair frame,1×1m tile, grain along each member's long axis. A restrained warm finish multiplier(.92,.65,.40 linearRGB) prevents ash/purple-looking wood under the cool fill. Existing ivory laminate desk top remains quiet; cloth, plastic and door paint are reused.

No reserve textures, Pinterest/Google-thumbnail/LOC photo pixels,16K maps or new material library. The initial synthetic atlas is still the source for retained cloth/plastic/paint, not falsely described as a real photograph. Source/ai_source_prompts.json records those bounded inputs. fragment_ai_finish_candidate.png was **not** accepted as a room background: its altered bag contour/trim details are not geometry. Only the existing30% door-front albedo patch survives inside the original mesh/UV projection; physical door boundaries/anchors are unchanged.

## Optional old hero and known limitations

working_hero_lightmatch.png and pilot_working_hero_1920/_960 are an **optional comparison**, not default replacement of the neutral reference. The old silhouette/alpha/pose/costume are preserved, with simple exposure/color matching only. Its eye-level painterly view is not fully compatible with18° photographic presentation; no new anatomy, rig or final character approval is claimed. Main required pilot uses the neutral reference.

Static furniture is still simplified measured geometry. Surface finish, physical light, contact and existing dimensions are delivered; this is not a finished library or walking animation. No new location, inventory UI, morning scene or DRIFT art was produced. No actual Unity import, E test, traversal screenshot or style acceptance is claimed here. Those are stage2/owner review.

## Reproduce / effort

Open the original saved `.blend` and run material_revision_v02/tools/refine_saved_materials.py in Blender. It updates materials/UVs only, saves a sibling `.blend`, renders beauty/isolation/neutral-light passes and checks geometry/camera registration. For the existing completed passes, run tools/render_visible_masks.py against the refined `.blend`, then Windows PowerShell tools/assemble.ps1. Source render passes and full packed `.blend` are retained; no stochastic regeneration is needed to rebuild the delivered composite.

The earlier turn stopped at an external service usage limit. This continuation used the available normal tools, local installed Blender and official material downloads, without another model/provider/account. Work observed: three scoped material downloads, one saved-scene material preview, one complete pass set, a missing diagnostic material fixed without rerendering the existing beauty frames, two technical visible mattes, alpha-edge/linear blending corrections and independent assembly checks. No invented hands-on hour estimate is supplied.
