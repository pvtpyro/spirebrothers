# Ancient cards (draft, not built)

Vanilla gives every character two Ancient-rarity cards, and the brothers have none yet:

- **Dusty Tome card** (from Darv's Dusty Tome relic): a build-defining Power that bends a rule, usually with a catch.
  Vanilla: Corruption (Ironclad), Wraith Form (Silent), Biased Cognition (Defect), Forbidden Grimoire (Necrobinder),
  The Sealed Throne (Regent).
- **Archaic Tooth card** (from Orobas's Archaic Tooth relic): the "ascended" version of one starter card, which the
  relic swaps in. Vanilla: Bash -> Break, Neutralize -> Suppress, Unleash -> Protector.

Both are given upgraded. Ancient cards never show up in card rewards or random card generation, and they use the
full-art Ancient frame, so their art covers the whole card, not just the portrait window.

Until these are built, `Relics/DustyTomeFallback.cs` makes Dusty Tome give the brothers a random upgraded Rare instead,
and Archaic Tooth isn't offered to them at all.

## Daniel

| Card | Relic | Cost / Type | Text | Upgrade |
|---|---|---|---|---|
| **Full Stack** | Dusty Tome | 3 Power | Your cards count as both **Logic** and **Hands**. | Cost 2 |
| **Have You Tried Hitting It Harder?** | Archaic Tooth (Percussive Maintenance) | 1 Attack | Deal 12 damage. Apply 2 Vulnerable. **Wired:** Hit twice. | 16 damage |

- Full Stack: software engineer *and* the guy who fixes the car. Every card turns on Wired, and every Stratagem step
  matches whatever you play, so Stratagems become free finishers. No catch; the cost is the catch. Watch the
  Train of Thought tracker: a card counting as both should log as one step that matches either input.
- Hitting It Harder: the mechanic's ascended fix-it card, the way Bash becomes Break.

## David

| Card | Relic | Cost / Type | Text | Upgrade |
|---|---|---|---|---|
| **The Meaning of Life** | Dusty Tome | 2 Power | Your **Rant** bonuses count every Rant this combat, not just this turn. At the start of your turn, lose 1 HP. | Cost 1 |
| **Don't Get Me Started** | Archaic Tooth (Here We Go Again) | 1 Attack | Deal 10 damage. Apply 3 Ranted At. Counts as 2 Rants. **Rant:** +4 damage per Rant this turn. | 14 damage, +5 per Rant |

- The Meaning of Life: the existential crisis that never ends. Rants snowball across the whole fight instead of
  resetting each turn (`DavidTracker.RantsThisCombat` already exists). The HP loss is the existential dread.
- Don't Get Me Started: Here We Go Again, but he's really going now.

## Joshua

| Card | Relic | Cost / Type | Text | Upgrade |
|---|---|---|---|---|
| **Sold-Out Show** | Dusty Tome | 3 Power | **Chorus** cards don't spend your Verses. Whenever you play a Chorus card, Exhaust it. | Cost 2 |
| **Everybody Sing!** | Archaic Tooth (Sing Along) | 1 Skill | **Chorus:** ALL players gain 3 Block and heal 1 per Verse. | 4 Block per Verse |

- Sold-Out Show: his Corruption. Verses stay up (max 10), so every Chorus fires at full strength, but each one
  is a one-time encore. Pairs naturally with Encore / Bridge / Choir, which already change how Verses get spent.
- Everybody Sing!: Sing Along plus the healing that's core to his kit. Big in co-op, since it hits ALL players.

## Tim

| Card | Relic | Cost / Type | Text | Upgrade |
|---|---|---|---|---|
| **Automate Everything** | Dusty Tome | 3 Power | Every card you play has **Script**. | Cost 2 |
| **Did You Hear the One About...** | Archaic Tooth (Dad Joke) | 1 Attack | Deal 8 damage to ALL enemies. Apply 2 Insulted and 1 Weak to ALL enemies. | 11 damage, 3 Insulted |

- Automate Everything: the ultimate Lua add-on. Automation Suite (Rare) does this for the first card each turn;
  this does it for all of them. Powers still never get Script, and the replays never re-queue (both already true
  in `ScriptPower`), so it can't loop forever.
- Did You Hear the One About...: the dad joke so bad it hits the whole room. Sets up Comebacks for everyone in co-op.

## Build notes

- **Dusty Tome**: give the card `CardRarity.Ancient` in the brother's pool. Vanilla Dusty Tome then finds it on its
  own, and the fallback patch steps aside automatically. (BaseLib's `ITomeCard` works too, but isn't needed.)
- **Archaic Tooth**: vanilla's starter -> ascended map is a private property (`ArchaicTooth.TranscendenceUpgrades`)
  rebuilt on every read, so add our four pairs with a Harmony postfix on its getter. Dusty Tome already excludes
  the ascended cards, so the two relics never hand out the same card.
- Each card needs a `glyphs.txt` line for the placeholder art, and its loc keys.
