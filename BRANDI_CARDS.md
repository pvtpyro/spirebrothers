# Brandi, Private Pyro: card plan (built 2026-10-28, untested in game)

Brandi is Daniel's wife, so she joins as **family: the Sister-in-Law** (select description "Sister-in-Law"), with her
own look apart from the brothers and the friends: red select button and frames, red map marker, red card art.

Who she is: an introvert who hates talking, so her face says everything. Army veteran (OIF/OEF), V Corps, the first
into Iraq (Camp Victory was named for V Corps). Signal by job (telecommunications) but placed in an MI unit. Nicknamed
**Private Pyro** after she accidentally started a big fire in AIT; she still loves fire because it's pretty. Some call
her **Sparky**: she shocks people constantly (one shoulder tap at work went through Daniel's earbuds and killed his sound
card, and she could power down his PC just by touching it). She always
wanted to be a sniper. Web developer since 1993. Loves all animals:
- **Peppi**: short-haired tabby, orange and silver like Sandi but blended (no clean split), and no white anywhere. Can't really meow; she goes "beep boop" like a robot.
  Has a vendetta against toilet paper (the bathroom doors stay closed).
- **Felicity**: Peppi's littermate, long gray hair that she leaves everywhere.
- **Lily Bean**: her new pug, traditional fawn, pink collar.
- **Yertle**: her 22-year-old red-eared slider. She's mean: she had 3 brothers, and she ate them.
- In WoW, her two alpaca hunter pets, **Al** and **Pacino**.
- **Pugsley** (fawn pug, red collar, bone-shaped tag) and **Sandi** (tabby with a split face: one side orange tabby, the
  other silver tabby, split right down her nose; white socks and a white chest patch), pets she got with her late husband, passed recently. Reference
  photos: `photos/pugsley.png`, `photos/sandi.jpg` (`photos/` is never packed into the mod). Heavy
metal and reggae. Collects little yellow smiley faces. Favorite color red. Has traveled all over the world. Plays WoW
since launch, every class, most often **Feral Druid, Discipline Priest, and Hunter**.

**Built 2026-10-28, untested in game.** The full set: 5 Basics, **88 reward cards (25 Common / 43 Uncommon / 20 Rare)** to match the
brothers, 7 Duo cards (one with everyone, two with Daniel), 2 Ancient cards, and 1 token. Every card lists its upgrade.
Numbers are first-pass balance based on vanilla (Strike 6, Defend 5); per the balance rule, weak cards get buffed and
only runaway scaling gets capped. Every class name below was checked against every card in the mod.

## Her mechanics

| Keyword | Rule |
|---|---|
| **Ember** | A debuff on an enemy. At the start of its turn it takes damage equal to its Ember, then its Ember **grows by 1** (Poison shrinks; fires spread). Growth stops at 10, but cards can still push it higher. When an enemy with Ember dies, its Ember jumps to another random enemy (run RNG). On a **player** (only Accidental Fire does this) it doesn't grow: it goes down by 1 each turn, like Poison. |
| **Aim** | A buff on Brandi that never wears off on its own. **Shot** attacks deal bonus damage per Aim (+3 unless the card says otherwise), then remove all of it. Most Aim comes from quiet Skills: patience, then one big hit. |
| **Intel** | MI work. A debuff on an enemy: Attacks from **any** player deal +Intel damage to it. Removed at the end of the round. Made for co-op. |
| **Static** | Sparky. A buff on Brandi that stays until it's used. Whenever she **touches** something (plays an Attack or a Share card) it discharges: deal damage equal to her Static to a random enemy, then it's gone. Share cards are her tapping a teammate on the shoulder. Cards that **spend** Static (Arc Flash, Power Down) take it before it can discharge. |
| **Al and Pacino** | Her WoW hunter pets, two alpacas who stand at her side every combat (pets like Huffman's Mastiff and Tim's Kids: 9999 HP, no health bar, can't be targeted). At the end of her turn, **Al spits** on a random enemy for 4 damage, and **Pacino guards** her for 3 Block. "They act" means both do it once more right now. |
| **Look** | Her face does the talking. A Look card reads the enemy's intent: one effect if it intends to attack, another if it doesn't. |

Shared mechanics she also uses: **Share** (co-op support; in solo it targets her), **Duo**, and David's **Bleed** (one Duo).

### She never speaks
Every other character talks in banter, pings, and bubbles. **Brandi's bubbles never have words**: each one is a
pixel-drawn yellow smiley making a face (flat stare, eye roll, smirk, soft smile, glare, raised eyebrow, heart eyes when
something catches fire). She answers the brothers' lines with faces, and a ping from her is the flat stare. Built on
`Banter.Bubble`, with a face image in place of text. Face set: `images/faces/<face>.png`.

## Starting deck (11 cards) and relics

| Card | Cost / Type | Effect | Upgrade |
|---|---|---|---|
| Strike ×4 | 1 Attack | Deal 6 damage. | 9 damage |
| Defend ×4 | 1 Skill | Gain 5 Block. | 8 Block |
| 🔥 **Spark** | 1 Attack | Deal 5 damage. Apply 2 Ember. | 7 damage, 3 Ember |
| 🎯 **Steady Breath** | 1 Skill | Gain 5 Block. Gain 2 Aim. | 7 Block, 3 Aim |
| 👀 **The Look** | 1 Skill | Look. If the enemy intends to attack, apply 2 Weak. Otherwise, apply 2 Vulnerable. | 3 Weak / 3 Vulnerable |

- **Starter relic: Engraved Zippo** ("PVT PYRO" on the side). The first time you apply Ember each combat, apply double.
  **Touch of Orobas upgrade: Eternal Flame**: the first time you apply Ember **each turn**, apply 2 more.
- **Dog Tags** (she always starts with these too, in memory of her late husband; stamped **33**). Once per combat, when
  your HP drops below 33%, gain 33 Block. Never offered anywhere else, and Touch of Orobas leaves them alone.
  (Needs a check that the game accepts two starting relics; if not, `MainFile` can grant them at run start.)

Legend: 🔥 Ember, 🎯 Aim / Shot, 📡 Intel, ⚡ Static, 👀 Look, 🦙 Al and Pacino, 🤝 co-op.

## Common (25)

| # | Name | Cost / Type | Effect | Upgrade | Theme |
|---|---|---|---|---|---|
| 1 | 🔥 Kindling | 1 Attack | Deal 6 damage. Apply 2 Ember. | 8 damage, 3 Ember | Fire |
| 2 | 🔥 Lighter Fluid | 0 Skill | Apply 2 Ember. | 3 Ember | Fire |
| 3 | 🔥 Zippo Flick | 0 Attack | Deal 3 damage. Apply 1 Ember. | 5 damage, 2 Ember | The flick of her lighter |
| 4 | 🔥 Fire in the Hole | 2 Attack | Deal 7 damage to ALL enemies. Apply 2 Ember to ALL enemies. | 10 damage, 3 Ember | Fire / Army |
| 5 | 🔥 Mesmerized | 1 Skill | Gain 6 Block. If any enemy has Ember, draw 1 card. | 9 Block | Staring into a fire |
| 6 | 🎯 Windage | 1 Skill | Gain 5 Block. Gain 2 Aim. | 7 Block, 3 Aim | Sniper |
| 7 | 🎯 Range Card | 0 Skill | Gain 1 Aim. Draw 1 card. | 2 Aim | Sniper |
| 8 | 🎯 Snap Shot | 1 Attack | Shot. Deal 7 damage (+3 per Aim). | 10 damage | Sniper |
| 9 | 🎯📡 Spotter | 1 Skill | Gain 2 Aim. Apply 2 Intel. | 3 Aim, 3 Intel | Sniper team |
| 10 | 🎯 Low Crawl | 1 Skill | Gain 8 Block. If you haven't played an Attack this turn, gain 1 Aim. | 11 Block, 2 Aim | Army |
| 11 | 📡 Radio Check | 1 Skill | Gain 6 Block. Apply 2 Intel. | 9 Block, 3 Intel | Signal |
| 12 | 📡 Need to Know | 1 Attack | Deal 8 damage. Apply 1 Intel. | 11 damage, 2 Intel | MI |
| 13 | 🤝 Five by Five | 1 Skill | Share. A player gains 5 Block and draws 1 card next turn. | 8 Block | Signal ("loud and clear") |
| 14 | 👀 Side-Eye | 1 Attack | Look. Deal 7 damage. If it intends to attack, apply 1 Weak; otherwise, 1 Vulnerable. | 10 damage, 2 Weak / 2 Vulnerable | Her face |
| 15 | 👀 View Source | 0 Skill | Look. Draw 1 card. If the enemy intends to attack, draw 1 more. | Draw 2 (+1) | Web dev |
| 16 | Hooah | 0 Skill | Draw 1 card. Gain 1 Energy next turn. | Draw 2 | Army |
| 17 | Hurry Up and Wait | 1 Skill | Retain. Gain 6 Block. Each time it's Retained, it gains 3 Block this combat. | 8 Block, +4 | Army |
| 18 | PT at 0500 | 1 Attack | Innate. Deal 9 damage. | 13 damage | Army |
| 19 | ⚡ Carpet Shuffle | 0 Skill | Gain 3 Static. Draw 1 card. | 5 Static | Sparky |
| 20 | ⚡ Static Shock | 1 Attack | Deal 6 damage. Gain 3 Static (after this card's own discharge). | 8 damage, 4 Static | Sparky |
| 21 | ⚡ Wool Socks | 1 Skill | Gain 6 Block. Gain 2 Static. | 9 Block, 3 Static | Shuffling around the house |
| 22 | Headbang | 1 Attack | Deal 5 damage twice. | 7 damage twice | Metal |
| 23 | Riddim | 1 Attack | Deal 5 damage. If you played a Skill this turn, deal 5 more. | 7 + 7 | Reggae |
| 24 | Passport Stamp | 1 Skill | Gain 5 Block. Draw 2 cards. | 8 Block | Travel |
| 25 | Carry-On | 1 Skill | Gain 4 Block. Put a card from your discard pile into your hand. | 7 Block | Travel |

## Uncommon (43)

### Fire
| # | Name | Cost / Type | Effect | Upgrade | Theme |
|---|---|---|---|---|---|
| 26 | 🔥 It's Pretty | 1 Skill | Gain Block equal to the total Ember on ALL enemies (at least 5). | At least 9 | Why she loves fire |
| 27 | 🔥 Fire Watch | 1 Skill | Gain 8 Block. At the start of your next turn, apply 3 Ember to a random enemy. | 11 Block, 4 Ember | Army duty |
| 28 | 🔥 Flashover | 1 Attack | Deal damage equal to an enemy's Ember, twice. | Costs 0 | Fire |
| 29 | 🔥📡 Smoke Signal | 1 Skill | Apply 3 Ember and 2 Intel. A teammate draws 1 card next turn (you, in solo). | 4 Ember, 3 Intel | Fire + Signal |
| 30 | 🔥 Wildfire | 2 Skill | Apply 4 Ember to ALL enemies. Exhaust. | 6 Ember | Fire |

### Sparky
| # | Name | Cost / Type | Effect | Upgrade | Theme |
|---|---|---|---|---|---|
| 31 | ⚡ Static Cling | 1 Skill | Gain 8 Block. Gain 5 Static. | 11 Block, 6 Static | Sparky |
| 32 | ⚡ Arc Flash | 1 Attack | Spend all your Static. Deal 4 damage plus the Static spent to ALL enemies. | 7 damage | Sparky |

### Sniper
| # | Name | Cost / Type | Effect | Upgrade | Theme |
|---|---|---|---|---|---|
| 33 | 🎯 One Shot | 2 Attack | Shot. Deal 12 damage (+4 per Aim). | 16 damage (+5 per Aim) | Sniper |
| 34 | 🎯 Overwatch | 1 Skill | Gain 2 Aim. Retain. If it's still in your hand at the end of your turn, gain 2 more Aim. | 3 and 3 | Sniper |
| 35 | 🎯 Hide Site | 1 Power | Whenever you end your turn without playing an Attack, gain 3 Aim. | 4 Aim | Sniper |
| 36 | 🎯 Zeroed In | 1 Skill | Double your Aim. Exhaust. | No longer Exhausts | Sniper |
| 37 | 🎯🔥 Tracer Round | 1 Attack | Shot. Deal 6 damage (+3 per Aim). Apply Ember equal to the Aim spent. | 9 damage | Sniper + fire |

### Signal and MI
| # | Name | Cost / Type | Effect | Upgrade | Theme |
|---|---|---|---|---|---|
| 38 | 📡 SIGINT | 1 Skill | Apply 2 Intel to ALL enemies. Draw 1 card. | 3 Intel | MI |
| 39 | 🤝 Relay | 1 Skill | Share. A player gains 1 Energy and draws 1 card next turn. | Draws 2 | Signal |
| 40 | 📡 Comms Up | 1 Power | Whenever you apply Intel, gain 2 Block. | 3 Block | Signal |
| 41 | 📡 Antenna Farm | 1 Power | At the start of your turn, apply 1 Intel to ALL enemies. | 2 Intel | Signal |
| 42 | 📡 Jamming | 1 Skill | Apply 1 Weak and 1 Vulnerable to ALL enemies. | 2 and 2 | Signal (electronic warfare) |
| 43 | Sandstorm | 1 Skill | Apply 2 Weak to ALL enemies. Gain 2 Aim. Exhaust. | 3 Weak, 3 Aim | OIF (the big sandstorm, March 2003) |
| 44 | 🤝 Battle Buddy | 1 Skill | Share. You and a player each gain 6 Block (in solo, you gain it twice). | 9 Block each | Army |

### Her face
| # | Name | Cost / Type | Effect | Upgrade | Theme |
|---|---|---|---|---|---|
| 45 | 👀 Resting Face | 1 Skill | Look. If it intends to attack, gain 11 Block. Otherwise, gain 2 Aim and draw 1 card. | 15 Block / 3 Aim | Her face |
| 46 | 👀 Unimpressed | 1 Skill | Look. If it intends to attack, it loses 3 Strength this turn. Otherwise, apply 2 Intel. | 5 Strength / 4 Intel | Her face |
| 47 | 👀 Silent Treatment | 1 Skill | An enemy loses 2 Strength. Exhaust. | 3 Strength | Hates talking |

### WoW (her three mains)
| # | Name | Cost / Type | Effect | Upgrade | Theme |
|---|---|---|---|---|---|
| 48 | Cat Form | 1 Power | At the start of your turn, add a **Claw** to your hand. | Adds upgraded Claws | Feral Druid |
| 49 | Ferocious Bite | 1 Attack | Deal 7 damage, +3 for each other Attack you played this turn. | 9, +4 | Feral Druid (combo points) |
| 50 | 🤝 Power Word: Shield | 1 Skill | Share. A player gains 9 Block. | 13 Block | Disc Priest |
| 51 | 🤝 Atonement | 1 Power | Whenever you play an Attack, the player with the lowest HP gains 2 Block. | 3 Block | Disc Priest (healing by doing damage) |
| 52 | 🤝 Penance | 2 Attack | Deal 4 damage 3 times. Each hit gives the player with the lowest HP 2 Block. | 5 damage, 3 Block | Disc Priest |
| 53 | 🎯📡 Hunter's Mark | 1 Skill | Apply 3 Intel. Gain 1 Aim. | 5 Intel, 2 Aim | Hunter |
| 54 | 🎯 Multi-Shot | 1 Attack | Shot. Deal 4 damage to ALL enemies (+3 per Aim). | 7 damage | Hunter |
| 55 | 🦙 Kill Command | 1 Skill | Al and Pacino act. Draw 1 card. | Draw 2 | Hunter |
| 56 | 🦙 Feed Pet | 1 Skill | For the rest of combat, Al spits for 2 more and Pacino guards for 2 more. Exhaust. | +3 and +3 | Hunter |

### Music
| # | Name | Cost / Type | Effect | Upgrade | Theme |
|---|---|---|---|---|---|
| 57 | Mosh Pit | 2 Attack | Deal 7 damage to ALL enemies twice. | 9 damage | Metal |
| 58 | Breakdown | 1 Attack | Deal 10 damage. If it's the last card in your hand, deal double. | 14 damage | Metal |
| 59 | 🤝 One Love | 1 Skill | ALL players heal 3 HP. Exhaust. | 5 HP | Reggae |
| 60 | Dub Echo | 1 Skill | The next card you play this turn is played twice. Exhaust. | Costs 0 | Reggae dub |

### Web dev since 1993
| # | Name | Cost / Type | Effect | Upgrade | Theme |
|---|---|---|---|---|---|
| 61 | `<blink>` | 0 Attack | Retain. Deal 9 damage. Only playable on odd turns. | 13 damage | The old blink tag |
| 62 | Hit Counter | 1 Attack | Deal 6 damage. Every time you play it, its damage goes up by 3 this combat. | 8, +4 | The page counter |
| 63 | Under Construction | 1 Power | At the end of your turn, your most expensive card is Retained (picked for you: vanilla's pick-a-card Well-Laid Plans is solo only). | Costs 0 | The little GIF |
| 64 | Dial-Up | 1 Skill | Gain 2 Energy next turn. | Also draw 1 card next turn | Web dev |
| 65 | 🤝 Web Ring | 1 Skill | ALL players draw 1 card. Gain 4 Block. | 7 Block | Web dev |

### Her animals
| # | Name | Cost / Type | Effect | Upgrade | Theme |
|---|---|---|---|---|---|
| 66 | Peppi | 0 Attack | Deal 2 damage to a random enemy 4 times. Shows a "beep boop" bubble over Brandi. | 3 damage | Shredding toilet paper; her robot meow |
| 67 | Felicity | 1 Skill | Gain 8 Block. Apply 1 Weak to ALL enemies (covered in cat hair). | 11 Block, 2 Weak | Long gray fur, everywhere |
| 68 | 🤝 Lily Bean | 1 Skill | Share. A player gains 6 Block and heals 2 HP. | 9 Block, 3 HP | Pug in your lap |

## Rare (20)

| # | Name | Cost / Type | Effect | Upgrade | Theme |
|---|---|---|---|---|---|
| 69 | 🔥 Private Pyro | 2 Power | Whenever you apply Ember, apply 1 more. | Costs 1 | Her nickname |
| 70 | 🔥 Accidental Fire | 1 Skill | Apply 5 Ember to ALL enemies. Apply 2 Ember to yourself. Exhaust. | 7 Ember to enemies | The AIT fire |
| 71 | 🔥 Bonfire | 2 Power | At the start of your turn, apply 1 Ember to ALL enemies. | Costs 1 | Fire |
| 72 | 🔥 Inferno | 2 Attack | Deal damage equal to the total Ember on ALL enemies to ALL enemies. Exhaust. | Costs 1 | Fire |
| 73 | ⚡ Sparky | 1 Power | At the start of your turn, gain 3 Static. | 5 Static | Her other nickname |
| 74 | ⚡ Static Field | 2 Power | Your Static discharges hit ALL enemies. | Costs 1 | Sparky |
| 75 | ⚡ Power Down | 2 Skill | Spend all your Static. If you spent 5 or more, Stun an enemy that isn't a Boss. Exhaust. | Costs 1 | Shutting off Daniel's PC with one touch |
| 76 | 🎯 Nobody Tells Me No | 3 Attack | Shot. Deal 20 damage (+6 per Aim). Costs 1 less for every 3 Aim you have. | 26 damage | In the Spire, she's the sniper |
| 77 | 🎯 Aimed Shot | 2 Attack | Shot. Deal 14 damage (+3 per Aim). Keep half your Aim (rounded down). | 18 damage | Hunter (Marksmanship) |
| 78 | 🎯 First In | 1 Attack | Innate. Deal 12 damage. Gain 2 Aim. If it's turn 1, gain 1 Energy. | 16 damage, 3 Aim | V Corps, first into Iraq |
| 79 | 🎯 Embrace the Suck | 1 Power | Whenever you lose HP, gain 2 Aim. | 3 Aim | Army ("fantastic and horrible") |
| 80 | 🦙 Bestial Wrath | 2 Skill | Al and Pacino act 3 times. Exhaust. | Costs 1 | Hunter |
| 81 | 🤝 Pain Suppression | 1 Skill | Share. A player takes 40% less damage until your next turn. Exhaust. | Costs 0 | Disc Priest |
| 82 | Prowl | 1 Skill | Your next Attack this turn deals triple damage. Exhaust. | Costs 0 | Feral Druid (stealth opener) |
| 83 | 🤝 Dual Spec | 1 Power | At the start of your turn, choose: gain 2 Strength this turn, or ALL players gain 5 Block. | 3 Strength / 7 Block | WoW (she plays everything) |
| 84 | Wall of Death | 3 Attack | Deal 24 damage to ALL enemies. Costs 1 less for each other Attack you played this turn. | 30 damage | Metal |
| 85 | Since 1993 | 1 Power | The first time you play your 3rd card each turn, draw 1 card. | Innate | 30+ years of web dev |
| 86 | Been Everywhere | 1 Skill | Choose 1 of 3 random cards from other characters' pools. It costs 0 this turn. Exhaust. | Costs 0 | Travel |
| 87 | Have a Nice Day | 1 Attack | Deal 10 damage. If this kills the enemy, gain 1 Energy and draw 1 card. | 14 damage | The smiley faces |
| 88 | Yertle | 1 Attack | Exhaust up to 3 other cards in your hand. Deal 8 damage, +8 for each card Exhausted. | 10, +10 | Her mean 22-year-old turtle, who ate her 3 brothers |

## Duo (Uncommon, only offered when the partner is in the party; not counted in the 88)

| Name | Partner | Cost / Type | Effect | Upgrade | Theme |
|---|---|---|---|---|---|
| Across the Room | Daniel | 1 Skill | You and Daniel each gain 1 Energy and draw 1 card next turn. | Draw 2 each | One look says everything |
| Shoulder Tap | Daniel | 1 Attack | Deal 10 damage to ALL enemies. Daniel's next card this turn is Wired. Daniel gets a startled face bubble. | 14 damage | The sound card she fried |
| Shake and Bake | David | 1 Skill | Apply 3 Ember and 3 Bleed. | 4 and 4 | Her fire, his Bleed |
| Battle of the Bands | Joshua | 1 Attack | Deal 9 damage. Joshua gains 2 Verses. | 12 damage, 3 Verses | Her metal vs his ballads |
| Petting Zoo | Tim | 1 Skill | Al and Pacino act. Each of Tim's Kids hits once. | Also draw 1 card | Alpacas and eight kids |
| Looking for Group | John | 1 Skill | You and John each gain 6 Block. Your next Attack this turn deals 4 more. | 9 Block, +6 | WoW |
| Dog Park | Huffman | 1 Skill | Al and Pacino act. Huffman's Mastiff bites. | Costs 0 | Alpacas and the Mastiff |

## Ancient (never in rewards; see ANCIENT_CARDS.md)

| Relic | Name | Cost / Type | Effect | Notes |
|---|---|---|---|---|
| Dusty Tome | **One Shot, One Kill** | 3 (2) Power | Shot attacks no longer remove your Aim. You can play only 1 Shot attack each turn. | Comes upgraded. Bends the Aim rule; the catch is the one-per-turn limit |
| Archaic Tooth | **Firestorm** (replaces Spark) | 1 Attack | Deal 8 damage. Apply 4 Ember to ALL enemies. | Upgraded: 11 damage, 5 Ember |

## Token

| Name | Cost / Type | Effect | Upgrade | From |
|---|---|---|---|---|
| Claw | 0 Attack | Deal 4 damage. Exhaust. | 6 damage | Cat Form |

## Class names that aren't just the title

`StrikeBrandi`, `DefendBrandi`, `BlinkTag` (`<blink>`; title written as "&lt;blink&gt;"), `PtAt0500`, `FiveByFive`,
`SideEye`, `CarryOn`, `ItsPretty`, `Sigint`, `PowerWordShield`, `MultiShot`, `DialUp`, `Since1993`, `OneShotOneKill`.
Taken by other cards, so not used: Ghillie Suit, Jet Lag, Raid Night, Leeroy, Hearthstone, Cover Fire, Night Vision, Mount Up.

## Relics (3, plus the starters)

| Name | Rarity | Effect | Theme |
|---|---|---|---|
| Smiley Shelf | Uncommon | Adds a smiley whenever you open a chest or finish a `?` room (shown as its counter). Start each combat with 1 Block per smiley. | Her collection |
| Camp Victory | Rare | At the start of each Elite or Boss combat, gain 1 Energy and 3 Aim. | V Corps |
| Passport | Common | Each new Act, gain a stamp. Draw 1 more card on turn 1 per stamp. | Traveled all over the world |

### In memory: Pugsley and Sandi (in both Brandi's and Daniel's relic pools)
Pooled to Brandi and added to Daniel's pool in `MainFile`, the way the Monkey Island relics are shared.

| Name | Rarity | Effect | Theme |
|---|---|---|---|
| Pugsley's Collar | Uncommon | When you Rest at a rest site, heal 10 more HP. | A pug napping in your lap. Icon: his red collar with the bone-shaped "PUGSLEY" tag |
| Sandi's Sunbeam | Uncommon | At the start of your 2nd turn each combat, gain 1 Energy and draw 1 card. | She always found the sunny spot. Icon: her face in a sunbeam, split down the nose, orange tabby on one side and silver on the other, white chest patch and a white paw |

Potions (3): **MRE** (heal 10, gain 5 Block), **Red Bull at 0300** (gain 2 Energy), **Jerry Can** (apply 6 Ember to ALL enemies).

## Type counts (reward pool)

| | Attack | Skill | Power | Total |
|---|---|---|---|---|
| Common | 10 | 15 | 0 | 25 |
| Uncommon | 12 | 25 | 6 | 43 |
| Rare | 7 | 6 | 7 | 20 |

## How she looks (drawn in pixelgen, `Brandi.cs`)

- Pixel body, facing the player like the others: dark-brown ponytail with a red tie, red short-sleeve tee, green camo
  pants, black boots, dog tags on a chain.
- Attack frames: rifle at the shoulder. Cast frames: flicking the Zippo. A little blue spark when Static discharges.
- Still to draw: campfire loop (sitting and happily staring into the fire), select background (Camp Victory at night),
  map marker (red), co-op treasure hands (her hand holding the Zippo), the smiley face bubbles, and Al and Pacino.
- Al and Pacino: two pixel alpacas in front of her (like `MastiffLayout`), reusing `KidMotion`'s idle bounce and hop.
  Al leans forward and spits; Pacino steps in front of her when he guards.

## Built so far (and what's still to do)

- Borrows the Silent's vanilla scenes (shared with David) for the shop and rest site.
- The smiley face bubbles aren't built: she has no banter yet (`Banter.Talks` leaves her out), and her pings show "...".
- Al and Pacino are drawn at runtime (`Monsters/Alpaca.cs`, `AlpacaArt`); drop `images/monsters/alpaca/al.png` and
  `pacino.png` in to replace them.
- The blue spark when Static discharges uses vanilla's lightning hit effect.
- No end-of-run stats for her yet (`RunStats.Lines` has no Brandi case).

## Later
- Family Bonds with Daniel (husband and wife) and the brothers (in-laws).
- Banter: her face bubbles in reply to everyone's lines.
