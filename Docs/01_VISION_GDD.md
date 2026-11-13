# Game Design Document — v0.2 (89.4 working title)

## Elevator pitch
In a nostalgic 1980s town, a journalist investigates a missing radio host (working case). Their portable radio apparently anchors both the town around them and their own continuity of memory/perception. Turning it off exposes erased people and evidence, but leaves the world and their mind vulnerable to drift. Enemy contact cannot kill the protagonist: instead, recent **unrecorded** recollections can become inaccessible, while alternative investigative paths remain. Radio recordings can preserve contradictory evidence from earlier versions of the town. The closer the player gets to the truth, the more often they must choose dangerous silence. This is a working model to validate, not settled lore.

**Genre:** narrative detective / psychological mystery / light survival tension. **Perspective:** 2D side-on, layered depth (provisional). **Target prototype:** 5–10 min; **jam target:** 20–30 min high-quality experience; **aspiration:** 30–60 min. **Platform:** Windows desktop first.

## Design pillars
1. **Safety conceals truth.** Radio protects; radio also suppresses investigative access.
2. **Dangers communicate information.** A pursuer may be a witness. Running away is understandable, approaching can be necessary.
3. **Nonlethal failure creates detective complications.** No HP or lethal attacks; a bad encounter alters knowledge, access or perception, with a fair recovery route.
4. **Deduction, not key collecting.** A clue matters because it changes the player's hypothesis.
5. **Recordings exist in the fiction.** Recorded evidence can contradict the current world.
6. **A small, dense place is richer than a broad, empty world.** Strong art, sound, local people, recurring places.

## Primary gameplay loop
Explore stable place → acquire testimony and leads → deliberately enter no-signal/unstable state → withstand navigational or entity risk → obtain contradictory clue → record/compare evidence → make explicit inference → unlock a new investigative action.

## Experience goals
- Wonder, dread and nostalgia without constant jumpscares.
- A town that seems to have an everyday routine and absurd local customs.
- The player is frightened by a figure and later genuinely wants to meet it again.
- Radio static, broadcast jingles and music interact with environmental sounds.
- A meaningful mystery with fair, testable rules; supernatural ≠ arbitrary.

## Out of scope for MVP
Combat, guns, health bar, player death from encounter, open-world map, dynamic day/night simulation, complex NPC schedules, multiple fully simulated timelines, procedural generation, branching narrative with dozens of endings, complex DID simulation, 3D cinematics.

## Player capabilities (prototype)
Walk, interact, inspect clue, toggle radio, replace battery, trigger an in-world record, compare evidence, make one deduction, intentionally approach the figure.

## Design warnings
- Do not make radio-off mode purely punitive.
- Do not hard-lock essential evidence behind finite consumables without recovery.
- Avoid equating psychiatric diagnoses with violence or supernatural evil.
- Avoid losing real save progress because a diegetic battery ran out.
- Do not settle a final twist before mechanics demonstrate the promised tension.

## Definition of done (vertical slice)
One complete mystery beat: see a figure → retreat to radio safety → learn it's a witness → voluntarily return without signal → hear testimony → compare with another clue → conclude a fact. Controllable, clear, audio-feedback present, no softlocks.

## Additional decision boundaries (2026-11-13)
- **LOCKED:** the motel witness reverses from apparent threat to essential informant; player voluntarily enters danger for testimony.
- **LOCKED:** investigative setbacks rather than death; no HP bar, no combat-oriented fail state.
- **WORKING:** portable receiver present in the normal exploration loop; stationary radios create safe areas with broader/stronger stabilization.
- **WORKING:** radio anchors city conditions and protagonist's sense of continuity, but exact relationship is not known yet.
- **TBD:** mental-state HUD (prefer subtle audio/visual cues first); identity splitting or psychological diagnosis; loss/restore behavior for world and memory.
- **TITLE:** 89.4 preferred working title; do not invent a city name or hardcode station frequency into foundational systems.
