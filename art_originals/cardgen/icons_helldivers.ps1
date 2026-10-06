# Daniel's Helldivers 2 stratagems (see icons.ps1 for the helpers).

# An Eagle jet dropping a line of bombs.
$CustomIcons['StratagemEagleAirstrike'] = {
  @(
    (Part (PPoly @(250, 260, 720, 260, 780, 290, 720, 320, 250, 320, 280, 290))),
    (Part (PPoly @(430, 260, 560, 150, 610, 150, 540, 290))), (Part (PPoly @(430, 320, 560, 420, 610, 420, 540, 290))),
    (Part (PPoly @(250, 260, 300, 200, 330, 200, 310, 280))),
    (Part (PRound 640 270 80 30 14) $Glass),
    (Part (PRound 380 460 40 80 20) $Dark), (Part (PRound 500 520 40 80 20) $Dark), (Part (PRound 620 580 40 80 20) $Dark)
  )
}

# Shells raining down from orbit onto a wall of fire.
$CustomIcons['StratagemOrbitalBarrage'] = {
  $parts = @()
  foreach ($s in @(330, 220), @(500, 160), @(670, 240)) {
    $parts += (Detail (PLine @(($s[0] + 60), ($s[1] - 120), $s[0], $s[1])) 14)
    $parts += (Part (Rot (PRound ($s[0] - 20) ($s[1] - 30) 40 90 20) 25 $s[0] $s[1]) $Dark)
  }
  $parts + @(
    (Part (PPoly @(240, 600, 280, 470, 340, 540, 400, 420, 460, 530, 540, 400, 600, 520, 660, 440, 720, 560, 760, 600)) $Orange),
    (Part (PPoly @(330, 600, 380, 520, 450, 570, 530, 480, 600, 570, 660, 520, 690, 600)) $Yellow)
  )
}

# One huge beam straight down from the ship.
$CustomIcons['StratagemOrbitalRailcannon'] = {
  @((Part (PRect 455 140 90 430) (C '#cfe8ff')), (Part (PRect 485 140 30 430)),
    (Part (PEll 500 590 200 50) $Orange), (Part (PEll 500 590 110 26) $Yellow), (Part (PEll 500 140 80 44) (C '#c8ccd4')))
}

# A reinforcement drop pod coming in hot.
$CustomIcons['StratagemReinforce'] = {
  @(
    (Part (PPoly @(440, 150, 560, 150, 590, 230, 410, 230)) $Orange), (Part (PPoly @(460, 100, 540, 100, 560, 150, 440, 150)) $Yellow),
    (Part (PRound 400 210 200 330 40)),
    (Part (PRound 440 270 120 120 30) $Glass),
    (Part (PPoly @(400, 500, 330, 620, 400, 600))), (Part (PPoly @(600, 500, 670, 620, 600, 600))), (Part (PPoly @(470, 520, 530, 520, 520, 640, 480, 640)))
  )
}

# The resupply pod: a capsule with yellow hazard bands and an ammo crate inside.
$CustomIcons['StratagemResupply'] = {
  @(
    (Part (PRound 390 150 220 450 50)),
    (Part (PRect 390 250 220 40) $Yellow), (Part (PRect 390 470 220 40) $Yellow),
    (Part (PRound 440 310 120 140 14) $Dark), (Detail (PLine @(470, 340, 530, 340)) 10),
    (Part (PPoly @(390, 560, 320, 640, 400, 620))), (Part (PPoly @(610, 560, 680, 640, 600, 620)))
  )
}

# A shield generator: the dome bubble over the little generator.
$CustomIcons['StratagemShieldGenerator'] = {
  @((Part (PArc 500 570 280 180 180) (C '#bfe8ff')), (Part (PRect 210 564 580 14)),
    (Part (PRound 440 470 120 100 18)), (Part (PRect 470 430 60 40) $Glass), (Detail (PLine @(500, 430, 500, 330)) 12), (Part (PEll 500 320 22 22) $Glass))
}

# The 500kg bomb.
$CustomIcons['StratagemEagle500kgBomb'] = {
  @((Part (PPoly @(420, 150, 580, 150, 560, 230, 440, 230))), (Part (PEll 500 400 140 200)), (Part (PRect 360 380 280 30) $Yellow),
    (Part (PPoly @(470, 590, 530, 590, 560, 650, 440, 650))), (Part (GlyphPath '500' 500 470 120 50 $bold) $Dark))
}

# A cluster of little bomblets scattering.
$CustomIcons['StratagemEagleClusterBomb'] = {
  $parts = @()
  foreach ($b in @(320, 260), @(470, 200), @(620, 280), @(380, 430), @(560, 420), @(470, 580), @(680, 520), @(300, 570)) {
    $parts += (Part (PEll $b[0] $b[1] 44 44) $Dark); $parts += (Part (PEll ($b[0] - 12) ($b[1] - 12) 10 10))
  }
  $parts
}

# A machine gun sentry on its tripod.
$CustomIcons['StratagemMachineGunSentry'] = {
  @(
    (Part (PPoly @(500, 430, 330, 640, 370, 640))), (Part (PPoly @(500, 430, 630, 640, 670, 640))), (Part (PPoly @(500, 430, 485, 650, 515, 650))),
    (Part (PRound 380 330 240 120 30)),
    (Part (PRect 600 360 220 40)), (Part (PRect 800 350 30 60) $Dark),
    (Part (PRound 300 360 90 100 20) $Yellow),
    (Part (PRound 420 280 120 60 20) $Glass)
  )
}

# The orbital laser carving a line into the ground.
$CustomIcons['StratagemOrbitalLaser'] = {
  @((Part (PPoly @(490, 90, 510, 90, 560, 540, 440, 540)) (C '#ff6a5a')), (Part (PPoly @(497, 90, 503, 90, 520, 540, 480, 540)) (C '#ffe0d8')),
    (Part (PEll 500 570 220 60) $Orange), (Part (PEll 500 570 120 30) $Yellow))
}

# A single shell falling onto a target marker.
$CustomIcons['StratagemOrbitalPrecisionStrike'] = {
  @((Part (PEll 500 480 210 140) (C '#ffd8d0')), (Detail (PEll 500 480 130 86) 12), (Detail (PLine @(300, 480, 700, 480)) 10), (Detail (PLine @(500, 350, 500, 610)) 10),
    (Detail (PLine @(640, 90, 545, 330)) 16), (Part (Rot (PRound 495 300 50 120 25) 22 520 360) $Dark))
}
