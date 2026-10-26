# Brother Banter

Speech bubbles over a brother in combat, using the same bubble as the insult cards (at least 4 seconds, longer for
long lines). Built 2026-10-26.

**Where the lines live in the game:** `SpireBrothers/localization/eng/powers.json`, as `SPIREBROTHERS-BANTER.*` keys.
This file is the planning copy; changing a line here does nothing until it's changed in powers.json too.
To add a line, add the next number to that group, e.g. after `SPIREBROTHERS-BANTER.SOLO.DAVID.2` add
`SPIREBROTHERS-BANTER.SOLO.DAVID.3`. The code counts the lines itself, so nothing else needs changing.

Not built: the Archipelago trap line. Joshua already shows a "sent it to..." bubble when a trap goes out, and a second
bubble would cover it.

Rules I followed:
- Short. A bubble shows for about 4 seconds, so roughly 60 characters or fewer.
- Original lines only. No quotes from games, just references.
- Teasing but warm. No "Tidbit" (that stays on its one card).
- Not too often: combat-start banter would trigger in about 1 of every 3 fights, and each other trigger at most once per combat.

---

## 1. Combat start (2+ brothers in the party)

One brother says something to another. Lines are grouped by **speaker → listener**.

**Daniel → David**
- "David, stop doing the math. Just hit it."
- "And you know what? I already have a plan."
- "You're thinking about the meaning of life again, aren't you."

**Daniel → Joshua**
- "Josh, can you hear me? I wired this place myself."
- "Play something heroic. I'm going in."
- "Is it morning or night in Japan right now?"

**Daniel → Tim**
- "Tim, I have a script for that. Mine's better."
- "Are the kids wired in? They should be wired in."
- "Bet your Lua can't do this."

**David → Daniel**
- "Statistically, your plan has a 12% chance of working."
- "Before we start, have you ever wondered why we fight?"
- "Don't break anything you can't fix. So, nothing."

**David → Joshua**
- "Josh, if you write a song about this, I want royalties."
- "I'll handle the numbers. You handle the feelings."
- "We're support players. Somebody has to be the adult."

**David → Tim**
- "Tim. Put the wallet down."
- "Every Gold you spend makes me a little sadder."
- "I'm not lending you money. Again." *(fits Family Loan)*

**Joshua → Daniel**
- "Daniel, try not to explain the enemy's code to it."
- "I'll heal you. Try not to need it."
- "Your ethernet's great, but your lag is in your decisions."

**Joshua → David**
- "Hey David, you okay? Like, actually okay?"
- "Cheer up! I wrote you a song. It's in a minor key."
- "Ready to send you whatever I find. Probably a trap."

**Joshua → Tim**
- "Tim! How many kids today? All of them?"
- "Kids, on three. One, two..."
- "What a save that's going to be." *(Rocket League nod)*

**Tim → Daniel**
- "Daniel, I drafted a plan. To scale."
- "I've already automated this fight. Mostly."
- "Kids, stay behind Uncle Daniel."

**Tim → David**
- "David, how much do you have? Asking for me."
- "I bought the best sword in the shop. You're welcome."
- "Trust me, it was a great deal."

**Tim → Joshua**
- "Josh, play the one the kids like."
- "Welcome home! Now help me with these monsters."
- "Advancing to the next Age. Musically."

---

## 2. Solo combat start (only one brother)

Talking to himself, or to the brothers who aren't there. About 1 in 4 fights.

**Daniel**
- "And you know what? I don't need help."
- "This would go faster with a multimeter."
- "What I mean by that is..."

**David**
- "Alone again. Is that a metaphor?"
- "Running the numbers. They're not great."
- "Maybe I should replace these 15 year old shoes. Na."

**Joshua**
- "Guess this one's a solo."
- "Should have brought the guitar. Oh wait."
- "I miss Japan."

**Tim**
- "Just me and eight kids. Easy."
- "Let's get this drafted and done before dinner."
- "I wonder how much that new gear is?"

---

## 3. A brother falls

Another living brother reacts, picked at random.
- "Nooo! Hold on, I'm coming!"
- "I'm telling Mom."
- "We're not leaving without you."
- "That's coming out of your share."
- "Who's going to carry now?"

Specific reactions, when that brother is the one who falls (or reacts):
- **David falls:** Tim: "Can I have his gold?"
- **Tim falls:** Joshua: "Kids, don't look!"
- **Daniel falls:** Daniel's last line: "The cat stepped on my keyboard again."
- **Joshua falls:** David: "Who's going to heal us now? Who heals the healer?"

## 4. A brother gets saved (Nobody Gets Left Behind, I'm Not Done Yet, Lizard Tail)

The saved brother says:
- "Not today."
- "I'm not done yet!" *(Daniel only; matches his card)*
- "Told you we don't leave anyone behind."
- "Ow. Okay. Okay, I'm good."

---

## 5. Big moments (solo or co-op, once per combat)

| Moment | Speaker | Lines |
|---|---|---|
| Daniel finishes Orbital Barrage | Daniel | "Danger close!" / "Stand back. Way back." |
| Daniel's 5th Gear in a combat | Daniel | "I could build a car out of these." |
| David passes 300 Gold | David | "I don't know what to buy. I never know what to buy." / "It's not saving if you just never spend it." |
| David's 4th Rant in one turn | David | "And another thing about existence..." / "Nobody asked. I'm answering anyway." |
| Joshua plays a Chorus with 8+ Verses | Joshua | "Everybody sing!" / "This is the big finish!" |
| Joshua's Check sends a trap to an enemy | Joshua | "Got a trap. It's yours now." |
| Tim reaches 8 Kids | Tim | "That's all of them. Everybody's here." / "Head count: eight. Good." |
| Tim Ages Up | Tim | "Advancing to the next Age!" / "Worth every coin." |
| Tim reaches Imperial | Tim | "Imperial. Now I need the best of everything." |

---

## 6. Victory

### After a boss

A random brother:
- "GG. Same time tomorrow?"
- "GG. Another round?"
- "Group photo! Everybody in!"
- "And you know what? We did it." *(Daniel)*
- "That was statistically improbable." *(David)*
- "Somebody tell Mom!"
- "Dad would be so proud!"

### After an elite

One line, about half the time:
- "Easy." / "Too easy." / "Let's never do that again."

---

## Questions for you
1. **Catchphrases:** what does each of them actually say all the time? Those should replace my **(fill in)** lines.
2. **Mom/Dad jokes:** okay to mention their parents, or leave family members out (besides Tim's kids)?
3. **Wives:** "Wife Aggro" is already a card. Want any banter about the wives (including you)?
4. **Frequency:** is 1 in 3 fights about right for combat start, or would they rather see it more often?
