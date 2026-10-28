# Joshua's custom card icons (see icons.ps1 for the helpers). Coordinates are in the 1000 x 760 card picture.

$CarBlue = C '#4a8af0'; $CarOrange = C '#f08a3a'; $Junimo = C '#7ad04a'; $Parsnip = C '#f2e2b0'

# A Rocket League car facing right, nose at (x + 370 * s), wheels on the line y. Tilt rotates it about its middle.
function RLCar($x, $y, $s, $color, $tilt = 0) {
  $cx = $x + 185 * $s; $cy = $y - 60 * $s
  $t = { param($p) if ($tilt) { Rot $p $tilt $cx $cy } else { $p } }
  @(
    (Part (& $t (PPoly @($x, ($y - 20 * $s), ($x + 30 * $s), ($y - 90 * $s), ($x + 150 * $s), ($y - 115 * $s), ($x + 280 * $s), ($y - 95 * $s), ($x + 370 * $s), ($y - 45 * $s), ($x + 370 * $s), ($y - 10 * $s), $x, $y))) $color),
    (Part (& $t (PPoly @(($x + 120 * $s), ($y - 108 * $s), ($x + 160 * $s), ($y - 150 * $s), ($x + 250 * $s), ($y - 150 * $s), ($x + 290 * $s), ($y - 95 * $s)))) $Glass),
    (Part (& $t (PEll ($x + 80 * $s) $y (40 * $s) (40 * $s)))), (Part (& $t (PEll ($x + 290 * $s) $y (40 * $s) (40 * $s)))),
    (Part (& $t (PEll ($x + 80 * $s) $y (14 * $s) (14 * $s))) $Dark), (Part (& $t (PEll ($x + 290 * $s) $y (14 * $s) (14 * $s))) $Dark)
  )
}
function RLBall($cx, $cy, $r) {
  @((Part (PEll $cx $cy $r $r)),
    (Part (PPoly @($cx, ($cy - $r * 0.35), ($cx + $r * 0.33), ($cy - $r * 0.1), ($cx + $r * 0.2), ($cy + $r * 0.3), ($cx - $r * 0.2), ($cy + $r * 0.3), ($cx - $r * 0.33), ($cy - $r * 0.1))) $Dark),
    (Detail (PLine @($cx, ($cy - $r * 0.35), $cx, ($cy - $r * 0.95))) 8), (Detail (PLine @(($cx + $r * 0.33), ($cy - $r * 0.1), ($cx + $r * 0.9), ($cy - $r * 0.3))) 8),
    (Detail (PLine @(($cx - $r * 0.33), ($cy - $r * 0.1), ($cx - $r * 0.9), ($cy - $r * 0.3))) 8),
    (Detail (PLine @(($cx + $r * 0.2), ($cy + $r * 0.3), ($cx + $r * 0.55), ($cy + $r * 0.8))) 8), (Detail (PLine @(($cx - $r * 0.2), ($cy + $r * 0.3), ($cx - $r * 0.55), ($cy + $r * 0.8))) 8))
}
function Boost($x, $y, $len, $angle = 0) {
  $p1 = PPoly @($x, ($y - 30), ($x - $len), $y, $x, ($y + 30)); $p2 = PPoly @($x, ($y - 15), ($x - $len * 0.6), $y, $x, ($y + 15))
  if ($angle) { $p1 = Rot $p1 $angle $x $y; $p2 = Rot $p2 $angle $x $y }
  @((Part $p1 $Orange), (Part $p2 $Yellow))
}
function QuickChat($text) {
  @((Part (PPoly @(380, 470, 330, 590, 470, 490))), (Part (PRound 200 200 600 300 60)), (Plain (GlyphPath $text 500 350 480 110 $bold) $Dark))
}

# ---- Rocket League

# Aerial: the car flying up on boost to meet the ball.
$CustomIcons['Aerial'] = { (Boost 300 600 190 -35) + (RLCar 250 640 1.25 $CarBlue -35) + (RLBall 690 190 110) }

# Boost Pad: the big glowing boost pad on the field.
$CustomIcons['BoostPad'] = {
  @((Part (PEll 500 520 260 80) (C '#c8ccd4')), (Part (PEll 500 505 200 58) $Orange), (Part (PEll 500 495 120 34) $Yellow),
    (Part (PPoly @(420, 470, 500, 160, 580, 470)) (C '#ffd060')), (Part (PPoly @(460, 470, 500, 260, 540, 470)) (C '#fff4c0')))
}

# Calculated.: the quick chat.
$CustomIcons['Calculated'] = { QuickChat 'Calculated.' }

# Nice Shot!: the quick chat, said the sarcastic way.
$CustomIcons['NiceShot'] = { QuickChat 'Nice shot!' }

# Demo: a car bursting apart in an explosion.
$CustomIcons['Demo'] = {
  @((Part (PPoly @(500, 120, 560, 260, 720, 180, 650, 330, 820, 380, 650, 440, 740, 600, 560, 500, 500, 650, 440, 500, 260, 600, 350, 440, 180, 380, 350, 330, 280, 180, 440, 260)) $Orange),
    (Part (PPoly @(500, 230, 540, 330, 640, 300, 590, 380, 680, 420, 580, 450, 600, 540, 520, 470, 500, 560, 470, 470, 390, 540, 420, 450, 320, 420, 410, 380, 360, 300, 460, 330)) $Yellow)) +
    (RLCar 330 470 0.95 $CarOrange -12)
}

# What a Save!: a car flying across the goal mouth, blocking the ball.
$CustomIcons['WhatASave'] = {
  @((Part (PRect 220 220 560 30)), (Part (PRect 220 220 30 400)), (Part (PRect 750 220 30 400))) +
  @(foreach ($x in 300, 400, 500, 600, 700) { (Detail (PLine @($x, 250, $x, 620)) 6) }) +
  @(foreach ($y in 330, 430, 530) { (Detail (PLine @(250, $y, 750, $y)) 6) }) +
  (RLCar 280 450 0.8 $CarBlue 0) + (RLBall 640 360 70)
}

# Zero-Second Goal: the scoreboard at 0:00 and the ball going in.
$CustomIcons['ZeroSecondGoal'] = {
  @((Part (PRound 250 170 500 200 30) $Dark), (Part (GlyphPath '0:00' 500 270 320 120 $bold) (C '#ff6a5a'))) + (RLBall 500 520 110) +
  @((Detail (PLine @(330, 470, 250, 430)) 14), (Detail (PLine @(330, 560, 240, 580)) 14))
}

# ---- Stardew Valley

# Junimos: a little forest spirit with a leaf sprout on its head.
$CustomIcons['Junimos'] = {
  @(
    (Part (PPoly @(500, 250, 460, 170, 500, 190)) (C '#3a8a3a')), (Part (PPoly @(500, 250, 545, 165, 510, 195)) (C '#5ac05a')),
    (Part (PEll 500 420 170 170) $Junimo),
    (Part (PRound 370 550 80 70 30) $Junimo), (Part (PRound 550 550 80 70 30) $Junimo),
    (Part (PEll 440 410 26 34) $Dark), (Part (PEll 560 410 26 34) $Dark),
    (Part (PEll 432 398 8 10)), (Part (PEll 552 398 8 10)),
    (Part (PEll 380 470 26 16) (C '#ffb0c0')), (Part (PEll 620 470 26 16) (C '#ffb0c0'))
  )
}

# Plant Parsnips: a fresh parsnip with its leaves.
$CustomIcons['PlantParsnips'] = {
  @(
    (Part (PPoly @(500, 260, 420, 120, 470, 140, 500, 220)) (C '#5ac05a')), (Part (PPoly @(500, 260, 540, 100, 560, 160, 510, 240)) (C '#3a8a3a')),
    (Part (PPoly @(500, 260, 600, 150, 610, 200, 520, 270)) (C '#5ac05a')),
    (Part (PPoly @(400, 280, 600, 280, 520, 640, 480, 640)) $Parsnip),
    (Detail (PLine @(440, 360, 480, 370)) 8), (Detail (PLine @(520, 430, 555, 420)) 8), (Detail (PLine @(470, 520, 500, 528)) 8)
  )
}

# Watering Can: watering the crops.
$CustomIcons['WateringCan'] = {
  @(
    (Detail (PArc 420 300 110 190 160) 26),                                               # handle
    (Part (PPoly @(560, 380, 760, 250, 790, 280, 600, 440)) (C '#8ab0d8')),               # spout
    (Part (PRound 280 330 320 270 50) (C '#8ab0d8')),
    (Part (PEll 790 265 40 40) (C '#8ab0d8')),
    (Part (PEll 820 360 12 18) $Glass), (Part (PEll 850 420 12 18) $Glass), (Part (PEll 800 450 12 18) $Glass)
  )
}

# ---- PlateUp!

# Dish Pit: the sink with a stack of dirty plates.
$CustomIcons['DishPit'] = {
  @(
    (Part (PRect 240 380 520 260) (C '#c8ccd4')), (Part (PRect 280 400 440 100) (C '#8ab0d8')),
    (Detail (PLine @(620, 380, 620, 290, 560, 290)) 22),
    (Part (PEll 420 360 110 30)), (Part (PEll 420 330 110 30)), (Part (PEll 420 300 110 30)),
    (Part (PEll 380 300 16 8) (C '#c8a060')), (Part (PEll 450 296 12 6) (C '#a06040')),
    (Part (PEll 560 440 18 18)), (Part (PEll 600 460 12 12)), (Part (PEll 640 430 14 14))
  )
}

# ---- Terraria

# Heart Crystal: the faceted red crystal heart.
$CustomIcons['HeartCrystal'] = {
  @((Part (PPoly @(500, 640, 260, 380, 300, 260, 400, 220, 500, 290, 600, 220, 700, 260, 740, 380)) (C '#ff5a7a')),
    (Part (PPoly @(500, 290, 400, 220, 360, 330, 500, 520)) (C '#ff8aa0')), (Part (PPoly @(500, 290, 600, 220, 640, 330, 500, 520)) (C '#e03a5a')),
    (Part (PPoly @(330, 290, 360, 260, 380, 300)) (C '#ffffff')))
}

# ---- Archipelago

# Location Check: an item box opening with a sparkle, ticked off.
$CustomIcons['LocationCheck'] = {
  @((Part (PPoly @(500, 160, 540, 260, 640, 280, 560, 330, 590, 430, 500, 370, 410, 430, 440, 330, 360, 280, 460, 260)) $Yellow),
    (Part (PRect 300 400 400 240) $Wood), (Part (PRect 280 360 440 70) (C '#d0a060')),
    (Part (PEll 640 590 70 70) (C '#5ac05a')), (Detail (PLine @(610, 590, 635, 615, 675, 560)) 18))
}

# Multiworld: several little worlds linked together.
$CustomIcons['Multiworld'] = {
  @((Detail (PLine @(330, 300, 670, 300, 500, 560, 330, 300)) 16),
    (Part (PEll 330 300 110 110) (C '#5ac0e0')), (Part (PEll 670 300 110 110) (C '#f0a040')), (Part (PEll 500 560 110 110) (C '#a060e0')),
    (Part (PEll 300 280 40 30) (C '#5ac05a')), (Part (PEll 690 320 40 28) (C '#c87030')), (Part (PEll 480 540 36 30) (C '#7040a0')))
}

# Release!: every item bursting out of the box at once.
$CustomIcons['Release'] = {
  @((Part (PRect 330 450 340 190) $Wood), (Part (PPoly @(330, 450, 260, 360, 330, 360)) (C '#d0a060')), (Part (PPoly @(670, 450, 740, 360, 670, 360)) (C '#d0a060'))) +
  @(foreach ($it in @(400, 260, '#5ac0e0'), @(500, 180, '#f2c530'), @(610, 250, '#e0483c'), @(320, 170, '#5ac05a'), @(690, 150, '#a060e0')) {
      (Part (PRound ($it[0] - 40) ($it[1] - 40) 80 80 16) (C $it[2])) }) +
  @((Detail (PLine @(430, 440, 410, 320)) 10), (Detail (PLine @(500, 440, 500, 240)) 10), (Detail (PLine @(570, 440, 600, 300)) 10))
}

# ---- Japan

# Thousand Cranes: a folded paper crane.
$CustomIcons['ThousandCranes'] = {
  @(
    (Part (PPoly @(480, 420, 300, 140, 600, 400)) (C '#ffc0d8')),                         # far wing
    (Part (PPoly @(260, 470, 500, 380, 740, 470, 500, 560))),                             # body
    (Part (PPoly @(270, 470, 170, 300, 220, 300, 340, 450))),                             # tail
    (Part (PPoly @(730, 470, 820, 280, 845, 285, 810, 320, 765, 470))),                   # neck and head
    (Part (PPoly @(520, 420, 760, 160, 640, 450)) (C '#ffd8e8')),                         # near wing
    (Detail (PLine @(500, 380, 500, 560)) 8)
  )
}
# ---- The Walls of Jericho

# Trumpet Blast: a shofar (ram's horn), what the priests blew at Jericho (Joshua 6). Narrow mouthpiece at the lower
# left, a curve down and up, flaring to the bell at the upper right, with ridges along it and sound lines.
$CustomIcons['TrumpetBlast'] = {
  $horn = C '#d9b98a'; $ridge = C '#8a6a44'
  $pts = @(); $n = 24
  for ($i = 0; $i -le $n; $i++) {
    $t = $i / $n; $u = 1 - $t
    $x = $u*$u*$u*170 + 3*$u*$u*$t*420 + 3*$u*$t*$t*690 + $t*$t*$t*780
    $y = $u*$u*$u*470 + 3*$u*$u*$t*720 + 3*$u*$t*$t*640 + $t*$t*$t*250
    $pts += ,@($x, $y, (12 + 95 * $t * $t))
  }
  $left = @(); $right = @()
  for ($i = 0; $i -le $n; $i++) {
    $a = $pts[[Math]::Max(0, $i - 1)]; $b = $pts[[Math]::Min($n, $i + 1)]
    $dx = $b[0] - $a[0]; $dy = $b[1] - $a[1]; $len = [Math]::Sqrt($dx * $dx + $dy * $dy)
    $nx = -$dy / $len; $ny = $dx / $len; $w = $pts[$i][2]
    $left += @(($pts[$i][0] + $nx * $w), ($pts[$i][1] + $ny * $w))
    $right = @(($pts[$i][0] - $nx * $w), ($pts[$i][1] - $ny * $w)) + $right
  }
  $parts = @((Part (PPoly ($left + $right)) $horn))
  # ridges across the horn
  foreach ($i in 4, 8, 11, 14, 17, 20) {
    $l = $i * 2; $r = ($n - $i) * 2
    $parts += Detail (PLine @($left[$l], $left[$l + 1], $right[$r], $right[$r + 1])) 9
  }
  # the bell opening, and sound lines coming out of it
  $parts += Part (PEll 792 235 70 52) $ridge
  foreach ($k in 0, 1, 2) {
    $r0 = 120 + $k * 55
    $parts += Detail (PLine @((780 + $r0 * 0.7), (235 - $r0 * 0.75), (780 + $r0 * 0.95), (235 - $r0 * 0.15))) 10
  }
  $parts
}

# ---- D&D

# A d20 seen face-on: a hexagon rim, the lit front face showing a 20, and the facets around it shaded top to bottom.
function D20($cx, $cy, $r) {
  $h = foreach ($i in 0..5) { $a = (-90 + $i * 60) * [math]::PI / 180; , @(($cx + [math]::Cos($a) * $r), ($cy + [math]::Sin($a) * $r)) }
  $t = @(@($cx, ($cy - $r * 0.62)), @(($cx + $r * 0.6), ($cy + $r * 0.42)), @(($cx - $r * 0.6), ($cy + $r * 0.42)))
  $tri = { param($a, $b, $c) PPoly @($a[0], $a[1], $b[0], $b[1], $c[0], $c[1]) }
  $hi = C '#e8566a'; $mid = C '#c43a52'; $lo = C '#962a40'
  @(
    (Part (& $tri $t[0] $h[0] $h[1]) $hi), (Part (& $tri $t[0] $h[5] $h[0]) $hi),
    (Part (& $tri $t[0] $h[1] $t[1]) $mid), (Part (& $tri $t[2] $h[5] $t[0]) $mid),
    (Part (& $tri $t[1] $h[1] $h[2]) $mid), (Part (& $tri $t[2] $h[4] $h[5]) $mid),
    (Part (& $tri $t[1] $h[2] $h[3]) $lo), (Part (& $tri $t[2] $h[3] $h[4]) $lo), (Part (& $tri $t[1] $h[3] $t[2]) $lo),
    (Part (& $tri $t[0] $t[1] $t[2]) (C '#ff9aa6')),
    (Plain (GlyphPath '20' $cx ($cy + $r * 0.12) ($r * 0.62) ($r * 0.42) $bold) (C '#4a0f1c'))
  )
}

# Natural Twenty: the d20 landing on a 20.
$CustomIcons['D20'] = { D20 500 395 300 }
