# Spire Brothers: art assets

Everything is a PNG with a transparent background, saved under `SpireBrothers/images/`. Anything without art yet
keeps using a placeholder, so art can be added a piece at a time. Run a build after adding files.

## Where to start

1. **Combat body**: the character standing in fights. It's on screen the whole run and makes the biggest difference.
2. **Character select portrait** and **character icon**, so he's recognizable in menus.
3. **Card art** last. It's the biggest job (about 76 cards per brother); do a few at a time.

Good first step: draw one brother's combat body (about 350 × 450, transparent background), save it in the project,
and ask Claude to hook it up so you can see it in a fight before deciding on a style for everything else.

## What each character needs

| Asset | Size (px) | Where it shows | Notes |
|---|---|---|---|
| **Combat body** | about 350 × 450, feet at the bottom | In every fight | A single still image, or animation frames (see below). |
| **Character select portrait** | 132 × 195, plus a locked version | Character select buttons | The locked one is usually a darkened silhouette. |
| **Character icon** | 128 × 128, plus an outline | Top-left during a run, and co-op map votes and player list | Until it exists, the map marker is used. The outline is the same shape in flat white, a few pixels bigger (generated from the map markers for now). |
| **Map marker** | 128 × 128 | His token on the map | |
| **Energy orb** | 74 × 74, plus 24 × 24 | The energy counter, and energy icons in card text | Right now all four brothers share one. |
| **Card art** | 1000 × 760, plus a 250 × 190 copy | The picture area of each card | The game adds the frame, so paint only the picture. Paint big and shrink it down. |
| **Relic icons** | 94 × 94, a 94 × 94 outline, and 256 × 256 | Relic bar and tooltips | The outline is the same shape filled in one flat color. |
| **Power icons** | 64 × 64, plus 256 × 256 | Buff and debuff icons over the character | |

Optional extras for later: a character select background scene, and rest site and shop poses.

### Brother-specific pieces

- **Tim:** a Kid sprite (about 56 × 88, standing; there are up to 8 on screen) and Age graphics. The Ages work as soon
  as you save them as `images/ages/age0.png` through `age3.png` (Dark, Feudal, Castle, Imperial). No code change needed.
- **Joshua:** a Verse note (about 48 × 48) for the orbs that float above him.

## How files get named

Card, relic and power images are named after the item's ID: lowercase, with underscores between words.

| Kind | Small image | Other images |
|---|---|---|
| Card | `card_portraits/palisade_wall.png` | `card_portraits/big/palisade_wall.png` |
| Relic | `relics/family_minivan.png` | `relics/family_minivan_outline.png`, `relics/big/family_minivan.png` |
| Power | `powers/proud_dad_power.png` | `powers/big/proud_dad_power.png` |

The ID comes from the class name, so "Palisade Wall" is `palisade_wall` and "Family Minivan" is `family_minivan`. The
card lists in `DANIEL_CARDS.md`, `DAVID_CARDS.md`, `JOSHUA_CARDS.md` and `TIM_CARDS.md` have every name.

Cards, relics and powers pick up their art automatically once the file is in the right place.

## Character art: where to save it

Each brother has his own folder. Drop a file in with the exact name below and run a build; it's used automatically.
Anything missing keeps the borrowed vanilla look (Daniel = Defect, David = Silent, Joshua = Regent, Tim = Ironclad).

```
SpireBrothers/images/characters/
  daniel/   david/   joshua/   tim/
```

| File | Size (px) | What it is |
|---|---|---|
| `body.png` | any size; trim so the feet touch the bottom edge | Combat body (a single still image). The game scales it to a standard height (400 units), so export it large to keep it sharp. |
| `select.png` | 132 × 195 | Character select button |
| `select_locked.png` | 132 × 195 | Same button while locked (usually a dark silhouette) |
| `map_marker.png` | 128 × 128 | His token on the map |
| `icon.png` | 128 × 128 | Character icon (top-left in a run, co-op map votes and player list). Optional: the map marker is used until it exists. |
| `icon_outline.png` | 128 × 128 | Flat white silhouette of the icon, a few pixels bigger. Shows behind it on co-op map votes. |

Example: Tim's combat body goes at `SpireBrothers/images/characters/tim/body.png`.

### Animated bodies (`frames/`)

All four brothers currently use generated pixel-art frames in `images/characters/<name>/frames/`. When a `frames/`
folder exists it wins over `body.png`. Daniel's painted body is kept, unused, as `daniel/daniel-old.png` (rename it back
to `body.png` and delete `frames/` to use it again).

Frames are named `<animation>_<number>.png`, numbered from 0, all the same size with the feet on the bottom edge:

| Animation | Frames now | Speed | When the game plays it |
|---|---|---|---|
| `idle` | 8 | 6 per second, loops | All the time (required) |
| `attack` | 6 | 14 per second | Playing an Attack |
| `cast` | 6 | 10 per second | Playing a Skill or Power |
| `hit` | 4 | 12 per second | Taking damage |
| `dead` | 5 | 8 per second | Dying (stays on the last frame). The code also tips the whole body over backwards onto the ground, so draw these standing up. |

Any number of frames works; add or remove files and rebuild. The current frames are 64 × 80 pixels, shown at 5×
with no smoothing, so you can repaint any frame in a pixel editor (Aseprite, Piskel, Photoshop with nearest-neighbor)
at the same size. Speeds live in `Anims` in `SpireBrothersCode/Character/BrotherCharacter.cs`.

Untouched originals (before background removal or trimming) are kept in `art_originals/`. That folder has a
`.gdignore` file, so the game never packs it.

The **character icon** (top-left during a run) and **energy orb** aren't wired per brother yet; ask Claude to connect
them when they're ready. (The energy orb is currently one shared file in `images/charui/`.)
