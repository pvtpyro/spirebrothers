# Ancient cards

Vanilla gives every character two Ancient-rarity cards:

- **Dusty Tome card** (from Darv's Dusty Tome relic): a build-defining Power that bends a rule, usually with a catch.
  Vanilla: Corruption (Ironclad), Wraith Form (Silent), Biased Cognition (Defect), Forbidden Grimoire (Necrobinder),
  The Sealed Throne (Regent).
- **Archaic Tooth card** (from Orobas's Archaic Tooth relic): the "ascended" version of one starter card, which the
  relic swaps in. Vanilla: Bash -> Break, Neutralize -> Suppress, Unleash -> Protector.

Dusty Tome's card comes upgraded; Archaic Tooth's is upgraded only if the starter card was. Ancient cards never show up in card rewards or random card generation, and they use the
full-art Ancient frame, so their art covers the whole card, not just the portrait window (`gen.ps1` draws them tall,
598x842 and 1196x1684, with the icon in the top third).

## Built: Dusty Tome cards (2026-10-27, untested in game)

| Brother | Card | Cost | Text | Code |
|---|---|---|---|---|
| Daniel | **Rubber Duck Mode** | 2 (1) | At the start of your turn, enter the first input of a random Stratagem in your hand for free. No Stratagem? Enter a random input. | `RubberDuckModePower` (after the draw) -> `TurnTracker.EnterFreeInput` |
| David | **Spreadsheet of Destiny** | 3 (2) | Exact also triggers with 1 energy left, and triggers an extra time. You can't gain extra energy. | `SpreadsheetOfDestinyPower`, read in `Exact`; `ModifyEnergyGain` -> 0 (the turn-start refill skips that hook) |
| Joshua | **World Tour** | 3 (2) | Whenever you play an Attack, gain a Verse. New Verses start Held. | `WorldTourPower`; `Verses.Gain` marks new orbs Held |
| Tim | **All Hands on Deck** | 2 (1) | At the start of your turn, gain a Kid. Your Kids deal 1 more damage per Age. | `AllHandsOnDeckPower`; `Kids.DamagePerKid` adds `Ages.Get` |

- Joshua's first pitch was "Verse cap 20", but Verses are orbs and the game hard-caps orb slots at 10, so it became
  "new Verses start Held" (each counts double for a Chorus, triple with Magnum Opus).
- Rubber Duck Mode's free input counts as the first entry of the turn, so Patch Panel (which only works on the first
  Logic/Handy entry) won't fire alongside it. It also makes Wired easier: any card of the other kind is Wired right away.
- Spreadsheet of Destiny stacks with Min-Max (3 triggers). It also blocks energy from teammates' Share cards and
  energy-next-turn effects.

With these in the pools, vanilla Dusty Tome picks them on its own and `DustyTomeFallback` steps aside.

### Unused Dusty Tome ideas (first draft)

- Daniel, **Full Stack** (3, Power): your cards count as both Logic and Handy.
- David, **The Meaning of Life** (2, Power): Rant bonuses count every Rant this combat; lose 1 HP each turn.
- Joshua, **Sold-Out Show** (3, Power): Choruses don't spend Verses; each Chorus card Exhausts.
- Tim, **Automate Everything** (3, Power): every card you play has Script.

## Built: Archaic Tooth cards (2026-10-27, untested in game)

| Brother | Card | Replaces | Cost / Type | Text | Upgrade |
|---|---|---|---|---|---|
| Daniel | **Have You Tried Hitting It Harder?** | Percussive Maintenance | 1 Attack (Handy) | Deal 12 damage. Apply 2 Vulnerable. **Wired:** Hit twice. | 16 damage |
| David | **Don't Get Me Started** | Here We Go Again | 1 Attack (Rant) | Deal 10 damage. Apply 3 Ranted At. **Rant:** +4 damage per Rant this turn. Counts as 2 Rants. | 14 damage, +5 per Rant |
| Joshua | **Everybody Sing!** | Sing Along | 1 Attack (Chorus) | **Chorus:** Gain 3 Block and deal 3 damage to ALL enemies per Verse. | 4 / 4 per Verse |
| Tim | **Did You Hear the One About...** | Dad Joke | 1 Attack (Insult) | Deal 8 damage to ALL enemies. Apply 2 Insulted and 1 Weak to ALL enemies. | 11 damage, 3 Insulted |

- Everybody Sing! was drafted as an ALL-players heal; changed to Block + damage since Joshua doesn't like playing support.
- Don't Get Me Started's double count is in `DavidTracker.AfterCardPlayed` (Soapbox still adds 1 on top for the first Rant).

## Build notes

- **Dusty Tome**: give the card `CardRarity.Ancient` in the brother's pool. Vanilla Dusty Tome then finds it on its
  own. (BaseLib's `ITomeCard` works too, but isn't needed.)
- **Archaic Tooth**: the starter card implements BaseLib's `ITranscendenceCard` (`GetTranscendenceTransformedCard()` returns
  the Ancient card); BaseLib patches it into vanilla's starter -> ascended map. Orobas only offers the Tooth if the starter
  is still in the deck. Dusty Tome excludes the ascended cards, so the two relics never hand out the same card.
- Each card needs a `glyphs.txt` line for the placeholder art, and its loc keys.
