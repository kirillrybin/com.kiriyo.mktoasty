using UnityEngine;
using UnityEngine.InputSystem;

namespace MkToasty
{
    /// <summary>
    /// Orchestrates all trigger sources:
    ///   1. Keyboard shortcut (configurable in <see cref="MkToastyConfig"/>)
    ///   2. Random timer
    ///   3. External code call via <see cref="TriggerFromCode"/>
    ///
    /// Requires <see cref="MkToastyPresenter"/> on the same or child GameObject,
    /// or assign it via the Inspector / Zenject injection.
    /// </summary>
    public class MkToastyTriggerController : MonoBehaviour
    {
        [SerializeField]
        private MkToastyConfig _config;

        [SerializeField]
        private MkToastyPresenter _presenter;

        private float _timerCountdown;

        private void Awake()
        {
            if (_presenter == null)
                _presenter = GetComponentInChildren<MkToastyPresenter>();

            if (_presenter == null)
                Debug.LogError("[MkToasty] MkToastyPresenter not found. " +
                               "Assign it in the Inspector or attach to the same GameObject.");

            ResetTimer();
        }

        private void Update()
        {
            HandleKeyboardTrigger();
            HandleTimerTrigger();
        }

        /// <summary>
        /// Call this from any game system to show the toasty on demand.
        /// Example: special kill streak, achievement unlock, secret input, etc.
        /// </summary>
        public void TriggerFromCode()
        {
            Trigger();
        }

        private void HandleKeyboardTrigger()
        {
            if (!_config.EnableKeyboardTrigger)
                return;

            var keyboard = Keyboard.current;
            if (keyboard == null)
                return;

            bool modifiersOk =
                (!_config.RequireCtrl || keyboard.ctrlKey.isPressed) &&
                (!_config.RequireShift || keyboard.shiftKey.isPressed);

            if (modifiersOk && keyboard[_config.TriggerKey].wasPressedThisFrame)
                Trigger();
        }

        private void HandleTimerTrigger()
        {
            if (!_config.EnableTimerTrigger)
                return;

            _timerCountdown -= Time.unscaledDeltaTime;

            if (_timerCountdown <= 0f)
            {
                ResetTimer();
                Trigger();
            }
        }

        private void ResetTimer()
        {
            _timerCountdown = Random.Range(_config.TimerMinInterval, _config.TimerMaxInterval);
        }

        private void Trigger()
        {
            if (_presenter == null)
                return;

            _presenter.Show();
        }
    }
}