# Spire Brothers: project notes for Claude

## What this is

A Slay the Spire 2 character mod. It adds playable characters based on three real brothers who are
friends of the user and play StS2 together, mostly in multiplayer co-op. The mod is a gift for them,
so the characters should feel personal, warm, and fun. Co-op support matters a lot.

- **Daniel, The Nerd Who Nerds Wrong**: built (v0.1.0), currently being debugged in game.
- **David, The Min-Maxer**: designed, not built.
- **Joshua, The Musician**: designed, not built.

Full designs for all three are in `DESIGN.md`. Build Daniel fully working before starting the others.

## The user

Has a BS in CIS (web programming) and an AA in graphic design, so she's comfortable with code
concepts but new to C#, .NET, Godot, and StS2 modding. Walk her through tooling steps explicitly.
She will likely make the card and relic art herself.

## Environment (user's machine, Windows)

- Project: `D:\projects\csharp\SpireBrothers`
- Game: `C:\Program Files (x86)\Steam\steamapps\common\Slay the Spire 2`, version **v0.107.1**
- Engine for .pck export: Godot/MegaDot **4.5.1 mono**, path set in `Directory.Build.props` (`GodotPath`)
- BaseLib **3.4.7** installed via Steam Workshop (id 3737335127)
- Build and install: `dotnet publish -c Release` (copies dll/pck/json to `...\Slay the Spire 2\mods\SpireBrothers\`)
- Game log: `%appdata%\SlayTheSpire2\logs\godot.log`
- She also runs ~13 other Workshop mods, including Downfall, RitsuLib, More Enchantments,
  LoadOrderManager, BetterSpire2 Lite, and Friend Trading. Watch for conflicts.

## Project layout

Started from Alchyr's BaseLib character mod template (https://github.com/Alchyr/ModTemplate-StS2).
Its wiki is the best reference: https://github.com/Alchyr/ModTemplate-StS2/wiki

- `SpireBrothersCode/Character/`: `Daniel.cs` (uses `PlaceholderCharacterModel` with the Defect's
  visuals) and `DanielPools.cs` (card/relic/potion pools)
- `SpireBrothersCode/Cards/DanielCard.cs`: base card class with art paths plus helpers for
  Wired glow, Stratagem playability, and `ShareTarget()`
- `SpireBrothersCode/Cards/Daniel/`: one file per card (24 cards)
- `SpireBrothersCode/Mechanics/TurnTracker.cs`: a `CustomSingletonModel` that records the ordered
  Logic/Hands keywords played each turn (per `PlayerCombatState` via `SpireField`), whether last turn
  mixed both, whether Wired fired this turn, and the Gear count
- `SpireBrothersCode/Mechanics/Wired.cs`: `IsActive(card)` and `Check(card)` (also triggers the
  Trusty Multimeter relic)
- `SpireBrothersCode/Powers/`, `SpireBrothersCode/Relics/`
- `SpireBrothersCode/BrotherKeywords.cs`: custom `CardKeyword`s. Logic/Hands/Stratagem auto-insert
  before the card text. Wired/Share/Diligent/Comeback use `AutoKeywordPosition.None` and are
  written into the card text by hand.
- `SpireBrothers/localization/eng/*.json`: all text

## Conventions and gotchas

- IDs are `SPIREBROTHERS-` + class name in UPPER_SNAKE (`RubberDuckDebugging` becomes
  `SPIREBROTHERS-RUBBER_DUCK_DEBUGGING`). Image files use the lowercase snake name.
- Dynamic var names in loc text: `Damage`, `Block`, `Cards`, `Energy`, `Heal`, `Repeat`, `Scry`
  (BaseLib `ScryVar`), `CalculatedDamage`/`CalculationBase`/`ExtraDamage`, `CalculatedBlock`/
  `CalculationExtra`, and `PowerVar<T>` uses the power's class name (`VulnerablePower`).
- The `Alchyr.Sts2.ModAnalyzers` package fails the build if loc keys are missing. Each character
  needs Architect dialogue in `ancients.json` (`THE_ARCHITECT.talk.SPIREBROTHERS-<CHAR>.0-0r.char`,
  `.0-0r.next`, `.0-1r.ancient`, `.0-attack`).
- **Share** cards use `TargetType.AnyPlayer`. BaseLib patches that type for multiplayer. In solo
  the target is null, so `ShareTarget()` falls back to the owner. Draw and energy given to
  teammates go through vanilla `DrawCardsNextTurnPower` / `EnergyNextTurnPower` to stay
  multiplayer-safe.
- Random choices must use the run's RNG (`Owner.RunState.Rng.CombatTargets`) or co-op will desync.
- **Monkey Island** (all three brothers loved it growing up): `InsultedPower` debuff plus Comeback
  cards that deal bonus damage per stack, then clear it. Use original insults only, never quote
  the game's lines.
- Use game names and short catchphrases as card names only. No copied art.

## Current status

v0.1.0 builds and loads. Daniel appears on character select.

**Open bug: the game freezes at the start of the first combat.** `godot.log` shows no exception.
The last lines are big-card-image lookups for Defend, Strike, then Percussive Maintenance, while
the opening hand is being drawn (3 of 5 cards drawn).

Leading theories:
1. The overrides of `ShouldGlowGoldInternal` / `IsPlayable` in `DanielCard`. Percussive Maintenance
   is the first card in hand with a Wired glow check.
2. A conflict with another mod that patches cards or descriptions (Downfall, RitsuLib, More Enchantments).

Fix 1 has been sent but not yet confirmed as applied or tested. It makes those overrides call
`base` first, return early outside combat, wrap in try/catch with logging, checks NerdsWrong via
`Powers.Any`, adds a turn-start log line in `TurnTracker`, and overrides `CharacterTransitionSfx`
to `event:/sfx/ui/wipe_ironclad` because `wipe_defect` doesn't exist.

Next steps:
- Confirm Fix 1 is in the project and rebuilt.
- Test with only BaseLib + Spire Brothers enabled to rule out mod conflicts.
- If it still hangs, add more `MainFile.Logger.Info` breadcrumbs, or temporarily remove the glow and
  playable overrides to bisect.

After Daniel is stable: playtest checklist (Wired, the starter relic draw, Stratagem gating,
Snappy Comeback damage preview, Diligent scaling, card text formatting), then build David and Joshua.

## Not built yet for Daniel

Crafting Table, Appease the Machine Spirit (transform statuses), Unfinished Business,
I'm Not Done Yet (death prevention), Grog potion, and the "Brothers in Arms" bonus for when
brothers share a co-op run.
