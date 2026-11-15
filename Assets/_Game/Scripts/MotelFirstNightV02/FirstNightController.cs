using System.Collections;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;

namespace DeadAir
{
    // Versioned first-night facts, independent of the earlier disappearance/investigation trials.
    public sealed class FirstNightController : MonoBehaviour
    {
        [SerializeField] private PrologueReadingPanel reader;
        [SerializeField] private Rigidbody2D player;
        [SerializeField] private InteractionController interactor;
        [SerializeField] private RadioController radio;
        [SerializeField] private RadioInput radioInput;
        [SerializeField] private BatteryComponent battery;
        [SerializeField] private GameObject reception, guestRoom, corridor, personVisual;
        [SerializeField] private SpriteRenderer roomReceiver;
        [SerializeField] private EvidenceDefinition checkIn, inspection, retrieval, retrievalAfterClaim, programBeforeClaim, program, introduction, emptyCorridor, needCheckIn, needPower;
        [SerializeField] private TMP_Text location, objective, status, controls, prompt;
        [SerializeField] private string productionCaptionKey = "first_night.production.caption";
        [SerializeField] private float standingFeetY;
        [SerializeField] private float receptionSpawnX = -4, roomEntryX = -4, roomReturnX = 4, corridorEntryX = -4;
        public event Action<FirstNightArea> AreaEntered;
        public event Action ReceiverRetrieved;
        private bool emptyPending, emptyPresented;
        public FirstNightArea Area { get; private set; }
        public bool CheckedIn { get; private set; }
        public bool HasReceiver { get; private set; }
        public bool MovementHeard { get; private set; }
        public bool Introduced { get; private set; }
        public bool ProgramReviewed { get; private set; }
        public bool ManDeparted { get; private set; }
        public bool ReturnedEmpty { get; private set; }
        public bool ManVisible => Area == FirstNightArea.Corridor && !ManDeparted && !radio.IsOn;
        public PrologueReadingPanel Reader => reader;

        private void OnEnable()
        {
            radio.StateChanged += OnPower; battery.ChargeChanged += OnCharge;
            LocalizationSettings.SelectedLocaleChanged += OnLocale;
            StartCoroutine(RefreshLocale());
        }
        private void Start()
        {
            radioInput.enabled = false;
            radio.SetPower(false);
            Enter(FirstNightArea.Reception, receptionSpawnX);
        }
        private void OnDisable()
        {
            radio.StateChanged -= OnPower; battery.ChargeChanged -= OnCharge;
            LocalizationSettings.SelectedLocaleChanged -= OnLocale; StopAllCoroutines();
        }
        private void OnLocale(Locale _) => StartCoroutine(RefreshLocale());
        private IEnumerator RefreshLocale()
        {
            yield return LocalizationSettings.InitializationOperation;
            Refresh();
        }
        private void OnCharge(float _) => Refresh();
        private void Update()
        {
            prompt.text = interactor.CurrentPrompt; status.enabled = !reader.IsOpen;
            if (emptyPending && !reader.IsOpen && ReadingNavigation.KeysReleased && PrototypeText.IsReady)
            {
                emptyPending = false; emptyPresented = true; reader.Show(emptyCorridor);
            }
        }
        public bool CanUse(FirstNightAction action)
        {
            switch (action)
            {
                case FirstNightAction.CheckIn: case FirstNightAction.GuestDoor: return Area == FirstNightArea.Reception;
                case FirstNightAction.RetrieveReceiver: return Area == FirstNightArea.GuestRoom && !HasReceiver;
                case FirstNightAction.ReviewReceiver: case FirstNightAction.CorridorDoor: return Area == FirstNightArea.GuestRoom;
                case FirstNightAction.ReturnToRoom: return Area == FirstNightArea.Corridor;
                case FirstNightAction.Man: return ManVisible;
                case FirstNightAction.LookAround: return Area == FirstNightArea.Corridor && ManDeparted;
                case FirstNightAction.TestBattery: return true;
                default: return false;
            }
        }
        public void Use(FirstNightAction action)
        {
            if (reader.IsOpen || !PrototypeText.IsReady || !CanUse(action)) return;
            switch (action)
            {
                case FirstNightAction.CheckIn: reader.Show(checkIn, () => { CheckedIn = true; Refresh(); }); break;
                case FirstNightAction.GuestDoor:
                    if (!CheckedIn) reader.Show(needCheckIn); else Enter(FirstNightArea.GuestRoom, roomEntryX);
                    break;
                case FirstNightAction.RetrieveReceiver:
                    // A physical E action, not a mandatory reading-completion or inventory puzzle.
                    HasReceiver = true; roomReceiver.enabled = false; radioInput.enabled = true;
                    ReceiverRetrieved?.Invoke();
                    reader.Show(Introduced ? retrievalAfterClaim : retrieval); Refresh(); break;
                case FirstNightAction.ReviewReceiver:
                    if (!HasReceiver) reader.Show(inspection);
                    else if (!radio.IsOn) reader.Show(needPower);
                    else reader.Show(Introduced ? program : programBeforeClaim, () => { if (Introduced) ProgramReviewed = true; Refresh(); });
                    break;
                case FirstNightAction.CorridorDoor: Enter(FirstNightArea.Corridor, corridorEntryX); break;
                case FirstNightAction.ReturnToRoom: Enter(FirstNightArea.GuestRoom, roomReturnX); break;
                case FirstNightAction.Man: reader.Show(introduction, () => { Introduced = true; Refresh(); }); break;
                case FirstNightAction.LookAround: reader.Show(emptyCorridor); break;
                case FirstNightAction.TestBattery: battery.ReplaceBattery(); break;
            }
        }
        private void Enter(FirstNightArea area, float spawnX)
        {
            bool leavingAfterConversation = Area == FirstNightArea.Corridor && area == FirstNightArea.GuestRoom && Introduced;
            Area = area;
            reception.SetActive(area == FirstNightArea.Reception);
            guestRoom.SetActive(area == FirstNightArea.GuestRoom);
            corridor.SetActive(area == FirstNightArea.Corridor);
            // The corridor has already been removed from view. Departure has no dependency on power.
            if (leavingAfterConversation) ManDeparted = true;
            if (area == FirstNightArea.GuestRoom) MovementHeard = true;
            if (area == FirstNightArea.Corridor && ManDeparted)
            {
                ReturnedEmpty = true;
                if (!emptyPresented) emptyPending = true;
            }
            player.position = new Vector2(spawnX, standingFeetY + .93f); player.linearVelocity = Vector2.zero;
            player.transform.position = player.position; Physics2D.SyncTransforms();
            personVisual.SetActive(ManVisible); Refresh();
            AreaEntered?.Invoke(area);
        }
        private void OnPower(bool on)
        {
            if (personVisual == null) return;
            // Optional premature toggles retain suppression; they never mark a witnessed event.
            personVisual.SetActive(ManVisible);
            if (on && reader != null && reader.ActiveDocument == introduction) reader.Dismiss(false);
            Refresh();
        }
        private void Refresh()
        {
            if (!PrototypeText.IsReady || location == null || objective == null || status == null || controls == null) return;
            string areaKey = Area == FirstNightArea.Reception ? "reception" : Area == FirstNightArea.GuestRoom ? "room" : "corridor";
            location.text = Text("location." + areaKey);
            string goal = Area == FirstNightArea.Reception ? (CheckedIn ? "room" : "checkin")
                : !Introduced ? (Area == FirstNightArea.GuestRoom ? "corridor" : "contact")
                : !HasReceiver ? "retrieve" : !ProgramReviewed ? "check" : !ReturnedEmpty ? "return" : "end";
            objective.text = Text("objective." + goal);
            status.text = Area == FirstNightArea.GuestRoom && !Introduced
                ? Text("movement.caption") + "\n" + Text("room.person_awake") : PrototypeText.Get(productionCaptionKey);
            controls.text = PrototypeText.Get("first_night.controls", Text(HasReceiver ? "receiver.carried" : "receiver.packed"), Text(radio.IsOn ? "radio.on" : "radio.off"), Mathf.CeilToInt(battery.Fraction * 100));
        }
        private static string Text(string suffix) => PrototypeText.Get("first_night." + suffix);
    }
}
