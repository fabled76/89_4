# Diegetic Recordings, Save/Load, Memory

## Critical architectural distinction
**Technical save:** durable game state required for correct resume, autosave/checkpoints, safe recovery after crash, independent of radio availability. **Diegetic recording:** an in-world action which can preserve data about a changing city and enable deductions. They may be presented together but must not be the same failure-prone subsystem.

## Options — explicitly NOT final
**A. Snapshot-as-evidence (recommended MVP).** A radio recording captures facts (date, population, official name, guest list clue, voice). Past records remain unchanged even after world shifts. Compare current observation to archival record. Low tech risk.

**B. Reality anchor.** A recorded state pins a region to the current version. More complex: which characters, rooms and evidence are affected?

**C. Memory anchor.** Recordings preserve the protagonist's memory across drift; without one they forget selected observations, but the *player's irreversible progression* must remain safe.

**D. Identity anchor.** The radio preserves a version of the self. OFF changes responses or identity; a recording restores/compares earlier identity. High narrative and testing complexity. Interesting; unresolved.

**E. World restore.** Replaying past recordings rewinds world state. Highest implementation complexity, likely omit for jam.

## Prototype-specific design
Record a voice memo `REC 001`: "Motel corridor: room 12 registered to [witness name]". After a shift, the guest ledger has a contradictory entry. Both observations stay on evidence board. Record can be played to contrast the past/current story.

## Example data schema (conceptual)
```
SaveGame { version, sceneId, playerPosition, inventory, battery, worldPhase,
           instability, unlockedClues, completedDeductions, recordings }
Recording { id, locationId, observedWorldPhase, timeLabel,
            capturedFactIds, optionalVoiceLineId }
```
Use stable IDs, versioned JSON (or Unity serialization wrapper), write atomic/save-temp-then-replace if implementing disk saves. Do not serialize scene GameObjects as object references.

## UX
- Visible radio REC button and distinctive audio signature.
- On recording, show what was captured and allow replay.
- Do not claim "saved" if only an in-world observation was captured; reserve "game saved" for durable state.
- Allow safe pause/quit and resume regardless of in-game charge.

## Unanswered
Can the signal preserve identity? Does a recording cost battery? Can recordings be falsified? How many simultaneously accessible snapshots? Narrative decision later.

## New consequence model — remembered vs recorded (WORKING)
A newly found observation can be **fresh** in the hero's mind but not anchored. Contact with an entity may mark it `Obscured`, temporarily removing its journal interpretation or dialog option. A **radio recording** can turn a relevant observation into `Anchored` evidence that remains readable even when city history changes. The player should not be punished for accurately remembering a fact outside the game: distinguish character knowledge from player knowledge.

**Recovery examples:** re-examine an accessible physical source; ask an independent witness; compare archival recording and current document; reconstruct the idea on the evidence board. Main mystery proof cannot be irretrievably lost.

**Explicit open questions:** Does radio store *observed facts*, *subjective memories*, *the protagonist's identity*, or some mix? Is the audio an immutable archive or a contested one? Does a stationary radio strengthen records? Prototype Snapshot-as-evidence first; leave Identity Anchor optional.

## Example state extension
```text
Observation { id, sourceId, status: Fresh|Anchored|Obscured|Recovered,
              revealedInPhase, recoveryLeads[] }
CognitionState { coherenceLevel, activeEffects[] } // optional, no HP/death
RadioContext { localReceiverOn, stationFieldStrength, currentFrequency }
```
All statuses are conceptual. Technical autosave must persist even if an in-world observation is obscured.
