# Spire Brothers

A Slay the Spire 2 character mod with four playable brothers, built for co-op:

- **Daniel, the Nerd Who Nerds Wrong**: Logic and Hands cards, Wired, Stratagems, Diligent, the Factory.
- **David, the Min-Maxer**: Exact, Hoard, Rants, and Bleed.
- **Joshua, the Musician**: Songs build Verses, Choruses spend them; healing, Share, and Archipelago Checks.
- **Tim, the Draftsman**: Kids, Script, and Age of Empires Ages that last the whole run.

Plus shared Monkey Island Insult/Comeback cards, relics, and potions. Every card is listed in `DANIEL_CARDS.md`,
`DAVID_CARDS.md`, `JOSHUA_CARDS.md`, and `TIM_CARDS.md`. Card and relic art is still placeholder.

Daniel has been played; David, Joshua, and Tim are new and still being playtested, so expect some fixes.

## One-time setup (Windows)

1. Install the **.NET 9 SDK**: https://dotnet.microsoft.com/download/dotnet/9.0
2. Install **MegaDot 4.5.1 (mono)**. This is Mega Crit's Godot build; the game won't load a .pck from a newer Godot.
   See the BaseLib mod template wiki: https://github.com/Alchyr/ModTemplate-StS2/wiki/Setup
3. Open `Directory.Build.props` and set `GodotPath` to your MegaDot .exe.
   If the game isn't found automatically, uncomment and set `Sts2Path` too.
4. Install BaseLib into the game: download `BaseLib.dll`, `BaseLib.pck`, `BaseLib.json` from
   https://github.com/Alchyr/BaseLib-StS2/releases and put them in `Slay the Spire 2/mods/BaseLib/`.

## Build and install

From this folder:

```
dotnet publish -c Release
```

That builds the .dll and exports the .pck (art + localization) straight into
`Slay the Spire 2/mods/SpireBrothers/`. Launch the game, accept the mod warning, and Daniel appears on character select.

For quick code-only changes, `dotnet build` copies just the .dll.

## Playing with friends

Everyone in a multiplayer lobby should have the same version of Spire Brothers and BaseLib installed.

## Adding art

Drop PNGs named after each card/relic/power ID (lowercase snake case) into:

- Cards: `SpireBrothers/images/card_portraits/<id>.png` (250x190) and `.../big/<id>.png` (1000x760)
  e.g. `rubber_duck_debugging.png`
- Relics: `SpireBrothers/images/relics/<id>.png`, `<id>_outline.png`, and `big/<id>.png`
- Powers: `SpireBrothers/images/powers/<id>.png` and `big/<id>.png`
- TIM: His Age shows above his head as a number: 1 Dark, 2 Feudal, 3 Castle, 4 Imperial. For your graphics later, save files as SpireBrothers/images/ages/age0.png



Anything missing falls back to the template placeholder art.

## Where things live

- `SpireBrothersCode/Character/` each brother and his card/relic/potion pools
- `SpireBrothersCode/Cards/<Brother>/` one file per card
- `SpireBrothersCode/Mechanics/` each brother's mechanics (Wired, Exact, Verses, Kids, Ages, ...)
- `SpireBrothersCode/Powers/`, `SpireBrothersCode/Relics/`
- `SpireBrothers/localization/eng/` all card/relic/power text
- `DESIGN.md` the design notes for all four brothers


## testing commands

#### General
- type backtick to open command
- type `unlock all`

#### David
`card SPIREBROTHERS-A_GLOVE_FRY`, `card SPIREBROTHERS-ALL_IN`, `card SPIREBROTHERS-YOU_BREAK_IT_YOU_BUY_IT`, `card SPIREBROTHERS-DEEP_SIGH`. Type card `SPIREBROTHERS-` and press Tab to browse everything. `gold 200`