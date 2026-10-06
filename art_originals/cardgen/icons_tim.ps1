# Tim's custom card icons (see icons.ps1 for the helpers; the Rocket League car and ball come from icons_joshua.ps1).
# Coordinates are in the 1000 x 760 card picture.

$NVGreen = C '#7cff6a'; $Stone = C '#b8b4ac'; $LegoRed = C '#e0483c'; $LegoBlue = C '#3a7ad8'; $LegoYellow = C '#f2c530'; $Robe = C '#c87a3a'

function LegoBrick($x, $y, $studs, $color, $deg = 0) {
  $w = $studs * 70
  $parts = @()
  for ($i = 0; $i -lt $studs; $i++) { $parts += (Part (Rot (PRect ($x + 12 + $i * 70) ($y - 26) 46 30) $deg ($x + $w / 2) ($y + 40)) $color) }
  $parts + @((Part (Rot (PRect $x $y $w 84) $deg ($x + $w / 2) ($y + 40)) $color))
}

# ---- Rocket League

# Bump: one car slamming into the side of another.
$CustomIcons['Bump'] = {
  @((Part (PPoly @(470, 300, 500, 220, 530, 300, 600, 260, 560, 330, 620, 380, 540, 380, 520, 450, 480, 380, 400, 400, 450, 330, 390, 270)) $Yellow)) +
    (RLCar 120 560 0.95 $CarBlue 0) + (RLCar 560 520 0.9 $CarOrange 18)
}

# Fifty-Fifty: two cars hitting the ball from both sides at once.
$CustomIcons['FiftyFifty'] = {
  # the orange car is the blue one mirrored, nose pointing left
  (RLCar 120 560 0.85 $CarBlue -8) + (RLBall 500 420 95) + @(
    (Part (Rot (PPoly @(880, 560, 565, 560, 565, 522, 642, 479, 752, 462, 854, 483, 880, 543)) 8 720 520) $CarOrange),
    (Part (Rot (PPoly @(778, 468, 744, 432, 668, 432, 634, 468)) 8 720 520) $Glass),
    (Part (PEll 812 568 34 34)), (Part (PEll 634 543 34 34)), (Part (PEll 812 568 12 12) $Dark), (Part (PEll 634 543 12 12) $Dark)
  )
}
# Flip Reset: the car upside down, all four wheels touching the ball.
$CustomIcons['FlipReset'] = {
  (RLBall 500 250 120) + (RLCar 315 500 1.0 $CarBlue 180) + @((Detail (PArc 500 250 170 200 50) 14), (Detail (PArc 500 250 170 290 50) 14))
}

# Grand Champion: the winged rank emblem.
$CustomIcons['GrandChampion'] = {
  @(
    (Part (PPoly @(380, 330, 200, 220, 230, 300, 180, 330, 260, 380, 220, 430, 380, 430)) (C '#d8c8ff')),
    (Part (PPoly @(620, 330, 800, 220, 770, 300, 820, 330, 740, 380, 780, 430, 620, 430)) (C '#d8c8ff')),
    (Part (PPoly @(500, 160, 640, 230, 620, 450, 500, 600, 380, 450, 360, 230)) (C '#c02a4a')),
    (Part (PPoly @(500, 230, 580, 270, 565, 430, 500, 520, 435, 430, 420, 270)) (C '#ff6a8a')),
    (Part (PPoly @(500, 290, 520, 340, 570, 345, 530, 375, 545, 425, 500, 395, 455, 425, 470, 375, 430, 345, 480, 340)) $Yellow)
  )
}

# Musty Flick: the car flipping backwards and flicking the ball up with its nose.
$CustomIcons['MustyFlick'] = { (RLCar 330 560 1.0 $CarOrange -55) + (RLBall 640 200 90) + @((Detail (PArc 520 390 220 200 110) 14)) }

# Overtime: the scoreboard with the +0:00 overtime clock.
$CustomIcons['Overtime'] = {
  @((Part (PRound 210 210 580 220 30) $Dark), (Plain (GlyphPath '+0:00' 500 320 420 120 $bold) $Yellow),
    (Part (PRound 360 470 280 90 30) $Red), (Plain (GlyphPath 'OVERTIME' 500 515 220 46 $bold) (C '#ffffff')))
}

# Air Dribble: the ball balanced on top of the car, both in the air.
$CustomIcons['AirDribble'] = { (Boost 300 560 150 -12) + (RLCar 290 590 1.1 $CarBlue -12) + (RLBall 530 250 110) }

# ---- Age of Empires

# Monk: the robed monk with his staff.
$CustomIcons['Monk'] = {
  @(
    (Part (PRect 640 160 26 470) $Wood), (Part (PEll 653 170 34 34) $Yellow),
    (Part (PPoly @(400, 330, 600, 330, 660, 640, 340, 640)) $Robe),
    (Part (PEll 500 270 80 85) $Fur), (Part (PArc 500 260 92 180 180) $Robe),
    (Part (PRound 590 380 90 50 25) $Fur),
    (Detail (PLine @(500, 330, 500, 640)) 8), (Part (PRect 420 450 160 26) (C '#7a4a28'))
  )
}

# Wololo: the monk raising his staff, conversion rings flowing out.
$CustomIcons['Wololo'] = {
  @((Detail (PArc 700 200 90 200 140) 14), (Detail (PArc 700 200 140 200 140) 14), (Detail (PArc 700 200 190 200 140) 14)) +
  @(
    (Part (Rot (PRect 600 150 26 420) 25 613 360) $Wood), (Part (PEll 700 175 34 34) $Yellow),
    (Part (PPoly @(360, 360, 560, 360, 610, 650, 310, 650)) $Robe),
    (Part (PEll 460 300 80 85) $Fur), (Part (PArc 460 290 92 180 180) $Robe),
    (Part (Rot (PRound 540 280 150 50 25) -40 560 330) $Fur),
    (Part (PEll 440 340 30 22) $Dark)                                                     # mouth wide open: "Wololooo"
  )
}

# Paladin: a knight on horseback with a lance.
$CustomIcons['Paladin'] = {
  @(
    (Part (PRound 300 380 360 160 70) (C '#d8c8b0')),                                     # horse body
    (Part (PRect 330 500 40 140) (C '#d8c8b0')), (Part (PRect 590 500 40 140) (C '#d8c8b0')),
    (Part (PPoly @(620, 400, 700, 250, 780, 260, 760, 320, 690, 420)) (C '#d8c8b0')),     # neck and head
    (Part (PPoly @(300, 420, 230, 520, 270, 520, 320, 450)) (C '#7a5a3a')),               # tail
    (Part (PRect 420 220 120 190) (C '#c8ccd4')), (Part (PRound 425 140 110 100 30) (C '#c8ccd4')),  # knight
    (Part (PRect 455 175 50 14) $Dark),
    (Part (PPoly @(480, 300, 860, 200, 866, 220, 490, 330)) $Wood),                       # lance
    (Part (PPoly @(420, 260, 360, 300, 420, 400)) (C '#3a7ad8'))                          # shield
  )
}

# Palisade Wall: sharpened wooden stakes in a row.
$CustomIcons['PalisadeWall'] = {
  $stakes = foreach ($x in 240, 330, 420, 510, 600, 690) { (Part (PPoly @($x, 640, $x, 280, ($x + 35), 200, ($x + 70), 280, ($x + 70), 640)) $(if ($x % 180 -eq 60) { $Wood } else { C '#c8945a' })) }
  $stakes + @((Part (PRect 220 380 560 30) (C '#7a5a3a')), (Part (PRect 220 530 560 30) (C '#7a5a3a')))
}

# Trebuchet: the frame, the throwing arm and the counterweight.
$CustomIcons['Trebuchet'] = {
  @(
    (Part (PRect 240 600 520 40) $Wood),
    (Part (PPoly @(330, 600, 480, 300, 510, 300, 380, 600)) $Wood), (Part (PPoly @(670, 600, 520, 300, 490, 300, 620, 600)) $Wood),
    (Part (Rot (PRect 300 290 460 30) -28 500 305) (C '#c8945a')),                        # arm
    (Part (PRound 610 340 110 120 14) $Stone),                                            # counterweight
    (Detail (PLine @(310, 180, 250, 260)) 10), (Part (PEll 245 280 36 36) $Stone),        # sling and stone
    (Part (PEll 500 305 22 22) $Dark)
  )
}

# ---- Splinter Cell

# Night Vision: the three-lens goggles with their green glow.
$CustomIcons['NightVision'] = {
  @(
    (Detail (PArc 500 470 260 200 140) 30),                                               # head strap
    (Part (PRound 300 330 400 150 50) $Dark),
    (Part (PRect 330 220 60 120) $Dark), (Part (PRect 470 200 60 140) $Dark), (Part (PRect 610 220 60 120) $Dark),
    (Part (PEll 360 220 46 46) $NVGreen), (Part (PEll 500 200 50 50) $NVGreen), (Part (PEll 640 220 46 46) $NVGreen),
    (Part (PEll 350 210 12 12)), (Part (PEll 490 190 12 12)), (Part (PEll 630 210 12 12))
  )
}

# Mark and Execute: three red target marks.
$CustomIcons['MarkAndExecute'] = {
  @(foreach ($m in @(300, 300, 90), @(500, 470, 110), @(700, 300, 90)) {
      $x = $m[0]; $y = $m[1]; $r = $m[2]
      (Detail (PEll $x $y $r $r) 16), (Detail (PLine @(($x - $r - 30), $y, ($x - $r * 0.4), $y)) 14), (Detail (PLine @(($x + $r * 0.4), $y, ($x + $r + 30), $y)) 14),
      (Detail (PLine @($x, ($y - $r - 30), $x, ($y - $r * 0.4))) 14), (Detail (PLine @($x, ($y + $r * 0.4), $x, ($y + $r + 30))) 14), (Part (PEll $x $y 22 22) $Red) })
}

# Split Jump: braced between two walls, legs in a split, high above the guard.
$CustomIcons['SplitJump'] = {
  @(
    (Part (PRect 200 120 90 540) $Stone), (Part (PRect 710 120 90 540) $Stone),
    (Part (PPoly @(290, 330, 480, 360, 520, 360, 710, 330, 710, 370, 520, 400, 480, 400, 290, 370)) $Dark),  # legs
    (Part (PRound 455 200 90 190 30) $Dark),                                              # body
    (Part (PEll 500 175 45 45) $Dark), (Part (PEll 482 165 8 8) $NVGreen), (Part (PEll 500 160 8 8) $NVGreen), (Part (PEll 518 165 8 8) $NVGreen),
    (Part (PPoly @(450, 230, 300, 210, 300, 240, 450, 270)) $Dark), (Part (PPoly @(550, 230, 700, 210, 700, 240, 550, 270)) $Dark)
  )
}

# ---- Airsoft

# BB Spray: an airsoft rifle spitting a stream of BBs.
$CustomIcons['BbSpray'] = {
  @(foreach ($b in 640, 690, 740, 790, 840) { (Part (PEll $b (360 + ($b % 3) * 6) 14 14) $Yellow) }) + @(
    (Part (PPoly @(180, 400, 300, 360, 320, 450, 200, 490))),
    (Part (PRound 280 340 320 90 20) $Dark), (Part (PRect 590 362 50 34) $Dark),
    (Part (PRound 400 420 60 120 16) $Dark), (Part (PRound 480 420 50 110 10) (C '#5a5a6a')),
    (Part (PRect 360 300 140 40) $Dark), (Part (PRound 500 380 80 20 8) $LegoRed)
  )
}

# Ghillie Suit: a shaggy mound of leaves with a scope peeking out.
$CustomIcons['GhillieSuit'] = {
  $tufts = foreach ($t in @(320, 470), @(420, 400), @(520, 380), @(620, 420), @(700, 490), @(380, 540), @(500, 500), @(620, 550), @(280, 580), @(720, 590)) {
    (Part (PPoly @(($t[0] - 60), ($t[1] + 60), ($t[0] - 30), ($t[1] - 40), $t[0], ($t[1] + 20), ($t[0] + 30), ($t[1] - 50), ($t[0] + 60), ($t[1] + 60))) $(if (($t[0] + $t[1]) % 3 -eq 0) { C '#5a7a3a' } else { C '#7a9a4a' }))
  }
  @((Part (PRound 560 330 260 40 18) $Dark), (Part (PEll 820 350 24 30) $Glass)) + $tufts
}

# Call Your Hits: a raised hand with a "HIT!" shout.
$CustomIcons['CallYourHits'] = {
  @(
    (Part (PRect 420 450 120 190) (C '#c8643c')),                                         # sleeve
    (Part (PRound 400 270 160 210 60) $Fur),                                              # palm
    (Part (PRound 400 170 40 140 20) $Fur), (Part (PRound 445 150 40 150 20) $Fur), (Part (PRound 490 160 40 140 20) $Fur), (Part (PRound 535 190 34 120 17) $Fur),
    (Part (Rot (PRound 330 330 40 110 20) -40 360 390) $Fur),
    (Part (PPoly @(610, 260, 660, 330, 590, 320)) (C '#ffffff')), (Part (PRound 600 150 240 120 40)), (Plain (GlyphPath 'HIT!' 720 210 180 70 $bold) $LegoRed)
  )
}

# ---- Legos

# Step on a Lego: a bare foot coming down on a brick.
$CustomIcons['StepOnALego'] = {
  (LegoBrick 380 540 2 $LegoRed) + @(
    (Part (PPoly @(300, 140, 420, 140, 440, 360, 620, 400, 640, 470, 330, 480)) $Fur),
    (Part (PEll 620 420 26 30) $Fur), (Part (PEll 580 405 20 24) $Fur),
    (Part (PPoly @(660, 300, 700, 250, 690, 320)) $Yellow), (Part (PPoly @(250, 300, 210, 260, 230, 330)) $Yellow)
  )
}

# Lego Masterpiece: a little stacked brick house built with the kids.
$CustomIcons['LegoMasterpiece'] = {
  (LegoBrick 290 540 6 $LegoBlue) + (LegoBrick 290 430 2 $LegoYellow) + (LegoBrick 570 430 2 $LegoYellow) +
  @((Part (PRect 430 430 140 110) $Glass)) + (LegoBrick 290 320 6 $LegoRed) + @((Part (PPoly @(290, 300, 500, 170, 710, 300)) $LegoRed))
}
