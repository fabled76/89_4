using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;

namespace DeadAir
{
    // One short-lived handoff object, not a persistent game manager or save service.
    public sealed class PrologueJourney : MonoBehaviour
    {
        [SerializeField] private TMP_Text heading;
        [SerializeField] private TMP_Text body;
        [SerializeField] private AudioSource travelCue;
        [SerializeField] private AudioSource tuningCue;
        private PrologueDefinition definition;
        private PlayerController2D movement;
        private readonly List<(Behaviour component, bool enabled)> targetControls = new List<(Behaviour, bool)>();
        public bool Arrived { get; private set; }
        private void OnEnable() => LocalizationSettings.SelectedLocaleChanged += OnLocale;
        private void OnDisable() => LocalizationSettings.SelectedLocaleChanged -= OnLocale;
        private void OnLocale(Locale _) => StartCoroutine(RefreshLocale());
        private IEnumerator RefreshLocale()
        {
            yield return LocalizationSettings.InitializationOperation;
            RefreshArrival();
        }
        private void RefreshArrival()
        {
            if (!Arrived || definition == null) return;
            heading.text = definition.Text("prologue.arrival.check_in");
            body.text = definition.Text("prologue.arrival.line") + "\n\n" + definition.Text("prologue.arrival.tuning_caption")
                + "\n\n" + definition.Text("prologue.arrival.prototype_bridge");
        }
        public static bool ControlsReleased()
        {
            var keyboard = Keyboard.current;
            return keyboard == null || !(keyboard.eKey.isPressed || keyboard.leftShiftKey.isPressed
                || keyboard.spaceKey.isPressed || keyboard.enterKey.isPressed || keyboard.tabKey.isPressed
                || keyboard.escapeKey.isPressed || keyboard.aKey.isPressed || keyboard.dKey.isPressed
                || keyboard.f1Key.isPressed || keyboard.digit1Key.isPressed || keyboard.digit2Key.isPressed);
        }
        public void Begin(PrologueDefinition content)
        {
            definition = content; DontDestroyOnLoad(gameObject);
            StartCoroutine(Travel());
        }
        private IEnumerator Travel()
        {
            heading.text = definition.Text("prologue.transition.later"); body.text = string.Empty;
            if (travelCue.clip != null) travelCue.Play();
            yield return new WaitForSecondsRealtime(definition.TravelSeconds);
            SceneManager.sceneLoaded += OnSceneLoaded;
            yield return SceneManager.LoadSceneAsync(definition.TargetScenePath, LoadSceneMode.Single);
            SceneManager.sceneLoaded -= OnSceneLoaded;
            // All Awake/Start calls initialize the target's own authored battery and radio state.
            yield return null;
            var radio = movement.GetComponentInChildren<RadioController>();
            if (definition.PlayableCheckInArrival)
            {
                Arrived = true;
                radio.SetPower(false); travelCue.Stop(); tuningCue.Stop();
                yield return new WaitUntil(ControlsReleased);
                RestoreTargetControls();
                Destroy(gameObject);
                yield break;
            }
            radio.SetPower(true);
            travelCue.Stop();
            if (tuningCue.clip != null) tuningCue.Play();
            Arrived = true;
            RefreshArrival();
            yield return new WaitUntil(ControlsReleased);
            // Requiring a fresh press prevents the departure key from dismissing the bridge.
            yield return new WaitUntil(() => Keyboard.current != null &&
                (Keyboard.current.spaceKey.wasPressedThisFrame || Keyboard.current.enterKey.wasPressedThisFrame));
            yield return new WaitUntil(ControlsReleased);
            RestoreTargetControls();
            // Let the short audio tail finish without keeping the player in the overlay.
            GetComponent<Canvas>().enabled = false;
            if (tuningCue.isPlaying && tuningCue.clip != null)
                yield return new WaitForSecondsRealtime(Mathf.Max(0f, tuningCue.clip.length - tuningCue.time));
            Destroy(gameObject);
        }
        private void RestoreTargetControls()
        {
            foreach (var control in targetControls)
                if (control.component != null) control.component.enabled = control.enabled;
        }
        private void OnSceneLoaded(Scene scene, LoadSceneMode _)
        {
            foreach (var root in scene.GetRootGameObjects())
            {
                var candidate = root.GetComponentInChildren<PlayerController2D>();
                if (candidate == null) continue;
                movement = candidate;
                break;
            }
            foreach (var root in scene.GetRootGameObjects())
            foreach (var component in root.GetComponentsInChildren<MonoBehaviour>(true))
            {
                if (!(component is PlayerController2D || component is InteractionController || component is RadioInput
                    || component is InvestigationUI || component is LanguageSettingsPanel)) continue;
                targetControls.Add((component, component.enabled)); component.enabled = false;
            }
        }
        private void OnDestroy() => SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}
