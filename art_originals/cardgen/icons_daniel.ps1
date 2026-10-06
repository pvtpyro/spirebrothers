# Daniel's custom card icons (see icons.ps1 for the helpers). Coordinates are in the 1000 x 760 card picture.

# Monkey Island: "Look behind you, a three-headed monkey!"
$CustomIcons['LookBehindYou'] = {
  # three big heads on one round body, the middle one a little higher
  $heads = foreach ($h in @(320, 300), @(680, 300), @(500, 220)) {
    $hx = $h[0]; $hy = $h[1]
    (Part (PEll ($hx - 82) ($hy + 6) 32 38) $Brown), (Part (PEll ($hx + 82) ($hy + 6) 32 38) $Brown),
    (Part (PEll $hx $hy 92 88) $Brown), (Part (PEll $hx ($hy + 24) 62 52) $Fur),
    (Detail (PEll ($hx - 24) ($hy + 6) 7 9) 14), (Detail (PEll ($hx + 24) ($hy + 6) 7 9) 14), (Detail (PArc $hx ($hy + 34) 22 20 140) 11)
  }
  @((Detail (PArc 700 560 110 -70 160) 24), (Part (PRound 330 380 340 280 140) $Brown)) + $heads + @((Part (PEll 500 530 100 100) $Fur))
}

# Subnautica: the Seaglide underwater scooter, headlight on.
$CustomIcons['Seaglide'] = {
  @(
    (Part (PPoly @(700, 300, 900, 230, 900, 450, 700, 380)) (C '#fff4b0')),               # light beam
    (Part (PEll 330 340 70 110)), (Detail (PLine @(330, 240, 330, 440)) 12), (Detail (PLine @(270, 340, 390, 340)) 12),
    (Part (PRound 330 270 380 140 70)),                                                   # body
    (Part (PRound 400 400 60 140 20)), (Part (PRound 560 400 60 140 20)),                 # handles
    (Part (PEll 700 340 40 52) (C '#fff4b0')),                                            # headlight
    (Part (PRound 460 300 140 50 22) $Glass)                                              # display
  )
}

# Subnautica: the Scanner Room, a glass dome with the hologram map spinning inside.
$CustomIcons['ScannerRoom'] = {
  @(
    (Part (PRound 270 470 460 150 30)),
    (Detail (PLine @(300, 540, 700, 540)) 10),
    (Part (PArc 500 470 210 180 180) $Glass), (Part (PRect 290 466 420 12)),
    (Part (PEll 500 360 70 70) (C '#ffb860')),
    (Detail (PEll 500 360 120 30) 10), (Detail (PEll 500 360 30 70) 8)
  )
}

# Factorio: an inserter arm lifting a gear off a belt.
$CustomIcons['AssemblyLine'] = {
  $gear = [System.Drawing.Drawing2D.GraphicsPath]::new(); $gear.FillMode = 'Winding'
  for ($k = 0; $k -lt 8; $k++) { $gear.AddPath((Rot (PRect 700 405 34 30) ($k * 45) 717 460), $false) }
  $gear.AddEllipse(672, 415, 90, 90)
  @(
    (Part (PRect 240 560 520 70) $Yellow),
    (Detail (PLine @(300, 615, 330, 595, 300, 575)) 10), (Detail (PLine @(450, 615, 480, 595, 450, 575)) 10), (Detail (PLine @(600, 615, 630, 595, 600, 575)) 10),
    (Part (PRound 300 500 140 70 14)),
    (Part (Rot (PRound 345 300 50 230 20) 25 370 520)),
    (Part (Rot (PRound 420 270 240 46 20) 10 440 290)),
    (Part (PEll 440 295 30 30)), (Part (PEll 370 520 30 30)),
    (Part (PPoly @(650, 300, 700, 300, 700, 330, 675, 345, 700, 360, 700, 395, 650, 385))),
    (Part $gear (C '#c8ccd4')), (Detail (PEll 717 460 16 16) 10)
  )
}

# Factorio: a yellow transport belt carrying a copper plate, an iron plate and a gear.
$CustomIcons['ConveyorBelt'] = {
  $parts = @((Part (Rot (PRect 200 330 600 150) -18 500 405) $Yellow))
  foreach ($x in 290, 430, 570, 710) { $parts += (Detail (Rot (PLine @($x, 440, ($x + 40), 405, $x, 370)) -18 500 405) 14) }
  $parts + @(
    (Part (Rot (PRound 300 285 110 70 10) -18 500 405) (C '#d0763a')),
    (Part (Rot (PRound 470 230 110 70 10) -18 500 405) (C '#c8ccd4')),
    (Part (PEll 690 290 50 50) (C '#c8ccd4')), (Detail (PEll 690 290 18 18) 10)
  )
}

# Warhammer 40k / Darktide: the cog-and-skull of the Machine God, with one bionic eye.
$CustomIcons['AppeaseTheMachineSpirit'] = {
  $cog = [System.Drawing.Drawing2D.GraphicsPath]::new(); $cog.FillMode = 'Winding'
  for ($k = 0; $k -lt 12; $k++) { $cog.AddPath((Rot (PRect 470 120 60 80) ($k * 30) 500 390), $false) }
  $cog.AddEllipse(260, 150, 480, 480)
  @(
    (Part $cog (C '#c8ccd4')),
    (Part (PEll 500 380 150 150)),
    (Part (PRound 430 470 140 110 30)),
    (Part (PEll 445 380 42 48) $Dark), (Part (PEll 555 380 42 48) $Dark),
    (Part (PEll 445 380 14 16) $Red),
    (Part (PPoly @(500, 430, 480, 470, 520, 470)) $Dark),
    (Detail (PLine @(460, 510, 460, 570)) 10), (Detail (PLine @(500, 510, 500, 575)) 10), (Detail (PLine @(540, 510, 540, 570)) 10)
  )
}

# Darktide: an Ogryn charging behind his slab shield.
$CustomIcons['BullRush'] = {
  @(
    (Detail (PLine @(200, 300, 300, 300)) 18), (Detail (PLine @(170, 400, 290, 400)) 18), (Detail (PLine @(200, 500, 300, 500)) 18),
    (Part (PPoly @(360, 160, 640, 140, 680, 620, 380, 650))),
    (Part (PRect 470 200 120 22) $Dark),
    (Detail (PLine @(400, 260, 650, 245)) 10), (Detail (PLine @(410, 560, 660, 545)) 10),
    (Part (PEll 520 400 40 40) (C '#c8ccd4')), (Part (PEll 520 400 16 16) $Dark)
  )
}

# Darktide: a lasgun firing red bolts.
$CustomIcons['LasgunVolley'] = {
  @(
    (Part (PRound 640 205 200 22 11) $Red), (Part (PRound 700 265 150 22 11) $Red), (Part (PRound 660 325 170 22 11) $Red),
    (Part (PPoly @(180, 430, 300, 380, 330, 470, 210, 520))),
    (Part (PRound 280 360 330 100 20)),
    (Part (PRect 600 380 130 34)),
    (Part (PRound 400 450 70 120 18)),
    (Part (PRound 500 450 60 150 14) $Dark),
    (Part (PRect 360 330 140 34)),
    (Detail (PLine @(300, 410, 590, 410)) 10)
  )
}

# Minecraft: a diamond chestplate, pixel style.
$CustomIcons['DiamondArmor'] = {
  @((Part (PPixels @('XXX    XXX', 'XXXXXXXXXX', 'XXXXXXXXXX', ' XXXXXXXX ', ' XXXXXXXX ', ' XXXXXXXX ', ' XXXXXXXX ', ' XXXXXXXX ') 260 160 48) $Diamond),
    (Part (PPixels @('  XX', ' XX ', 'XX  ') 420 250 32) (C '#d8ffff')))
}

# Minecraft: a diamond pickaxe, pixel style.
$CustomIcons['DiamondPickaxe'] = {
  # head along the top and down the right side, handle on the diagonal between them
  @((Part (PPixels @('      ', '      ', '     X', '    X ', '   X  ', '  X   ', ' X    ', 'X     ') 265 160 56) $Wood),
    (Part (PPixels @(' XXXXX  ', '   XXXX ', '     XXX', '      XX', '      XX', '       X') 265 160 56) $Diamond))
}

# Minecraft: the crafting table block, with a saw and a hammer on its sides.
$CustomIcons['CraftingTable'] = {
  @(
    (Part (PPoly @(500, 160, 760, 280, 500, 400, 240, 280)) (C '#c89a5a')),
    (Part (PPoly @(240, 280, 500, 400, 500, 660, 240, 540)) $Wood),
    (Part (PPoly @(500, 400, 760, 280, 760, 540, 500, 660)) (C '#8a6038')),
    (Detail (PLine @(413, 200, 673, 320)) 10), (Detail (PLine @(327, 240, 587, 360)) 10),   # 3x3 grid on top
    (Detail (PLine @(327, 320, 587, 200)) 10), (Detail (PLine @(413, 360, 673, 240)) 10),
    (Detail (PLine @(290, 360, 290, 520)) 12), (Detail (PLine @(330, 380, 450, 435)) 12),
    (Detail (PLine @(570, 430, 690, 380)) 12), (Detail (PLine @(640, 400, 650, 500)) 12)
  )
}

# Minecraft: a redstone repeater, the stone slab with two little torches.
$CustomIcons['RedstoneRepeater'] = {
  @(
    (Part (PPoly @(240, 420, 560, 320, 780, 420, 460, 540)) (C '#c8ccd4')),
    (Part (PPoly @(240, 420, 460, 540, 460, 600, 240, 480)) (C '#9aa0aa')),
    (Part (PPoly @(460, 540, 780, 420, 780, 480, 460, 600)) (C '#7a808a')),
    (Detail (PLine @(330, 440, 680, 380)) 16),
    (Part (PRect 385 300 34 120) $Wood), (Part (PRect 575 255 34 120) $Wood),
    (Part (PEll 402 290 34 34) $Red), (Part (PEll 592 245 34 34) $Red)
  )
}

# Cars: jumper cables, a red clamp and a black clamp, with a spark.
$CustomIcons['JumperCables'] = {
  @(
    (Detail (PLine @(330, 420, 360, 560, 500, 600, 640, 560, 670, 420)) 26),
    (Part (Rot (PPoly @(280, 200, 340, 200, 360, 420, 300, 420)) -12 320 320) $Red),
    (Part (Rot (PPoly @(340, 200, 400, 210, 360, 420, 320, 410)) 12 320 320) $Red),
    (Part (Rot (PPoly @(620, 200, 680, 200, 700, 420, 640, 420)) -12 660 320) $Dark),
    (Part (Rot (PPoly @(680, 200, 740, 210, 700, 420, 660, 410)) 12 660 320) $Dark),
    (Part (PPoly @(480, 200, 520, 200, 490, 290, 550, 290, 490, 410, 505, 320, 460, 320)) $Yellow)
  )
}
