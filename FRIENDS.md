# The Friends: John and Huffman (dossier, 2026-10-27)

The brothers always play with two friends, John and Huffman. These two would be characters five and six.
This is the research and concept stage only: nothing is built yet. Open questions for Daniel are at the bottom.

Naming: the friend Joshua goes by **Huffman** everywhere in the mod (cards, IDs, files) so he never gets mixed up
with Joshua the brother. IDs would be `SPIREBROTHERS-HUFFMAN_*` and `SPIREBROTHERS-JOHN_*`.

## Who they are
- **Huffman is John's uncle.** John is his nephew, but they're close in age: both over 30, nearly 40. Write them as peers
  who grew up together, not as a grown-up and a kid.
- **John's grandma recently passed away.** Leave her out of the mod entirely: no lines, jokes or cards about her.
- **Huffman** worked with Daniel and Brandi. He was the IT guy: set up the servers for the company's websites and email,
  and handled purchases for the boss.
- Daniel and Brandi first met **John** at a Bible study at their house. (The others probably don't remember it, they've been
  gaming together so long.)
- **John and David are really good friends**: they chat and hang out online all the time, and when David comes to town he
  always goes to see John and Huffman.

## Telling them apart from the brothers (decided 2026-10-27)
- **Select buttons: the "plain" look.** Charcoal background with steel-grey rings (the brothers have colored rings) and the
  name in silver block letters (theirs are gold). Mockups: `art_originals/friendbuttons/` (its `out/compare_plain.png`).
  A "darktide" look (gunmetal frame, rivets, red purity seal) was tried and not picked.
- Their select description says **"Honorary Brother"**, in a different color from the brothers' text.
- They go after the four brothers in the character row.
- Background scenes: dark, night or underground versions of their own games (John in Enshrouded's Shroud, Huffman in a
  server room with AE2 drives and a jetpack). Drawn in pixelgen's `Backgrounds.cs` (`John`, `Huffman`); the friendbuttons
  program renders them to `out/<name>/select_bg.png`. John: Shroud fog, a Flame Altar, a hive city on the horizon, an anvil
  with a stimm, and a loot chest. Huffman: server racks, a storage drive wall, a jetpack, a Dyson sphere on the monitor, and a dog bowl.
- Pixel bodies are in `art_originals/pixelgen/Friends.cs`, drawn from their photos. John has short wavy hair, a big full beard,
  and a black tee. Huffman has short spiky dark hair, a goatee and mustache, a blue button-up, and an IT badge. Brandi says
  Huffman is accurate.

## What we know

| | John | Huffman |
|---|---|---|
| Darktide favorite | **Hive Scum** (added Dec 2025) | **Arbitrator** (the Dredd one, added June 2025) |
| Other games | Enshrouded, WoW Forever (favorite classes: waiting on Daniel) | Modded Minecraft, Total Annihilation, Dyson Sphere Program |
| What it says about him | Likes grindy games with deep systems: long-term progress, lots of knobs to tune | Builds things that run themselves at huge scale, and lays down the law |

Darktide is the overlap: Daniel plays it too (it's on his list in DESIGN.md), so Daniel, John and Huffman are a
natural Darktide strike team.

## The games, as mechanic material

### Hive Scum (John)
A Necromunda gang-ganger: dual autopistols, chems, dirty tricks.
- **Cartel Special / Stimm Lab**: the class builds its *own* custom stimm from a talent tree that levels up, and gives up
  found stimms to use it. A tuned, upgradable personal item is exactly the "grindy with complexity" itch.
- **Chem Toxin**: a damage-over-time the class applies; many talents trigger off applying it.
- **Blitz**: blinding grenade (stagger), Chem Grenade (lingering poison pool), Boom Bringer (one-shot rocket launcher).
- **Gunslinger aura**: picking up ammo refills a little for the whole team (co-op hook).

### Enshrouded (John)
Survival crafting. You venture into **the Shroud**, a fog you can only survive in for a limited time (a countdown);
the timer grows as you upgrade your **Flame Altar**. Base building, crafting stations you level up, a big skill tree.

### WoW Forever (John)
Blizzard's new Classic-style version of WoW (launches Nov 4, 2026): "crunchier" systems, levelling to 60, professions,
grinding for gear. Card hooks depend on his favorite classes (pending). Universal WoW ideas: Rested XP, talent points,
loot rolls (Need/Greed), a raid wipe, "Leeroy".

### Arbitrator (Huffman)
Adeptus Arbites: imperial law enforcement, very Judge Dredd.
- **Cyber-mastiff**: a named, customizable dog that attacks on its own; tag an enemy to send it in. (The Lone Wolf
  talent drops the dog for personal power.)
- **Nuncio-Aquila**: a drone that makes a zone where allies regain Toughness and enemies take +15% damage.
- **Castigator's Stance** (restore Toughness, take less damage), **Break the Line** (bash that staggers).
- Blitz: Arbites grenade, Voltaic Shock Mine, Remote Detonation (an electric blast centered on the dog).
- Dredd flavor: "I am the law", sentencing, verdicts. Original lines only, no quoting the films or comics.

### Total Annihilation (Huffman)
Classic RTS. Two resources, **metal and energy**, that flow per second rather than being spent in lumps. Your
**Commander** builds everything (and losing him loses the game). Huge armies, build queues, nanolathing.

### Dyson Sphere Program (Huffman)
Factory and logistics sim at solar-system scale: you start with a mech and end by building a sphere around a star,
one shell piece at a time. A mega-project that pays off slowly and then enormously.

### Modded Minecraft (Huffman)
He loves **tech packs**. Brandi doesn't remember all the names, but **Applied Energistics 2 (AE2)** was definitely one,
and they **always had jetpacks** (Simply Jetpacks / Iron Jetpacks style, powered by RF energy).
- **AE2**: a digital storage network. Items get turned into energy and stored on **storage cells** inside **drives**; one
  **ME terminal** reaches everything; **autocrafting** makes whatever you request from patterns; **channels** limit how
  many devices the network can run. Card ideas: *ME Network* (a power: cards you Retain or Exhaust get "stored" and can be
  pulled back later), *Storage Cell*, *Crafting Pattern* (pick a card; it gets made for you each turn), *Out of Channels*
  (a joke downside card), *Quartz Grindstone*.
- **Jetpack**: a natural card or relic for him: fly over a hit (Block or dodge), or reposition. Needs fuel: ties nicely
  into the Metal/Energy flow idea. Maybe his starter relic is a jetpack instead of something Darktide.
- AE2 also gives a second answer to "how is his factory different from Daniel's Gears": Daniel *builds* the parts, and Huffman
  *stores and requests* them.

## Overlap to steer around
The existing brothers already cover some of this ground, so each friend needs his own angle:
- **Daniel**: Factorio-style Gears and Assembly Line, Darktide, Minecraft. Huffman's factory side can't just be more Gears.
- **Tim**: Kids are pets (Osty-like), Script replays cards next turn (automation), Ages are run-long progress.
  John's run-long progress brushes against Ages. **Pets are fine to repeat** (Brandi, 2026-10-27): more pets are welcome
  even if they're close to Kids, as long as they do different things.
- **David**: Bleed is already the damage-over-time. John's Chem Toxin must work differently.
- **Joshua**: Roll/d20 covers randomness.

## Concept pitches (rough, to react to)

### John, The Grinder *(title placeholder: "The Hive Scum", "The Completionist", "Just One More Run")*
The one who plays the deepest systems and has the spreadsheet to prove it.
- **Grind** (signature): a Grind card permanently gets better each time it's played, saved for the whole run (like
  vanilla Genetic Algorithm). Daniel's Diligent resets every combat; Grind never does. Rewarded for sticking with
  the same cards.
- **Cartel Special** (starter relic): a personal stimm he can use once per combat that refills every fight. At rest sites
  he can spend a choice upgrading it (a small tree: Block, Strength, draw, energy) instead of resting/smithing. His own
  Stimm Lab, built over the run.
- **Chem Toxin**: a debuff that does nothing on its own. His "Chem" cards *react* with it: each one consumes or spreads
  Toxin for an effect. Pairs with the existing Insulted idea of one brother setting up, another cashing in.
- **Into the Shroud** (Enshrouded): a stance. While in it his cards are stronger, but a countdown ticks; when it runs out
  he takes damage and leaves. Upgrades lengthen the timer, like the Flame Altar.
- Co-op: Gunslinger-style "when you pick up X, the team gets a little too."
- WoW cards wait on his classes.

### Huffman, The Arbitrator *(title placeholder: "The Judge", "The Commander", "The IT Guy")*
The law, and he brought his dog and a production line. Also the office IT guy: servers, email, purchase orders.
- **Multi-Pass** (Brandi, 2026-10-27): he loves quoting the "Multipass" from *The Fifth Element* and acts like it gets him
  out of everything. Use the name only (no other lines from the film). It's his "get out of jail free" card, so it should
  get him out of things:
  - **Starter relic: decided** (Brandi, 2026-10-27): *Multi-Pass*. The first time each combat something bad would happen to him (a debuff,
    a hit that would break his Block, a Status card added to his deck), it doesn't. A little bubble: "Multi-Pass!"
    Upgraded version (Touch of Orobas) works twice per combat, or covers a teammate too.
  - **Card**: *Multi-Pass* (0 cost, Retain, Exhaust). Remove all your debuffs, and skip the enemy's next attack on you.
  - **Run-level gag** (more work, maybe later): events get an extra option "Show your Multi-Pass" that skips the event's
    cost. Or shops: one free item per run. Ties in with the IT guy's office badge too.
  - **Banter: must have** (everyone really does roll their eyes at it). When the relic fires, Huffman says his line and,
    if someone else is in the party, they answer with an eye-roll. New pools in the existing Banter system:
    `MULTIPASS.HUFFMAN` (his brag) and `MULTIPASS.<NAME>` (the reply), once per combat so it never gets old.
    Draft lines, all original (Brandi, reword freely):
    - Huffman: "Multi-Pass!" / "Can't touch this. Multi-Pass." / "Says right here I'm exempt." / "Multi-Pass. Next."
    - Daniel: "That's not a real thing." / "It's a laminated piece of cardboard."
    - David: "That pass expired in 1997." / "I've run the numbers. It still shouldn't work."
    - Joshua: "Every single time." / "Somebody take that thing away from him."
    - Tim: "Kids, don't let Huffman teach you that." / "My kids have better excuses than that."
    - John: "Uncle Huffman, please." / "You've been pulling that since we were kids." / "One of these days that thing
      is going to fail you."
    - Solo (no one to answer): Huffman to himself: "Multi-Pass. Nobody saw that."
  - Build note: the rest of the mod's bubbles go through `Banter.Bubble`, and pool sizes are counted with
    `LocString.Exists`, so adding lines later needs no code change.
- **IT cards**: Reboot, Server Room, Spam Filter (blocks the enemy's debuffs), Purchase Order (the boss's credit card:
  a shop or gold card), Uptime. These fit his "runs itself" side well.
- **The Mastiff** (signature): one dog companion that stands with him (Necrobinder's Osty is the vanilla model, and
  the Necrobinder is also the one vanilla body no brother has used yet). Unlike Tim's Kids, it's one dog you *command*:
  **Sic 'Em** cards mark a target, and the dog attacks it. Ask what his in-game dog is named and use it. Can reuse the
  Kids code (`Mechanics/Kids.cs`, `Monsters/Kid.cs`, `KidLayout`) for standing at his feet. Room for more: an AE2-style
  helper drone or the Nuncio-Aquila drone as a second pet that buffs instead of attacking.
- **Laws** (Dredd): Power cards that set a rule for the fight: "No enemy may gain Block," "Whoever attacks a teammate
  is Sentenced." When an enemy breaks a Law, it gets punished (damage, Stun, Vulnerable). The enemy's own intents
  become the trigger, which nobody else in the mod does.
- **Sentence**: a debuff stacking up "charges" that a Verdict card executes. (Close to Insulted/Comeback; maybe fold
  it into Laws instead.)
- **Metal and Energy** (Total Annihilation): a second resource that *flows*. Some cards give +Metal per turn, and
  Build cards spend Metal. Strong engine for a long fight, slow start.
- **Dyson Sphere**: a once-a-run mega-project. Shell pieces come from cards and elites. When it's finished, the payoff is
  huge. (Similar in shape to Tim's Ages; pick one of the two for Huffman or make it a single rare power instead.)
- Co-op: the Nuncio-Aquila drone as a team buff zone (allies gain Block, enemies take more damage).

### Possible bonds (Family Bonds style, a new "Strike Team" set)
- Daniel + John + Huffman: **Strike Team** (the Darktide crew). Effect TBD.
- **John + Huffman: Uncle and Nephew.** Family bond. Something handed down: Huffman's first Build/Law each combat also
  gives John a Grind tick, or John's stimm also hits his uncle.
- **John + David: best friends** (chat all the time). The strongest pairing of the bunch, so it deserves a good bond and a
  Duo card each way. David is the support player and John the systems grinder: maybe David's gold or Exact feeds John's Grind.
- **Huffman + Daniel: coworkers** (the IT guy and the software engineer). "Have you tried turning it off and on again?"
  Servers, email, the boss's purchase orders. Natural Duo card material.
- Visiting town: David always drops in on John and Huffman, which could be an event (a Road Trip–style shared event).
- **Bible study** (where Daniel and Brandi met John): a small nod fits next to the Bible elites. Maybe John's version of
  the hero line, or a Banter callback nobody else remembers.
- StS2 co-op caps at 4 players, so the six of them can never all be in one run. Bonds should be pairs and trios.

## Questions for Daniel
1. John's favorite WoW Forever classes (already asked).
2. Huffman's favorite Minecraft mods: partly answered (tech packs, AE2, jetpacks). Any others Daniel remembers?
3. What's Huffman's cyber-mastiff named in Darktide? John and Huffman's character names?
4. How do they each play? Who carries, who dies first, who loots everything, who's always AFK?
5. Catchphrases, running jokes, things they always say on voice chat.
6. Their favorite StS character and playstyle (support, big damage, weird combos?).
7. Jobs, hobbies, family, anything outside games we can put on a card (and anything that's off limits).
8. Anything they'd *hate* to see (like Tim's "Tidbit")?
9. ~~How do they know the brothers?~~ Answered (see "Who they are"). Still useful: any stories from David's visits, or
   from the office days with Huffman?
