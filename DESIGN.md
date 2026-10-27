# Spire Brothers design notes

## The family
- Everyone but Daniel lives in Idaho (parents, David, Tim's family). Daniel and Brandi live farther south, and
  Grandma lives south of them, so a family trip to Grandma's drives down past Daniel's.
- Mom and Dad spend a lot of time on the lake: fishing and water sports.
- Thanksgiving: Dad deep-fries the turkey; Mom does all the other cooking, and she's the one who always does the dishes.
- Dad has a giant shop, and the boys end up out there helping with his projects. (He doesn't watch football.)
- The family goes hiking a lot together.
- Parents are fine to mention in cards, events, and banter.

## Shared
- **Share** (co-op): choose a player; in solo it targets you. Teammate draw/energy arrives next turn to keep multiplayer in sync.
- **Monkey Island**: Insult cards apply *Insulted*; Comeback cards deal bonus damage per stack, then clear it.
  One brother insults, another lands the comeback. Original insults only.
  Every brother has several insult cards but only about 2 Comebacks (Daniel, the Monkey Island hub, has 3),
  so in co-op anyone can set up a Comeback for whoever holds one.
- **Monkey-speak items** (built for Daniel 2026-10-04; add to each brother's pool as they're built, not vanilla's shared pool,
  since other characters can't use Insulted). The description is pure monkey-speak; a hover tip titled
  "Translation" gives the real effect. Original sounds only, no lines from the games.

  | Item | Type | Description (as shown) | Translation (hover) |
  |---|---|---|---|
  | Ook Ook Eek | Common potion | "Ook ook eek! Eek!" | Apply 3 Insulted to ALL enemies. |
  | Grog | Uncommon potion | (pirate drink, normal text) | Deal 12 damage to ALL enemies and apply 2 Insulted to ALL enemies. *(the planned Grog potion)* |
  | Monkey Business | Rare potion | "Eek? Ook ook. EEK!" | This turn, Comebacks don't remove Insulted. |
  | Monkey Phrasebook | Common relic | "Eek ook! Ook ook eek." | At the start of each combat, apply 1 Insulted to ALL enemies. |
  | Stone Monkey Head | Uncommon relic | "OOK." | The first time each enemy is Insulted in a combat, it also loses 1 Strength. |
  | Monkey Wrench | Rare relic | "Eek eek. Ook!" | Comeback cards deal 2 extra damage for each Insulted. *(also a nod to Daniel the mechanic)* |

  Build note: the translation is an extra hover tip (`new HoverTip(title, text)`, like Train of Thought's
  "This Turn" box). Comebacks read a shared helper so Monkey Business and Monkey Wrench apply to every
  brother's Comebacks.
- **Family Bonds** (built 2026-10-26, untested): brothers in the same party unlock bonuses. Every brother gets a
  Family Bonds icon on turn 1 (number = active bonds, hover lists them). They stack.
  | Who | Bond | Effect |
  |---|---|---|
  | Daniel + David | Heated Debate | David's Rants give the enemy 1 Insulted |
  | Daniel + Joshua | Wired for Sound | Daniel's first Wired each turn: Joshua gains 1 Verse |
  | Daniel + Tim | Code Review | Tim's first Script effect each turn: Daniel gets a Gear in hand |
  | David + Joshua | Royalties | Joshua's Chorus: David gains 5 Gold |
  | David + Tim | Family Loan | Age Up costs 25 less Gold; David gains 10 Gold after each win |
  | Joshua + Tim | Kids' Choir | Joshua's Chorus: all of Tim's Kids hit a random enemy |
  | Any 3 | Group Chat | Turn 1: ALL players gain 1 Energy |
  | All 4 | Nobody Gets Left Behind | Each brother's first death each combat: heal 10 instead |
- **Relics added 2026-10-25** (untested in game). Shared ones are `[Pool]`ed to Daniel and added to the others in `MainFile.ShareMonkeyIslandItems()`.
  - Shared: Blown Cartridge (C, draw 2 on turn 2), Party Headset (C, first Share each turn: you + target gain 3 Block; not in Tim's pool, he has no Share cards),
    Shared Steam Library (U, random card from another brother's pool, free this turn), Brothers in Arms (R, ALL players +1 Str/Dex per Spire Brother in the party).
  - David: Pocket Knife (C, +1 Bleed), Graphing Calculator (C, first Exact each turn: 4 Block), Savings Account (U, 5% Gold interest after combat, max 20),
    Soapbox (U, first Rant each turn counts as 2), Payday Mask (R, 20 Gold per enemy death).
  - Joshua: Lucky Pick (C, 2 Verses at combat start), Omamori (C, any player who heals in combat gains 3 Block), Capo (U, Chorus counts +1 Verse),
    Spoiler Log (U, first Check each combat finds 2 items), Gold Record (R, Chorus: ALL players heal 2).
  - Tim: Loose Lego Brick (C, 2 Thorns), Juice Boxes (C, +1 Kid), Garrisoned Relic (U, 12 Gold after combat), Full Boost (U, turn 1: +1 energy, draw 2),
    Startup Script (R, first 2 non-Power cards each combat gain Script).
  - Daniel: Patch Panel (U, first Logic/Hands card each turn also counts as the other), Super Destroyer (R, random Stratagem at combat start).

## Daniel, The Nerd Who Nerds Wrong (built in v0.1.0)
Software engineer who can fix cars, hard-wired his whole house for internet (every room has ethernet), and does electrical work too. "And you know what?" Never gives up.
- Logic / Hands tags, **Wired** bonus when you mix them.
- **Stratagems** (Helldivers 2): play Logic/Hands in a set order to unlock.
- **Diligent**: grows every time it's played this combat.
- Games: Subnautica, Helldivers 2, Factorio, Darktide, Minecraft.
- Full card list (72 in the reward pool) is in DANIEL_CARDS.md.

## David, The Min-Maxer (built 2026-10-04)
Great with numbers, loves puzzles, loves hiking and camping. Goes on a short walk or hike with Dad every Sunday, and calls Mom a lot to chat or vent. Always in the middle of an existential crisis, and whoever has to listen pays the price. Never spends money, not to save it, but because nothing interests him enough to buy, so it piles up. Took one road trip, Idaho to NJ and back.
- **Exact**: bonus if you end the card with exactly 0 energy. Card-draw bonuses happen next turn (you have no energy to play them now).
- **Hoard**: scales with unspent gold. Starter relic Old Wallet: Block per 20 gold at combat start.
- **Rant** (replaced teaching): Rant cards snowball the more of them you play in a turn, and wear down the listener (the enemy loses Strength, takes damage, or gets Weak). Delegate: ally's next attack doubles.
- **Support** (2026-10-25, David loves playing support): his gold, math, and rants help the team. Old Wallet gives ALL players Block, about 22 cards can target a teammate (Share), and Talk Them Down / Ranted At soften enemy hits for everyone. He is the team's strategist and financier; Joshua is the healer.
- **Bleed** (replaced Thorns): a debuff that hits hard at the start of the enemy's turn (before it acts, like Poison), then halves. Messing with David costs you.
- Games (what he does all day): Minecraft, Payday 2, Terraria, Satisfactory, ARK. The road trip gets one card: Idaho to Jersey and Back (rare power).
- Full card plan (draft): DAVID_CARDS.md.

## Joshua, The Musician (next)
Piano, guitar, vocals. Lived in Japan for a while; now back home in the US.
- **Verse / Chorus**: Song cards build Verses; Chorus cards spend them for effects on ALL players.
- Starter relic Well-Worn Guitar: gain 1 Verse at the start of each turn (Signature Guitar: 2). Starter Basic card Lend an Ear (Share Block).
- **Check** (Archipelago, which he loves): find random items and send them to random players. Traps go to enemies.
- **Healing**: a big part of his kit, because it fits his personality. The team's support. Chorus heals on ALL players fit naturally (as Pastor already does).
- Games: PlateUp!, Terraria, Rocket League, Stardew Valley. Omiyage, What a Save!, Karaoke Night.
- Tribute cards: **Pastor** (all players heal and gain Block, Exhaust) and **Coming Home**.
- Full card plan (draft): JOSHUA_CARDS.md.

## Tim (Timothy), The Draftsman (oldest brother)
Married with 8 kids. Draftsman at a civil engineering firm; writes Lua add-ons for AutoCAD.
Loves, in order: Rocket League, Age of Empires, Splinter Cell, airsoft, Legos.
Loves to spend money: he has to have the best of everything, and gold burns a hole in his pocket (the opposite of David, who never spends). Age Up costing gold fits him.
Old nickname "Tidbit": he doesn't like it, so it appears on exactly one card.
- **Kids** (max 8): each Kid deals 1 damage to a random enemy at the end of your turn.
- **Script** (Lua automation): the card's effect happens again at the start of your next turn.
- **Age** (Age of Empires): Dark → Feudal → Castle → Imperial; some cards get stronger at later Ages.
- Starter relic Family Minivan: start each combat with 2 Kids. Dad Jokes apply Insulted for Daniel's Comebacks.
- Full card plan (draft): TIM_CARDS.md.
