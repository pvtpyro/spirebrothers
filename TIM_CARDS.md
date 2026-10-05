# Tim, The Draftsman: card plan (draft for review)

Timothy, the oldest brother. Married with 8 kids. A draftsman at a civil engineering firm, where he
writes Lua add-ons for AutoCAD. Loves, in order: Rocket League, Age of Empires, Splinter Cell, airsoft,
and Legos. Old nickname "Tidbit" (he doesn't love it, so it appears exactly once).

**Built 2026-10-04, untested in game.** Numbers below match the code. Each Age Up card advances one Age (Imperial Age jumps straight to Imperial). Cut, rename, or rewrite anything, especially to add inside jokes.
Numbers are first-pass balance based on vanilla (Strike 6, Defend 5).

## His mechanics

| Keyword | Rule |
|---|---|
| **Kids** | A counter on Tim, **max 8** (of course). At the end of your turn, **each Kid deals 1 damage to a random enemy.** Cards add Kids, and payoffs scale with how many you have. Shown on his tracker, like Daniel's Train of Thought. |
| **Script** | Lua automation. **At the start of your next turn, this card's effect happens again** for free (it is not played again). Write it once, let it run. |
| **Age** | Age of Empires. Every combat starts in the **Dark Age**. **Age Up** cards advance it: Feudal → Castle → Imperial. Some cards get bonuses at later Ages. |

Shared mechanics he also uses: **Insulted** (Monkey Island, from Daniel) through Dad Jokes, and **Share**.

## Starting deck (10 cards) and relic

- 4 Strike, 4 Defend
- **Dad Joke** (Basic Attack, 1): Deal 5 damage. Apply 1 Insulted. *(Daniel's Comebacks love this in co-op.)*
- **Lua Script** (Basic Skill, 1): Gain 4 Block. **Script.**
- **Starter relic: Family Minivan.** Start each combat with 2 Kids.

Legend: 👶 Kids, 📜 Script, 🏰 Age, 🗡️ Monkey Island (Insult / Comeback; Dad Joke in his starting deck is an insult too).

## Common (20)

| # | Name | Cost / Type | Effect | Theme |
|---|---|---|---|---|
| 1 | 👶 Family Photo | 1 Skill | Gain 5 Block. Gain 1 Kid. | Family |
| 2 | 👶 Chores | 1 Skill | Gain 2 Block for each Kid. | Family |
| 3 | 👶 Carpool Line | 1 Attack | Deal 6 damage. If you have 4+ Kids, deal 4 more. | Family |
| 4 | 👶 Step on a Lego | 1 Attack | Deal 8 damage. Apply 1 Vulnerable. | Legos + kids |
| 5 | 📜 Hotkey | 0 Skill | Draw 1 card. **Script.** | AutoCAD |
| 6 | 📜 Polyline | 1 Attack | Deal 5 damage. **Script.** | AutoCAD |
| 7 | 📜 Batch Process | 1 Skill | Gain 5 Block. **Script.** | Lua |
| 8 | 🏰 Villager | 1 Skill | Gain 3 Block. Next turn, gain 1 Energy. | Age of Empires |
| 9 | 🏰 Scout Rush | 1 Attack | Deal 7 damage. **Feudal Age or later:** draw 1 card. | Age of Empires |
| 10 | 🏰 Palisade Wall | 1 Skill | Gain 7 Block. **Castle Age or later:** gain 3 more. | Age of Empires |
| 11 | 🏰 Feudal Age | 1 Skill | **Age Up.** Gain 4 Block. | Age of Empires |
| 12 | Air Dribble | 1 Attack | Deal 3 damage 3 times. | Rocket League |
| 13 | Fifty-Fifty | 1 Attack | Deal 8 damage. 50% chance: deal 8 more. | Rocket League |
| 14 | Rotate Back | 1 Skill | Gain 8 Block. | Rocket League |
| 15 | Night Vision | 1 Skill | Scry 4. Draw 1 card. | Splinter Cell |
| 16 | Takedown | 1 Attack | Deal 8 damage. If the enemy isn't attacking this turn, deal 6 more. | Splinter Cell |
| 17 | BB Spray | 1 Attack | Deal 2 damage to a random enemy 5 times. | Airsoft |
| 18 | Call Your Hits | 1 Skill | Gain 6 Block. Apply 1 Weak. | Airsoft (honor system) |
| 19 | **Tidbit** | 0 Skill | Draw 1 card. Gain 2 Block. *(just a little bit)* | His old nickname, used once |
| 20 | Morning Coffee | 0 Skill | Gain 1 Energy. Exhaust. | Dad of eight |

## Uncommon (36)

### Kids
| # | Name | Cost / Type | Effect | Theme |
|---|---|---|---|---|
| 21 | 👶 Bedtime Story | 1 Skill | Gain 1 Kid. Draw 1 card. | Family |
| 22 | 👶 Family Game Night | 1 Skill | ALL players gain 2 Block per Kid. | Family / co-op |
| 23 | 👶 Honey-Do List | 1 Skill | Your Kids act right now (deal their end-of-turn damage). | Married life |
| 24 | 👶 Minivan Ram | 2 Attack | Deal 10 damage, +2 per Kid. | Family |
| 25 | 👶 Big Family | 1 Power | At the end of your turn, also gain 1 Block per Kid. | Family |
| 26 | 👶 Lego Minefield | 1 Skill | ALL enemies lose HP equal to your Kids. | Legos + kids |
| 27 | 👶 Diaper Duty | 1 Skill | Exhaust a Status or Curse in your hand. Gain 1 Kid. | Dad of eight |
| 28 | 👶 Date Night | 1 Skill | Gain 12 Block. Your Kids don't act this turn (they're at Grandma's). | Married life |

### Script (AutoCAD and Lua)
| # | Name | Cost / Type | Effect | Theme |
|---|---|---|---|---|
| 29 | 📜 Lua Add-On | 1 Power | Whenever a Script effect happens, deal 2 damage to a random enemy. | Lua |
| 30 | 📜 Command Line | 1 Skill | Draw 2 cards. **Script.** | AutoCAD |
| 31 | 📜 Revision Cloud | 1 Skill | Gain 7 Block. **Script.** | AutoCAD |
| 32 | 📜 Dimension Line | 1 Attack | Deal 6 damage. **Script.** | AutoCAD |
| 33 | 📜 Xref | 1 Skill | Choose a card in your hand. It gains **Script** this combat. | AutoCAD |
| 34 | 📜 Snap to Grid | 0 Skill | Your next card this turn has **Script.** | AutoCAD |
| 35 | 📜 Plot Sheet | 2 Attack | Deal 12 damage. **Script.** | AutoCAD (printing) |
| 36 | 📜 Survey Stakes | 1 Skill | Apply 2 Vulnerable. **Script.** | Civil engineering |
| 37 | 📜 Grading Plan | 1 Skill | Gain 5 Block. Scry 2. **Script.** | Civil engineering |

### Age of Empires
| # | Name | Cost / Type | Effect | Theme |
|---|---|---|---|---|
| 38 | 🏰 Castle Age | 1 Skill | **Age Up.** Draw 2 cards. | Age of Empires |
| 39 | 🏰 Trebuchet | 2 Attack | Deal 14 damage. **Castle Age or later:** hits ALL enemies. | Age of Empires |
| 40 | 🏰 Wololo | 1 Skill | An enemy loses 2 Strength. You gain 2 Strength this turn. *(conversion!)* | Age of Empires |
| 41 | 🏰 Town Center | 2 Power | At the start of your turn, gain 4 Block. | Age of Empires |
| 42 | 🏰 Petard | 1 Attack | Deal 18 damage. Exhaust. | Age of Empires |
| 43 | 🏰 Monk | 1 Skill | Heal 3. Remove 1 stack of a random debuff. Exhaust. | Age of Empires |
| 44 | 🏰 Paladin | 2 Attack | Deal 12 damage. **Imperial Age:** deal 8 more. | Age of Empires |

### Rocket League, Splinter Cell, airsoft, Legos
| # | Name | Cost / Type | Effect | Theme |
|---|---|---|---|---|
| 45 | Flip Reset | 1 Skill | Draw 2 cards. Your next Attack this turn costs 0. | Rocket League |
| 46 | Musty Flick | 1 Attack | Deal 9 damage. If it's the last card in your hand, deal 9 more. | Rocket League |
| 47 | Bump | 0 Attack | Deal 3 damage. Apply 1 Weak. | Rocket League |
| 48 | Overtime | 2 Skill | Next turn, gain 2 Energy and draw 2 cards. Exhaust. | Rocket League |
| 49 | 🗡️ Hi Hungry, I'm Dad | 1 Attack | **Comeback.** Deal 5 damage, +3 for each Insulted on the target, then remove it. | Comeback (dad joke) |
| 50 | 🗡️ Not Mad, Just Disappointed | 1 Skill | Apply 2 Insulted and 1 Weak. | Insult (dad of eight) |
| 51 | Lights Out | 1 Skill | Gain 4 Block. Apply 2 Weak to ALL enemies. | Splinter Cell |
| 52 | Split Jump | 1 Skill | Gain 6 Block. Next turn, gain 6 Block. | Splinter Cell |
| 53 | 🗡️ Back in My Day | 0 Skill | Apply 1 Insulted to ALL enemies. | Insult (oldest brother) |
| 54 | Ghillie Suit | 1 Power | Gain 3 Plating. | Airsoft |
| 55 | Speedsoft | 0 Skill | Draw 2 cards, then discard 1. | Airsoft |
| 56 | Instruction Manual | 1 Skill | Choose a card from your draw pile and put it into your hand. | Legos |

## Rare (16)

| # | Name | Cost / Type | Effect | Theme |
|---|---|---|---|---|
| 57 | 👶 **The Whole Crew** | 2 Skill | Gain Kids until you have 8. Exhaust. | All eight of them |
| 58 | 👶 Proud Dad | 2 Power | Your Kids deal 2 damage each instead of 1. | Family |
| 59 | 👶 Family Reunion | 2 Skill | ALL players gain 3 Block per Kid. Exhaust. | Family / co-op |
| 60 | 👶 Lego Masterpiece | 3 Attack | Deal 4 damage per Kid to ALL enemies. Exhaust. | Legos (built with the kids) |
| 61 | 📜 Automation Suite | 3 Power | The first card you play each turn has **Script.** | Lua add-ons |
| 62 | 📜 while true do | 1 Skill | Gain 3 Block. Draw 1 card. Its **Script** repeats every turn for the rest of combat. Exhaust. | Lua (the infinite loop) |
| 63 | 📜 Markup Pass | 1 Skill | Upgrade ALL cards in your hand for this combat. Exhaust. | Drafting (redlines) |
| 64 | 🏰 Imperial Age | 2 Power | **Age Up** to Imperial. Gain 1 Strength and 1 Dexterity. | Age of Empires |
| 65 | 🏰 Wonder | 3 Power | At the start of your 5th turn after playing this, deal 60 damage to ALL enemies. | Age of Empires (wonder victory) |
| 66 | 🏰 Castle | 2 Power | At the start of your turn, gain 5 Block. At the end of your turn, deal 3 damage to ALL enemies. | Age of Empires |
| 67 | 🏰 Mass Conversion | 2 Skill | ALL enemies lose 3 Strength. You gain that much Strength this turn. Exhaust. | Age of Empires (monk army) |
| 68 | Grand Champion | 2 Power | Your Attacks deal 3 extra damage. | Rocket League (his #1) |
| 69 | 🗡️ The Last Word | 2 Attack | **Comeback.** Deal 8 damage to ALL enemies, +4 for each Insulted on each, then remove their Insulted. | Comeback (dad always gets the last word) |
| 70 | Mark and Execute | 1 Attack | Can only be played if you've played 3+ cards this turn. Deal 12 damage to ALL enemies. | Splinter Cell |
| 71 | Ghost Run | 2 Skill | Gain 1 Intangible (take only 1 damage per hit until your next turn). Exhaust. | Splinter Cell |
| 72 | Full Auto | X Attack | Deal 3 damage to a random enemy 3X times. | Airsoft |

## Wife Aggro (shared joke with Daniel)

Both brothers have an uncommon called **Wife Aggro**; same title, different card.

| Name | Cost / Type | Effect | Theme |
|---|---|---|---|
| 👶 Wife Aggro | 1 Skill (upgrade: 0) | *"Yes, dear."* Your Kids act twice at the end of this turn. Next turn, draw 1 fewer card. | Married life |

## Co-op links with his brothers

- **Dad Joke** applies Insulted, which Daniel's Comebacks cash in.
- **Rocket League** is his #1 game and Joshua plays it too (Joshua has What a Save!, Kickoff, Demo).
  A future "Brothers in Arms" bonus could connect their RL cards.

## Build notes (for Claude)

- **Kids**: a `KidsPower` counter (max 8) on Tim with an end-of-turn hook that hits random enemies using
  the run RNG. Proud Dad and Big Family read it. Date Night sets a skip flag for one turn.
- **Script**: when a Script card is played, queue its effect in a power; at the start of the next turn,
  run the effect again without paying or moving the card. Simplest approach: re-run the card's OnPlay
  logic on a fresh clone. Xref, Snap to Grid, Automation Suite, and while true do add Script dynamically.
- **Age**: a per-combat counter (0 Dark, 1 Feudal, 2 Castle, 3 Imperial) stored like Daniel's `TurnTracker`
  fields, with a tracker icon. Age bonuses use `AddExtraArgsToDescription` to show what's active.
- **Takedown** needs the enemy's intent; check the vanilla intent API before building.
- **Flip Reset** can use vanilla `FreeAttackPower`. **Ghost Run** uses `IntangiblePower`.
  **Ghillie Suit** uses `PlatingPower`. **Mark and Execute** and **Musty Flick** use playability or
  hand-count checks like Stratagems and Zero-Second Goal.
- **Full Auto** is X-cost, the same as David's X cards.
- **Wonder** needs a countdown power (vanilla `CountdownPower` may already do this).
