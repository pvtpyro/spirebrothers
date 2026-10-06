# cardgen: placeholder card portraits

A PowerShell script that draws every card portrait: a colored background (brother color, tinted red for Attacks
and gold for Powers) with one big icon in the middle, white with a thick dark outline.

The icon is either an **emoji** (Windows' Segoe UI Emoji font, drawn as a white outline), or a **custom drawing**
for cards that reference a specific game or thing, like the Prawn Suit, the Payday clown mask, or a Rocket League
car flicking the ball.

## Files

- `glyphs.txt`: one line per card, `ClassName Icon`. The icon is an emoji, or `@Name` for a custom drawing.
- `icons.ps1`: the drawing helpers (ellipses, polygons, rounded boxes, pixel-art blocks, lines) and shared colors.
- `icons_daniel.ps1`, `icons_helldivers.ps1`, `icons_david.ps1`, `icons_joshua.ps1`, `icons_tim.ps1`: the custom
  drawings, one per card, in `$CustomIcons['Name']`. Each is a list of parts drawn back to front in the 1000 x 760
  picture: `Part` (outlined shape), `Plain` (shape with no outline, for text), `Detail` (a dark line).
  The Rocket League car and ball (`RLCar`, `RLBall`) live in `icons_joshua.ps1` and Tim uses them too.
- `gen.ps1`: the script that renders everything.

## Running it

From this folder, in PowerShell:

```
.\gen.ps1 -CardsOnly                  # every card, straight into SpireBrothers/images/card_portraits/
.\gen.ps1 -Only Chores                # just one card
.\gen.ps1 -Preview -Only Chores       # one card into .\preview\ instead, to look before replacing
```

Always use `-CardsOnly` or `-Only`. Without them the script also redraws relics, powers, potions, the character
select buttons, map markers and the Age badges, and some of those have been replaced since (pixel-art select
buttons, the pizza and sunglasses map markers).

Then rebuild the mod. When real card art exists for a card, save it over that card's two files
(`card_portraits/<id>.png`, 250 x 190, and `card_portraits/big/<id>.png`, 1000 x 760) and don't re-run the script
over it.
