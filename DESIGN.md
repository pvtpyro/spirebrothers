# Spire Brothers design notes

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
- Idea: **Brothers in Arms**, starter relics get a bonus when another brother is in the run. (Not built yet.)

## Daniel, The Nerd Who Nerds Wrong (built in v0.1.0)
Software engineer who can fix cars, hard-wired his whole house for internet (every room has ethernet), and does electrical work too. "And you know what?" Never gives up.
- Logic / Hands tags, **Wired** bonus when you mix them.
- **Stratagems** (Helldivers 2): play Logic/Hands in a set order to unlock.
- **Diligent**: grows every time it's played this combat.
- Games: Subnautica, Helldivers 2, Factorio, Darktide, Minecraft.
- Full card list (72 in the reward pool) is in DANIEL_CARDS.md.

## David, The Min-Maxer (built 2026-10-04)
Great with numbers, loves puzzles, loves hiking and camping. Always in the middle of an existential crisis, and whoever has to listen pays the price. Never spends money, not to save it, but because nothing interests him enough to buy, so it piles up. Took one road trip, Idaho to NJ and back.
- **Exact**: bonus if you end the card with exactly 0 energy. Card-draw bonuses happen next turn (you have no energy to play them now).
- **Hoard**: scales with unspent gold. Starter relic Old Wallet: Block per 20 gold at combat start.
- **Rant** (replaced teaching): Rant cards snowball the more of them you play in a turn, and wear down the listener (the enemy loses Strength, takes damage, or gets Weak). Delegate: ally's next attack doubles.
- **Bleed** (replaced Thorns): a debuff that hits hard at the end of the enemy's turn, then halves. Messing with David costs you.
- Games (what he does all day): Minecraft, Payday 2, Terraria, Satisfactory, ARK. The road trip gets one card: Idaho to Jersey and Back (rare power).
- Full card plan (draft): DAVID_CARDS.md.

## Joshua, The Musician (next)
Piano, guitar, vocals. Living in Japan, moving back to the US soon.
- **Verse / Chorus**: Song cards build Verses; Chorus cards spend them for effects on ALL players.
- Starter relic Well-Worn Guitar: start each combat with 1 Verse.
- **Check** (Archipelago, which he loves): find random items and send them to random players. Traps go to enemies.
- **Healing**: a big part of his kit, because it fits his personality. The team's support. Chorus heals on ALL players fit naturally (as Pastor already does).
- Games: PlateUp!, Terraria, Rocket League, Stardew Valley. Omiyage, What a Save!, Karaoke Night.
- Tribute cards: **Pastor** (all players heal and gain Block, Exhaust) and **Coming Home**.
- Full card plan (draft): JOSHUA_CARDS.md.

## Tim (Timothy), The Draftsman (oldest brother)
Married with 8 kids. Draftsman at a civil engineering firm; writes Lua add-ons for AutoCAD.
Loves, in order: Rocket League, Age of Empires, Splinter Cell, airsoft, Legos.
Old nickname "Tidbit": he doesn't like it, so it appears on exactly one card.
- **Kids** (max 8): each Kid deals 1 damage to a random enemy at the end of your turn.
- **Script** (Lua automation): the card's effect happens again at the start of your next turn.
- **Age** (Age of Empires): Dark → Feudal → Castle → Imperial; some cards get stronger at later Ages.
- Starter relic Family Minivan: start each combat with 2 Kids. Dad Jokes apply Insulted for Daniel's Comebacks.
- Full card plan (draft): TIM_CARDS.md.
