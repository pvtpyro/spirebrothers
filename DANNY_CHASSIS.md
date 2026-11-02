# Danny: pick your Titan (Atlas, Stryder, Ogre)

Plan, 2026-11-01. Nothing built yet. This is the first trial of the select-screen picker from `someday.md`, but
the choice is a Titan chassis, not a subclass. Card plan for Danny himself: `DANNY_CARDS.md`.

## The idea

On the character select screen there's one Danny button. Clicking it opens a picker with three Titans
(Titanfall 1's chassis). The Pilot side of his cards is the same for all three. The Titan he calls in is different:
its HP, armor, how fast the meter fills, a passive, its **Core**, and a handful of cards only that chassis gets.

This fits Titanfall 1: every chassis can carry every weapon, and what sets them apart is the body and the Core. So
the existing 27 cards (weapons and abilities) stay shared, and the chassis adds a layer on top.

## The three Titans

| | **Atlas** (all-rounder) | **Stryder** (fast, fragile) | **Ogre** (heavy tank) |
|---|---|---|---|
| Meter size | 40 | 30 (shows up sooner) | 50 (takes longer) |
| Hull | 30 | 20 | 45 |
| Titan Armor (hits this or less do nothing) | 5 | 3 | 7 |
| Passive | **Targeting Link:** Titan-side attacks deal +2 damage | **Thrusters:** in the Titan, draw 1 more card each turn | **Bulwark:** while the Ogre stands, teammates take 1 less damage from each hit |
| Core (TF1) | **Damage Core** | **Dash Core** | **Shield Core** |
| Core effect (first idea) | This turn, Titan-side attacks deal 50% more | Gain 2 Energy, draw 3 | The Titan gains a 20 HP shield until your next turn |
| Plays like | Today's Danny | Get in early and often, use it fast, eject | Rarely in the Titan, but once he is, he holds the line for the team |

Ogre's passive is the co-op hook: the big Titan protects everyone, not just Danny. Starting numbers only, to tune
after playtesting (nobody weak, nobody steamrolls).

**The Core:** when the Titan drops in, he gets his chassis's Core card (0 cost, Exhaust, Retain), a second token like
Stand By for Titanfall. One per Titanfall.

## Chassis cards

About 6 cards per chassis to start (2 Common, 3 Uncommon, 1 Rare), each with a Pilot side and a Titan side like the
rest of his cards. Only that chassis sees them in rewards, shops, and random card effects. Card ideas, to fill in
together:

- **Atlas:** damage and Vortex-style tricks (Amped Wall's Vortex Shield fits here as the flavor anchor).
- **Stryder:** draw, cost cuts, dodging (a "Dash" that makes the next hit on the Titan miss), cheap Eject and
  quick re-Titanfall.
- **Ogre:** Repair, shields, protecting teammates (Share cards that send the Titan's armor or a shield to a teammate).

The current Titan-side Cores (Flight Core, Sword Core, Laser Core, Smart Core, Upgrade Core) are Titanfall 2 Titans'
Cores. They can stay as generic weapons, or move into one chassis each if Daniel wants the chassis to feel more different.

## How it's built

### 1. The chassis characters

- **Danny stays the Atlas.** Saved runs with Danny keep working, and the grid keeps one button for him.
- `DannyStryder` and `DannyOgre` **extend `Danny`**, so every `is Character.Danny` check (`Titan.IsDanny`,
  `DannyTracker`, `DannyTitan`) keeps working with no changes.
- Each has its own character entry (name, select description, Architect dialogue in `ancients.json`, which the
  analyzer requires), and shares Danny's art folder, pools, starting deck, and Neural Link.
- `Danny` gets a virtual `Chassis` record: `MeterSize`, `Hull`, `Armor`, `CoreCard`. `Titan.MeterSize`, `BaseHull`,
  and `ArmorThreshold` turn from constants into `Titan.MeterSize(player)` etc. Callers to update:
  `PrepareForTitanfall`, `TitanMeterDisplay`, `TitanHullDisplay`, `DannyTitan` (initial HP), `Titan.MaxHull`.

### 2. Chassis cards: one pool, filtered

- All Danny cards stay in `DannyCardPool`. Chassis cards implement a new `IChassisCard` (`Chassis` = the character
  type, like `IDuoCard.Partner`).
- A filter like `DuoCards` hides them from the wrong chassis: card rewards (`ModifyCardRewardCreationOptions`), shops
  (`ModifyMerchantCardPool`), and in-combat generation (the same `CardFactory` patch `DuoCardsCombatPatch` uses).
  Simplest is to add the chassis check to `DuoCards.Allowed`, so every place that already calls it is covered.
- The compendium shows all of them; that's fine (it's a library).
- Alternative considered: three separate pools with the shared cards added to each (`ModHelper.AddModelToPool`, like
  Monkey Island relics). No filter needed, but every shared card would show up three times in the compendium.

### 3. The picker

- Decompile the character select screen first (CLAUDE.md, tooling notes) to find where the buttons are built and the
  method that selects a character. That's the one unknown.
- Hide the `DannyStryder` / `DannyOgre` buttons from the grid. Clicking Danny's button opens a small window with three
  Titan cards (picture, name, the stats row, one line of flavor). Picking one calls the game's own select with that
  character, so the portrait, description, co-op lobby sync, and Embark are all vanilla.
- Keep the random character button from picking a hidden chassis directly (or allow it; decide when testing).
- Click, not hover: works with a controller. Hover can show the three icons as a preview later.

### 4. Art

- `TitanArt` (runtime pixel art in `Monsters/DannyTitan.cs`) draws one Titan. Give it three silhouettes: Atlas as
  today, Stryder thin with long legs, Ogre wide and boxy. Drop-in PNGs can come later.
- Picker cards: one Titan portrait each (could reuse the same art, scaled up).

## Build order

1. **Chassis stats** in `Titan` + `DannyStryder` / `DannyOgre` as plain characters on the grid (no picker yet).
   That way the three play differently and can be playtested right away.
2. **Passives and Cores** (three powers, three Core tokens).
3. **Picker** on the select screen, hiding the two extra buttons.
4. **Chassis cards** (6 each) and the filter.
5. **Titan art** per chassis.

Steps 1-2 are the quickest way to find out whether Daniel likes the idea before building the picker.

## Open questions

- Do the chassis names stay Titanfall's (Atlas, Stryder, Ogre), or get Danny-flavored names?
- Should each chassis also get its own starter relic (Neural Link has no effect yet), or keep one shared relic?
- Do the TF2 Cores stay generic, or get split between the chassis?
- Ogre's team protection: flat 1 less per hit, or something bigger and rarer (e.g., the Titan takes one hit meant for
  a teammate each turn)?

## Playtest checklist

- The picker opens from Danny's button, each choice shows its own name and description, and Embark starts that chassis.
- Co-op lobby: the other players see the chassis you picked.
- Meter size, hull, and armor match the table for each chassis (meter bar and TITAN bar labels too).
- The Core shows up in hand on each Titanfall and works.
- Chassis cards only appear for their own chassis (rewards, shop, Discovery-type effects).
- A saved Danny run from before this change loads as Atlas.
