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
| David | Patron of the Arts | Joshua | 1 Skill, Exact | Lose 15 Gold; Joshua gains 3 Verses; ALL gain 6 Block. Exact: keep the Gold |
| David | IOU | Tim | 0 Skill, Exact, Exhaust | Tim gains 30 Gold. Exact: you gain 30 too |
| Joshua | Hype Man | Daniel | 1 Skill, Song, Share | A player gains 3 Strength this turn |
| Joshua | Minor Key | David | 1 Skill, Chorus | 1 Bleed per Verse to ALL enemies |
| Joshua | Babysitting | Tim | 1 Skill, Song | Tim gains 1 Kid; ALL gain 4 Block |
| Tim | LAN Party | Daniel | 1 Skill, Script | ALL gain 5 Block and draw 1 next turn |
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
- [x] David: Gas Station Coffee (this turn, Exact also triggers at 1 energy), Trail Mix (heal 10, ALL players 6 Block),
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

### 7. Monkey Island elite
A pirate enemy who insults *you*; Comeback cards counter him. Original insults only.
- [ ] Check BaseLib support for custom monsters/encounters
- [ ] Design moveset and art

### 8. End-of-run stats
"David hoarded 812 gold", "Tim's kids dealt 340 damage", "Joshua healed the team for 210".
- [ ] Pick the stats per brother and where to show them (victory/defeat screen?)
- [ ] Track them over the run (co-op safe)

### 9. A fifth character: Brandi
A surprise extra character as part of the gift.
- [ ] Design concept, mechanic, and starter deck
- [ ] Card list (like the other *_CARDS.md files)


### 10. Anchient Card Options

### 11. Status, Curse and Quest cards maybe?