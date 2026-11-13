# 89.4 — Title, Radio Hardware and Signal Geography

## Title
**Preferred working title: 89.4**. Communicates an exact frequency on a vintage radio dial and creates an immediate question: what happens on this frequency? It can be used as logo, title-screen display and recurring motif. **Not yet legally/marketplace checked** for uniqueness, nor locked to a technically valid broadcast range in every locale. Leave frequency configurable for gameplay and localization.

**DEAD AIR** remains an internal codename for older assets and docs during prototyping; rename the Unity repo / directories only when it doesn't disrupt work. The fictional town has **no confirmed name**. Location-based naming remains an option if the setting develops a stronger identity.

## Core spatial design hypothesis (WORKING)
A **station/transmitter** influences the town's stable world-state, while the **portable radio** stabilizes the protagonist's immediate perception, memory coherence and sometimes local manifestations. Two overlapping layers:

```text
TOWN LAYER: transmitter / official broadcast -> which version of the city is dominant
PERSONAL LAYER: portable receiver -> what protagonist perceives, remembers and resists
DRIFT: signal absent/weak locally -> suppressed people/places become accessible
```

This is a *game-design hypothesis*, not settled science or lore. A single handheld radio should **not** have to recalculate the state of every building in town each time it is toggled. The first slice can simulate local suppression in a single hallway.

## Portable vs stationary devices
- **Carried receiver:** normally always in the player's possession and accessible via SHIFT. Battery-limited. Enables risk-versus-information choices at any suitable scene.
- **Stationary receivers:** possible safe pockets in diner, motel lobby, homes. Could anchor recordings or recover memory. Not required in initial MVP.
- **Transmitter / radio tower:** atmospheric landmark and mystery source, not directly player-controlled at first.
- **Exceptional radio-loss scene:** potential later scripted set piece only; avoid frequent forced loss of the primary mechanic.

## Proposed questions to test
1. Does pressing SHIFT toggle receiver power or tune two frequencies? Prototype power toggle first.
2. Does ON protect only when station signal reaches the player? Avoid coverage simulation in MVP.
3. Does stationary radio offer wider safety, recording, or battery replenishment? Add only if fun.
4. Does OFF allow the protagonist to remember *different* things, or actually forget recent facts? Treat both as experiments.
5. Can the entire city drift while player is ON? Possibly, but don't implement until story rules are precise.

## UI and audio motifs
- Frequency `89.4` on LCD/dial and title.
- Reception strength via audible static, tuning clicks and lighting changes rather than giant progress bars.
- Concentric coverage visual: stable town broadcast and weaker personal reception, only if the mechanic becomes spatial.
- Repeat memorable radio jingle in distorted/off-signal variants; sonic identity is likely more important than technical realism.
