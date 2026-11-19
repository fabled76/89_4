# Furniture batch01 — frozen technical handoff v1

READY FOR DESIGN EXPORT. Authoritative machine-readable file: furniture_batch01_contract.json beside this file. Camera/architecture frozen. Use the existing clean game_with_slots.png until the matched material-after view is published; new furniture guides will be in furniture_batch01_guide.png.

Native output density: 1080 / 4.10 = 263.4146341463415 pixels per projected metre (1920x1080 reference). Unity PPU equals this density, unit scale. Do not use the character's old source PPU. Camera is orthographic, no yaw, 12 degrees down. Sprite visual is camera-aligned at rotation (12,0,0) once. Render furniture through this projection first; do not tip/project its bitmap twice.

Axes: X right, Y up, Z into room. Floor Y=0; hero path Z=-.15. All three root transforms are preserved from the saved scene, rotation zero and scale one:

- desk root (-.85,0,.55), footprint 1.35 X x .65 Z, top .74 m.
- chair root (-.90,0,.15), footprint .45 X x .45 Z, seat .45, overall .85 m. Chair is in front of desk, behind hero path. A modest visual tuck is expected.
- bed root (2.05,0,.65), footprint 1.95 X x .90 Z, mattress top .50 m, headboard .85 m at +X end. The .90-high rectangle is not the mattress height.

ROOT IS CENTRE OF FOOTPRINT ON FLOOR, not bottom alpha pixel. Export a transparent padded canvas per asset. Supply semantic ground_anchor_px from top-left pixel edges even if this pixel is transparent/inside the support polygon. Project supports with u = D*dx; v_up = D*(.9781476007*dy + .2079116908*dz). Imported pivot=(anchor_x/width,1-anchor_y/height). Record four support pixels or corresponding world support points. No manual image stretching to fill placeholder rectangles.

Each root will contain Visual, ContactShadow and a legacy PlaceholderReference. Use one independently movable image + its own separate projected contact-shadow layer. The shadow shares root and density/pivot semantics, with its own canvas/pivot metadata; no whole-room shadow or opaque floor patch. Unity will billboard it once with the visual and move the whole root together. All items are in SortingGroups behind hero (group order -10; hero 0), distance sorted between item roots; chair is nearer than desk. No occlusion split is required in this composition. Baked soft form shading is allowed; directional lamp glow and shared dynamic lighting are not part of this batch.

Please publish output/art/sprite_room_furniture_batch01/READY.json only after actual files exist. Include status READY and manifest path; manifest should list desk/chair/bed file, SHA256, canvas_px, ground_anchor_px, alpha_bounds_px, export_pixels_per_projected_metre, dimensions_m, separate shadow metadata when present, provenance and baked-light assumptions. HANDOFF.md should flag deviations honestly. Contract frozen as sprite_room_furniture_batch01_v1; ask through the shared filesystem if a dimension or support cannot be met. No Unity edits by designer.

Integration depth note: Visual plane is offset -camera.forward * .50 m, ContactShadow by .51 m. This has ZERO screen-space shift under the frozen orthographic camera and does not change export dimensions, pivot or projection. It prevents the camera-facing bitmap/front supports from clipping against the real floor. Semantic ground roots remain unchanged; group order keeps furniture behind hero.
