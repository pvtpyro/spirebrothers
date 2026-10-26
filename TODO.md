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
Rare cards that only show up in rewards when a specific brother is also in the party.
- [ ] Brainstorm one or two per pair. Starting ideas: "LAN Party" (Daniel + Tim, everyone draws 2),
      "Split the Check" (David + Joshua)
- [ ] Figure out how to filter reward pools by party makeup (co-op safe)
- [ ] Build, add glyphs.txt lines, test

## Content gaps

### 4. Potions for David, Joshua, and Tim
Only Daniel has potions of his own right now (plus the shared Monkey Island ones).
- [ ] Joshua: Ramune (gain 3 Verses), more TBD
- [ ] David: Gas Station Coffee (Exact counts this turn even at 1 energy), more TBD
- [ ] Tim: Juice Box (+2 Kids), more TBD
- [ ] Add each to its brother's pool, plus art lines

### 5. Custom ? room events
- [ ] Check BaseLib support for custom events (biggest unknown)
- [ ] Event ideas: "Thanksgiving at Mom's" (pick a dish), "The Group Chat" (co-op vote),
      "Road Trip Detour" (David's Idaho-to-NJ trip)

### 6. Sound effects
- [ ] Joshua: guitar strum on Chorus
- [ ] David: register "cha-ching" on gold cards
- [ ] Tim: kids giggling when they hit
- [ ] Daniel: TBD
- [ ] Source or record sounds (original or properly licensed)

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