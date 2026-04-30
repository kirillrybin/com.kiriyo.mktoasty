using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace MkToasty
{
    /// <summary>
    /// Handles the visual presentation of the MK toasty:
    /// spawning the Canvas/Image, animating slide-in/out, playing audio.
    /// Call <see cref="Show"/> from <see cref="MkToastyTriggerController"/>
    /// or directly from any game code.
    /// </summary>
    public class MkToastyPresenter : MonoBehaviour
    {
        [SerializeField]
        private MkToastyConfig _config;

        private Canvas _canvas;
        private RectTransform _imageRect;
        private Image _image;
        private AudioSource _audioSource;
        private Coroutine _activeRoutine;

        public bool IsVisible { get; private set; }

        public event Action OnShown;

        public event Action OnHidden;

        private void Awake()
        {
            BuildUI();
            BuildAudio();
        }

        /// <summary>
        /// Trigger the toasty appearance.
        /// Safe to call while already visible — resets the visible timer.
        /// </summary>
        public void Show()
        {
            if (_activeRoutine != null)
                StopCoroutine(_activeRoutine);

            _activeRoutine = StartCoroutine(ShowRoutine());
        }

        /// <summary>
        /// Immediately hide the toasty (animated).
        /// </summary>
        public void Hide()
        {
            if (_activeRoutine != null)
                StopCoroutine(_activeRoutine);

            _activeRoutine = StartCoroutine(SlideOutRoutine());
        }

        private void BuildUI()
        {
            // Create a dedicated full-screen overlay Canvas so the toasty
            // always renders on top regardless of scene Canvas sort order.
            var canvasGo = new GameObject("MkToasty_Canvas");
            canvasGo.transform.SetParent(transform, false);

            _canvas = canvasGo.AddComponent<Canvas>();
            _canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            _canvas.sortingOrder = 9999;
            // CanvasScaler intentionally omitted — ScreenSpaceOverlay stretches
            // to fill the screen automatically; adding CanvasScaler with default
            // settings would shrink the canvas to 100x100.
            canvasGo.AddComponent<GraphicRaycaster>();

            // Image object
            var imageGo = new GameObject("MkToasty_Image");
            imageGo.transform.SetParent(_canvas.transform, false);

            _image = imageGo.AddComponent<Image>();
            _image.sprite = _config.ToastySprite;
            _image.preserveAspect = true;
            _image.color = Color.white;

            _imageRect = _image.rectTransform;
            _imageRect.sizeDelta = _config.ToastySize;

            // Position anchor to the configured corner
            ApplyCornerAnchors(_imageRect, _config.Corner, _config.EdgeOffset, _config.ToastySize);

            // Start off-screen
            _imageRect.anchoredPosition = OffScreenPosition();
            _image.gameObject.SetActive(false);
        }

        private static void ApplyCornerAnchors(
            RectTransform rt,
            ScreenCorner corner,
            Vector2 offset,
            Vector2 size)
        {
            Vector2 anchor;
            Vector2 pivot;
            Vector2 anchoredPos;

            switch (corner)
            {
                case ScreenCorner.BottomRight:
                    anchor = new Vector2(1f, 0f);
                    pivot = new Vector2(1f, 0f);
                    anchoredPos = new Vector2(-offset.x, offset.y);
                    break;
                case ScreenCorner.BottomLeft:
                    anchor = new Vector2(0f, 0f);
                    pivot = new Vector2(0f, 0f);
                    anchoredPos = new Vector2(offset.x, offset.y);
                    break;
                case ScreenCorner.TopRight:
                    anchor = new Vector2(1f, 1f);
                    pivot = new Vector2(1f, 1f);
                    anchoredPos = new Vector2(-offset.x, -offset.y);
                    break;
                case ScreenCorner.TopLeft:
                default:
                    anchor = new Vector2(0f, 1f);
                    pivot = new Vector2(0f, 1f);
                    anchoredPos = new Vector2(offset.x, -offset.y);
                    break;
            }

            rt.anchorMin = anchor;
            rt.anchorMax = anchor;
            rt.pivot = pivot;
            // Store the visible position; will be used during animation
            rt.anchoredPosition = anchoredPos;
        }

        private void BuildAudio()
        {
            _audioSource = gameObject.AddComponent<AudioSource>();
            _audioSource.playOnAwake = false;
            _audioSource.clip = _config.ToastySfx;
            _audioSource.volume = _config.SfxVolume;
            _audioSource.spatialBlend = 0f; // 2D sound
        }

        private IEnumerator ShowRoutine()
        {
            IsVisible = true;
            _image.gameObject.SetActive(true);

            // Play audio immediately on appearance, not after slide-in.
            PlaySfx();

            yield return SlideInRoutine();

            OnShown?.Invoke();

            yield return new WaitForSeconds(_config.VisibleDuration);

            yield return SlideOutRoutine();
        }

        private IEnumerator SlideInRoutine()
        {
            Vector2 hiddenPos = OffScreenPosition();
            Vector2 visiblePos = OnScreenPosition();

            float elapsed = 0f;
            float duration = _config.SlideInDuration;

            while (elapsed < duration)
            {
                elapsed += Time.unscaledDeltaTime;
                float t = Mathf.SmoothStep(0f, 1f, elapsed / duration);
                _imageRect.anchoredPosition = Vector2.LerpUnclamped(hiddenPos, visiblePos, t);
                yield return null;
            }

            _imageRect.anchoredPosition = visiblePos;
        }

        private IEnumerator SlideOutRoutine()
        {
            Vector2 visiblePos = OnScreenPosition();
            Vector2 hiddenPos = OffScreenPosition();

            float elapsed = 0f;
            float duration = _config.SlideOutDuration;

            while (elapsed < duration)
            {
                elapsed += Time.unscaledDeltaTime;
                float t = Mathf.SmoothStep(0f, 1f, elapsed / duration);
                _imageRect.anchoredPosition = Vector2.LerpUnclamped(visiblePos, hiddenPos, t);
                yield return null;
            }

            _imageRect.anchoredPosition = hiddenPos;
            _image.gameObject.SetActive(false);
            IsVisible = false;
            _activeRoutine = null;
            OnHidden?.Invoke();
        }

        /// <summary>Returns the fully off-screen position for the current corner.
        /// Right corners slide in from the right, left corners from the left.
        /// anchoredPosition is relative to the corner anchor.</summary>
        private Vector2 OffScreenPosition()
        {
            Vector2 size = _config.ToastySize;
            Vector2 offset = _config.EdgeOffset;

            return _config.Corner switch
            {
                // Anchor bottom-right: visible X is -offset.x, hidden: shift right by full width
                ScreenCorner.BottomRight => new Vector2(size.x + offset.x, offset.y),
                // Anchor bottom-left: visible X is +offset.x, hidden: shift left by full width
                ScreenCorner.BottomLeft => new Vector2(-(size.x + offset.x), offset.y),
                // Anchor top-right: visible X is -offset.x, hidden: shift right by full width
                ScreenCorner.TopRight => new Vector2(size.x + offset.x, -offset.y),
                // Anchor top-left: visible X is +offset.x, hidden: shift left by full width
                ScreenCorner.TopLeft => new Vector2(-(size.x + offset.x), -offset.y),
                _ => Vector2.zero
            };
        }

        /// <summary>Returns the fully on-screen (visible) position for the current corner.</summary>
        private Vector2 OnScreenPosition()
        {
            Vector2 offset = _config.EdgeOffset;

            return _config.Corner switch
            {
                ScreenCorner.BottomRight => new Vector2(-offset.x, offset.y),
                ScreenCorner.BottomLeft => new Vector2(offset.x, offset.y),
                ScreenCorner.TopRight => new Vector2(-offset.x, -offset.y),
                ScreenCorner.TopLeft => new Vector2(offset.x, -offset.y),
                _ => Vector2.zero
            };
        }

        private void PlaySfx()
        {
            if (_config.ToastySfx == null)
            {
                Debug.LogWarning("[MkToasty] No audio clip assigned in _config. " +
                                 "Assign ToastySfx in MkToastyConfig.");
                return;
            }

            _audioSource.Play();
        }
    }
}