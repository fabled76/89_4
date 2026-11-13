# Investigation Design & Threats

## LOCKED canonical scene — The Rejected Witness
1. Player enters motel corridor with radio OFF (or loses signal); sees unnerving human figure approaching.
2. Reacts by inserting batteries / switching radio ON. The figure vanishes. Player feels safe.
3. Later player reads motel guest register. The strange figure's name appears as a witness to the missing radio host's disappearance.
4. Player infers that the "enemy" might be a source of necessary truth.
5. Player **chooses** to return, turn radio OFF while able to protect themselves, and **allow the figure to approach**.
6. Encounter yields testimony/key evidence. The risk was intentional, rewarded, and narratively coherent.

**Non-negotiable design intent:** the threat must first be perceived as dangerous, then reframed as potentially informative. The reveal must be supported by the ledger, not an arbitrary NPC announcement.

## Threat catalogue — options, not all committed
- **Rejected / Unaccepted:** erased residents, figures visible only in drift, may pursue, alter paths, sometimes communicate. Strong MVP candidate.
- **Dead Zones:** corridor loops, warped doors, broken spatial expectations, an environmental risk with little AI cost. Strong MVP candidate.
- **Tuners:** town technicians/administrators enforce signal stability and hide information; later stretch goal.
- **Echo:** apparent double of protagonist repeats or anticipates actions; only if identity plot merits it; experimental.

## Encounter implementation for MVP
- An entity has distinct states: `Hidden`, `Appearing`, `Approaching`, `ReadyToSpeak`, `Suppressed`.
- Radio ON suppresses the figure; radio OFF permits the encounter.
- First meeting is a **safe scripted scare**, not instant lethal enemy.
- Follow-up meeting allows deliberate approach and testimony after `GuestLedgerRead` flag.
- If player retreats too soon, figure remains available and no progress is lost.
- Use a clear audio/animation tell as it approaches. Avoid random insta-fail.

## Investigation graph (slice)
`Initial encounter` → `Radio safety established` → `Guest ledger inspected` → `Witness identified` → `Return to corridor / OFF` → `Witness testimony` → `Compare testimony with official radio transcript` → `Deduction: official chronology is false`.

## Evidence board (minimum)
- `GuestLedger`: name and date from motel guest list.
- `OldBroadcastTranscript`: official timeline for missing radio host.
- `WitnessStatement`: first-hand alternate timeline, only when approached without signal.

## Real deduction interaction
Prompt: "Which evidence contradicts the official account?" Let player choose two items and state conclusion. Correct response unlocks next lead; incorrect response gives specific feedback, without consuming evidence or blocking progression. Do not make clues auto-combine just because collected.

## Threat principles
- Being pursued is not identical to being attacked.
- A danger must be legible enough to be strategically approached.
- Environmental hazards and witnesses must share fiction rules.
- Minimize repeated chase loops; prioritize a single memorable reversal.

## LOCKED: nonlethal failure philosophy
Do not add health points, lethal chase hits or a traditional enemy Game Over. A contact can:
1. **Obscure a fresh unrecorded observation** in the hero's notebook (never delete the technical save or anchored evidence).
2. Change the hero's perception temporarily: mistaken door labels, muffled voices, uncertain dialogue options.
3. Move the player to a safe but altered nearby area, forcing new wayfinding.
4. Increase instability without an instant fail screen.

A strong setback should unlock an **alternative path** to recover understanding, not simply require repeating the previous five minutes. Critical witnesses cannot become permanently unavailable.

## Revised motel witness beat (LOCKED)
- FIRST: a silhouette approaches in silence; the player's instinctive defense is to power the radio, which suppresses the figure.
- DISCOVERY: the guest register associates the figure with the missing radio host. The danger is reframed as an investigative lead.
- SECOND: the player, with a functioning radio and the *option* of safety, consciously turns it off and lets the figure approach. Instead of attacking, it provides testimony.
- OPTIONAL EXPERIMENT: if the player is caught by a different aggressive manifestation, one fresh observation becomes temporarily obscured. It can be restored from a recording or external clue. **Do not make this punishment mandatory in the canonical witness conversation.**

## Pending threat roster
**MVP:** the Rejected witness and one spatial distortion; **later:** signal-maintenance technicians (human threat); **experimental:** protagonist's Echo double if identity design warrants it. All threats must have visible tells, deliberate response possibilities and useful rewards for taking risks.
