# Spire Brothers

A Slay the Spire 2 character mod. Version 0.1.0 adds **Daniel, the Nerd Who Nerds Wrong**.
David (The Min-Maxer) and Joshua (The Musician) are coming next.

## What's in v0.1.0

- Daniel: 75 HP, uses the Defect's model as a placeholder.
- 24 cards: starter deck, Logic/Hands/Wired, Stratagems, Share (co-op), Diligent, the Factory, and Monkey Island Insult/Comeback cards.
- 4 relics: And You Know What? (starter), Trusty Multimeter, Rubber Chicken with a Pulley, Never Paid a Mechanic.
- 5 powers: Insulted, Wire Up the House, The Factory Must Grow, Pair Programming, Nerds Wrong.

The code compiles against BaseLib 3.4.7 and the game's reference assemblies, but it has **not been run in-game yet**.
Expect some first-launch fixes.

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

Anything missing falls back to the template placeholder art.

## Where things live

- `SpireBrothersCode/Character/` Daniel and his card/relic/potion pools
- `SpireBrothersCode/Cards/Daniel/` one file per card
- `SpireBrothersCode/Mechanics/` turn tracking, Wired, Stratagem combos
- `SpireBrothersCode/Powers/`, `SpireBrothersCode/Relics/`
- `SpireBrothers/localization/eng/` all card/relic/power text
- `DESIGN.md` the design notes for all three brothers


## give yourself all cards for testing

- type backtick to open command
- type `unlock all`