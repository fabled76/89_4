# 89.4 — Motel Corridor prototype

Unity **6000.3.8f1**, Universal 2D/URP, installed Unity Input System. Early mechanics foundation; all art is placeholder. Design context is in `Docs/00_README.md` onward.

Open this repository in Unity Hub, let compilation finish, then open `Assets/_Game/Scenes/MotelCorridor.unity`. If absent, run **89.4 → Create Motel Corridor Prototype**. Press Play and click Game view.

- **A/D or arrows:** walk.
- **Left Shift:** toggle portable radio. ON → STABLE; OFF → DRIFT and visible silhouette.
- **E:** nearest valid interaction. Green station near start refills battery indefinitely; marker farther right logs a test message.

Battery drains only while ON; empty forces OFF. Refilling does not turn the radio on automatically. Default battery lasts about 20 seconds to make exhaustion easy to test. No AI, combat, inventory, narrative encounter, saves or recordings are implemented.

Test walking against both walls, toggling repeatedly, waiting for exhaustion, refilling at the station and interacting in/out of range. Exit and re-enter Play with Console visible. Run EditMode/PlayMode tests from Window → General → Test Runner.

See [exact setup and subsystem teaching guide](Docs/PROTOTYPE_SETUP.md) and [implementation and verification checklist](Docs/PROTOTYPE_VALIDATION.md) for references, troubleshooting, actual verification limits and the next three tasks.
