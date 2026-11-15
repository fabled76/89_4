using UnityEngine;

namespace DeadAir
{
    public sealed class PrologueAudio : MonoBehaviour
    {
        [SerializeField] private PrologueController controller;
        [SerializeField] private PrologueDefinition definition;
        [SerializeField] private RadioController radio;
        [SerializeField] private PrologueReadingPanel reader;
        [SerializeField] private AudioSource ambience;
        [SerializeField] private AudioSource broadcast;
        [SerializeField] private AudioSource ring;
        [SerializeField] private AudioSource receiver;
        [SerializeField] private AudioSource switchSource;
        [SerializeField] private AudioSource hangupSource;
        [SerializeField] private AudioClip switchOnClip;
        [SerializeField] private AudioClip switchOffClip;
        [SerializeField] private AudioClip hangupClip;
        [SerializeField, Min(1f)] private float ringInterval = 6f;
        [SerializeField, Range(0f, 1f)] private float callDuck = .25f;
        private float ringTimer = .5f, previousVolume;
        private bool ducked;
        private bool audibleSwitches;
        public AudioSource Broadcast => broadcast;
        private void OnEnable()
        {
            radio.StateChanged += OnPower;
            reader.Opened += OnDocumentOpened; reader.Closed += OnDocumentClosed;
            ResumePlayback();
        }
        private void Start()
        {
            ResumePlayback();
        }
        private void OnDisable()
        {
            radio.StateChanged -= OnPower;
            reader.Opened -= OnDocumentOpened; reader.Closed -= OnDocumentClosed;
            RestoreVolume();
            foreach (var source in new[] { ambience, broadcast, ring, receiver, switchSource, hangupSource })
                if (source != null) source.Stop();
        }
        private void ResumePlayback()
        {
            if (ambience.clip != null && !ambience.isPlaying) ambience.Play();
            SetBroadcast(radio.IsOn);
        }
        private void LateUpdate() => audibleSwitches = true;
        private void OnPower(bool on)
        {
            SetBroadcast(on);
            if (!audibleSwitches || switchSource == null) return;
            switchSource.Stop();
            var clip = on ? switchOnClip : switchOffClip;
            if (clip != null) switchSource.PlayOneShot(clip);
        }
        private void SetBroadcast(bool on)
        {
            if (on && broadcast.clip != null) { if (!broadcast.isPlaying) broadcast.Play(); }
            else broadcast.Stop();
        }
        private void Update()
        {
            if (controller.CallCompleted || reader.ActiveDocument == definition.Call || controller.DepartureStarted) return;
            ringTimer -= Time.deltaTime;
            if (ringTimer > 0f) return;
            if (ring.clip != null) ring.Play();
            ringTimer = ringInterval;
        }
        private void OnDocumentOpened(EvidenceDefinition document)
        {
            if (document != definition.Call) return;
            ring.Stop();
            if (hangupSource != null) hangupSource.Stop();
            if (receiver.clip != null) receiver.Play();
            previousVolume = broadcast.volume; broadcast.volume *= callDuck; ducked = true;
        }
        private void OnDocumentClosed(EvidenceDefinition document, bool _)
        {
            if (document == definition.Call)
            {
                if (receiver != null) receiver.Stop();
                if (hangupSource != null && hangupClip != null)
                {
                    hangupSource.Stop(); hangupSource.PlayOneShot(hangupClip);
                }
                RestoreVolume(); ringTimer = ringInterval;
            }
        }
        private void RestoreVolume()
        {
            if (!ducked) return;
            if (broadcast != null) broadcast.volume = previousVolume;
            ducked = false;
        }
    }
}
