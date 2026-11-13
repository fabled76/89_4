# Radio, SHIFT, Batteries — Mechanic Spec v0.2

## Fixed principle
**Radio ON:** relatively stable, safer, conceals some truths. **Radio OFF / no working batteries:** more unstable, harder, access to hidden routes, clues, people, voices and evidence.

**SHIFT** is the thematic and mechanical radio action; exact key mapping is provisional. Suggest `Left Shift` for toggle during prototype, `E` for interact, `R` for insert batteries, `Tab` for evidence board.

## Suggested state machine (TEST, not canon)
- `RadioOnStable`: signal audible, town's stable presentation, entity hidden, low exposure.
- `RadioOffDrift`: static fades to raw ambience; instability rises; anomalies reveal.
- `RadioOnReanchor`: switch on after drift; current version stabilizes rather than necessarily rewinding to old version.
- `Record`: deliberate audio log / snapshot of observation; battery cost optional pending tuning.

Avoid implying world automatically reverts whenever radio turns on. Earlier direction: **it might not return**. A state-specific reset must be explicitly designed.

## Risk / reward table
| Feature | Radio ON | Radio OFF |
|---|---|---|
| Navigation | Known layout | Misleading or changed passages |
| Ordinary witnesses | Available | Some unavailable or altered |
| Rejected witnesses | Suppressed | Can approach / speak |
| Evidence | Official objects, documents | Erased names, anomalous objects |
| Risk | Low | Exposure to attention / instability |
| Progress | Safe investigation | Required investigative breakthroughs |

## Battery economy
- Battery is a **risk budget**, not a punishment countdown.
- Battery drains while radio is ON (hypothesis); radio OFF can preserve energy while increasing danger.
- Player can intentionally switch OFF while batteries remain.
- Consider one free emergency charge or guaranteed battery on the critical path to prevent softlocks.
- Optional replacement consumes a discrete battery item, not an opaque permanent failure.
- Reveal exact costs in UI/audio feedback. Never require memorization of hidden percentages.

## Instability (optional prototype system)
`0–24`: stable; `25–49`: mild distortions and hidden signs; `50–74`: figure notices protagonist; `75–100`: severe spatial drift, higher detection. Thresholds are exploratory placeholders, NOT balancing commitments.

A timer alone is not sufficient: create consequences visible in the world (sound detuning, changed signage, witness silhouette, corridor loop).

## Suggested first-pass tunables (all placeholders)
- Battery charge: 100 units.
- Radio ON drain: 1 unit per 5 seconds.
- OFF raises instability: 1 unit per 2 seconds.
- ON lowers instability: 1 unit per 1.5 seconds.
- No automatic instant death at high instability.
- Recording cost: start at 0 for playtests, then test a cost if it adds a meaningful choice.

These numbers are chosen for iteration only, not as implied agreed values. Expose in `ScriptableObject` so Codex needn't hardcode.

## Fairness rules
Every danger has: a tell before impact, an intelligible consequence, a response, and a payoff. An entity cannot destroy an irreplaceable clue. Drift can't permanently trap player. Battery exhaustion cannot make mission impossible.

## Open design questions
What exactly does a station frequency change? Does SHIFT toggle power, tune station, or switch personality? Can recording stabilize the world? Can an encounter override ON safety? Answer after prototype.

## Updated model: TWO TARGETS of stabilization (WORKING)
The same broadcast may anchor (1) **a citywide state sustained by the transmitter/network** and (2) **the protagonist's local perception/continuity via their portable receiver**. Their effects need not be identical: the station might keep the town broadly coherent while the handheld radio grants a **personal bubble** of clarity. Model these as separate flags/variables, not one global bool. The exact ontology remains an investigation mystery.

- Portable receiver: carried in ordinary play, battery-powered, can be switched off *voluntarily*; does not need to physically rebuild the whole city for each toggle.
- Stationary radios: hotel lobby, diner, homes or transmitter; possible regional safe pockets or functions (record, recover observations, tune unavailable frequencies). These are concepts, not confirmed unlocks.
- Silence: allows alternate paths, witnesses, and evidence; increases uncertainty and risk.
- Switching ON: can suppress an entity in local area, but need not revert previously altered city history.
- Never bind permanent story progression to a finite battery without a guaranteed way forward.

## Contact consequence instead of damage (LOCKED intent, implementation TBD)
An Unaccepted/Rejected entity reaching the character can obscure **recent observations not yet secured as radio records**. Do not remove the player's durable save, permanently delete critical evidence, or force repeated unskippable content. Prefer `Observation.status = Fresh | Anchored | Obscured | Recovered` and afford at least two practical recovery routes for a key observation. Contact may also increase local confusion (sound mix, changed labels, temporarily misleading navigation), then release the player. **No health or death meter.**

## Game-feel test
Give player one choice: ON keeps corridor stable but hides a person; OFF reveals their testimony and lets local distortion rise. The right reward must be *information*, not simply collectible batteries or score. Show the player that an avoidable unsettling encounter can be useful.
