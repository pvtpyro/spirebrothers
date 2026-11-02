# Danny, The Pilot (working name)

Daniel's idea, from Titanfall. Built 2026-11-01 as a playtest version: 4 Basic cards, the Stand By for Titanfall token,
and 27 reward cards (11 / 11 / 5). If Daniel likes how it plays, the pool grows to the usual ~88 and he gets a real name,
a pixel body, and his own select art. Until then he borrows the Defect's visuals.

## The rules

- **Titan Meter** (the bar on his left): every point of damage he deals fills it (Block soaked or not). Size **40**.
  It starts empty each combat.
- **Full meter:** he gets **Stand By for Titanfall** (0 cost, Retain, Exhaust) and plays it when he wants.
  If it gets discarded, a new one shows up at the start of his next turn.
- **Titanfall:** the Titan drops out of the sky and he climbs in: his body is hidden and the Titan stands in his place,
  with **30 HP**, shown on a labelled "TITAN" bar under his own health bar and powers (Titan Kit adds more). **Titan Armor:** hits on it of 5 or
  less do nothing (`Titan.ArmorThreshold`). His own bar stays
  under it so his Block and powers still show.
  - Damage meant for Danny hits the Titan first. Anything past its last HP spills onto him.
  - His own cards can't give him Block. Teammates' cards and his powers still can, and that Block protects the Titan.
  - The meter goes away while the Titan stands.
  - **Every card flips** to its Titan side: new name, new effect, sometimes new targeting.
- **Eject:** when the Titan is destroyed (or he plays Eject!), the cards flip back and the meter returns, empty.
  He can earn a second Titan in the same fight.
- **Repair N:** the Titan gets back N HP (never above its max). Nothing happens if it isn't standing.
- Reading the other side: hover any of his cards. The hover tip shows the side it isn't showing, in or out of combat.
- One card picture for both sides for now.

## Starter

Relic: **Neural Link** (Touch of Orobas: **Protocol 1**): no effect yet. It used to start the meter at 10; the meter
now starts at 0, and the relic's job is to be decided after playtesting.
Deck: 4 Strike, 4 Defend, Smart Pistol, Grapple. 70 HP.

## Cards (Pilot side / Titan side; upgrades in parentheses)

| Card | Cost | Rarity | Pilot side | Titan side |
|---|---|---|---|---|
| Strike | 1 | Basic | Deal 6 (9) | **Titan Punch:** deal 10 (14) |
| Defend | 1 | Basic | 5 (8) Block | **Electric Smoke:** 1 (2) Weak to ALL |
| Smart Pistol | 1 | Basic | 3 to a random enemy, 3 (4) times | **Predator Cannon:** 3 to a random enemy, 5 (7) times |
| Grapple | 1 | Basic | Draw 2 (3) | **Tether Trap:** 2 (3) Vulnerable |
| Stand By for Titanfall | 0 | Token | Call in your Titan. Retain. Exhaust. | (already here) |
| Wall Run | 1 | Common | 6 (8) Block, draw 1 | **Dash:** Repair 4 (6), draw 1 |
| Kraber | 2 | Common | Deal 16 (20) | **Railgun:** deal 24 (30) |
| Arc Grenade | 1 | Common | 6 (9) to ALL | **Cluster Missile:** 9 (12) to ALL |
| Pulse Blade | 1 | Common | 2 (3) Vulnerable, draw 1 | **Sonar Lock:** 2 (3) Vulnerable to ALL |
| Battery | 1 | Common | 12 (16) Titan Meter | **Battery Swap:** Repair 8 (11) |
| Phase Shift | 1 | Common | 8 (11) Block | **Phase Dash:** next turn 1 (2) Energy and draw 1 |
| Alternator | 1 | Common | 4 (5) twice | **40mm Tracker:** 6 (8) twice |
| Firestar | 1 | Common | 5 (7), 2 (3) Poison | **Thermite:** 4 (5) and 2 (3) Poison to ALL |
| Stim | 0 | Common | Draw 2 (3). Exhaust. | **Overcore:** 1 (2) Energy. Exhaust. |
| Execution | 1 | Common | 8 (11); kill: 10 Titan Meter | **Termination:** 12 (16); kill: Repair 8 |
| Rodeo | 1 | Uncommon | 10 (13), fills the meter twice | **Rocket Punch:** 13 (17) |
| Holo Pilot | 2 (1) | Uncommon | 1 Buffer | **Energy Siphon:** 10, Repair what got through Block |
| Amped Wall | 1 | Common | 8 (11) Block | **Vortex Shield:** catch the next 1 (2) attacks before your next turn: no damage, thrown back at the attacker |
| Gravity Star | 1 | Uncommon | 1 Weak + 1 (2) Vulnerable to ALL | **Arc Wave:** 7 (10) and 1 Weak to ALL |
| Satchel Charge | 1 | Uncommon | Next turn 16 (22) to ALL | **Rocket Salvo:** 3 to a random enemy, 6 (8) times |
| Titan Kit | 1 | Uncommon (Power) | Titan has 12 (18) more HP | **Nuclear Ejection:** when destroyed, 15 (20) to ALL |
| Titan Hunter | 1 (0) | Uncommon (Power) | Gain 50% more Titan Meter | **Fast Repair:** Repair 3 each turn |
| Last Mag | 1 | Uncommon | 7 (9), twice if you're below half HP | **Doomed:** 9 (12), twice if the Titan is below half |
| Wingman Elite | 1 | Uncommon | 9 (12). Retain. | **Leadwall:** 5 (6) to ALL twice. Retain. |
| Double Jump | 0 | Uncommon | 3 (5) Block, draw 1 | **Eject!:** Titan destroyed, 10 (14) to ALL |
| Hardpoint | 1 | Uncommon | 7 (9) Block, 6 (9) meter | **Hold the Point:** Repair 5 (7), 1 Weak to ALL |
| Amped Weapons | 1 | Uncommon | 2 (3) Strength this turn | **Smart Core:** 4 (5) Strength this turn |
| Prepare for Titanfall | 1 (0) | Rare | Fill your Titan Meter. Exhaust. | **Flight Core:** 5 to a random enemy 8 (10) times |
| Phase Reflex | 1 | Rare (Power) | 4 (6) Block each turn | **Sword Core:** 3 (4) Strength |
| Headshot / Laser Core | 3 | Rare | Deal 30 (38) | **Laser Core:** 22 (28) to ALL |
| Warpfall | 2 | Rare (Power) | Titanfall draws 3 (4) | **Upgrade Core:** 1 Energy each turn, Repair 5 (12) |
| Protocol 2 / 3 | 1 | Rare | 12 (16) Block, 10 meter. Exhaust. | **Protocol 3: Protect the Pilot:** Repair 15 (20) |

Powers played on either side stay for the rest of the combat, even after he ejects.

Potions: **Titan Battery** (20 meter, or Repair 20 in the Titan), **Smoke Canister** (2 Weak to ALL). Blown Cartridge is in his relic pool.

## Playtest checklist

- The meter shows on turn 1 at 0, fills as he hits things, turns gold, and the token shows up in his hand.
- Titanfall: the Titan drops in where he stands with 30 HP, his body disappears, the hand flips, and names
  and text change. The meter disappears. Check the TITAN bar shows under his and drops as it takes hits (`TitanHullDisplay`).
- Hovering or targeting the Titan reaches Danny (for Share cards and his power tips).
- Enemy attacks hit the Titan, not Danny. Overkill spills onto him. Teammates' Block on him soaks it first.
- His own Defend (if he had one left over) gives 0 Block in the Titan. A teammate's Share Block works.
- Destroying the Titan (or Eject!) flips the hand back and brings the meter back at 0. Nuclear Ejection fires.
- Hover tips: the other side reads right in the deck view, rewards, and in combat.
- Vortex Shield: the caught hit does no damage to the Titan, and the attacker takes it. A 6+ hit should still be caught
  even though the armor would not stop it.
- Titan Armor: hits of 5 or less on the Titan show no damage; 6 goes through in full.
- Co-op: two clients agree on the meter, the Titan's HP, and the random targets.
- Balance: 40 meter is meant to come up mostly in elites and bosses, not every hallway fight. Hull is 30 with Titan Armor 5
  (`Titan.BaseHull`).
