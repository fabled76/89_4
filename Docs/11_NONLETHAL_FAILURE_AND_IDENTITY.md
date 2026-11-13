# Nonlethal Failure, Memory, and Identity — Design Exploration v0.2

## Status labels
- **LOCKED:** player does not die from threats, and there is no HP bar / regular Game Over. The game remains a detective investigation.
- **LOCKED:** entity encounters can complicate investigation, particularly by obscuring recently gained **unrecorded** information. Never permanently destroy essential clues.
- **WORKING:** town signal and portable receiver may stabilize protagonist perception and continuity; this is not equivalent to declaring a mental health condition.
- **OPEN:** whether mental stability, memory retention or identity change is represented numerically; whether multiple personas exist at all.

## Core principle
**Failure changes the investigation, not whether you are allowed to continue it.** A lost encounter should create a new problem worth solving, not force replay of already completed content.

## Contact outcomes (choose one, test before scaling)
| Consequence | Player-visible behavior | Reward / recovery | Scope |
|---|---|---|---|
| Obscured observation | Hero's note becomes fragmented or unreadable; item remains logged | Replay a recorded fact, talk to another witness, re-examine source | Best initial experiment |
| Perception drift | Wrong room labels / distant voices / transient visual mismatch | Reveal otherwise hidden writing, then stabilize via radio | Moderate |
| Spatial displacement | Player emerges in another variant of same hallway | New clue or shortcut; guaranteed path to safety | Moderate |
| Identity bleed | Different recognition/dialogue reactions | Learn a hidden connection to town | Narrative stretch |

## Evidence integrity contract
- Facts can be `Fresh`, `Anchored`, `Obscured`, or `Recovered`.
- Obscured != erased from disk; persistence includes evidence IDs and original acquisition state.
- Anchoring via radio recording should preserve facts against subsequent contact (prototype hypothesis).
- Every mandatory clue has an accessible restoration path; avoid dependencies on a single finite battery.
- Knowledge the **human player** has inferred isn't forcibly taken from them; the protagonist may nevertheless lose a dialogue option until the evidence is reconstructed.
- If a consequence adds no interesting recovery route, drop it. It is just busywork.

## Sanity/identity presentation options
A. **No visible meter (recommended first):** radio static, speech hesitation, room perspective and subtle HUD notes convey destabilization. Avoid simplistic 'insanity = evil'.
B. **Coherence meter:** a stability indicator, not life points. Low values unlock anomalous content as well as hazards; no loss screen at zero.
C. **Identity layers:** another set of memories/dialogue preferences, potentially an external version of the hero. Not required and no clinical label implied.

## Fair encounter example
1. In an altered motel hallway, the protagonist finds a fresh entry in a register.
2. In the drift, a separate manifestation touches the protagonist. The name in their notes appears unreadable, while its physical source or saved recording persists.
3. Player can replay radio evidence, ask the clerk, or return to the register through a changed route.
4. Recovery produces new information or recontextualizes a contradiction, rather than mere repetition.

## Playtest questions
- Did the threat feel serious even though you couldn't die?
- Did you understand exactly **what** changed after contact?
- Was recovery itself interesting, or was it grind?
- Did you voluntarily take a risk to learn something?
- Did the system ever remove a mandatory fact with no way forward?
