# MK Toasty

A Mortal Kombat-style **"Toasty!"** easter egg for Unity.  
A silhouette slides in from the screen edge accompanied by a sound effect.

![Unity](https://img.shields.io/badge/Unity-2021.3%2B-black?logo=unity)
![Input System](https://img.shields.io/badge/Input%20System-1.7%2B-blue)
![License](https://img.shields.io/badge/license-MIT-green)

---

## Requirements

- Unity 2021.3+
- [Input System](https://docs.unity3d.com/Packages/com.unity.inputsystem@latest) 1.7+

---

## Installation

Open **Window → Package Manager**, click **+** → **Add package from git URL** and enter:

```
https://github.com/kirillrybin/com.kiriyo.mktoasty.git
```

To lock a specific version use a tag:

```
https://github.com/kirillrybin/com.kiriyo.mktoasty.git#v1.0.0
```

---

## Setup

### 1. Create the config asset

**Assets → Create → MkToasty → Config**

Assign your assets in the Inspector:

| Field | Description |
|---|---|
| Toasty Sprite | Silhouette sprite with transparent background |
| Toasty Sfx | AudioClip with the "Toasty!" voice line |
| Corner | Screen corner where the silhouette appears |
| Trigger Key | Key that triggers the effect (default: `T`) |
| Require Ctrl / Shift | Modifier keys required alongside the trigger key |
| Timer Min / Max Interval | Random interval range in seconds for the auto-trigger |

### 2. Add the prefab to your scene

Drag **Packages/com.kiriyo.mktoasty/Prefabs/MkToasty.prefab** into your scene.  
Assign your `MkToastyConfig` asset to both `MkToastyPresenter` and `MkToastyTriggerController`.

---

## Triggering from code

```csharp
[SerializeField]
private MkToastyTriggerController _mkToasty;

// Show on a kill streak, achievement, or any game event:
_mkToasty.TriggerFromCode();
```

You can also drive the presenter directly if you don't need the trigger controller:

```csharp
[SerializeField]
private MkToastyPresenter _mkToasty;

_mkToasty.Show();
_mkToasty.Hide();
```

---

## Events

`MkToastyPresenter` exposes two events:

```csharp
presenter.OnShown  += () => Debug.Log("Toasty appeared!");
presenter.OnHidden += () => Debug.Log("Toasty gone!");
```

---

## License

MIT
