# cardgen: placeholder card portraits

A PowerShell script that draws every card portrait: a colored background (brother color, tinted red for Attacks
and gold for Powers), a big white icon for what the card is, and a small **badge** in the bottom-right corner for
the game or hobby it references (a monkey for Monkey Island insults and comebacks, a helmet for Helldivers, a
castle for Age of Empires, and so on).

## Changing a card's icons

Edit `glyphs.txt`. Each line is:

```
ClassName MainIcon Badge
```

- `ClassName` is the card's C# class name (the file name in `SpireBrothersCode/Cards/<Brother>/`).
- `MainIcon` and `Badge` are emoji. The script draws their outline in white, so any emoji in Windows' Segoe UI
  Emoji font works. Leave the badge off for no badge.

Badges in use: 🐒 Monkey Island, 🪖 Helldivers 2, 💀 Darktide, ⚙ Factorio, 🐠 Subnautica, ⛏ Minecraft, 🤡 Payday 2,
🦖 ARK, 🌳 Terraria, 🏗 Satisfactory, ⚽ Rocket League, 🐔 Stardew Valley, 🍳 PlateUp!, 🏝 Archipelago, 🗾 Japan,
🏰 Age of Empires, 🥽 Splinter Cell, 🎯 airsoft, 🧱 Legos, 📐 AutoCAD / drafting, 🌙 Lua, 😩 existential crisis,
🥾 hiking and camping, 🔢 numbers and puzzles, ⚡ electrician, 🔧 cars and tools, 💻 software, 🎵 music,
💍 married life, 👶 family, 📦 moving, 📜 National Treasure, 🤝 co-op.

## Running it

From this folder, in PowerShell:

```
.\gen.ps1 -CardsOnly                  # every card, straight into SpireBrothers/images/card_portraits/
.\gen.ps1 -CardsOnly -Only Chores     # just one card
.\gen.ps1 -Preview -Only Chores       # one card into .\preview\ instead, to look before replacing
```

Always use `-CardsOnly` (or `-Only`). Without it the script also redraws relics, powers, potions, the character
select buttons, map markers and the Age badges, and some of those have been replaced since (pixel-art select
buttons, the pizza and sunglasses map markers).

Then rebuild the mod. When real card art exists for a card, just save it over that card's two files
(`card_portraits/<id>.png`, 250 x 190, and `card_portraits/big/<id>.png`, 1000 x 760) and leave it out of future
runs with `-Only`.
