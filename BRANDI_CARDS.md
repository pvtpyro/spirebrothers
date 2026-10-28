# Brandi, Private Pyro: card plan (draft for review, 2026-10-28)

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

**Not built yet.** First set: 5 Basics, 73 reward cards (20 / 37 / 16), and 4 Duo cards, like John and Huffman.
Numbers are first-pass balance based on vanilla (Strike 6, Defend 5). Card class names below were checked against
every card in the mod (Ghillie Suit, Jet Lag, Raid Night, Leeroy, and Hearthstone are taken).

## Her mechanics

| Keyword | Rule |
|---|---|
| **Ember** | A debuff on an enemy. At the start of its turn it takes damage equal to its Ember, then its Ember **grows by 1** (Poison shrinks; fires spread). Growth stops at 10, but cards can still push it higher. When an enemy with Ember dies, its Ember jumps to another random enemy (run RNG). |
| **Aim** | A buff on Brandi that never wears off on its own. **Shot** attacks deal +3 damage per Aim, then remove all of it. Most Aim comes from quiet Skills: patience, then one big hit. |
| **Intel** | MI work. A debuff on an enemy: Attacks from **any** player deal +Intel damage to it. Removed at the end of the round. Made for co-op. |
| **Static** | Sparky. A buff on Brandi that stays until it's used. Whenever she **touches** something (plays an Attack or a Share card) it discharges: deal damage equal to her Static to a random enemy, then it's gone. Share cards are her tapping a teammate on the shoulder. |
| **Al and Pacino** | Her WoW hunter pets, two alpacas who stand at her side every combat (pets like Huffman's Mastiff and Tim's Kids: 9999 HP, no health bar, can't be targeted). At the end of her turn, **Al spits** on a random enemy for 4 damage, and **Pacino guards** her for 3 Block. Some cards make them act again or hit harder. |
| **Look** | Her face does the talking. A Look card reads the enemy's intent: one effect if it intends to attack, another if it doesn't. |

Shared mechanics she also uses: **Share** (co-op support) and **Duo**.

### She never speaks
Every other character talks in banter, pings, and bubbles. **Brandi's bubbles never have words**: each one is a
pixel-drawn yellow smiley making a face (flat stare, eye roll, smirk, soft smile, glare, raised eyebrow, heart eyes when
something catches fire). She answers the brothers' lines with faces, and a ping from her is the flat stare. Built on
`Banter.Bubble`, with a face image in place of text. Face set: `images/faces/<face>.png`.

## Starting deck (11 cards) and relic

- 4 Strike, 4 Defend
- **Spark** (Basic Attack, 1): Deal 5 damage. Apply 2 Ember.
- **Steady Breath** (Basic Skill, 1): Gain 5 Block. Gain 2 Aim.
- **The Look** (Basic Skill, 1, Look): If the enemy intends to attack, apply 2 Weak. Otherwise, apply 2 Vulnerable.
- **Starter relic: Engraved Zippo** ("PVT PYRO" on the side). The first time you apply Ember each combat, apply double.
  **Touch of Orobas upgrade: Eternal Flame**: the first time you apply Ember **each turn**, apply 2 more.
- **Dog Tags** (she always starts with these too, in memory of her late husband; stamped **33**). Once per combat, when
  your HP drops below 33%, gain 33 Block. Never offered anywhere else, and Touch of Orobas leaves them alone.
  (Needs a check that the game accepts two starting relics; if not, `MainFile` can grant them at run start.)

Legend: 🔥 Ember, 🎯 Aim / Shot, 📡 Intel, ⚡ Static, 👀 Look, 🤝 co-op.

## Common (20)

| # | Name | Cost / Type | Effect | Theme |
|---|---|---|---|---|
| 1 | 🔥 Kindling | 1 Attack | Deal 6 damage. Apply 2 Ember. | Fire |
| 2 | 🔥 Lighter Fluid | 0 Skill | Apply 2 Ember. | Fire |
| 3 | 🔥 Fire in the Hole | 2 Attack | Deal 7 damage to ALL enemies. Apply 2 Ember to ALL enemies. | Fire / Army |
| 4 | 🔥 Mesmerized | 1 Skill | Gain 6 Block. If any enemy has Ember, draw 1 card. | Staring into a fire |
| 5 | 🎯 Windage | 1 Skill | Gain 5 Block. Gain 2 Aim. | Sniper |
| 6 | 🎯 Range Card | 0 Skill | Gain 1 Aim. Draw 1 card. | Sniper |
| 7 | 🎯 Snap Shot | 1 Attack | Shot. Deal 7 damage (+3 per Aim). | Sniper |
| 8 | 🎯📡 Spotter | 1 Skill | Gain 2 Aim. Apply 2 Intel. | Sniper team |
| 9 | 📡 Radio Check | 1 Skill | Gain 6 Block. Apply 2 Intel. | Signal |
| 10 | 📡 Need to Know | 1 Attack | Deal 8 damage. Apply 1 Intel. | MI |
| 11 | 🤝 Five by Five | 1 Skill | Share. A player gains 5 Block and draws 1 card next turn. | Signal ("loud and clear") |
| 12 | 👀 Side-Eye | 1 Attack | Look. Deal 7 damage. If it intends to attack, apply 1 Weak; otherwise, 1 Vulnerable. | Her face |
| 13 | Hooah | 0 Skill | Draw 1 card. Gain 1 Energy next turn. | Army |
| 14 | Hurry Up and Wait | 1 Skill | Retain. Gain 6 Block. Each time it's Retained, it gains 3 Block this combat. | Army |
| 15 | PT at 0500 | 1 Attack | Innate. Deal 9 damage. | Army |
| 16 | 👀 View Source | 0 Skill | Look. Draw 1 card. If the enemy intends to attack, draw 1 more. | Web dev |
| 17 | ⚡ Carpet Shuffle | 0 Skill | Gain 3 Static. Draw 1 card. | Sparky |
| 18 | Headbang | 1 Attack | Deal 5 damage twice. | Metal |
| 19 | ⚡ Static Shock | 1 Attack | Deal 6 damage. Gain 3 Static (after this card's own discharge). | Sparky |
| 20 | Passport Stamp | 1 Skill | Gain 5 Block. Draw 1 card. | Travel |

## Uncommon (37)

### Fire
| # | Name | Cost / Type | Effect | Theme |
|---|---|---|---|---|
| 21 | 🔥 It's Pretty | 1 Skill | Gain Block equal to the total Ember on ALL enemies (at least 5). | Why she loves fire |
| 22 | 🔥 Fire Watch | 1 Skill | Gain 8 Block. At the start of your next turn, apply 3 Ember to a random enemy. | Army duty |
| 23 | 🔥 Flashover | 1 Attack | Deal damage equal to an enemy's Ember, twice. | Fire |
| 24 | 🔥📡 Smoke Signal | 1 Skill | Apply 3 Ember and 2 Intel. A teammate draws 1 card next turn. | Fire + Signal |
| 25 | 🔥 Wildfire | 2 Skill | Apply 4 Ember to ALL enemies. Exhaust. | Fire |
| 26 | ⚡ Static Cling | 1 Skill | Gain 7 Block. Gain 4 Static. | Sparky |

### Sniper
| # | Name | Cost / Type | Effect | Theme |
|---|---|---|---|---|
| 27 | 🎯 One Shot | 2 Attack | Shot. Deal 12 damage (+4 per Aim). | Sniper |
| 28 | 🎯 Overwatch | 1 Skill | Gain 2 Aim. Retain. If it's still in your hand at the end of your turn, gain 2 more Aim. | Sniper |
| 29 | 🎯 Hide Site | 1 Power | Whenever you end your turn without playing an Attack, gain 3 Aim. | Sniper |
| 30 | 🎯 Zeroed In | 1 Skill | Double your Aim. Exhaust. | Sniper |
| 31 | 🎯🔥 Tracer Round | 1 Attack | Shot. Deal 6 damage (+3 per Aim). Apply Ember equal to the Aim spent. | Sniper + fire |

### Signal and MI
| # | Name | Cost / Type | Effect | Theme |
|---|---|---|---|---|
| 32 | 📡 SIGINT | 1 Skill | Apply 2 Intel to ALL enemies. Draw 1 card. | MI |
| 33 | 🤝 Relay | 1 Skill | Share. A player gains 1 Energy and draws 1 card next turn. | Signal |
| 34 | 📡 Comms Up | 1 Power | Whenever you apply Intel, gain 2 Block. | Signal |
| 35 | 📡 Jamming | 1 Skill | Apply 1 Weak to ALL enemies. Gain 6 Block. | Signal (electronic warfare) |
| 36 | Sandstorm | 1 Skill | Apply 1 Weak to ALL enemies. Gain 1 Aim. Exhaust. | OIF (the big sandstorm, March 2003) |

### Her face
| # | Name | Cost / Type | Effect | Theme |
|---|---|---|---|---|
| 37 | 👀 Resting Face | 1 Skill | Look. If it intends to attack, gain 11 Block. Otherwise, gain 2 Aim and draw 1 card. | Her face |
| 38 | 👀 Unimpressed | 1 Skill | Look. If it intends to attack, it loses 3 Strength this turn. Otherwise, apply 2 Intel. | Her face |
| 39 | 👀 Silent Treatment | 1 Skill | An enemy loses 2 Strength. Exhaust. | Hates talking |

### WoW (her three mains)
| # | Name | Cost / Type | Effect | Theme |
|---|---|---|---|---|
| 40 | Cat Form | 1 Power | At the start of your turn, add a **Claw** to your hand (0-cost Attack token: deal 4 damage, Exhaust). | Feral Druid |
| 41 | Ferocious Bite | 1 Attack | Deal 7 damage, +3 for each other Attack you played this turn. | Feral Druid (combo points) |
| 42 | 🤝 Power Word: Shield | 1 Skill | Share. A player gains 9 Block. | Disc Priest |
| 43 | 🤝 Atonement | 1 Power | Whenever you play an Attack, the teammate with the lowest HP gains 2 Block (you, in solo). | Disc Priest (healing by doing damage) |
| 44 | 🎯📡 Hunter's Mark | 1 Skill | Apply 3 Intel. Gain 1 Aim. | Hunter |
| 45 | Kill Command | 1 Skill | Al and Pacino act now (Al spits, Pacino guards). Draw 1 card. | Hunter |

### Music
| # | Name | Cost / Type | Effect | Theme |
|---|---|---|---|---|
| 46 | Mosh Pit | 2 Attack | Deal 7 damage to ALL enemies twice. | Metal |
| 47 | Breakdown | 1 Attack | Deal 10 damage. If it's the last card in your hand, deal double. | Metal |
| 48 | 🤝 One Love | 1 Skill | ALL players heal 3 HP. Exhaust. | Reggae |
| 49 | Dub Echo | 1 Skill | The next card you play this turn is played twice. Exhaust. | Reggae dub |

### Web dev since 1993
| # | Name | Cost / Type | Effect | Theme |
|---|---|---|---|---|
| 50 | `<blink>` | 0 Attack | Deal 9 damage. Only playable on odd turns. (Title shows as "&lt;blink&gt;".) | Web dev (class `BlinkTag`) |
| 51 | Hit Counter | 1 Attack | Deal 4 damage. Every time you play it, its damage goes up by 2 this combat. | Web dev |
| 52 | Under Construction | 1 Power | At the end of your turn, Retain 1 card. | The little GIF |
| 53 | Dial-Up | 1 Skill | Gain 2 Energy next turn. | Web dev |
| 54 | 🤝 Web Ring | 1 Skill | ALL players draw 1 card. Gain 4 Block. | Web dev |

### Her animals
| # | Name | Cost / Type | Effect | Theme |
|---|---|---|---|---|
| 71 | Peppi | 0 Attack | Deal 2 damage to a random enemy 4 times. Shows a "beep boop" bubble over Brandi. | Shredding toilet paper; her robot meow |
| 72 | Felicity | 1 Skill | Gain 7 Block. Apply 1 Weak to ALL enemies (covered in cat hair). | Long gray fur, everywhere |
| 73 | 🤝 Lily Bean | 1 Skill | Share. A player gains 6 Block and heals 2 HP. | Pug in your lap |

## Rare (16)

| # | Name | Cost / Type | Effect | Theme |
|---|---|---|---|---|
| 55 | 🔥 Private Pyro | 2 Power | Whenever you apply Ember, apply 1 more. | Her nickname |
| 56 | 🔥 Accidental Fire | 1 Skill | Apply 5 Ember to ALL enemies. Apply 2 Ember to yourself. Exhaust. | The AIT fire |
| 57 | 🔥 Bonfire | 2 Power | At the start of your turn, apply 1 Ember to ALL enemies. | Fire |
| 58 | ⚡ Sparky | 1 Power | At the start of your turn, gain 3 Static. | Her other nickname |
| 59 | 🎯 Nobody Tells Me No | 3 Attack | Shot. Deal 20 damage (+6 per Aim). Costs 1 less for every 3 Aim you have. | In the Spire, she's the sniper |
| 60 | 🎯 Aimed Shot | 2 Attack | Shot. Deal 14 damage (+3 per Aim). Keep half your Aim. | Hunter (Marksmanship) |
| 61 | 🤝 Pain Suppression | 1 Skill | Share. A player takes 40% less damage until your next turn. Exhaust. | Disc Priest |
| 62 | Prowl | 1 Skill | Your next Attack this turn deals triple damage. Exhaust. | Feral Druid (stealth opener) |
| 63 | Dual Spec | 1 Power | At the start of your turn, choose: gain 1 Strength this turn, or ALL players gain 4 Block. | WoW (she plays everything) |
| 64 | ⚡ Power Down | 2 Skill | Spend all your Static. If you spent 5 or more, Stun an enemy that isn't a Boss. Exhaust. | Shutting off Daniel's PC with one touch |
| 65 | Since 1993 | 1 Power | Whenever you play your 3rd card in a turn, draw 1 card. | 30+ years of web dev |
| 66 | First In | 1 Attack | Innate. Deal 12 damage. Gain 2 Aim. If it's turn 1, gain 1 Energy. | V Corps, first into Iraq |
| 67 | Embrace the Suck | 1 Power | Whenever you lose HP, gain 2 Aim. | Army ("fantastic and horrible") |
| 68 | Been Everywhere | 1 Skill | Choose 1 of 3 random cards from other characters' pools. It costs 0 this turn. Exhaust. | Travel |
| 69 | Have a Nice Day | 1 Attack | Deal 10 damage. If this kills the enemy, gain 1 Energy and draw 1 card. | The smiley faces |
| 70 | Yertle | 1 Attack | Exhaust up to 3 other cards in your hand. Deal 8 damage, +8 for each card Exhausted. | Her mean 22-year-old turtle, who ate her 3 brothers |

## Relics (3, plus the starter)

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

## Duo (Uncommon, only offered when the partner is in the party)

| Name | Partner | Cost / Type | Effect | Theme |
|---|---|---|---|---|
| Across the Room | Daniel | 1 Skill | You and Daniel each gain 1 Energy and draw 1 card next turn. | One look says everything |
| Shoulder Tap | Daniel | 1 Attack | Deal 10 damage to ALL enemies. Daniel's next card this turn is Wired. Daniel gets a startled face bubble. | The sound card she fried |
| Battle of the Bands | Joshua | 1 Attack | Deal 9 damage. Joshua gains 2 Verses. | Her metal vs his ballads |
| Looking for Group | John | 1 Skill | You and John each gain 6 Block. Your next Attack this turn deals 4 more. | WoW |

## How she looks

- Pixel body: long wavy dark-brown hair, red T-shirt, green camo pants, black boots. Dog tags on a chain.
  Three-quarters turned right, like the others.
- Attack frames: rifle at the shoulder. Cast frames: flicking the Zippo. A little blue spark when Static discharges.
- Campfire loop: sitting and happily staring into the fire.
- Select background: Camp Victory at night. Map color red.
- Co-op treasure hands: her hand holding the Zippo.
- Al and Pacino: two pixel alpacas in front of her (like `MastiffLayout`), reusing `KidMotion`'s idle bounce and hop.
  Al leans forward and spits; Pacino steps in front of her when he guards. Art drawn in pixelgen like the Mastiff.

## Questions for Brandi

- Borrowed vanilla visuals: all five vanilla characters are already used. The Silent (shared with David) fits a
  sniper best.

## Later
- Duo cards with David, Tim, and Huffman.
- Family Bonds with Daniel (husband and wife) and the brothers (in-laws).
- Grow to ~88 reward cards.
