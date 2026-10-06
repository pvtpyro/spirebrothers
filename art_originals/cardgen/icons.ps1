# Custom card icons, drawn from simple shapes in the same style as the emoji ones (white parts, thick dark outline).
# A card uses one when its line in glyphs.txt says @Name instead of an emoji, e.g. "PrawnSuit @PrawnSuit".
# Each icon is a list of parts drawn back to front, in a 1000 x 760 picture with the middle at (500, 390).
# A part is @{ P = <GraphicsPath>; Fill = <color or $null for white> }, or @{ Line = <GraphicsPath> } for a dark detail line.

function Pt($x, $y) { [System.Drawing.PointF]::new($x, $y) }
function PEll($cx, $cy, $rx, $ry) { $p = [System.Drawing.Drawing2D.GraphicsPath]::new(); $p.AddEllipse($cx - $rx, $cy - $ry, $rx * 2, $ry * 2); $p }
function PPoly([float[]]$xy) {
  $p = [System.Drawing.Drawing2D.GraphicsPath]::new(); $pts = for ($i = 0; $i -lt $xy.Count; $i += 2) { Pt $xy[$i] $xy[$i + 1] }
  $p.AddPolygon([System.Drawing.PointF[]]$pts); $p
}
function PRound($x, $y, $w, $h, $r) { RoundRect $x $y $w $h $r }
function PLine([float[]]$xy) {
  $p = [System.Drawing.Drawing2D.GraphicsPath]::new(); $pts = for ($i = 0; $i -lt $xy.Count; $i += 2) { Pt $xy[$i] $xy[$i + 1] }
  $p.AddLines([System.Drawing.PointF[]]$pts); $p
}
function PArc($cx, $cy, $r, $start, $sweep) { $p = [System.Drawing.Drawing2D.GraphicsPath]::new(); $p.AddArc($cx - $r, $cy - $r, $r * 2, $r * 2, $start, $sweep); $p }
function Rot($path, $deg, $cx, $cy) { $m = [System.Drawing.Drawing2D.Matrix]::new(); $m.RotateAt($deg, (Pt $cx $cy)); $path.Transform($m); $path }
function Part($p, $fill = $null) { @{ P = $p; Fill = $fill } }
function Detail($p, $w = 14) { @{ Line = $p; W = $w } }

# Draws the parts: one soft shadow under everything, then each part outlined and filled, then detail lines.
function DrawParts($g, $parts, $stroke) {
  $sh = [System.Drawing.SolidBrush]::new((C '#000000' 90)); $shPen = [System.Drawing.Pen]::new((C '#000000' 90), 40); $shPen.LineJoin = 'Round'
  $m = [System.Drawing.Drawing2D.Matrix]::new(); $m.Translate(14, 14)
  foreach ($pt in $parts) { if ($pt.P) { $s = $pt.P.Clone(); $s.Transform($m); $g.DrawPath($shPen, $s); $g.FillPath($sh, $s) } }
  $pen = [System.Drawing.Pen]::new($stroke, 26); $pen.LineJoin = 'Round'
  foreach ($pt in $parts) {
    if ($pt.P) {
      $g.DrawPath($pen, $pt.P)
      $g.FillPath([System.Drawing.SolidBrush]::new($(if ($pt.Fill) { $pt.Fill } else { C '#ffffff' })), $pt.P)
    }
    elseif ($pt.Line) {
      $lp = [System.Drawing.Pen]::new($stroke, $pt.W); $lp.LineJoin = 'Round'; $lp.StartCap = 'Round'; $lp.EndCap = 'Round'
      $g.DrawPath($lp, $pt.Line)
    }
  }
}

# ---------------------------------------------------------------- icons

$CustomIcons = @{}

# Subnautica's Prawn Suit: a chunky diving mech with a round glass cockpit, a grappling claw and a drill arm.
$CustomIcons['PrawnSuit'] = {
  @(
    (Part (PRound 395 545 70 120 30)), (Part (PRound 535 545 70 120 30)),                 # legs
    (Part (PRound 370 640 110 50 22)), (Part (PRound 520 640 110 50 22)),                 # feet
    (Part (PRound 275 330 90 210 40)),                                                    # left arm
    (Part (PPoly @(250, 520, 390, 520, 360, 590, 320, 560, 280, 600))),                   # claw
    (Part (PRound 635 330 90 190 40)),                                                    # right arm
    (Part (PPoly @(640, 505, 720, 505, 680, 640))),                                       # drill
    (Detail (PLine @(655, 545, 705, 535)) 10), (Detail (PLine @(665, 580, 695, 573)) 10),
    (Part (PRound 345 250 310 330 120)),                                                  # body
    (Part (PEll 500 330 105 95) (C '#9fd8ec')),                                           # cockpit glass
    (Detail (PArc 500 330 70 200 70) 12),                                                 # glint
    (Part (PRound 430 470 140 50 18)),                                                    # chest plate
    (Detail (PLine @(455, 495, 545, 495)) 10)
  )
}

# Rocket League's air dribble: the ball balanced on top of a car, both in the air.
$CustomIcons['AirDribble'] = {
  @(
    (Detail (PLine @(270, 600, 330, 560)) 12), (Detail (PLine @(240, 545, 310, 515)) 12),  # motion lines
    (Part (PPoly @(330, 560, 360, 480, 470, 455, 600, 470, 690, 520, 700, 575, 620, 600, 340, 600))),  # car body
    (Part (PPoly @(420, 470, 470, 425, 560, 425, 600, 470)) (C '#9fd8ec')),               # windshield
    (Part (PEll 405 600 42 42)), (Part (PEll 625 600 42 42)),                              # wheels
    (Detail (PEll 405 600 14 14) 10), (Detail (PEll 625 600 14 14) 10),
    (Part (PPoly @(330, 545, 270, 520, 285, 560, 255, 585, 330, 585)) (C '#ffb84a')),     # boost flame
    (Part (PEll 520 270 120 120)),                                                        # ball
    (Part (PPoly @(520, 238, 547, 258, 537, 290, 503, 290, 493, 258)) (C '#2a2a36')),     # ball patches
    (Part (PPoly @(430, 230, 446, 220, 458, 246, 440, 262, 424, 254)) (C '#2a2a36')),
    (Part (PPoly @(610, 230, 594, 220, 582, 246, 600, 262, 616, 254)) (C '#2a2a36')),
    (Part (PPoly @(496, 352, 520, 344, 544, 352, 536, 376, 504, 376)) (C '#2a2a36')),
    (Detail (PLine @(520, 238, 520, 175)) 10), (Detail (PLine @(493, 258, 446, 220)) 10), (Detail (PLine @(547, 258, 594, 220)) 10),
    (Detail (PLine @(503, 290, 470, 330)) 10), (Detail (PLine @(537, 290, 570, 330)) 10)
  )
}

# Monkey Island's insult swordfighting: crossed cutlasses with an angry speech bubble.
$CustomIcons['InsultSwordfighting'] = {
  $blade1 = Rot (PPoly @(480, 150, 520, 150, 520, 560, 500, 600, 480, 560)) -38 500 380
  $blade2 = Rot (PPoly @(480, 150, 520, 150, 520, 560, 500, 600, 480, 560)) 38 500 380
  @(
    (Part $blade1), (Part (Rot (PRound 420 560 160 34 14) -38 500 380)), (Part (Rot (PRound 482 594 36 90 14) -38 500 380)),
    (Part $blade2), (Part (Rot (PRound 420 560 160 34 14) 38 500 380)), (Part (Rot (PRound 482 594 36 90 14) 38 500 380)),
    (Part (PPoly @(625, 260, 590, 350, 690, 280))),                                     # bubble tail
    (Part (PEll 710 205 150 95)),                                                         # speech bubble
    (Part (GlyphPath '#@%!' 710 205 210 90 $bold) (C '#2a2a36'))
  )
}
