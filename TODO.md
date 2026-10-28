# Spire Brothers TODO

Ideas list from 2026-10-26. Check items off as they're done; add notes under each as we expand them.

## 0. Playtest first

- [ ] David in game (solo and co-op)
- [ ] Joshua in game (solo and co-op)
- [ ] Tim in game (solo and co-op)
- [ ] The 21 relics added 2026-10-25 (list in DESIGN.md, Shared section)

## Co-op and family touches

### 1. Family Bonds (pair bonuses)
Brothers in the same party unlock bonuses. Shown as a Family Bonds status icon on each brother (hover lists the
active bonds); starter relics have a hover tip explaining it. Code: `Mechanics/FamilyBonds.cs`, `Powers/FamilyBondsPower.cs`.
- [x] Decide the pairings and bonuses (Heated Debate, Wired for Sound, Code Review, Royalties, Family Loan,
      Kids' Choir; Group Chat for 3, Nobody Gets Left Behind for 4)
- [x] Build it (2026-10-26)
- [ ] Test in co-op: icon shows with 2+ brothers, each pair's effect fires, Group Chat energy, death save with 4

### 2. Brother banter
Short speech-bubble lines when brothers react to each other. Lines in BANTER.md, code in `Mechanics/Banter.cs`.
- [ ] Collect real catchphrases and in-jokes for each brother (from Brandi; add them whenever they come to mind)
- [x] Pick trigger moments
- [x] Build the trigger hooks and loc keys (2026-10-26)
- [ ] Playtest: are the bubbles readable, and is the frequency right?

### 3. Duo cards
Uncommon cards that only show up in rewards and shops when a specific brother is also in the party.
12 cards, one per brother per partner. Code: `Mechanics/DuoCards.cs` (`IDuoCard`, the Duo keyword).

| Owner | Card | With | Cost / type | Effect |
|---|---|---|---|---|
| Daniel | Hold That Thought | David | 1 Attack, Logic, Comeback | 8 damage, +3 per Insulted (doubled if David Ranted this turn) |
| Daniel | Fixed Your Amp | Joshua | 1 Skill, Hands | Joshua gains 3 Verses. Wired: draw 2 |
| Daniel | Wire the Kids' Rooms | Tim | 1 Skill, Logic, Exhaust | Tim gains 2 Kids. Wired: gain 8 Block |
| David | Agree to Disagree | Daniel | 1 Attack, Rant | 6 damage, apply 2 Insulted |
| David | Patron of the Arts | Joshua | 1 Skill, Exact | Lose 15 Gold; Joshua gains 3 Verses; ALL gain 4 Block. Exact: keep the Gold |
| David | IOU | Tim | 0 Skill, Exact, Exhaust | Tim gains 30 Gold. Exact: you gain 30 too |
| Joshua | Hype Man | Daniel | 1 Skill, Song, Share | A player gains 3 Strength this turn |
| Joshua | Minor Key | David | 1 Skill, Chorus | 1 Bleed per Verse to ALL enemies |
| Joshua | Babysitting | Tim | 1 Skill, Song | Tim gains 1 Kid; ALL gain 3 Block |
| Tim | LAN Party | Daniel | 1 Skill, Script | ALL gain 3 Block and draw 1 next turn |
| Tim | Gift Card | David | 0 Skill, Exhaust | Lose 20 Gold; David gains 40 |
| Tim | Centering Pass | Joshua | 1 Attack, Script | 8 damage; Joshua gains 2 Verses |

- [x] Brainstorm
- [x] Filter rewards, shops, Shared Steam Library, and random Songs by party makeup
- [x] Build, add glyphs.txt lines (2026-10-26)
- [ ] Test in co-op: cards appear only with the partner, effects reach the partner
- [ ] Maybe: a card for all four brothers together ("Family Photo" and "Reunion" are taken names)

## Content gaps

### 4. Potions for David, Joshua, and Tim
Built 2026-10-26 (common / uncommon / rare each, in `SpireBrothersCode/Potions/`):
- [x] David: Gas Station Coffee (this turn, Exact also triggers at 1 energy), Trail Mix (heal 10, ALL players 4 Block),
      Piggy Bank (gain 100 Gold, any time)
- [x] Joshua: Ramune (gain 3 Verses), Honey Lemon Tea (ALL players heal 8), Matcha (fill Verses to 10)
- [x] Tim: Juice Box (gain 2 Kids), Sugar Rush (gain 1 Kid, every Kid hits twice), Hotfix (next 3 cards this turn have Script)
- [x] Art (`cardgen/gen.ps1 -Section potions [-Only <file_name>]`)
- [ ] Test in game
- [x] Daniel: Grapefruit Juice (heal 8, gain 1 energy), Spare Battery (this turn, every Wired effect triggers),
      Stim (heal 30% Max HP, gain 2 energy; Helldivers 2)

### 5. Custom ? room events
Built 2026-10-26 (`SpireBrothersCode/Events/`, text in `events.json`). Only appear with a brother in the party.
- [x] Road Trip to Grandma's (shared vote, 2 pages), Thanksgiving with Mom and Dad, A Day at the Lake,
      Family Hike (shared vote), Calling Mom (Vent for an Hour locked for Daniel)
- [x] Extras from the family details: Food Coma curse (Thanksgiving pie), Sunday Walk (David power card),
      Dad's Tackle Box (shared relic), banter lines (David calling Mom, Dad's bigger fish)
- [x] Brandi: reword Thanksgiving if wanted (events.json, `SPIREBROTHERS-THANKSGIVING_WITH_MOM_AND_DAD.*`)
- [ ] Real event art (placeholders are 1720x806; vanilla is 3440x1613, same shape)
- [ ] Test: each event shows, options work, shared votes in co-op, the two-page road trip
- [ ] Maybe later: "The Group Chat" (co-op vote), "Road Trip Detour" (David's Idaho-to-NJ trip)

### 6. Sound effects
The code is done (2026-10-26, `Mechanics/Sounds.cs`): every cue plays a file from `SpireBrothers/audio/` if it's there,
otherwise a built-in game sound or nothing. File names and recording tips: `SpireBrothers/audio/README.md`.
- [x] Wire up all the cues (Wired zap, Orbital Barrage, David's gold and rants, Chorus, group heals, Kids, Age Up)
- [ ] Brandi: get recordings (could take months; nothing else needs changing when they arrive)
  - [ ] `joshua_chorus`: Joshua strumming his guitar
  - [ ] `tim_kids`: Tim's kids giggling
  - [ ] `david_gold`: "cha-ching" (David saying it?)  /  `david_rant`: an exasperated sigh
  - [ ] `daniel_wired`: a zap  /  `daniel_barrage`: something big
  - [ ] `joshua_heal`, `tim_age_up`
- [ ] Test the fallback sounds in game (lightning zap, coins, heal)

## Stretch ideas

### 7. Monkey Island elite: Captain Blockbeard
Built 2026-10-26. Monkey Island meets Minecraft: square beard, tricorn, eye patch, blocky cyan cutlass, parrot.
`godmode` `fight SPIREBROTHERS-BLOCKBEARD_ELITE` and `event SPIREBROTHERS-INSULT_DUEL`
- [x] Elite (Act 3; about 250 HP, tuned with the vanilla Act 3 elites): `Monsters/Blockbeard.cs` + `BlockbeardElite.cs`. Insult duel (`Powers/InsultDuelPower.cs`): after
      Cutting Remark, retort before your turn ends (a Comeback or Rant at him, or leave him Insulted) and he's Speechless
      (skips his next move); otherwise he gloats (+3 Strength from the players' next turn until his next attack, `GloatingPower`). Moves loop: Cutting Remark, Swashbuckle (4 hits),
      Look Behind You! (2 Weak + 2 Vulnerable to all), Cutting Remark, Swig of Grog (heal + Block), Swashbuckle.
- [x] Insult Duel event (Act 3, shared vote): pick the comeback that answers each insult, best 2 of 3. Win: relic.
      Lose: pay 125 Gold, or draw swords and fight him. 8 original insult/comeback pairs in events.json.
- [x] Pixel art (`art_originals/pixelgen/Pirate.cs`; frames in `images/monsters/blockbeard/frames/`)
- [ ] Test: speech bubbles, Speechless stun, gloat Strength, the event's dynamic answers, Draw Swords into combat
- [ ] Maybe: a Powder Monkey crewmate at half HP; a guaranteed Grog drop
- [ ] Brandi: add or reword insult pairs (events.json `SPIREBROTHERS-INSULT_DUEL.pairs.N`) and his fight lines
      (monsters.json `SPIREBROTHERS-BLOCKBEARD.insults/gloats/speechless.N`); any count works

### 7b. Bible elites: one per brother, named for his namesake
Built 2026-10-27. Act 1: Goliath (David), the Lions' Den (Daniel). Act 2: the Walls of Jericho (Joshua), Alexander the
Coppersmith (Tim). Any party can meet them; if the namesake is there he starts with a token card, says the opening line
(the foe answers ~2 s later), and has the last word. Without him the foe opens with its own taunt.
`godmode` then `fight SPIREBROTHERS-GOLIATH_ELITE` / `LIONS_DEN_ELITE` / `JERICHO_ELITE` / `COPPERSMITH_ELITE`
- [x] **Goliath** (1 Samuel 17): 120-124 HP, 6 Plating. Choose Your Champion (2 Weak to all, once), then Weaver's Beam (16),
      Bronze Charge (7 x2), Forty Days (+3 Strength, taunts). **Shield Bearer** minion in front (26-28 HP): Raise Shield
      (Goliath +10 Block) / Shield Bash 6. David: **Smooth Stone** (0, Retain, Exhaust): 5 damage, 25 to Goliath.
- [x] **Lions' Den** (Daniel 6): Lion 54-56 HP + Lioness 46-48. Roar (all lions +2 Strength), Pounce 10, Maul 4 x2; the Lioness
      is one step behind so they don't roar together. Daniel: **Shut Their Mouths** (0, Retain, Exhaust): an enemy skips its next move.
- [x] **Walls of Jericho** (Joshua 6): Wall 150 HP (Fortify +16 Block / Falling Rubble 10) between a Jericho Guard (30-32 HP,
      Spear Thrust 9 / Hold the Gate: Wall +8 Block) and a Jericho Archer (26-28 HP, Volley 4 x2 / Aimed Shot 11), both minions.
      **Seven Trumpets** counts down from 7: 1 per round, 1 per Song or Chorus anyone plays. At 0 the walls fall: the Wall loses
      its Block, gets 99 Vulnerable, and stops acting; the defenders lose half their HP; Joshua says his line then.
      Joshua: **Trumpet Blast** (0, Song, Retain, Exhaust): 4 damage to ALL, counts down twice.
- [x] **Alexander the Coppersmith** (2 Timothy 4:14): 150 HP. Hammer 17, Hot Rivets 5 x3, then Forge (adds a Copper Automaton
      while fewer than 2 stand) or Much Harm (2 Weak + 2 Frail to all). Starts with one **Copper Automaton** (minion, 20-22 HP,
      Clank 7 / Grind 3 x2). Tim: **Sound Doctrine** (0, Retain, Exhaust): 8 damage; takes an Automaton apart outright.
- [x] Closing lines: the rewards screen waits until the last line has been up 3.5 s (`Banter.MarkClosingLine`).
- [x] Pixel art: `Giants.cs` (Goliath, Shield Bearer), `Foes.cs` (Jericho Guard/Archer, Alexander, Automaton), `Beasts.cs`
      (Lion, Lioness, Wall; drawn without the Rig, 72x48 for lions). Goliath is drawn 540 tall, intent capped at 500.
- [ ] Test: placement (helpers in front), bubble timing, the rewards-screen hold, tokens in co-op, minions ending the fight,
      Automatons sliding in when forged (no encounter scene, so `Alexander.Summon` re-runs the game's layout), the Wall
      falling, lion sizes, sounds (all borrowed from vanilla)
- [ ] Brandi: reword any lines (monsters.json `SPIREBROTHERS-<GOLIATH|LIONS_DEN|JERICHO|ALEXANDER>.intro/heroIntro/heroReply/taunts/heroWin.N`)
### 8. End-of-run stats
Built 2026-10-26. A second column of score lines on the game over summary page, beside vanilla's floors/gold/elites,
with each brother's icon. Every brother in the party is listed on every player's screen (3 lines each with 1-2 brothers,
2 each with 3-4). Lines at 0 are skipped. Code: `Mechanics/RunStats.cs` (counters), `Mechanics/RunStatsScreen.cs` (screen).
Text: powers.json `SPIREBROTHERS-RUN_STATS.*`.
- [x] Pick the stats, best first:
  - Daniel: times Wired, Stratagems called in, Gears played
  - David: most gold carried (from the run's own history), Rants, Exact triggers
  - Joshua: HP healed by his cards and potions (only real healing, no overheal; "healed the team" in co-op), biggest Chorus, Verses sung
  - Tim: damage his Kids dealt, Age reached, Script effects
- [x] Track them over the run (saved with the run, counted the same way on every client)
- [ ] Test: lines show and slide in after vanilla's, numbers look right, solo and co-op, a save/reload mid-run keeps them
- [ ] Brandi: reword any line (powers.json), or pick different stats

### 9. A fifth character: Brandi
A surprise extra character as part of the gift.
- [ ] Design concept, mechanic, and starter deck
- [ ] Card list (like the other *_CARDS.md files)


### 10. Anchient Card Options

### 11. Status, Curse and Quest cards maybe?

### 12. The friends: John and Huffman
Built 2026-10-27 (cards, potions, relics, banter). Plans in JOHN_CARDS.md / HUFFMAN_CARDS.md, dossier in FRIENDS.md.
- [ ] Playtest both, solo and co-op. Riskiest first:
  - [ ] John's Stimm Lab rest site button (icon, the pick-1-of-3 screen, co-op sees the right choice)
  - [ ] Huffman's Mastiff: stands in front of him at a good size, bites the Marked enemy at his turn end
  - [ ] Grind growth survives save and quit, and shows dimmed-then-green
  - [ ] Laws trigger (especially No Loitering and No Armor Permitted)
  - [ ] Their banter and Ping lines show up
- [ ] Ask Daniel: John's favorite WoW classes, the Mastiff's name, their Darktide character names, catchphrases,
  how they play, anything they'd hate to see (full list at the bottom of FRIENDS.md)
- [ ] WoW class cards for John once we know his classes
- [ ] Rename the Mastiff to his real dog's name (monsters.json `SPIREBROTHERS-MASTIFF_DOG.name`, card text says "Mastiff")
- [ ] Grow both decks from 70 toward the brothers' ~88 reward cards
- [x] Bonds (2026-10-27): Uncle and Nephew (John + Huffman), Best Friends (John + David), Coworkers (Huffman + Daniel),
  Strike Team (Daniel + John + Huffman). Effects in DESIGN.md. Friends get the Family Bonds icon too.
  - [ ] Test in co-op: each one fires, and the icon lists them
- [ ] Duo cards the other way: David's card with John, Daniel's card with Huffman
- [x] Ancient cards (2026-10-27): Realm First! and Cartel Masterwork (John), Home Sweet Home and Release the Hound
  (Huffman). Details in ANCIENT_CARDS.md.
- [x] End-of-run stats (2026-10-27): John (times Ground, Toxin spread, turns in the Shroud), Huffman (Mastiff bites, Laws
  enforced, Metal spent)
- [x] Events (2026-10-27): Weekend in Town (David visits; shows up with David, John, or Huffman in the party), and
  "Let Huffman Pick the Route" in the Road Trip (only with Huffman along; the van goes back to his house)
- [ ] More banter: big-moment lines (like the brothers' RANTS / KIDS) for a big Grind or the dog's Bite getting high
- [x] Sounds (2026-10-27): john_stimm, john_shroud, huffman_bark, huffman_law (list in SpireBrothers/audio/README.md)
  - [ ] Record or find real clips (a bark from his real dog would be perfect)

### 13. Regular fights (approved 2026-10-28)
Normal (non-elite) fights. Code: `Monsters/PixelMonster.cs` base + one file per fight (encounter + its monsters), text in
monsters.json / encounters.json. Art: `art_originals/pixelgen/Mobs.cs`, rendered by `art_originals/mobgen` (`dotnet run -c Release [name]`),
then copy `out/<name>/frames` to `SpireBrothers/images/monsters/<name>/frames`. Built one at a time, start to finish.
- [x] 1. Night Falls (Minecraft, Act 1): Creeper (Fuse 3, explodes 16 and dies), Skeleton (3x2, 3x2, Take Aim +2 Str), Spider (bite 6 + Weak, climb 8 Block). `Monsters/NightFalls.cs`, `FusePower`.
- [x] 2. The Enderman (Minecraft, Act 2, `Monsters/TheEnderman.cs`, `ProvokedPower`): ~110 HP, light hits; hit 3+ times in one turn and it gets angry (next attack hits hard); sometimes teleports (immune to damage for a turn).
- [x] 3. Blockbeard's Deckhands (Monkey Island, Act 1 weak, `Monsters/Deckhands.cs`; Lookout insults are monsters.json `LOOKOUT.insults.N`): Lookout (12 HP, insults = Weak) + Powder Monkey (10 HP, lights a keg that blows next turn unless he dies). Both start Insulted.
- [x] 4. Philistine Scouts (Bible, Act 1, `Monsters/PhilistineScouts.cs`; crested helmets, since feathered crowns read as war bonnets): 3 scouts ~15 HP; each turn one Guards (Block to the others) while two attack.
- [x] 5. The Jackals (Bible, Act 1 weak, `Monsters/TheJackals.cs`): 2 jackals ~11 HP, bite 3x2; when one dies the other howls (+Strength).
- [x] 6. Runaway Roomba (Act 1 weak, `Monsters/RunawayRoomba.cs`, status card `Cards/Shared/DustBunny.cs`): ~30 HP; bumps 5, backs off (Block); vacuums a card from your discard and spits back a Dust Bunny junk card.
- [x] 7. Windows Update (Act 2, `Monsters/WindowsUpdate.cs`, `UpdatingPower`, status card `CloseAd`): laptop ~90 HP, "Updating x%" climbs each turn; at 100% it restarts (full heal + hits everyone); pop-ups add junk cards.
- [x] 8. Chores (Act 2, `Monsters/Chores.cs`, `PilingUpPower`): Laundry Pile, Dirty Dishes, Overgrown Lawn (~35 HP each); each grows stronger every turn it isn't attacked; the icon drops to 0 once it has been hit this turn (pets count). Laundry is in a basket with a sock hanging out.
- [x] 9. Poxwalker Horde (Darktide, Act 2, `Monsters/PoxwalkerHorde.cs`; summons via `Alexander.Summon(..., minion: false)`): 4 poxwalkers ~12 HP, 1 more shuffles in each round (max 6).
- [x] Playtest all nine (tested 2026-10-28, all work). Riskiest were: the Creeper's explode-then-die, the Enderman switching to Ender Rage
  mid-turn, Windows Update's Restart (full heal), Philistine Scouts taking turns guarding, and the horde lining up 6 bodies.
- [ ] Tune numbers after playing; Act 2 fights may need more HP/damage. The art is first-draft pixel art (mobgen previews).


### 14. See if it's possible to select wardrobes or shirt colors when selecting character. then maybe changing the appearance for getting certain items.


## More ideas (2026-10-28)

### 15. Birthdays and holidays
Check the system date. On a brother's (or friend's) real birthday: party hat on his combat frames, a birthday banter line,
maybe a small gift relic at run start. Holiday weeks (Christmas, Thanksgiving) swap in themed banter.
- [ ] Brandi: collect everyone's birthdays (all four brothers, John, Huffman)
- [ ] Build it once the dates are in

### 16. MVP shout-out after each fight
A bubble at the end of co-op fights, reusing the RunStats counters.
- [ ] Brandi: decide how it should work. Problem: a support player would never win on damage. Ideas: rotate award
      categories (Most Damage, Most Block Given, Most Healing, Most Rants), give one award per player so everyone gets
      something, or make the awards joke titles instead of rankings

### 17. Family photo album
Save a snapshot after elite wins, boss wins, run victories, and defeats: the party, the floor, a caption built from the
fight's stats ("Goliath, down in 3 turns!"). Mockup (Brandi loved it): https://claude.ai/artifact/TihCPJBTNTpu8WsFgLschk
- Where it shows: a "Family Album" button on the main menu (leather scrapbook, taped polaroids, filters All / Victories /
  Elites / Defeats), a "Snapshot saved!" note after the moment, maybe a last page on the game over screen. Each player's
  machine keeps its own album (a profile file, not the run save).
- Both kinds of photo (decided 2026-10-28):
  - **Drawn pixel scene** when we have art for everyone in it: our characters, our monsters (elites, regular fights), the
    Mastiff and Kids. Foes mirrored to face left; beaten foes and fallen brothers tipped over.
  - **Real screenshot** (capture the game viewport at the moment of victory/defeat) for everything else: vanilla bosses and
    monsters, or a vanilla character in the co-op party.
  - If the moment can't be captured (a defeat on the map or at a rest site), draw just the party.
  - (Built: a defeat outside combat is a screenshot of wherever it happened; the VICTORY photo is the party alone.)
- [x] Build (2026-10-28): `Mechanics/Album.cs`, `AlbumTracker.cs`, `AlbumScreen.cs`
- [ ] Test: the main menu button (under Compendium), the album screen and its filters/pages, an elite win (drawn), a
      vanilla elite or boss (screenshot), a defeat in and out of combat, the "Saved" note, co-op (each player saves their own)
- [ ] Maybe: Tim's Kids in the photos (their art is drawn in code, so they're left out for now)

### 18. A custom boss
No custom boss yet (only elites and normal fights). Family-themed Act boss ideas: "Moving Day" (a mountain of boxes that
summons furniture), "Family Game Night Gone Wrong" (a giant board game that changes the rules every turn).
- [ ] Pick the boss and act
- [ ] Build (reuse the Bible elites' / regular fights' machinery)

### 19. Mom as an Ancient
An act-start Ancient (like Neow) offering family-flavored blessings: "Pack a lunch" (heal), "Take your brother with you"
(a Duo card), "Call me when you get there" (a relic). Uses the "the boys' mom" framing from the events.
- [ ] Check that BaseLib supports custom Ancients
- [ ] Design the blessings, build

### 20. Co-op rest site option: "Game Night"
Only with 2+ brothers/friends at the same campfire; everyone who picks it gets a shared bonus instead of resting alone.
Reuses the Stimm Lab rest site button plumbing.
- [ ] Design the bonus, build, test in co-op

### 21. Unlocks between runs
Meta-progression, e.g. winning with Tim unlocks a choice of second starter relic, or #14's wardrobe colors are earned.
- [ ] Decide what unlocks and how it's saved (a profile file, not the run save)

### 22. Custom run modifier: "Sibling Rivalry"
Brothers compete for a damage leaderboard; whoever is last at the end of each act gets a curse. Optional chaos for game night.
- [ ] Check how StS2 custom modifiers work with BaseLib, then design
