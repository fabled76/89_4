# Vertical Slice — 5–10 Minute Playable

## Goal
Demonstrate the core promise in one motel corridor: **turning OFF the radio can reveal essential truth despite the fear it creates**.

## Proposed level
Reception (ledger, battery pickup) ↔ corridor (entity) ↔ room with radio broadcast clipping. Side-scrolling 2D, one continuous scene; graphics may be boxes and temporary sprites.

## Player flow
1. Start in motel lobby, portable radio ON, introductory audio line; see battery UI.
2. In corridor, prompt a first deliberate radio-off or brief forced battery outage. See silhouette and threatening sound.
3. Switching ON removes silhouette; player feels control.
4. Reception: discover ledger naming silhouette as witness to missing presenter.
5. Obtain official radio transcript in room / on reception desk.
6. Go back to corridor, switch OFF voluntarily, allow figure close enough to speak.
7. Figure gives alternate account; journal captures evidence.
8. Combine witness statement and transcript to conclude broadcast timeline was fabricated.
9. Exit / end-of-slice card. No plot-origin reveal.

## Inputs (prototype defaults)
A/D or arrows = walk; E = interact; Left Shift = toggle radio; R = replace battery; Tab = journal; Esc = pause.

## Tests / acceptance
- OFF always visibly/audibly changes environment.
- Radio ON always conveys safety from the figure, without destroying evidence already discovered.
- Figure doesn't kill or permanently softlock player in first encounter.
- Player can exit and resume; technical save persists.
- Witness dialogue cannot be accessed before appropriate clues if that would break deductive arc.
- Player can choose OFF even at full charge.
- Every critical battery requirement has a guaranteed recovery path.
- Deduction is player-initiated; wrong choice recoverable.

## Scope guard
No combat, no open town, no multiple story arcs, no complex inventory crafting, no simulated day cycle. Make this first; then add diner/radio-station if schedule allows.

## Suggested manual playtest questions
- Why did you initially turn the radio on?
- Why did you turn it back off?
- Did you infer the figure might know something before the game told you?
- Were you confused by "record" versus "save game"?
- Did batteries create a meaningful decision or merely annoyance?

## Version 0.2 additions
- No HP UI, death animation, health pickup, or game-over screen.
- Radio's physical presence: show the receiver on the protagonist or as an accessible UI panel; local power ON/OFF changes visibility of the witness. The town-wide influence remains background fiction in this slice.
- One optional **recoverable cognitive setback**: a nonessential fresh observation becomes `Obscured` after an ill-advised contact, then can be reconstructed from the ledger. Keep this out of the initial critical path if it expands scope.
- A powered stationary lobby radio is a **stretch** demonstration of a safe area, not required for first test.
- Use title **89.4** in UI placeholders while assets/repository names may still say DeadAir.

## Revised acceptance tests
1. On-signal is safer; off-signal has unique useful evidence; player understands tradeoff.
2. The witness doesn't kill; intentional contact delivers a meaningful clue.
3. Obscured facts (if implemented) have a clear recovery path; anchored facts aren't erased.
4. Stable/unstable changes have audio and visual feedback and cannot softlock.
5. Save/load works with empty batteries independently of radio recording.
