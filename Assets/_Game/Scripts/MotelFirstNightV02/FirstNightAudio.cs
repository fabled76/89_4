using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;

namespace DeadAir
{
    public sealed class FirstNightAudio : MonoBehaviour
    {
        [SerializeField] private FirstNightController controller;
        [SerializeField] private PrologueReadingPanel reader;
        [SerializeField] private RadioController radio;
        [SerializeField] private Rigidbody2D body;
        [SerializeField] private EvidenceDefinition[] programDocuments;
        [SerializeField] private AudioSource ambience, texture, voice, switches, door, handling, outsideMovement, footsteps;
        [SerializeField] private AudioClip onClip, offClip, doorClip, retrieveClip, clothClip, stepClip;
        private bool enteredOnce, movementPlayed, audibleSwitches;
        private float nextStep;
        public AudioSource Ambience => ambience;
        public AudioSource Texture => texture;
        public AudioSource Voice => voice;
        private void OnEnable()
        {
            controller.AreaEntered += OnArea;
            controller.ReceiverRetrieved += OnRetrieve;
            radio.StateChanged += OnPower;
            reader.Opened += OnReading; reader.Closed += OnClosed;
            LocalizationSettings.SelectedLocaleChanged += OnLocale;
            ResumeLoops();
        }
        private void Start() => ResumeLoops();
        private void ResumeLoops()
        {
            if (ambience != null && ambience.clip != null && !ambience.isPlaying) ambience.Play();
            SyncTexture();
        }
        private void LateUpdate() => audibleSwitches = true;
        private void Update()
        {
            if (Mathf.Abs(body.linearVelocity.x) < .1f || Time.time < nextStep) return;
            nextStep = Time.time + .55f;
            One(footsteps, stepClip);
        }
        private void OnArea(FirstNightArea area)
        {
            if (enteredOnce) One(door, doorClip);
            enteredOnce = true;
            if (area == FirstNightArea.GuestRoom && !movementPlayed)
            {
                movementPlayed = true; StartCoroutine(OrdinaryMovement());
            }
        }
        private IEnumerator OrdinaryMovement()
        {
            One(outsideMovement, clothClip);
            yield return new WaitForSeconds(.65f);
            One(outsideMovement, stepClip);
        }
        private void OnRetrieve() => One(handling, retrieveClip);
        private void OnPower(bool on)
        {
            SyncTexture();
            if (!on && voice != null) voice.Stop();
            if (audibleSwitches && controller.HasReceiver) One(switches, on ? onClip : offClip);
        }
        private void SyncTexture()
        {
            if (texture == null || radio == null || controller == null) return;
            if (radio.IsOn && controller.HasReceiver && texture.clip != null)
            { if (!texture.isPlaying) texture.Play(); }
            else texture.Stop();
        }
        private void OnReading(EvidenceDefinition document)
        {
            if (!radio.IsOn || !controller.HasReceiver || !programDocuments.Contains(document)) return;
            // Only an English synthetic candidate was delivered. RU stays text-first, never forced EN speech.
            if (LocalizationSettings.SelectedLocale?.Identifier.Code == "en" && voice.clip != null)
            { voice.Stop(); voice.Play(); }
        }
        private void OnClosed(EvidenceDefinition document, bool completed)
        {
            if (!completed && programDocuments.Contains(document)) voice.Stop();
        }
        private void OnLocale(Locale locale)
        {
            if (locale.Identifier.Code != "en") voice.Stop();
        }
        private static void One(AudioSource source, AudioClip clip)
        {
            if (clip == null || source == null) return;
            source.Stop(); source.PlayOneShot(clip);
        }
        private void OnDisable()
        {
            controller.AreaEntered -= OnArea; controller.ReceiverRetrieved -= OnRetrieve;
            radio.StateChanged -= OnPower; reader.Opened -= OnReading; reader.Closed -= OnClosed;
            LocalizationSettings.SelectedLocaleChanged -= OnLocale; StopAllCoroutines();
            foreach (var source in new[] { ambience, texture, voice, switches, door, handling, outsideMovement, footsteps })
                if (source != null) source.Stop();
        }
    }
}
