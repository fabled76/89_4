# 89.4 — Design & Prototype Pack

**Status:** pre-production / prototype, updated 2026-11-13. **Target:** Game Off 2026 (theme: SHIFT). **Preferred working title:** **89.4** (pronounced “eighty-nine point four”); **DEAD AIR** retained as an internal codename, not a final release title. The number currently denotes a provisional in-world radio frequency, not a canon scientific rule.

This folder consolidates creative discussions into actionable documentation for a Unity/Codex prototype. It is **not** a final approved narrative or a complete specification. Sections distinguish **LOCKED**, **WORKING**, **OPTION**, and **TBD** to avoid turning suggestions into canon.

## Read in this order
1. `01_VISION_GDD.md` — concept, design pillars, scope.
2. `02_RADIO_AND_SHIFT.md` — radio states, battery economy, risk/reward.
3. `03_SAVE_AND_MEMORY.md` — diegetic records versus durable technical saves.
4. `04_WORLD_AND_LORE.md` — world, atmosphere, rules and open mysteries.
5. `05_INVESTIGATION_AND_ENCOUNTERS.md` — real deduction and the **locked** Rejected encounter.
6. `06_VERTICAL_SLICE.md` — first playable 5–10 minute test.
7. `07_UNITY_ARCHITECTURE.md` — implementation boundaries, data and tests.
8. `08_UNITY_LEARNING_PATH.md` — learning Unity without losing the jam.
9. `09_CODEX_TASKS.md` — incremental prompts for Codex.
10. `10_BACKLOG_AND_DECISIONS.md` — priorities, hypotheses, outstanding decisions.
11. `11_NONLETHAL_FAILURE_AND_IDENTITY.md` — nonlethal threats, recoverable memory loss, cognition model.
12. `12_TITLE_AND_SIGNAL_GEOGRAPHY.md` — 89.4 naming decision, personal versus citywide signal, spatial design.

## What is fixed
- Small-town, roughly 1970s–1980s, melancholic mystery with genuine supernatural phenomena.
- Investigative protagonist (journalist is the current preferred role).
- A portable **radio is the defining SHIFT interaction**.
- Batteries constrain safety, investigation and recording; no-signal play is harder but offers meaningful rewards.
- **Locked encounter:** a strange figure appears in a motel corridor when radio is off. The player turns on the radio, making it disappear. Later a guest ledger reveals the figure was a witness to the radio host's disappearance. To get the truth, the player must return and **voluntarily turn off the radio and let the figure approach**.
- **Core promise:** “You can avoid most dangers by keeping the radio on, but then you will never learn the whole truth.”
- In-world recordings/save points are part of the narrative and may become evidence.
- **LOCKED design intent:** no conventional HP, combat or player death / Game Over from enemy contact. Threats create recoverable investigative and perceptual consequences.
- **WORKING hypothesis:** radio stabilizes **both town reality and protagonist cognition/continuity**, not necessarily at identical scale or strength. Exact radius and mechanism remain TBD.
- **WORKING hardware:** protagonist typically carries a battery-powered portable receiver; larger stationary receivers/transmitters provide stronger or wider safe pockets. Temporary scripted loss of access may be tested, but is not the default loop.

## What is deliberately not fixed
- Whether protagonist has dissociative identity disorder, a second supernatural identity, dual perceptions or neither.
- Precise supernatural mechanism, origin of signal, town conspiracy and ending.
- Whether recording fixes the world, memories, identity, merely witnesses an older state, or combinations.
- Exact range of portable/stationary signals; what changes immediately versus gradually; battery numbers; enemy AI specifics.
- Whether *memory*, *perception*, or *identity* are stored by diegetic recording; a sanity meter is experimental, not approved.
- Town name and final marketed title; frequency 89.4 is an evocative working motif, not locked canon.
- Art production route: pure 2D, layered 2D (recommended prototype) or true hybrid 2D/3D.

## Success metric
The graybox is successful when a tester can independently choose to expose themselves to danger in order to obtain a useful clue, and explain why the radio is not merely a visual toggle.

## Documentation revision v0.2
Added no-death threats, recoverable unrecorded-memory consequences, proposed carried/stationary radio split, the **town + protagonist** dual-stabilization hypothesis, and **89.4** as preferred working title. These changes supersede earlier suggestions of lethal pursuers or generic HP/sanity-as-health mechanics; other old codenames remain for compatibility.
