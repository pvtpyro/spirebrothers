# Spire Brothers: project notes for Claude

## What this is

A Slay the Spire 2 character mod. It adds playable characters based on four real brothers who are
friends of the user and play StS2 together, mostly in multiplayer co-op. The mod is a gift for them,
so the characters should feel personal, warm, and fun. Co-op support matters a lot.

- **Daniel, The Nerd Who Nerds Wrong**: built and playable, now being playtested and expanded.
- **David, The Min-Maxer**: built (2026-10-04), untested in game. Uses the Silent's visuals.
- **Joshua, The Musician**: built (2026-10-04), untested in game. Uses the Regent's visuals.
- **Tim (Timothy), The Draftsman**: the oldest brother. Built (2026-10-04), untested in game. Uses the Ironclad's visuals. Old nickname "Tidbit": he doesn't like it, so use it only once (it's one card).

Two friends they always play with, **John** and **Huffman** (a friend named Joshua; always "Huffman" in the mod), are
characters five and six ("Honorary Brothers"): dossier in `FRIENDS.md`, card plans in `JOHN_CARDS.md` / `HUFFMAN_CARDS.md`.
Both are built (2026-10-27), untested in game.

## The friends (built like the brothers, but kept visibly apart)

- They extend `BrotherCharacter` like the brothers. Friends look: charcoal select buttons with steel rings and silver
  names, darker select scenes, silver-grey card art (`gen.ps1` color), "Honorary Brother" in their select description.
  Their buttons, frames, hands, map marker (pixel face on a charcoal disc), icon outline, and select background come from
  `art_originals/friendbuttons/` (borrows pixelgen's code; their bodies are `pixelgen/Friends.cs`, scenes in
  `pixelgen/Backgrounds.cs`). Copy `out/<name>/...` and `out/plain/<name>/select*.png` into `images/characters/<name>/`.
- Character order: `MainFile` calls BaseLib `CustomCharacterUtils.TryOrderCustomCharacters` (brothers, then friends).
- Card class names must be unique across the whole mod (the ID is the class name). Huffman's "Have You Tried Rebooting?",
  "Jetpack Boost", and "Blue Screen of Death" are named that way because Daniel and David already have the shorter ones.
- `gen.ps1 -CardsOnly -Character John` draws only one character's cards (a full run would redraw every brother's, and any
  hand-drawn art with it). Relics/powers one at a time with `-Section relics|powers -Only <name>`.
- Friends' campfire loops (2026-10-27): John raids on a laptop on his lap, screen glow on his face, fist pump at the loot (`JohnArt.DrawFront`; anything held by his mouth reads as food), Huffman pets the Mastiff lying at his knee (`HuffmanArt.LyingDog`). Chest hands: John's fingerless glove + stimm injector, Huffman's Arbites gauntlet (`Hands.DrawHand` glove overload). Map colors: John amber, Huffman steel.
- Leave John's grandma out of the mod. John and Huffman are uncle and nephew but the same age: write them as peers.
- John: `Character/John.cs` + `JohnPools.cs` (Ironclad scenes), `Cards/JohnCard.cs`, `Cards/John/` (5 Basic, 70 reward, Stimm
  token, 8 talent cards + `TalentCard`, Duo Discord Call with David). Mechanics:
  - **Grind** (`Mechanics/Grind.cs`): BaseLib `SavedSpireField<CardModel,int>` on the deck copy; `JohnCard.GrindTarget` names
    the var, `SyncGrind()` adds the run's growth onto it (tracked by `_grindApplied`, which clones copy along with the number),
    `GrindAfterPlay(ctx)` grinds once per combat, `GrindTimes(ctx, n)` forces it. Text uses `{GrindGrown}`/`{GrindStart}`.
  - **Toxin** (`Mechanics/Toxin.cs`, `ToxinPower`): inert debuff; Chem cards cash it in. All Toxin goes through `Toxin.Apply` (Cartel Kingpin).
  - **Shroud** (`Mechanics/Shroud.cs`, `ShroudPower`): turns left as the amount, x1.5 Attack damage, 1 HP lost at his turn end.
  - **Stimm Lab talents** (`Mechanics/StimmLab.cs`): `JohnRunTracker.TryModifyRestSiteOptions` adds `StimmLabRestSiteOption`
    (loc in the new `rest_site_ui.json`, whose hover lists learned talents via `StimmLab.LearnedLines`; icon `images/ui/stimm_lab.png`, swapped in by `StimmLabIconPatch` since the game's
    icon path is private). Pick 1 of 3 talent cards (`FromChooseACardScreen`), seeded from the run + floor, saved per player.
    `JohnTracker` (combat) applies Pack Mule / Toxic Payload / Potency, and gives `TalentsPower` (icon listing talents + ranks) on turn 1. Riskiest to test: the rest site button in co-op.
- Huffman: `Character/Huffman.cs` + `HuffmanPools.cs` (Necrobinder scenes), `Cards/HuffmanCard.cs` (`BuildCost` / `LawBonus` /
  `ExtraGlow` glow, `Build(ctx)`, `Laws` count), `Cards/Huffman/` (5 Basic, 70 reward, Duo Water Cooler Talk with Daniel and
  Same Age, Technically with John; John's Grew Up Together pairs with him). Starter relic `Relics/ArbitesBadge.cs` (first Power each combat costs 0, like vanilla Brilliant Scarf; Touch of Orobas: Judge's Badge). Mechanics:
  - **Mastiff** (`Mechanics/Mastiff.cs`, `Monsters/MastiffDog.cs`): one pet dog like Tim's Kids (9999 HP, no bar, reuses `KidMotion`
    bounce/hop; `MastiffLayout` puts it in front of him). Art `images/monsters/mastiff/mastiff.png` (friendbuttons draws it).
    Bite is `BitePower` on Huffman (type None); `MarkedPower` picks the target (one at a time); `HuffmanTracker` spawns it on
    turn 1 and bites at his turn end. Lone Wolf / Best in Show / Loyal Companion are read in `Mastiff.Attack`.
  - **Laws**: Powers extending `Powers/LawPower.cs` (Martial Law doubles via `Times`). Most react in vanilla `AfterAttack`
    (`AttackCommand.Attacker` / `Results`); No Armor Permitted uses `AfterBlockGained` with `new ThrowingPlayerChoiceContext()`
    like vanilla Juggernaut; No Loitering snapshots non-attacking intents at the enemy turn start.
  - **Metal / Build** (`Mechanics/Metal.cs`, `MetalPower`): `Metal.Build` spends the cost if he has it (Build Queue makes one free).
  - I'll Wait Till I Get Home grows in `HuffmanTracker.AfterFlush` when Retained.
- Friends' extras: 3 potions each (`JohnPotion` / `HuffmanPotion` in `Potions/BrotherPotion.cs`), 3 relics each (Loot Chest is
  paid in `JohnCard.GrindTimes`, Survivor's Lantern (5 Block on entering the Shroud; was a duplicate of Shroud Lungs) in `Shroud.Enter`, Chew Toy in `Mastiff.Spawn`), plus Blown Cartridge
  added to their pools in `MainFile` (not Monkey Island, the family relics, or Party Headset: removed 2026-10-27, too few Share cards).
- Friends' bonds, Ancient cards, stats, events, sounds (2026-10-27, untested): bonds Uncle and Nephew / Best Friends / Coworkers /
  Strike Team are in `FamilyBonds` (hooks: `FamilyBonds.AfterExact` from `Exact.Check`, `AfterBite` from `Mastiff.Attack`); friends
  get `FamilyBondsPower` via `Banter.Talks`. Ancient: Realm First!, Home Sweet Home, Cartel Masterwork, Release the Hound (ANCIENT_CARDS.md).
  Stats: `RunStats` ToxinApplied (`Toxin.Apply`), ShroudTurns (`ShroudPower`), Bites, MetalSpent (`Metal.Spend`), LawsEnforced
  (`LawPower.Enforce()`, which every Law calls instead of `Flash()`). Events: `WeekendInTown`, Road Trip's `TurnAroundAndGoHome`.
- Banter: `Banter.Talks(player)` (brothers + friends) gates banter and Pings; shared pools keep Mom/Dad lines in `<POOL>.FAMILY` (BOSS, FALL), added only for brothers by `Banter.WithFamily`; `BrothersInArms.IsBrother` stays brothers-only
  for relics, Family Bonds, and events. Friends' lines are in powers.json like the brothers'. What's left for them: TODO.md #12.

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
- `SpireBrothersCode/Cards/Daniel/`: one file per card (reward pool 87 as of 2026-10-27, plus Basics, the Gear token, and Duo cards; see DANIEL_CARDS.md)
- `SpireBrothersCode/Mechanics/TurnTracker.cs`: a `CustomSingletonModel` that records the ordered
  Logic/Hands keywords played each turn (per `PlayerCombatState` via `SpireField`), whether last turn
  mixed both, whether Wired fired this turn, and the Gear count
- `SpireBrothersCode/Mechanics/Wired.cs`: `IsActive(card)` and `Check(card)` (also triggers the
  Trusty Multimeter relic)
- David: `Character/David.cs` + `DavidPools.cs` (Silent visuals), `Cards/DavidCard.cs` (glow for Exact/Rant,
  `Hoard()`, `Rants`, `RantAt()`, `LoseGold()`, `ShareTarget()`), `Cards/David/` (99 files: 5 Basic,
  3 tokens, 3 Duo, 88 reward cards), `Mechanics/DavidTracker.cs` (Rant counts per turn/combat; `RantsThisTurn` adds `OverthinkingPower`),
  `Mechanics/Exact.cs` (`Check()` returns 0/1/2 triggers, Min-Max doubles, runs `IExactListener` powers),
  `Powers/BleedPower.cs` (start of owner's turn like Poison, then halves; Deep Cuts makes it -1), `Powers/RantedAtPower.cs`
  (15% less damage per stack, max 40%, until the end of the enemy's turn; a damage modifier so Artifact can't backfire. Was flat X less per hit until 2026-10-27). Starter relic
  `Relics/OldWallet.cs` (Block for ALL players). Support cards (Share) target a teammate via `ShareTarget()`; `BaseCampPower` / `RainyDayFundPower` protect the team. `DavidRelic` base lives in `BrothersRelic.cs`.
- Monkey Island relics/potions are `[Pool]`ed to Daniel and added to other brothers' pools in
  `MainFile.ShareMonkeyIslandItems()` via vanilla `ModHelper.AddModelToPool`. Add each new brother there.
- Joshua: `Character/Joshua.cs` + `JoshuaPools.cs` (Regent visuals), `Cards/JoshuaCard.cs` (Verse-threshold glow, `Chorus()`,
  `ChorusAttack()` + `ChorusVerses` multiplier so damage uses the Verses just spent, `AllPlayers`, `ShareTarget()`),
  `Cards/Joshua/` (5 starting-deck Basics, 24 retired (Basic rarity, kept for saved runs), 90 reward cards, 3 Duo, 1 token), `Mechanics/Verses.cs` (count, gain, `SpendForChorus` with
  Encore/Bridge/Choir, `IChorusListener`, `AddRandomSongs`), `Orbs/VerseOrb.cs` (each Verse is an orb above him like the Defect's, max 10; labels hidden by a Harmony patch; Chorus removes them quietly, no evoke), `Mechanics/JoshuaTracker.cs` (Song cards add Verses after
  play), `Mechanics/Archipelago.cs` (Check: rolls an item on the run RNG, thought bubble over the receiver; lines are
  `SPIREBROTHERS-ARCHIPELAGO.*` in powers.json). Starter relic `Relics/WellWornGuitar.cs`.
- Tim: `Character/Tim.cs` + `TimPools.cs` (Ironclad visuals), `Cards/TimCard.cs` (`AgeBonusAt` / `KidsThreshold` / `ExtraGlow`  glow, `KidCount` multiplier, `AllPlayers`), `Cards/Tim/` (97 files: 88 reward cards, incl. Wife Aggro, which shares its title with Daniel's), `Mechanics/Kids.cs` + `Monsters/Kid.cs` (Kids are pets standing at his feet like Osty, untargetable like vanilla Byrdpip: 9999 HP, no health bar; `Monsters/KidLayout.cs` re-lines them up side by side after the game spreads pets across his body width; placeholder art drawn pixel by pixel in `KidArt`, in the order of his real kids (boy, girl, boy, girl, girl, boy, girl, boy; blue / pink shades); `KidMotion` bounces them idle and hops each one forward when it hits; `TimTracker.BeforeSideTurnEnd` makes them hit), `Mechanics/Ages.cs` (0 Dark .. 3 Imperial; lasts the whole run in a BaseLib `SavedSpireField<Player,int>` (`Ages.RunAge`, touched in `MainFile.Initialize` so it registers in time); +1 damage/Block per Age on all his cards via `TimTracker.ModifyDamageAdditive`/`ModifyBlockAdditive`; the only way up is the Basic `AgeUp` card (3 energy + 50/100/150 gold, 40/80/120 upgraded; removes itself at Imperial); `Mechanics/TimRunTracker.cs` (a Run-hook singleton) removes Smith from his rest sites) + `Mechanics/AgeDisplay.cs` (number above his head; drop `images/ages/age0.png`..`age3.png` in to use art instead). `TimCard.RefreshHand()` redraws hand text after Age/Kids changes. `Mechanics/TimTracker.cs` (cards played this turn; queues Script cards in `ScriptPower`, which auto-plays a  `CreateDupe()` of each at the start of the next turn, like vanilla History Course; dupes never re-queue, Powers never get  Script; `ScriptPower.IsReplay(card)` marks them, and `Powers/RingingScriptFix.cs`
  makes Ringing Ears ignore them so the replay doesn't use up the 1 allowed card). Snap to Grid / Automation Suite / Xref add Script; Script uses `AutoKeywordPosition.After` so it shows when added.  Wololo / Mass Conversion use vanilla `FlexPotionPower` for Strength this turn. Starter relic `Relics/FamilyMinivan.cs`.
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
  Faces and hair follow the reference photos in `photos/` (2026-10-27): Daniel messy hair + wire glasses + stubble, David
  fringe + rectangular sunglasses + stubble, Joshua crew cut + dark glasses + stubble, Tim buzzed + glasses + full beard +
  over-ear headphones. `photos/` has a `.gdignore` so the family photos never get packed into the Workshop .pck; keep it.
  Re-run it to change the art, then copy the frames in. A copy is on her Desktop (`pixelgen`).
  The `dead_N.png` frames are registered as the animation "die" (BaseLib's death cue only tries "Dead"/"Die"/"die", case-sensitive). The sprite's pivot is moved to the feet and it tweens to -90° on `die` (back up on `revive`), because the dead frames only slump.
  Co-op treasure room hands: `hands/point.png`, `rock`, `paper`, `scissors` (422 x 1200, from pixelgen `Hands.cs`); without them the game shows the borrowed vanilla arms.
  It also sets the character icon (`icon.png`, falling back to `map_marker.png`, plus `icon_outline.png`): co-op map votes and the
  player list use it, and the top-left icon reuses the placeholder's icon scene with our textures swapped in.
  Character select background: `CustomCharacterSelectBg` points at `SpireBrothers/scenes/char_select/<name>_bg.tscn`
  (hand-written scene: full-screen TextureRect, nearest filter, keep-aspect-covered) showing `<name>/select_bg.png`.
  Shop and rest sites only take a Spine scene, so `Character/StandIns.cs` keeps the placeholder's scene, hides its
  SpineSprite, and adds `BrotherCharacter.CreateIdleSprite()` in its place (sizes are constants there; untested in game).
- `SpireBrothersCode/Powers/`, `SpireBrothersCode/Relics/`, `SpireBrothersCode/Potions/`
- `SpireBrothersCode/BrotherKeywords.cs`: custom `CardKeyword`s. Logic/Hands/Stratagem auto-insert
  before the card text. **Hands is shown as "Handy" in game** (renamed 2026-10-27: Daniel read "Hands" as
  "this is an Attack"); the code name and loc keys stay `Hands`/`HANDS`, so always write "Handy" in card text. Wired/Share/Diligent/Comeback use `AutoKeywordPosition.None` and are
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
- **Collection fields on powers/relics/cards need `DeepCloneFields`.** The game makes each live model by `MemberwiseClone` of a
  template, so a `List`/`HashSet` field initialized inline is shared by every copy (ScriptPower's queue leaked into the next
  combat and between co-op Tims, fixed 2026-10-27). Override `DeepCloneFields()`, call base, and assign a new collection.
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
  - **Tim is the exception:** `Mechanics/TimDiffText.cs` patches `DynamicVar.ToHighlightedString` so every `:diff()`
    on a `TimCard` shows dimmed printed + green (red if lower) whenever the value differs (Age bonus, Kids, Strength).
    His loc text uses plain `{Damage:diff()}` / `{CalculatedDamage:diff()}`; do NOT add `CalcGrown` wrappers (it would double).

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
- **Redrawn art not showing up in game:** the .pck export can reuse Godot's stale import cache when a PNG is overwritten
  (seen with Blockbeard's frames). Delete the `.ctex`/`.md5` pair in `.godot/imported/` that the image's `.png.import`
  points to (the `path=` line), then publish again to force a re-import.
- `dotnet publish` prints a lot of Godot error traces and `MSB3073 ... exited with code -1` from the
  .pck export. That's noise: the .pck still gets written. Check the C# build with
  `dotnet build -c Release --no-restore` (looks for "0 Error(s)") and the dll timestamp in the mods folder.
- To read game code: the game ships `data_sts2_windows_x86_64/sts2.xml` (partial API docs). For full
  source, decompile `sts2.dll` with `ilspycmd` 9.1.0.7988 (newer versions fail to install) into the
  scratchpad, setting `DOTNET_ROLL_FORWARD=Major` because it targets .NET 8 and only the 9 SDK is installed.
- Vanilla card text is in `SlayTheSpire2.pck` and can be grepped as plain text.

## Not built yet for Daniel

Card art for everything (all cards, relics, and powers use placeholders).

## Added 2026-10-06

- `Mechanics/AgeCardLine.cs`: once Tim has Aged Up, his damage/Block cards get a gold line at the bottom ("Castle Age: +2 damage.",
  keys `SPIREBROTHERS-AGES.cardDamage/cardBlock/cardBoth`). Normal upgrades still work for him (events, relics, Markup Pass); only
  the rest-site Smith is removed.
- Each brother sets `MapDrawingColor` and `RemoteTargetingLineColor/Outline` (BaseLib leaves them black).
- `Character/LobbyRowFix.cs`: re-sorts the co-op lobby player list after the game's character-change shake (rows overlapped on the host).
- `Mechanics/HitchLogger.cs`: logs `[Hitch] N ms` to godot.log on frames over 150 ms, for tracking lag spikes. Remove before release if unwanted.
- `cardgen/gen.ps1 -Section relics|powers -Only <name>` redraws one relic or power. A relic glyph can be `@Name` to reuse a card icon from
  `$CustomIcons` (scaled to fit by `RelicParts`; Lucky d20 uses `@D20`, the same drawing as Natural Twenty).

## Added 2026-10-25

- 21 new relics (list in DESIGN.md, Shared section), untested in game. Hooks they rely on: `IExactListener` and `IChorusListener`
  now also run on the owner's relics (Graphing Calculator, Gold Record); Soapbox is counted in `DavidTracker`, Capo in `Verses.ForChorus`,
  Spoiler Log in `Archipelago.Check`, Startup Script in `TimTracker`, Patch Panel in `TurnTracker`.

## Added 2026-10-26

- **Family Bonds** (table in DESIGN.md): `Mechanics/FamilyBonds.cs` (a combat singleton; `Active(players)` lists the bonds,
  `Bond` enum) applies `Powers/FamilyBondsPower.cs` to each brother on turn 1. The power shows the count and ONE extra hover tip listing the
  bond titles (`SPIREBROTHERS-FAMILY_BONDS_POWER.<BOND>.title`; the `.description` keys aren't shown anywhere now). One tip per
  bond (8 with all four brothers) went with crashes to desktop when hovering a teammate in co-op, so keep creature hover stacks short, and handles the 4-brother death save and Family
  Loan's gold. Other hook points: `Verses.SpendForChorus` -> `FamilyBonds.AfterChorus`, `ScriptPower.AfterScriptRan` ->
  `FamilyBonds.AfterScriptRan`, `Ages.GoldCost(player, ...)` subtracts `AgeUpDiscount`. Starter relics show the power as a hover tip.
  Ideas backlog: `TODO.md`.
- **Banter** (lines planned in BANTER.md, live in powers.json as `SPIREBROTHERS-BANTER.<POOL>.N`; pool sizes are counted with
  `LocString.Exists`, so adding a line needs no code change): `Mechanics/Banter.cs` (combat singleton). Combat start
  (1 in 3 with 2+ brothers, `START.<SPEAKER>_<LISTENER>`; 1 in 4 alone, `SOLO.<NAME>`), `AfterDeath` (`FALL`, `FALL.<NAME>`),
  `BanterSavedPatch` on `Hook.AfterPreventingDeath` (`SAVED`), boss/elite wins, and once-per-combat `Banter.Moment` calls from
  DavidTracker (4 Rants), Verses (8+ Verse Chorus), Kids (reaching 8), Ages (Age Up / Imperial). Picks are seeded from the run
  plus a counter, never the run RNG. `Banter.Bubble` is the shared speech bubble (InsultBanter uses it too).
  Co-op Ping button: `Mechanics/PingBanter.cs` patches `FlavorSynchronizer.CreateEndTurnPingDialogueIfNecessary` and picks
  from `PING.<PINGER>_<WAITEE>` / `PING.<PINGER>.GROUP` / `PING.<PINGER>` / `PING.DEAD.<PINGER>` by who hasn't ended their
  turn (`CombatManager.IsPlayerReadyToEndTurn`); its own per-pinger shuffle counter, so it never shifts Banter's picks. The
  `<CHAR>.banter.*.endTurnPing` lines in characters.json are only the fallback now.
- **Duo cards** (12, list in TODO.md #3): Uncommon cards in each brother's own pool that implement `Mechanics.IDuoCard`
  (`Partner` = the other brother's character type) and carry the `Duo` keyword, written by hand as "[gold]Duo: David.[/gold]".
  `DuoCards` (run singleton) filters them out of card rewards (`ModifyCardRewardCreationOptions`) and shops
  (`ModifyMerchantCardPool`) unless the partner is in the party; Shared Steam Library and `Verses.AddRandomSongs` call
  `DuoCards.Allowed` too. Any new random-card generator should as well. In combat, `DuoCardsCombatPatch` also filters vanilla `CardFactory.GetDistinctForCombat`/`GetForCombat` (Choices Paradox, Discovery, ...). Effects reach the partner via `DuoCards.Partners(combat, Partner)`.
  Card counts above don't include them (+3 per brother).
- **Events** (`SpireBrothersCode/Events/`, BaseLib `CustomEventModel`, no Acts = any act): `BrotherEvent` base (portrait
  `images/events/<event_id>.png`, wide 3440x1613 like vanilla; `IsAllowed` needs a brother in the party; helpers for heal %,
  HP loss, Max HP, Gold, random potion/relic, upgrades, removal, transform, add to deck). `Option(Method)` builds loc keys from
  the method name: `<ID>.pages.<PAGE>.options.<METHOD_SLUG>.title/description`; results use `Finish("<PAGE>")` ->
  `<ID>.pages.<PAGE>.description`. Text lives in a new `events.json` (the analyzer doesn't check event keys). `IsShared => true`
  means co-op votes and the winner runs for everyone (Road Trip, Family Hike). Art: `cardgen/gen.ps1 -Section events [-Only <id>]`.
  The family details behind them are in DESIGN.md ("The family").
- **Potions**: `Potions/BrotherPotion.cs` holds the David/Joshua/Tim potion bases (pooled like `DanielPotion`); 3 each (list in
  TODO.md #4). Gas Station Coffee applies `GasStationCoffeePower`, which `Exact.WouldTrigger`/`Check` read (1 energy left counts).
  Hotfix reuses `SnapToGridPower`. Daniel's own 3 extend `DanielPotion`; Spare Battery's `SpareBatteryPower` makes `Wired.IsActive` true this turn. Potion art: the `$potions` table in gen.ps1, `-Section potions [-Only <file_name>]`.
- **Sounds** (`Mechanics/Sounds.cs`, a Run-hook singleton since `AfterGoldGained` only reaches run listeners): `Sounds.Play(cue)`
  plays `SpireBrothers/audio/<cue>.ogg|.wav` via BaseLib `ModSound` if it exists, else an FMOD fallback (`SfxCmd.Play`) or
  nothing; 0.4s per-cue throttle. Cues are called from Wired.Check, DavidTracker, Verses.SpendForChorus, Kids.Act, Ages.AgeUp,
  Honey Lemon Tea, and the singleton itself (Orbital Barrage, `IGroupHealCard` cards, David's in-combat gold). Built-in FMOD
  event names can be pulled from sts2.dll's UTF-16 strings (`event:/sfx/...`). File list for the user: `SpireBrothers/audio/README.md`.
- **Captain Blockbeard** (TODO.md #7): `Monsters/Blockbeard.cs` (CustomMonsterModel; MoveState loop; body from
  `images/monsters/blockbeard/frames/` via `Character/PixelBody.cs`, mirrored with `faceLeft`), `Monsters/BlockbeardElite.cs`
  (CustomEncounterModel, `IsValidForAct` Act 3; the event is Act 3 too), `Powers/InsultDuelPower.cs` (retort tracking; shows `AwaitingRetortPower` while waiting, taunts at the players' turn start,
  and every brother's card base glows retort cards via `InsultDuelPower.WantsRetort`; Gloating = `GloatingPower`, +3 Strength applied at the players' next turn start, spent by his next attack; Speechless = `CreatureCmd.Stun`
  with `NextMove.Id`), `Events/InsultDuel.cs` (dynamic answer options via `Option(func, LocString, LocString)`, Draw Swords uses
  `EnterCombatWithoutExitingEvent`). Text: monsters.json, the new encounters.json, events.json. `PixelBody` now holds the
  frame/animation code that used to live in BrotherCharacter. The Desktop copy of pixelgen doesn't have `Pirate.cs` yet.
- Also new: `Cards/Shared/FoodComa.cs` (curse, `CurseCardPool`; cardgen draws `Cards\Shared` too), David's `SundayWalk` +
  `SundayWalkPower`, `Relics/DadsTackleBox.cs` (shared relic, registered for every brother in `MainFile`).
- **End-of-run stats** (TODO.md #8): `Mechanics/RunStats.cs` holds per-player `SavedSpireField<Player,int>` counters (touched in
  `MainFile.Initialize` like Ages), bumped in Wired.Check, Exact.Check, DavidTracker, Verses (Gain / SpendForChorus), Kids.Act,
  `ScriptPower.AfterScriptRan`, and `RunStatsTracker` (combat singleton: Stratagem and Gear plays, plus a stack of whose card or
  potion is resolving so the `HealCredit` prefix on `CreatureCmd.Heal` credits real healing to that player). David's gold is
  read from the run's MapPointHistory. `RunStats.Lines` builds the text (`SPIREBROTHERS-RUN_STATS.*` in powers.json).
  `Mechanics/RunStatsScreen.cs` patches `NGameOverScreen._Ready` (moves `%ScoreLineContainer` into an HBox with our column of
  vanilla `NScoreLine`s) and wraps `AnimateScoreLines` so ours slide in after. Untested in game.
- **Joshua ballad pass** (he doesn't like playing support, doesn't play Rocket League, and is a ballad guy): new
  **Heartstrings** debuff (`Powers/HeartstringsPower.cs`: any Song played makes the enemy lose HP equal to it, -1 per enemy
  turn; `HauntingMelodyPower` stops the decay) and **Held Note** (`VerseOrb.Held` + glow; `JoshuaTracker.AfterPlayerTurnStartEarly`
  holds every Verse before the guitar adds one; `Verses.ForChorus` counts Held ones twice, three times with `MagnumOpusPower`).
  Choruses now help Joshua or hit enemies, not ALL players. Starter Lend an Ear -> Serenade. Cut cards are **retired, not deleted**:
  set to `CardRarity.Basic` (the game never offers Basic cards in rewards, shops, transforms, or combat generation) so saved
  runs still load; any new random-card picker of ours must filter to Common/Uncommon/Rare (`Verses.AddRandomSongs` does).
  Full list in JOSHUA_CARDS.md. Untested in game.
- **Joshua D&D/Zelda pass** (his own favorites: Slay the Spire, 2D Zelda, D&D, board games): **Roll** keyword (`Mechanics/Dice.cs`,
  d20 on `Rng.CombatTargets`, speech bubble with the result; 1 = half, 20 = double via `JoshuaCard.RollAttack`/`RollMultiplier`;
  Roll attacks set CalculationBase AND ExtraDamage to the printed damage (upgrade both), since the game's CalculatedVar uses a
  multiplier of 0 outside combat and a base of 0 showed "0 damage" in the deck view),
  `AdvantagePower` / `LuckyPower` / `DungeonMasterPower`, relic `LuckyD20`. Zelda cards incl. the `MasterSword` token and Ballad of
  the Wind Fish (`Chorus(ctx, keepVerses: true)`). Joshua's reward pool is now 83 (23 / 40 / 20). Untested in game.

## Added 2026-10-27

- **Bible elites** (TODO.md #7b), one per brother: Goliath + Shield Bearer and the Lions' Den (Act 1), the Walls of Jericho and
  Alexander the Coppersmith (Act 2). Each encounter extends `Monsters/BibleElite.cs` (`LinesKey`, `IsHero`, `MakeToken`, `IsMainFoe`,
  `Speaker`; does the lines, seeded per round, never the run RNG). Its talking foe carries a `Powers/FoePower.cs` subclass
  (ChampionOfGath / DenOfLions / SevenTrumpets / Coppersmith) that hands the hero his token card and runs the opening on turn 1.
  `Banter` skips its combat-start line in these fights, calls `BibleElite.AfterDeath`, and skips its elite-win line if the hero lives.
  Helpers are vanilla `MinionPower` and listed first in `GenerateMonsters` so they stand in front. `Alexander.Summon` adds a minion
  mid-fight (no encounter scene, so it re-runs `NCombatRoom.PositionEnemies` by reflection and tweens the others over).
  The Wall's fallen state is `WallOfJericho.IsFallen` (the countdown power is removed at 0). Tokens: `SmoothStone`, `ShutTheirMouths`,
  `TrumpetBlast`, `SoundDoctrine`. Lines: monsters.json `SPIREBROTHERS-<LinesKey>.intro/heroIntro/heroReply/taunts/heroWin.N`.
- `PixelBody.AnimatedBody` options: `maxIntentHeight` (tall bodies; Goliath) and `tipOver: false` (lions, the Wall).
- Bubbles vs end screens: `Banter.Bubble` (every bubble in the mod goes through it) notes the time. `Banter.AfterCombatVictory` holds (`Cmd.Wait(..., ignoreCombatEnd: true)`) and `GameOverWaitPatch` (on `NRun.ShowGameOverScreen`) defers the game over screen until the
  last bubble has been up 3.5 s (`Banter.TakeClosingWait`). Jericho uses `GetCameraScaling() => 0.85f` (three bodies were off-screen).
- pixelgen: `Program.cs` renders every foe through `IFrameArt` (Rig ones wrapped in `RigFrames`); `Giants.cs`, `Foes.cs`, `Beasts.cs`.
  The Desktop copy of pixelgen doesn't have these.

## Added 2026-10-27 (later)

- **Reader's Shelf** (Daniel, 9 cards: Wheel of Time, Tom Clancy, Inheritance Cycle, Space Base) and **Board games** (Joshua, 7: Catan
  and Space Base), lists in DANIEL_CARDS.md / JOSHUA_CARDS.md. Untested in game. New powers: SpaceBase, TheWheelWeaves (vanilla
  `AfterShuffle` hook), DragonRider, LongestRoad, DeployedFleet (rolls the d20 every turn), RaceToForty (`DisplayAmount` = points).
  The Hunt for Red October and Charge Cube react while held via `BeforeSideTurnEnd` + `Pile.Type == PileType.Hand` (like vanilla Regret).
- `gen.ps1` must run under PowerShell 7 (`pwsh`); Windows PowerShell 5 misreads its emoji and fails to parse.

## Added 2026-10-27 (David and Tim expansion)

- David and Tim each went up to **88 reward cards** (25 / 43 / 20), matching Daniel (87) and Joshua (90). David got mostly
  Attacks (he had only 15), Tim mostly Powers (he had only 11). Lists: "Expansion" sections in DAVID_CARDS.md / TIM_CARDS.md. Untested in game.
- Hooks: `OverthinkingPower` is read in `DavidTracker.RantsThisTurn`; `RepeatLastCommandPower` makes `ScriptPower` replay each
  queued card extra times; `TotemOfUndyingPower` is an empty subclass of Daniel's `ImNotDoneYetPower` (given to any player via Share);
  Overnight Shipping pays through `TimGold.Spend` so Burning a Hole reacts.
- Tim's AutoCAD "Array" card is the class `ArrayCommand` (a class named `Array` would hide `System.Array`).
- Joshua's retired cards already use the titles "Kickoff" and "Demo", so Tim's are Speed Flip and Demolition.

## Added 2026-10-27 (Ancient cards)

- Dusty Tome cards built, one per brother (`CardRarity.Ancient`, untested in game): Rubber Duck Mode, Spreadsheet of Destiny,
  World Tour, All Hands on Deck. Archaic Tooth cards too: each starter (Percussive Maintenance, Here We Go Again, Sing Along,
  Dad Joke) implements BaseLib's `ITranscendenceCard`. Details in ANCIENT_CARDS.md.
- `gen.ps1` draws any card with `CardRarity.Ancient` as tall full-card art (598x842, big 1196x1684).
