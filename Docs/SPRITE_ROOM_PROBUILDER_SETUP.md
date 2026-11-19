# Sprite room — editable ProBuilder study

2026-11-19. Technical construction example for owner review; no final art, room narrative, animation or lighting approval.

## Open and edit

Open `Assets/_Game/Scenes/Studies/SpriteRoom_ProBuilder.unity`, or use **89.4 > Studies > Sprite Room > Open Saved Scene**. Nothing rebuilds on open or Play. Work in Edit mode and save with Ctrl+S. No gameplay scripts, startup/build-list changes, package installations or pipeline changes belong to this study.

Hierarchy:

- **Architecture**: independent ProBuilder floor, wall sections around real openings, short side returns, architectural trim and a sill with depth.
- **SpriteSlots**: separate named foot anchors, furniture placeholder planes, selected static character, separate flat contact support, window-frame/curtain placeholders and exterior preview. Disable this root for an empty shell.
- **Lighting**: warm lamp and cool exterior source anchors. Their ordinary Light components are deliberately disabled; see limitations below.
- **Camera**: fixed orthographic study camera.
- **ReviewGuides**: disabled by default. Toggle the root to see the horizontal foot-path line. No gameplay depth movement is implemented.

## Measured contract

1 unit = 1 metre. Reference: `VisualBlockoutBuilder.cs` / preserved VisualBlockout scene. Width 6.4 m, nominal depth 3 m, revised height 2.7 m. Floor top y=0, thickness .20 m. Main floor extends beyond the room width to keep its presentation edges outside the Game frame; front apron extends to z=-5.8. These extensions supply continuous visible ground, not new navigable space.

Rear wall centre z=1.6, thickness .18, inner face z=1.51. Walls x=-3.2..3.2, y=0..2.7. Short side returns are .65 m deep. Front and ceiling are open.

Door clear opening: centre x=-2.3, width .95, height 2.20. A future .85 x 2.10 leaf fits this frame; no solid door leaf is currently inserted. Window clear opening: x=-.60..1.00, y=.90..2.10 (1.60 x 1.20). The sill is a separate .32 m deep slab. Window-frame strips and curtains are flat stand-ins, with space left transparent between them. Exterior image plane is behind the wall at z=2.2; door preview plane at z=2.3. Neither fills the architectural mesh opening.

Static foot path z=-.15, y=0; rear seam is z=1.51. Hero foot anchor (.85,0,-.15). Nominal hero height 1.80 m. Desk placeholder 1.35 x .74, chair .45 x .85, bed 1.95 x .90, receiver .18 x .11. These rectangles communicate reserved image bounds, not furniture shapes or collision/navigation volumes.

Camera: rotation (12,0,0), target (0,1.25,0), distance 10, orthographic size 2.05, near/far .1/30, 16:9. Position = target - forward * 10, approximately (0,3.3291,-9.7815). **Restore Camera and Scene Framing** restores this frame and an oblique Scene view. Use 16:9 in Game view; changing aspect changes the horizontal crop.

## Five common owner edits

1. **Move a sprite:** select its `...FootAnchor` under SpriteSlots, press W, edit X/Y/Z. Keep feet on y=0 for floor objects and z=-.15 for the reference path. Move the separate character contact placeholder by the same X/Z amount. For a new sprite, add a SpriteRenderer child with its foot pivot at the anchor; disable/delete the placeholder plane and its label. Use Sprite-Unlit-Default initially. Set camera-aligned rotation to (12,0,0) once for preprojected art compatible with this frame; do not apply a second perspective/height correction.
2. **Resize a wall:** select the relevant `RearWall...` child, open **Tools > ProBuilder > ProBuilder Window**, enter face/vertex mode and move the desired boundary. Snap .05 m. Wall pieces and mesh assets belong only to this study. Avoid moving both coincident boundary faces in inconsistent directions; save the scene after editing. ProBuilder remains present on each architectural mesh.
3. **Texture scale:** architectural UVs are world aligned during initial construction, 2 m per repeat for stucco and floor, 1 m for untextured trim. Adjacent wall sections share phase at construction. On the task-owned material, Base Map tiling (2,2) halves the physical repeat size everywhere using that material; duplicate the material to vary one surface. After substantial face edits, use ProBuilder UV Editor / manual UVs to preserve density and align wall-section boundaries. Floor apron's initial seam is continuous.
4. **Window width:** in ProBuilder move both opening jamb boundaries, then adjust SillWall and Header endpoints to meet them. Move/resize the separate frame strips, curtains, sill and outside plane independently. At this revision left edge=-.60, right=1.00; increasing width .20 symmetrically means left=-.70, right=1.10. The wall must retain an actual gap.
5. **Restore framing:** menu **Restore Camera and Scene Framing**, then Ctrl+S if keeping that camera. This is explicit; simply reopening never resets manual composition. Capture menu exports current framing and runs scoped reference/opening checks. It temporarily hides slots for the empty-shell capture and restores them in a finally block.

## Finished technical parts and missing art

The shell, openings, UV mapping, fixed camera, hierarchy and editable slots are implemented. All furniture/decoration rectangles remain placeholders. No whole-room rendered image or detailed 3D furniture/character is used.

Actual character: byte-for-byte copy of `output/art/protagonist_sprite_v01/revision_02/sprites/three_quarter_selected_registered.png`. The completed manifest uses root (240,610) on 480x640, PPU 331.03447, opaque projected height 582.84 px. Imported with pivot (.5,30/640) and a single uniform scale ~.99999 so projected height matches 1.80 m at 12 degrees. The art is an image-edited visual reference, not a geometrically calibrated camera render. It is static and has baked tonal shading; no walk system, dynamic relighting or shadow casting is proven. Active revision_03_walk is untouched. Contact support is a separately selectable flat ellipse placeholder, not the source's room-size shadow and not a physical cast shadow.

Missing designer deliverables: independent desk, chair, bed, portable receiver, curtains, window frame, door leaf (if desired), lamp fixture, camera-matched clean outside view and a final character contact/cast-shadow solution. Existing receiver/lamp studies were reviewed but their prelit glow/perspective are unsuitable as clean reusable assets for this shell. No cropped furniture from a room image was substituted.

Exterior: byte-for-byte copy of the existing gas-station assembled PNG. Temporary window preview only; neither geographic/setting canon nor parallax. It may be hidden without affecting architecture. Its lens, dusk lighting and illustrative treatment are not calibrated to the room.

Surface textures: existing Poly Haven CC0 `white_stucco_02` diffuse (Dimitrios Savva) and `old_wooden_floor_02` diffuse (Charlotte Baglioni / Jenelle van Heerden). Original provenance and license note are copied alongside task textures. Only diffuse maps are used. Coral tint #CE9589, floor tint #D7BAA5, ivory trim #CFC1AC, teal #507F80, burgundy #7C4258. These are study inputs, not approved final colour standards. No new external download or asset generation was needed.

## Light limitations

Active project quality uses the existing URP asset with a 2D Renderer. The architectural meshes use **Universal Render Pipeline/Unlit** and the selected character uses the package **Sprite-Unlit-Default** material. Geometry, placeholders and hero do **not** receive real-time 3D point light, cast shadows or share a unified relighting response. Texture/tint differences are material presentation, not simulated lamp lighting. Two disabled point-light anchors expose intended warm lamp/cool window locations for later work; enabling them alone will not light these materials. No successful dynamic lamp/exterior light study is claimed. No renderer assets, quality settings or new rendering framework were introduced. Assess architecture/placement here; commission compatible sprites and a bounded light test separately.

## Evidence and validation

`output/sprite_room_probuilder/` contains Unity camera exports at 1600x900, empty shell and populated slots, an oblique Scene camera export, actual editor UI screenshots, and `verification.txt`. Exports come from real Unity cameras via RenderTexture, not Blender or an offline room illustration. UI screenshots corroborate Game/Scene state. The saved scene is reopened before scoped validation: missing mesh/material/script references, supported material shaders and physical collider rays through door/window vs solid wall. Guides are disabled for clean captures. No cosmetic test framework was added. Captures are review evidence; they are not runtime backgrounds.

## Furniture batch01 preparation (2026-11-19)

Shared contract: `output/sprite_room_probuilder/furniture_batch01_contract.json`, short handoff and diagnostic guide beside it. New furniture export density is **263.4146341463415 px/projected m** at 1920x1080 (1080/4.10); do not reuse the character's 331.03447 source PPU. Camera, architecture and three existing root positions are frozen. Intended footprints: desk 1.35 x .65, chair .45 x .45, bed 1.95 x .90 m. Bed mattress top .50 m, headboard .85 m. Root is footprint centre at floor Y=0, not alpha-bottom centre.

Each furniture root now contains `PlaceholderReference`, `Visual` and `ContactShadow`. Move the root to carry art and local shadow together. The prepared SpriteRenderers remain disabled until a real READY package arrives. Visual and shadow have rotation (12,0,0), unit scale, and will use semantic PNG pivots from the designer manifest. A projection-neutral offset along -camera.forward of .50 m for Visual / .51 m for ContactShadow prevents floor clipping at front supports; these render-plane offsets do not move the semantic ground root or change any projected pixel. Each root SortingGroup order=-10 puts furniture behind hero order=0; item groups sort by camera distance (chair in front of desk). No foreground split is requested.

Task-local derived surface textures `Batch01_Wood_Quiet.png` and `Batch01_Stucco_Quiet.png` reduce high-contrast detail without changing shared source bytes, UVs, scale, camera or geometry. Recipe in `SpriteRoomFurnitureBatch01.Quiet`: per-channel output = chosen mean + (source - .5) * contrast; wood mean(.52,.40,.30), contrast .32; stucco mean(.85,.84,.81), contrast .24. Existing material tints stay. Before/after captures are `furniture_batch01_material_before.png` / `furniture_batch01_material_after.png`. These are presentation changes, not new lighting. All surfaces and sprites remain unlit.

Toggle `ReviewGuides/FurnitureBatch01_Targets` with its parent to inspect floor footprints and height targets. Exported guide temporarily hides the three legacy rectangles so supports can be seen; the scene saves guides off and placeholders on until art import. The initial batch preparation is separate from final designer-art integration.
