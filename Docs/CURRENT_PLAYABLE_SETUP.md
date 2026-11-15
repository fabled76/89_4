# Current connected playable

2026-11-15. Implements CURRENT_PLAYABLE_INTEGRATION.md using approved first-night v02 chronology. This is the recommended route, not an isolated trial. Historical scenes/production sources remain unchanged; their old startup/bridge reports are historical.

## One entry and complete route

**89.4 → Play Current Game**, outside Play, opens `Assets/_Game/Scenes/CurrentPrologue.unity` and starts Play. Alternatively open that scene directly. Focus Game view, choose 16:9, turn Gizmos off. A/D walk/read; E interacts; Esc closes; Left Shift powers the carried receiver; F1/1/2 selects EN/RU. No arrows/Space reading or vertical walking.

Build Settings begins with **CurrentPrologue → CurrentMotel**, both enabled. SampleScene is no longer first or recommended. Historical scene entries remain available in the shared checkout. No executable is claimed tested or published.

1. Answer the home telephone, finish the case reading in either order, prepare the bag and use the door. Phone, envelope, bag and receiver are separate layers. Packing hides the desk receiver/envelope without tinting painted artwork green.
2. Existing ordinary travel sound leads into CurrentMotel reception. The copied CurrentPrologue definition explicitly enables **Playable Check In Arrival**. It bypasses automatic ON, tuning, the obsolete registration-bypass card and its Space acknowledgement. Held departure input must be released; controls restore their prior states, including the still-disabled uncarried RadioInput.
3. Complete registration at the painted counter; use the left door to reach the guest room. This grants room access only.
4. Ordinary movement outside is captioned/audible before radio use. Leave the receiver beside the bag, use the left room door, approach the man and finish his short claim. No attack or witnessed disappearance is required.
5. Return through the left corridor door. The man leaves only after the corridor is out of view. E retrieves the receiver; Shift powers it; E at the bag reviews the program. Reading has no audio-duration gate and supports Esc/retry.
6. Return to an empty corridor ON or OFF. Close the explicit first-night endpoint to look around or return to the room. Morning, identifying evidence, ledger access and later testimony are not implemented.

Early pickup remains allowed. Optional premature ON can retain prototype suppression before the man's ordinary departure: this is a known staging risk, not a taught rule or observed-disappearance flag. Later absence is independent of power. Battery drain is locally zero; original motel mechanics are unchanged. No inventory/save/time framework was added.

## Assigned art and floor

`Assets/_Game/Art/CurrentPlayable/` contains runtime PNGs copied unchanged and SHA-256 checked against the designer's **v03_floor_integration** manifest, plus v02 reception. Retained manifests and PROVENANCE identify their origins. Production folders were not modified; no guide lines/sheets were imported as art.

- Home: corrected clean home_base; separate telephone, radio, envelope, bag; home-clothing hero.
- Reception: supplied reception_plate including its WORKING counter/clerk. This plate was not repainted in the narrow correction.
- Guest: corrected guest_room_base, separate bag and desk receiver. Retrieval hides only the desk receiver and shows one carried receiver; bag stays.
- Corridor: corrected corridor_base, separate doors/lamps, ordinary man and outdoor hero. Man remains absent after offscreen departure.
- Supplied contact_shadow tracks feet. Facing flips artwork/carried receiver only, never physics or interaction reach. Fixed idle/carry poses remain WORKING.

Camera: orthographic size **2.25**, position **(4,2.25,-10)**, 8×4.5 units. Hero remains **1.8 units** tall. Intended feet/floor-collider top: **Y=.5**, source pixel Y=960. Dynamic collision skin can place visible feet a few pixels above that line. Designer seams: home 850, guest 856, corridor 831; foreground 1080. Approximately 104–129 px of visible floor lie behind the feet and 120 px below. Neither back-wall seam nor a platform cut is the support line. No added movement axis or fake character/furniture stretching.

Local interaction reach is .8 units. Home: phone around X=1.85 (player starts 2.15), case 4.35, bag 5.14, exit 6.32. Reception counter 6.2, door 1.8. Guest receiver 3.64, bag 3.75, door 1.5. Corridor own-room door 1.92, man 5.08. Colliders match sprite bounds/visible doors. Approach the receiver for pickup; bag inspection alone never collects it.

## Assigned runtime sound

PrologueAudio and copied CurrentJourney use supplied prologue v01 clips. Current arrival mode never plays tuning or shows the old bridge; historical definitions/prefabs retain their previous behavior.

CurrentMotel/FirstNightAudio has serialized sources and clips from `Assets/_Game/Audio/CurrentFirstNight/`:

| File | Runtime event |
| --- | --- |
| amb_motel_guest_room_loop | One power-independent ambience loop; reception/corridor reuse is working material. |
| sfx_corridor_cloth_soft_01 | First guest-room entry, before any radio use. |
| sfx_corridor_step_soft_01 | Sparse ordinary outside movement; separate actual walking contacts. |
| sfx_guestroom_door_soft_01 | Actual area-door action, never a timed man's departure cue. |
| sfx_radio_retrieve_soft_01 | Explicit E retrieval, independent of power. |
| sfx_radio_on / sfx_radio_off | Actual carried Shift transitions, no arrival/startup click. |
| radio_motel_receiver_texture_loop | One carried ON-only receiver bed. |
| radio_motel_announcement_WORKING_TTS | English program review while ON; synthetic working voice, no presenter identity. |

WAVs are 48 kHz/24-bit PCM, Decompress On Load, original channels. They were copied unchanged and hash-checked with manifest/provenance. Sources/REAPER sessions/demo mixes were not imported as a fixed soundtrack. Gains are adjustable per source; no normalization was baked into files.

OFF stops speech/texture immediately; ambience remains. Esc stops program speech; finishing lets its short tail continue while ON. Switching to RU stops English speech: RU uses localized readable text and receiver texture. Reopening EN review restarts one voice instead of stacking it. Disable/unload stops all sources; re-enable resumes appropriate loops. Missing clips never block reading.

**Missing:** complete ordinary music break/radio music program, score and Russian voice. Hiss is receiver texture, not delivered music. Synthetic English speech/procedural effects require human listening/mix review; no final human performance or morning voice is implied.

## Components and maintenance

- FirstNightController keeps existing v02 facts. New serialized floor/spawn fields configure only CurrentMotel; default values preserve old v02. AreaEntered/ReceiverRetrieved events notify presentation without coupling player movement to audio.
- FirstNightAudio on the first-night root references controller, reader, radio, body, program document assets, eight sources and six event clips. OnEnable subscribes and resumes loops, OnDisable unsubscribes/stops. Speech language follows Unity Localization. All Inspector slots are assigned.
- CurrentActorVisuals on Player references Rigidbody2D and a scale-compensated facing root; motel also assigns controller/carried sprite. LateUpdate mirrors art with horizontal velocity and follows HasReceiver. Home intentionally leaves those last two optional slots unset.
- PrologueDefinition.PlayableCheckInArrival is set only on Data/CurrentPlayable/CurrentPrologue.asset. PrologueJourney restores destination inputs after release without an arrival overlay in that mode; historical mode remains unchanged. No old witness flags are copied.
- CurrentPlayableInstaller opens an already-assigned current entry without rebuilding it. Initial construction copies historical scene wiring once. Do not delete/recreate current scenes to apply manual changes. All runtime references are serialized; normal play requires no installer or manual reassignment.

## Actual checks and screenshots

The end-to-end test starts only at CurrentPrologue and walks using Input System A/D/E/Shift events. It never teleports or manually opens intermediate scenes. It completes call/case/packing, real travel, held-E release, registration, first excursion without receiver, introduction, retrieval/program and empty return on EN and RU. It verifies source playback, RU speech suppression, OFF cleanup, independent ambience and no old investigation flags.

Actual Unity URP renders with panels closed and feet visible, at **1280×720 and 960×540 for EN/RU**, are under `TestResults/CurrentPlayable/`:

1. `01-home-phone-{locale}-{width}.png`
2. `02-home-desk-{locale}-{width}.png`
3. `03-home-door-{locale}-{width}.png`
4. `04-reception-{locale}-{width}.png`
5. `05-guest-before-pickup-{locale}-{width}.png`
6. `06-corridor-meeting-{locale}-{width}.png`
7. `07-guest-carried-{locale}-{width}.png`
8. `08-corridor-empty-{locale}-{width}.png`

The programmer inspected these positions: feet sit within the floor, scale/facing and single carried receiver match the working placement. A HUD backing was added after a lamp reduced control-text contrast. Capture snapshots UI temporarily rather than changing the live Canvas mode; world imagery is real Unity rendering, not offline composition. Generated reports/images remain Git-ignored local review artifacts.

Final shared-checkout run: **33/33 PlayMode, 7/7 EditMode**, both exit 0. Reports: `TestResults/current-all-playmode.xml`, `TestResults/current-all-editmode.xml`; full route also has its focused `current-route-playmode.xml` report. The scoped export on HEAD passed **19/19 PlayMode, 5/5 EditMode** with a fresh Library: `TestResults/current-isolated-playmode.xml` / `current-isolated-editmode.xml`. Its master output alone was muted to avoid mixing a second Editor into the owner's live play session; runtime source gains/clips and playback assertions were unchanged.

A native visible Editor pass observed the real CurrentMotel reception and then the guest room after pickup while the owner supplied input. Floor depth, facing/carried sprite, HUD and zero Console counters were visible. The automation did not stop or take over that active Play session. The independent full-route test drove the whole route and inspected camera renders; a full human atmosphere/pacing and listening approval is still not claimed. Main-project audio is enabled with master volume 1; no main-project mute was applied.

The staged file list/partial localization and Build Settings slices were reviewed. Runtime files match the tested export, excluding the report updated with these results. Whitespace checks apply to source/docs/JSON; generated Unity YAML/meta keep Unity formatting. The actual commit hash is supplied in the final delivery, since this report cannot embed its own hash.

## Preservation and gaps

Commit scope: current scenes, definition/prefab, supplied assigned runtime art/audio, required previously uncommitted prologue code/document/audio dependencies, small shared hooks, current launch entries, metadata, tests, README and this guide. Index selection excludes unrelated narrative/production edits and historical v01 localization rows. Existing scene contents are preserved. No push is authorized.

Remaining production gaps are fixed poses/static painted furniture/doors, final human/localized voice, a music program/score, first-time human pacing review and the unwritten morning inquiry. The playable endpoint is the empty corridor; no save system, morning scene, later testimony or published build is implied.
