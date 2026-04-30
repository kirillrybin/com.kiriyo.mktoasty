using UnityEngine;
using UnityEngine.InputSystem;

namespace MkToasty
{
    [CreateAssetMenu(fileName = "MkToastyConfig", menuName = "MkToasty/Config")]
    public class MkToastyConfig : ScriptableObject
    {
        [Header("Visuals")]
        [Tooltip("Toasty sprite. Replace with actual MK silhouette asset.")]
        [SerializeField]
        private Sprite _toastySprite;

        [Tooltip("Which corner of the screen to appear in.")]
        [SerializeField]
        private ScreenCorner _corner = ScreenCorner.BottomRight;

        [Tooltip("Offset from the screen edge in pixels.")]
        [SerializeField]
        private Vector2 _edgeOffset = new Vector2(20f, 20f);

        [Tooltip("Size of the toasty image in pixels.")]
        [SerializeField]
        private Vector2 _toastySize = new Vector2(120f, 200f);

        [Header("Animation")]
        [Tooltip("Duration of the slide-in animation in seconds.")]
        [SerializeField]
        private float _slideInDuration = 0.3f;

        [Tooltip("Duration of the slide-out animation in seconds.")]
        [SerializeField]
        private float _slideOutDuration = 0.3f;

        [Tooltip("How long the toasty stays visible before hiding (seconds).")]
        [SerializeField]
        private float _visibleDuration = 3f;

        [Header("Audio")]
        [Tooltip("'Toasty!' sound clip. Replace with actual MK audio asset.")]
        [SerializeField]
        private AudioClip _toastySfx;

        [Tooltip("Volume of the sound effect.")]
        [SerializeField]
        [Range(0f, 1f)]
        private float _sfxVolume = 1f;

        [Header("Keyboard Trigger")]
        [Tooltip("Enable keyboard shortcut trigger.")]
        [SerializeField]
        private bool _enableKeyboardTrigger = true;

        [Tooltip("Key to trigger the toasty.")]
        [SerializeField]
        private Key _triggerKey = Key.T;

        [Tooltip("Require Ctrl to be held along with the trigger key.")]
        [SerializeField]
        private bool _requireCtrl = true;

        [Tooltip("Require Shift to be held along with the trigger key.")]
        [SerializeField]
        private bool _requireShift = false;

        [Header("Timer Trigger")]
        [Tooltip("Enable random timer trigger.")]
        [SerializeField]
        private bool _enableTimerTrigger = true;

        [Tooltip("Minimum seconds between random appearances.")]
        [SerializeField]
        private float _timerMinInterval = 30f;

        [Tooltip("Maximum seconds between random appearances.")]
        [SerializeField]
        private float _timerMaxInterval = 120f;

        public Sprite ToastySprite => _toastySprite;
        public ScreenCorner Corner => _corner;
        public Vector2 EdgeOffset => _edgeOffset;
        public Vector2 ToastySize => _toastySize;

        public float SlideInDuration => _slideInDuration;
        public float SlideOutDuration => _slideOutDuration;
        public float VisibleDuration => _visibleDuration;

        public AudioClip ToastySfx => _toastySfx;
        public float SfxVolume => _sfxVolume;

        public bool EnableKeyboardTrigger => _enableKeyboardTrigger;
        public Key TriggerKey => _triggerKey;
        public bool RequireCtrl => _requireCtrl;
        public bool RequireShift => _requireShift;

        public bool EnableTimerTrigger => _enableTimerTrigger;
        public float TimerMinInterval => _timerMinInterval;
        public float TimerMaxInterval => _timerMaxInterval;
    }

    public enum ScreenCorner
    {
        BottomRight,
        BottomLeft,
        TopRight,
        TopLeft
    }
}