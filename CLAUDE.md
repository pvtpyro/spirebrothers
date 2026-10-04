# Spire Brothers: project notes for Claude

## What this is

A Slay the Spire 2 character mod. It adds playable characters based on three real brothers who are
friends of the user and play StS2 together, mostly in multiplayer co-op. The mod is a gift for them,
so the characters should feel personal, warm, and fun. Co-op support matters a lot.

- **Daniel, The Nerd Who Nerds Wrong**: built and playable, now being playtested and expanded.
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
- `SpireBrothersCode/Cards/Daniel/`: one file per card (77 card files: 4 Basic, 1 Gear token, 72 reward-pool cards; see DANIEL_CARDS.md)
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

Daniel is playable: combat works and cards can be played.

The first-combat freeze is **fixed**. `TurnTracker.Seq()` called itself instead of
`Sequence.Get(state)`, causing a stack overflow at turn start (uncatchable in .NET, so nothing was
logged). The glow/playable overrides and mod conflicts were not the cause.

Changes since v0.1.0 (2026-10-03):
- **Defend is now a Logic card** (Strike is Hands), so Wired, the starter relic, and Stratagem inputs
  work from the starting deck.
- **Snappy Comeback** added to the starting deck (permanent).
- **Stratagems all cost 0** (entering the input is the cost). Inputs: Resupply = Hands, Logic
  (Block 15 -> 10); Reinforce = Logic, Hands; Orbital Barrage = Logic, Hands, Logic (the big finisher).
  The old 3-input + 1-2 cost versions were unplayable on 3 energy.
- New, from Daniel's love of grapefruit:
  - **Grapefruit** (Common relic): your potions are 50% stronger (rounded up). Implemented by scaling
    every positive `DynamicVar` on the potion in `BeforePotionUsed`. Safe because the potion is
    already removed from the belt by then. Only affects the owner's potions.
  - **Want Some?** (Common skill, Hands, Share, Exhaust, cost 1): a player heals 3 (6 if it's you) and
    gains 1 energy next turn. Exhaust was added so the heal can't be repeated every turn.
- **Train of Thought** tracker: `TrainOfThoughtPower` (type None, so buff-stripping can't remove it)
  is applied to Daniel on his first turn each combat by `TurnTracker`. Its number is the count of
  Logic/Hands cards played this turn; an extra hover tip lists them in order. Stratagem card text
  uses `{Input}`, built in `DanielCard.AddExtraArgsToDescription`, with entered steps in green.
- **TEMP, remove before release:** `WantSome` in the starting deck and `Grapefruit` in the starting
  relics, for playtesting. Both lines in `Daniel.cs` are marked `// TEMP`.

Decided: no greyed-out "Wired:" text. Vanilla doesn't dim conditional text; the gold glow is enough.

Next steps:
- Playtest checklist: Wired glow, the starter relic draw, Stratagem gating and the new inputs,
  Snappy Comeback damage preview, Diligent scaling, Grapefruit potion boost, Want Some? (solo and
  co-op), card text formatting.
- **52-card expansion built (2026-10-03), untested in game.** Reward pool is now 72 (20 common /
  36 uncommon / 16 rare); the list is in `DANIEL_CARDS.md`. New powers: GroundWire, AssemblyLine,
  PrawnSuit, IonBattery, ForDemocracy, TenThousandHours (read in `DanielCard.GrowDiligent`),
  ImNotDoneYet (Lizard Tail's `ShouldDieLate`/`AfterPreventingDeath` hooks). Shared helpers in
  `DanielCard`: `AddGears(n)`, `GrowDiligent()` + `DiligentTarget`.
  Riskiest to playtest: I'm Not Done Yet, And Another Thing! (vanilla `DuplicationPower`),
  Appease the Machine Spirit (`CardCmd.TransformTo<Gear>`), Ctrl+Z / Ctrl+C, Ctrl+V / Refactor
  (card selection screens), Recursion, Squad Up! in co-op.
- Card text gotcha: the `energyIcons()` formatter only accepts `EnergyVar`/`CalculatedVar`/numbers.
  A `PowerVar` throws at runtime, so cards that grant energy via a power also carry an `EnergyVar`
  for the text. Cards using `SelectionScreenPrompt` need a `.selectionScreenPrompt` loc key, and
  the analyzer does NOT check for it.
- When generating many card files from the shell, keep each heredoc batch to ~5 cards; very long
  commands get truncated and fail with "unexpected EOF".
- Then build David and Joshua.

## Tooling notes

- `dotnet` is not on PATH in Claude's shells. Use `"C:\Program Files\dotnet\dotnet.exe"`. Python is
  not installed.
- `dotnet publish` prints a lot of Godot error traces and `MSB3073 ... exited with code -1` from the
  .pck export. That's noise: the .pck still gets written. Check the C# build with
  `dotnet build -c Release --no-restore` (looks for "0 Error(s)") and the dll timestamp in the mods folder.
- To read game code: the game ships `data_sts2_windows_x86_64/sts2.xml` (partial API docs). For full
  source, decompile `sts2.dll` with `ilspycmd` 9.1.0.7988 (newer versions fail to install) into the
  scratchpad, setting `DOTNET_ROLL_FORWARD=Major` because it targets .NET 8 and only the 9 SDK is installed.
- Vanilla card text is in `SlayTheSpire2.pck` and can be grepped as plain text.

## Not built yet for Daniel

Grog potion, and the "Brothers in Arms" bonus for when brothers share a co-op run.
Card art for everything (all cards, relics, and powers use placeholders).
