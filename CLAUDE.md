# Spire Brothers: project notes for Claude

## What this is

A Slay the Spire 2 character mod. It adds playable characters based on four real brothers who are
friends of the user and play StS2 together, mostly in multiplayer co-op. The mod is a gift for them,
so the characters should feel personal, warm, and fun. Co-op support matters a lot.

- **Daniel, The Nerd Who Nerds Wrong**: built and playable, now being playtested and expanded.
- **David, The Min-Maxer**: built (2026-10-04), untested in game. Uses the Silent's visuals.
- **Joshua, The Musician**: built (2026-10-04), untested in game. Uses the Regent's visuals.
- **Tim (Timothy), The Draftsman**: the oldest brother. Built (2026-10-04), untested in game. Uses the Ironclad's visuals. Old nickname "Tidbit": he doesn't like it, so use it only once (it's one card).

Full designs are in `DESIGN.md`, with full card plans in `DANIEL_CARDS.md`, `DAVID_CARDS.md`, `JOSHUA_CARDS.md`, and `TIM_CARDS.md`. Build Daniel fully working before starting the others.

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
- `SpireBrothersCode/Cards/Daniel/`: one file per card (78 card files: 4 Basic, 1 Gear token, 73 reward-pool cards incl. Wife Aggro; see DANIEL_CARDS.md)
- `SpireBrothersCode/Mechanics/TurnTracker.cs`: a `CustomSingletonModel` that records the ordered
  Logic/Hands keywords played each turn (per `PlayerCombatState` via `SpireField`), whether last turn
  mixed both, whether Wired fired this turn, and the Gear count
- `SpireBrothersCode/Mechanics/Wired.cs`: `IsActive(card)` and `Check(card)` (also triggers the
  Trusty Multimeter relic)
- David: `Character/David.cs` + `DavidPools.cs` (Silent visuals), `Cards/DavidCard.cs` (glow for Exact/Rant,
  `Hoard()`, `Rants`, `RantAt()`, `LoseGold()`, `ShareTarget()`), `Cards/David/` (77 files: 4 Basic,
  Valley Forge token, 72 reward cards), `Mechanics/DavidTracker.cs` (Rant counts per turn/combat),
  `Mechanics/Exact.cs` (`Check()` returns 0/1/2 triggers, Min-Max doubles, runs `IExactListener` powers),
  `Powers/BleedPower.cs` (end of owner's turn, then halves; Deep Cuts makes it -1), `Powers/RantedAtPower.cs`
  ("loses X Strength this turn", done as a damage modifier so Artifact can't backfire). Starter relic
  `Relics/OldWallet.cs`; `DavidRelic` base lives in `BrothersRelic.cs`.
- Monkey Island relics/potions are `[Pool]`ed to Daniel and added to other brothers' pools in
  `MainFile.ShareMonkeyIslandItems()` via vanilla `ModHelper.AddModelToPool`. Add each new brother there.
- Joshua: `Character/Joshua.cs` + `JoshuaPools.cs` (Regent visuals), `Cards/JoshuaCard.cs` (Verse-threshold glow, `Chorus()`,
  `ChorusAttack()` + `ChorusVerses` multiplier so damage uses the Verses just spent, `AllPlayers`, `ShareTarget()`),
  `Cards/Joshua/` (76 files: 4 Basic, 72 reward cards), `Mechanics/Verses.cs` (count, gain, `SpendForChorus` with
  Encore/Bridge/Choir, `IChorusListener`, `AddRandomSongs`), `Orbs/VerseOrb.cs` (each Verse is an orb above him like the Defect's, max 10; labels hidden by a Harmony patch; Chorus removes them quietly, no evoke), `Mechanics/JoshuaTracker.cs` (Song cards add Verses after
  play), `Mechanics/Archipelago.cs` (Check: rolls an item on the run RNG, thought bubble over the receiver; lines are
  `SPIREBROTHERS-ARCHIPELAGO.*` in powers.json). Starter relic `Relics/WellWornGuitar.cs`.
- Tim: `Character/Tim.cs` + `TimPools.cs` (Ironclad visuals), `Cards/TimCard.cs` (`AgeBonusAt` / `KidsThreshold` / `ExtraGlow`  glow, `KidCount` multiplier, `AllPlayers`), `Cards/Tim/` (77 files, incl. Wife Aggro, which shares its title with Daniel's), `Mechanics/Kids.cs` + `Monsters/Kid.cs` (Kids are pets standing at his feet like Osty, untargetable like vanilla Byrdpip: 9999 HP, no health bar; `Monsters/KidLayout.cs` re-lines them up side by side after the game spreads pets across his body width; placeholder art drawn pixel by pixel in `KidArt`, in the order of his real kids (boy, girl, boy, girl, girl, boy, girl, boy; blue / pink shades); `KidMotion` bounces them idle and hops each one forward when it hits; `TimTracker.BeforeSideTurnEnd` makes them hit), `Mechanics/Ages.cs` (0 Dark .. 3 Imperial; lasts the whole run in a BaseLib `SavedSpireField<Player,int>` (`Ages.RunAge`, touched in `MainFile.Initialize` so it registers in time); +1 damage/Block per Age on all his cards via `TimTracker.ModifyDamageAdditive`/`ModifyBlockAdditive`; the only way up is the Basic `AgeUp` card (3 energy + 50/100/150 gold, 40/80/120 upgraded; removes itself at Imperial); `Mechanics/TimRunTracker.cs` (a Run-hook singleton) removes Smith from his rest sites) + `Mechanics/AgeDisplay.cs` (number above his head; drop `images/ages/age0.png`..`age3.png` in to use art instead). `TimCard.RefreshHand()` redraws hand text after Age/Kids changes. `Mechanics/TimTracker.cs` (cards played this turn; queues Script cards in `ScriptPower`, which auto-plays a  `CreateDupe()` of each at the start of the next turn, like vanilla History Course; dupes never re-queue, Powers never get  Script). Snap to Grid / Automation Suite / Xref add Script; Script uses `AutoKeywordPosition.After` so it shows when added.  Wololo / Mass Conversion use vanilla `FlexPotionPower` for Strength this turn. Starter relic `Relics/FamilyMinivan.cs`.
- Touch of Orobas: each starter relic overrides BaseLib's `GetUpgradeReplacement()` to return its upgraded version
  (Well, Technically / Overstuffed Wallet / Signature Guitar / Fifteen-Passenger Van, all Starter rarity in the brother's pool).
- Insult cards implement `Mechanics.IInsultCard`; `Mechanics/InsultBanter.cs` plays the Merchant's laugh and shows a speech bubble (4s+) from
  `SPIREBROTHERS-INSULT_LINES.0..39` (powers.json; bump `InsultBanter.Count` when adding lines), shuffled per run.
- `Powers/BleedHealthBar.cs` patches `NHealthBar.RefreshForeground` to draw Bleed's upcoming damage in crimson, like Poison.
- Card portraits are drawn by `art_originals/cardgen/gen.ps1 -CardsOnly` (or `-Only <Class>`) from `glyphs.txt` (ClassName Emoji,
  or `@Name` for a custom drawing in `icons_<brother>.ps1`; game-reference cards use custom drawings). New cards need a line there.
- Combat bodies: `Character/BrotherCharacter.cs` plays `images/characters/<name>/frames/<anim>_<n>.png` (idle/attack/cast/hit/dead,
  pixel art at 64x80, nearest filter) on an `AnimatedSprite2D`, which BaseLib drives from the game's animation cues;
  falls back to `body.png`, then the vanilla placeholder. The frames come from `art_originals/pixelgen/` (see its README;
  bodies are three-quarters turned right, David wears sunglasses; a `rest` loop for rest sites, played by `StandIns`).
  Re-run it to change the art, then copy the frames in. A copy is on her Desktop (`pixelgen`).
  The `dead_N.png` frames are registered as the animation "die" (BaseLib's death cue only tries "Dead"/"Die"/"die", case-sensitive). The sprite's pivot is moved to the feet and it tweens to -90° on `die` (back up on `revive`), because the dead frames only slump.
  It also sets the character icon (`icon.png`, falling back to `map_marker.png`, plus `icon_outline.png`): co-op map votes and the
  player list use it, and the top-left icon reuses the placeholder's icon scene with our textures swapped in.
  Character select background: `CustomCharacterSelectBg` points at `SpireBrothers/scenes/char_select/<name>_bg.tscn`
  (hand-written scene: full-screen TextureRect, nearest filter, keep-aspect-covered) showing `<name>/select_bg.png`.
  Shop and rest sites only take a Spine scene, so `Character/StandIns.cs` keeps the placeholder's scene, hides its
  SpineSprite, and adds `BrotherCharacter.CreateIdleSprite()` in its place (sizes are constants there; untested in game).
- `SpireBrothersCode/Powers/`, `SpireBrothersCode/Relics/`, `SpireBrothersCode/Potions/`
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
- **Exact cards never draw this turn when Exact triggers** (user feedback): Exact means 0 energy left, so drawn
  cards are useless. Any draw on an Exact card, base draw included, moves to next turn via `DrawNextTurn` when
  Exact fires (see A Glove Fry, Spreadsheet, Do the Math). Energy gains (Sudoku) and 0-cost tokens are fine now.
- **Monkey Island** (all four brothers loved it growing up): `InsultedPower` debuff plus Comeback
  cards that deal bonus damage per stack, then clear it. Use original insults only, never quote
  the game's lines.
  Every Comeback card must use `Mechanics/Comeback.cs` (`Stacks`, `PerInsultBonus`, `ClearInsulted`) so the
  Monkey Wrench relic and Monkey Business potion work for all brothers. Monkey-speak items show their real
  effect through `MonkeySpeak.Translation` (a `<ID>.translation` loc key the analyzer does not check).
- Use game names and short catchphrases as card names only. No copied art.
- **Growing numbers (user preference, applies to every brother):** when a card's number grows above its
  printed value (Diligent, Hoard, Rant scaling, turn count, etc.), show the printed number dimmed
  (`[color=#ffffff8c]`, white at ~55% opacity) and the current number in green right after it, with no
  extra words: "Deal 6 9 damage". `[s]` strikethrough does NOT render in card text (tested).
  - Calculated-var cards: `Mechanics/Growth.AddCalcArgs` (called from each brother's card base class in
    `AddExtraArgsToDescription`) adds `{CalcGrown}`/`{CalcStart}`. Text:
    `{CalcGrown:cond:[color=#ffffff8c]{CalcStart}[/color] [green]{CalculatedDamage:diff()}[/green]|{CalculatedDamage:diff()}} damage`
    (or `CalculatedBlock` followed by `[gold]Block[/gold]`).
  - Diligent cards use `{DiligentGrown}`/`{DiligentStart}` from `DanielCard`, same dimmed-then-green shape.
  - Every brother's card base class calls `Growth.AddCalcArgs`; any new base class must too.

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
- All four brothers are built. Next: playtest David, Joshua, and Tim.

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

The "Brothers in Arms" bonus for when brothers share a co-op run. (Monkey Island relics and potions, including Grog, are built.)
Card art for everything (all cards, relics, and powers use placeholders).

## Added 2026-10-06

- `Mechanics/AgeCardLine.cs`: once Tim has Aged Up, his damage/Block cards get a gold line at the bottom ("Castle Age: +2 damage.",
  keys `SPIREBROTHERS-AGES.cardDamage/cardBlock/cardBoth`). Normal upgrades still work for him (events, relics, Markup Pass); only
  the rest-site Smith is removed.
- Each brother sets `MapDrawingColor` and `RemoteTargetingLineColor/Outline` (BaseLib leaves them black).
- `Character/LobbyRowFix.cs`: re-sorts the co-op lobby player list after the game's character-change shake (rows overlapped on the host).
- `Mechanics/HitchLogger.cs`: logs `[Hitch] N ms` to godot.log on frames over 150 ms, for tracking lag spikes. Remove before release if unwanted.
- `cardgen/gen.ps1 -Section relics|powers -Only <name>` redraws one relic or power.
