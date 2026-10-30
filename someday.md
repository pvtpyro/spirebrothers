# Someday: subclasses on the character select screen

Idea (2026-10-29): pick a base character on the select screen, then pick a subclass (or outfit) from a popup.
First trial: Brandi. Her class ideas are already in `future-notes.md` (priest bubble, druid HoTs, hunter pets, warlock pets).
Nothing is built yet.

## The plan: every subclass is its own character

Each variant (say `BrandiPriest`, `BrandiHunter`) is registered as a normal `BrotherCharacter`, with its own cards,
starting deck, relic, and art. The select screen shows only one button per base character, and clicking it opens a picker.

Why separate characters: the co-op lobby already syncs "which character did you pick", and saved runs already
remember it. The picker just chooses one of those characters, so co-op and saves need no new code.

1. **Register each variant** as its own character (own pools, Basics, starter relic).
2. **Hide the variants from the grid.** Patch the select screen to remove their buttons after they're built.
3. **The base button opens a picker** instead of selecting: a small window with 2-3 variant cards (portrait, name,
   one-line pitch).
4. **Picking a variant calls the game's own select function** with that character. The big portrait, description,
   lobby sync, and Embark all stay vanilla.

First step when starting: decompile the character select screen (`sts2.dll`, see CLAUDE.md tooling notes) and find
the functions that build the buttons and select a character. That's the one unknown.

## Click popup vs. hover select

- **Click popup (recommended):** simple, works with a controller and Steam Deck, room for descriptions.
- **Hover select:** variant icons fan out above the button. Slick but fiddly (the mouse crosses other buttons on
  its way up and closes it; no controller support), so it would need a click fallback anyway.
- **Combination:** hover shows the variant icons as a preview, click opens the picker.

## Watch out for

- **Random character button** can pick a hidden variant directly. Filter it out, or allow it for fun.
- **Per-character extras multiply.** Each variant needs Architect dialogue in `ancients.json` (the analyzer fails the
  build without it), a select background, an icon, and frames. Most can be shared with the base character.
- **Code that checks for a specific character** (Duo cards' `Partner`, Family Bonds, Banter, events,
  `BrothersInArms.IsBrother`) compares character types. Give the variants a shared base class or interface
  ("this is a Brandi") and make those checks use it. Plan this before building.
- **Run history and the compendium** will list each variant as its own character. Probably fine.

## Outfits (cosmetic only)

Same picker, but the variants differ only in art. They could be variant characters too (simplest, synced for free),
or one character plus a saved per-player choice (like `Ages.RunAge`), where `BrotherCharacter` loads
`frames/<outfit>/...` and falls back to the default frames. pixelgen would draw each outfit.

## Alternatives considered

- **Custom buttons on the select screen with our own network sync:** the choice wouldn't reach the other players
  unless we send it ourselves through the game's network messages. Most fragile option.
- **Choose at run start** (a Stimm Lab-style `FromChooseACardScreen` on turn 1 or an opening event), saved per player
  like Ages; rewards and shops filtered by class the way `DuoCards` filters Duos. Safe, but less fun than picking
  on the select screen.
- **Plain separate entries on the grid** ("Brandi", "Brandi (Medic)"): no risk, but crowds the screen.

## Effort

The picker is small (one patch plus a popup; about a day plus testing in game). The real work is content: each
subclass needs its own card set (30-40 cards) to feel different.
