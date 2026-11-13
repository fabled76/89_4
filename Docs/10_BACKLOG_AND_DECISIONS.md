# Backlog, Decision Log & Risk Register

## LOCKED: foundational decision (2026-11-13)
Player can avoid most danger with radio ON but cannot obtain the entire truth that way. A seemingly hostile figure disappears when radio ON; motel records identify it as a witness, incentivizing intentional return under OFF risk. This is the strongest confirmed playable beat. Do **not** invert its meaning without deliberate redesign.

## Must / should / could
**MUST:** walk + interact; radio on/off; visible/audio state differences; battery feedback; witness reversal; ledger and transcript; explicit deduction; safe quit/load.

**SHOULD:** one diegetic recording, ambient audio contrast, instability cue, a short NPC conversation, a distinctive no-signal visual treatment.

**COULD:** multiple frequencies, identity effects, extra town district, human Tuners, Echo double, nested replayable snapshots, multiple endings.

## Open decisions to playtest
- Radio action: power toggle versus frequency switch.
- Battery: charge consumption per second, replenishment and save cost.
- What exactly is stabilized: world, information, memory, identity?
- Whether diegetic save changes the world or only records it.
- Protagonist identity and origin of apparent split.
- Threat severity: stalking, distortion, loss of optional clue vs damage.
- Whether all radio-OFF changes reverse after ON.
- Which deduction is fair with three items.

## Risk register
| Risk | Likelihood | Mitigation |
|---|---|---|
| Scope explosion: multiple timelines | High | Build one corridor with two presentations |
| Radio OFF only frustrates | High | Gate valuable clue and witness behind OFF |
| Running out of batteries causes softlock | High | Guaranteed emergency option and puzzle checks |
| Diegetic save confuses real game save | High | Separate services and UI language |
| Detective becomes fetch quest | Medium | Require explicit hypothesis selection |
| Overdone 'split personality = evil' trope | Medium | Leave diagnosis undecided; no violent stereotype |
| Art/lighting complexity | High | Use plain sprites + layered parallax first |
| Codex scripts too tightly coupled | Medium | Narrow tasks, small APIs, playtest each stage |

## Next milestone
Produce a first complete 5–10 min motel prototype with the Rejected witness reversal. If it doesn't feel interesting, revise core loop before producing story, character backstory or polished art.

## 2026-11-13 follow-up decisions
**LOCKED:** no conventional HP/death/Game Over from encounters; threat causes nonlethal, recoverable disruptions. An Unaccepted may obscure recent **unrecorded** discoveries, but never delete permanent progress or irreplaceable evidence.

**LOCKED:** first threat can be deliberately approached for testimony; risk must pay information. Radio ON protects but suppresses some truth.

**WORKING:** player carries portable radio through ordinary gameplay; fixed radios/stations might create regional safe zones and offer expanded recording functions.

**WORKING:** signal stabilizes BOTH town reality and the protagonist's continuity of perception/memory. Exact coupling, transmission reach and origin TBD; should be prototyped with two separate state channels.

**WORKING TITLE:** **89.4**, with DEAD AIR retained only as former codename. Frequency/city name not permanently set.

**OPEN:** visible sanity meter vs implicit feedback; whether cognition shift becomes personality shift; effect of signal to entire city vs local receiver; which memories recording secures.

## New next steps
1. Implement simple radio ON/OFF and one world presentation switch; no HP.
2. Implement the locked witness reversal in motel.
3. Add snapshot-as-evidence and one contradictory observation.
4. Only then experiment with reversible loss of **one** unrecorded clue and restoration through an independent source.
5. Test stationary-radio safe zone and cognition changes if schedule permits.
