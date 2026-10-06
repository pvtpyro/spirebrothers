# David's custom card icons (see icons.ps1 for the helpers). Coordinates are in the 1000 x 760 card picture.

$Emerald = C '#3ed46a'; $Purple = C '#a060e0'; $Cash = C '#7cc46a'; $Bag = C '#4a4a58'

# ---- Minecraft

# A beacon: glass block with the glowing core, beam shooting up, on a pyramid of gold blocks.
$CustomIcons['Beacon'] = {
  @(
    (Part (PRect 470 90 60 250) (C '#bff4ff')),                                           # beam
    (Part (PPixels @('  XXXX  ', ' XXXXXX ', 'XXXXXXXX') 260 520 60) $Yellow),            # gold pyramid
    (Part (PRect 380 330 240 200) (C '#d8f8ff')),                                         # glass block
    (Part (PRect 440 390 120 110) $Diamond),                                              # core
    (Detail (PLine @(380, 430, 620, 430)) 8), (Detail (PLine @(500, 330, 500, 390)) 8)
  )
}

# The End Poem: the End portal frame with its eyes, and the dark void inside.
$CustomIcons['TheEndPoem'] = {
  $frames = foreach ($f in @(260, 300), @(440, 220), @(620, 300), @(260, 480), @(620, 480), @(440, 560)) {
    (Part (PRect $f[0] $f[1] 120 100) (C '#d8d8a0')), (Part (PEll ($f[0] + 60) ($f[1] + 40) 26 22) (C '#2a8a6a')), (Detail (PEll ($f[0] + 60) ($f[1] + 40) 8 8) 10)
  }
  @((Part (PRect 330 330 340 260) $Dark)) + @(foreach ($s in @(400, 400), @(560, 380), @(480, 500), @(600, 520)) { (Part (PEll $s[0] $s[1] 8 8) (C '#c8f0ff')) }) + $frames
}

# Thorns III: an enchanted book, shimmering purple, with a thorny vine on the cover.
$CustomIcons['ThornsIii'] = {
  @(
    (Part (PRound 300 190 400 420 24) $Purple),                                           # cover
    (Part (PRect 660 210 40 380)),                                                        # page edges
    (Detail (PLine @(670, 240, 670, 560)) 6),
    (Detail (PLine @(380, 560, 430, 470, 400, 400, 450, 330, 420, 250)) 14),              # thorny vine
    (Part (PPoly @(425, 482, 380, 470, 412, 446)) (C '#5ac05a')), (Part (PPoly @(425, 342, 475, 330, 452, 362)) (C '#5ac05a')),
    (Part (PPoly @(408, 410, 380, 396, 400, 386)) (C '#c8ccd4')), (Part (PPoly @(436, 290, 470, 280, 452, 302)) (C '#c8ccd4')),
    (Part (PPoly @(560, 250, 575, 290, 615, 305, 575, 320, 560, 360, 545, 320, 505, 305, 545, 290)) (C '#f0d8ff')),   # enchantment sparkle
    (Part (PPoly @(560, 430, 568, 452, 590, 460, 568, 468, 560, 490, 552, 468, 530, 460, 552, 452)) (C '#f0d8ff'))
  )
}
# Villager Trading: an emerald, pixel style.
$CustomIcons['VillagerTrading'] = {
  @((Part (PPixels @('   XX   ', '  XXXX  ', ' XXXXXX ', 'XXXXXXXX', 'XXXXXXXX', ' XXXXXX ', '  XXXX  ', '   XX   ') 300 160 56) $Emerald),
    (Part (PPixels @('X ', 'XX', ' X') 412 272 28) (C '#c8ffd8')))
}

# ---- Payday 2

# All In: the heist clown mask.
$CustomIcons['AllIn'] = {
  @(
    (Part (PEll 500 380 210 240)),
    (Part (PEll 420 320 50 40) $Dark), (Part (PEll 580 320 50 40) $Dark),
    (Part (PPoly @(370, 280, 420, 220, 470, 280)) $Red), (Part (PPoly @(530, 280, 580, 220, 630, 280)) $Red),
    (Part (PEll 500 410 40 40) $Red),
    (Part (PPoly @(370, 480, 500, 540, 630, 480, 600, 520, 500, 580, 400, 520)) $Red),
    (Detail (PLine @(420, 500, 580, 500)) 8)
  )
}

# Drill: the thermal drill set up on a vault door.
$CustomIcons['Drill'] = {
  @(
    (Part (PEll 340 400 150 230) (C '#9aa0aa')), (Detail (PEll 340 400 100 160) 12), (Part (PEll 340 400 36 36) $Dark),
    (Part (PRound 420 300 300 170 30)),                                                   # drill body
    (Part (PRect 690 340 90 90) $Dark), (Part (PPoly @(780, 350, 860, 385, 780, 420)) (C '#c8ccd4')),  # bit
    (Part (PRound 470 330 120 60 14) (C '#7cf07c')),                                      # timer screen
    (Part (PRect 470 470 30 120)), (Part (PRect 640 470 30 120)), (Part (PRect 440 580 260 30))       # legs
  )
}

# Loot Bag: the big grey duffel bag, stuffed.
$CustomIcons['LootBag'] = {
  @(
    (Detail (PArc 500 300 110 200 140) 26),                                               # handles
    (Part (PRound 260 290 480 300 120) $Bag),
    (Detail (PLine @(300, 360, 700, 360)) 12),
    (Part (PRect 360 450 90 60) $Cash), (Part (PRect 470 440 90 60) $Cash), (Part (PRect 580 450 90 60) $Cash)
  )
}

# Money Bags: two duffels with cash spilling out.
$CustomIcons['MoneyBags'] = {
  @(
    (Part (PRound 230 320 300 230 90) $Bag), (Part (PRound 470 290 300 260 100) $Bag),
    (Detail (PLine @(500, 380, 740, 380)) 12),
    (Part (Rot (PRect 300 520 150 70) -10 375 555) $Cash), (Part (Rot (PRect 500 540 150 70) 8 575 575) $Cash), (Part (Rot (PRect 640 500 140 66) -18 710 533) $Cash),
    (Part (GlyphPath '$' 330 420 70 90 $bold) $Cash), (Part (GlyphPath '$' 620 410 70 90 $bold) $Cash)
  )
}

# Heist Planner: the blueprint with the route and the X.
$CustomIcons['HeistPlanner'] = {
  @(
    (Part (PRound 240 180 520 420 20) (C '#3f78c8')),
    (Detail (PLine @(300, 260, 460, 260, 460, 380, 300, 380, 300, 260)) 8), (Detail (PLine @(520, 260, 700, 260, 700, 520, 520, 520, 520, 260)) 8),
    (Detail (PLine @(330, 540, 400, 470, 480, 470, 560, 400, 620, 400)) 14),
    (Part (Rot (PRect 600 380 90 20) 45 645 390) $Red), (Part (Rot (PRect 600 380 90 20) -45 645 390) $Red)
  )
}

# ---- ARK

# Bola: two stone weights spinning on a cord.
$CustomIcons['Bola'] = {
  @((Detail (PArc 500 400 170 200 230) 18), (Detail (PLine @(500, 400, 360, 520)) 14), (Detail (PLine @(500, 400, 660, 280)) 14),
    (Part (PEll 340 540 80 80) (C '#a8a0a0')), (Part (PEll 680 260 80 80) (C '#a8a0a0')), (Part (PEll 500 400 30 30) $Wood),
    (Detail (PArc 340 540 50 200 80) 10), (Detail (PArc 680 260 50 200 80) 10))
}

# Plant Species X: the spiky defensive plant, mouth open.
$CustomIcons['PlantSpeciesX'] = {
  @(
    (Part (PPoly @(330, 640, 380, 520, 440, 640)) (C '#3a8a3a')), (Part (PPoly @(560, 640, 620, 520, 670, 640)) (C '#3a8a3a')),
    (Part (PRect 470 430 60 220) (C '#3a8a3a')),
    (Part (PEll 500 330 200 140) (C '#6ac04a')),
    (Part (PPoly @(330, 330, 670, 330, 600, 400, 400, 400)) $Dark),
    (Part (PPoly @(380, 330, 400, 360, 420, 330, 440, 360, 460, 330, 480, 360, 500, 330, 520, 360, 540, 330, 560, 360, 580, 330, 600, 360, 620, 330))),
    (Part (PPoly @(300, 260, 250, 200, 330, 230)) (C '#6ac04a')), (Part (PPoly @(700, 260, 750, 200, 670, 230)) (C '#6ac04a')),
    (Part (PPoly @(500, 190, 480, 130, 520, 130)) (C '#6ac04a'))
  )
}

# Tranq Dart: a dart with a fluffy tail and a green dose.
$CustomIcons['TranqDart'] = {
  $d = Rot (PRound 320 360 360 60 30) -30 500 390
  @(
    (Part (Rot (PPoly @(680, 370, 800, 390, 680, 410)) -30 500 390) (C '#c8ccd4')),
    (Part $d), (Part (Rot (PRound 420 372 160 36 18) -30 500 390) $Emerald),
    (Part (Rot (PPoly @(330, 360, 220, 300, 250, 390, 220, 480, 330, 420)) -30 500 390) $Red)
  )
}

# ---- Terraria

# Spike Trap Room: a row of floor spikes in a dungeon brick room.
$CustomIcons['SpikeTrapRoom'] = {
  $spikes = foreach ($x in 260, 360, 460, 560, 660) { (Part (PPoly @($x, 520, ($x + 40), 330, ($x + 80), 520)) (C '#c8ccd4')) }
  @((Part (PRect 220 520 560 120) (C '#5a6aa0')), (Detail (PLine @(220, 580, 780, 580)) 10), (Detail (PLine @(400, 520, 400, 580)) 10), (Detail (PLine @(600, 580, 600, 640)) 10)) + $spikes +
    @((Part (PEll 500 250 40 40) $Red), (Detail (PLine @(500, 210, 500, 150)) 12))
}

# ---- Satisfactory

# Jetpack: two tanks with thruster flames.
$CustomIcons['Jetpack'] = {
  @(
    (Part (PPoly @(370, 520, 430, 520, 410, 650)) $Orange), (Part (PPoly @(570, 520, 630, 520, 590, 650)) $Orange),
    (Part (PPoly @(385, 520, 415, 520, 400, 600)) $Yellow), (Part (PPoly @(585, 520, 615, 520, 600, 600)) $Yellow),
    (Part (PRound 330 200 140 330 60) (C '#f0a040')), (Part (PRound 530 200 140 330 60) (C '#f0a040')),
    (Part (PRect 450 260 100 200)),
    (Part (PRect 360 500 80 30) $Dark), (Part (PRect 560 500 80 30) $Dark),
    (Detail (PLine @(470, 300, 530, 300)) 10)
  )
}

# Nobelisk: the cylinder charge with its glowing detonator light.
$CustomIcons['Nobelisk'] = {
  @(
    (Detail (PLine @(320, 260, 280, 210)) 12), (Detail (PLine @(300, 300, 240, 290)) 12),
    (Part (Rot (PRound 330 290 360 180 60) -20 510 380) (C '#f0a040')),
    (Part (Rot (PRect 400 290 40 180) -20 510 380) $Dark), (Part (Rot (PRect 580 290 40 180) -20 510 380) $Dark),
    (Part (PEll 330 330 40 40) $Red), (Part (PEll 330 330 16 16) (C '#ffe0e0'))
  )
}

# Power Slug: the glowing slug blob.
$CustomIcons['PowerSlug'] = {
  @(
    (Part (PEll 500 400 230 170) (C '#6ae0ff')), (Part (PEll 500 400 150 105) (C '#b8f4ff')), (Part (PEll 470 370 60 40)),
    (Part (PEll 330 460 60 60) (C '#6ae0ff')), (Part (PEll 670 470 50 50) (C '#6ae0ff')),
    (Detail (PEll 420 380 10 12) 12), (Detail (PEll 560 380 10 12) 12), (Detail (PArc 490 420 30 20 140) 10)
  )
}
