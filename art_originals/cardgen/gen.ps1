param([switch]$Preview, [switch]$CardsOnly, [string]$Only = "", [ValidateSet('', 'relics', 'powers')][string]$Section = "")
Add-Type -AssemblyName System.Drawing
$ErrorActionPreference = 'Stop'
$here = $PSScriptRoot
# -CardsOnly: only the card portraits. Use it! The other sections (relics, powers, potions, character select,
# map markers, Ages) would overwrite art that was replaced later (pixel select buttons, pizza / sunglasses markers).
# -Only <ClassName>: render just that card. -Preview: write into .preview instead of the mod.
# -Section relics|powers: only that section (no cards); add -Only <file_name> (relics) or <ClassName> (powers) for one.
$proj = 'D:\projects\csharp\SpireBrothers'
$img = Join-Path $proj 'SpireBrothers\images'
$loc = Join-Path $proj 'SpireBrothers\localization\eng'
$out = if ($Preview) { Join-Path $here 'preview' } else { $img }

$emoji = [System.Drawing.FontFamily]::new('Segoe UI Emoji')
$bold = [System.Drawing.FontFamily]::new('Segoe UI Black')
function C($hex, $a = 255) { $c = [System.Drawing.ColorTranslator]::FromHtml($hex); [System.Drawing.Color]::FromArgb($a, $c) }
function Mix($a, $b, $t) { [System.Drawing.Color]::FromArgb(255, [int]($a.R + ($b.R - $a.R) * $t), [int]($a.G + ($b.G - $a.G) * $t), [int]($a.B + ($b.B - $a.B) * $t)) }
function Hsv($h, $s, $v) {
  $i = [math]::Floor($h * 6); $f = $h * 6 - $i; $p = $v * (1 - $s); $q = $v * (1 - $f * $s); $t = $v * (1 - (1 - $f) * $s)
  $r, $g, $b = switch ($i % 6) { 0 { $v, $t, $p } 1 { $q, $v, $p } 2 { $p, $v, $t } 3 { $p, $q, $v } 4 { $t, $p, $v } 5 { $v, $p, $q } }
  [System.Drawing.Color]::FromArgb(255, [int]($r * 255), [int]($g * 255), [int]($b * 255))
}
function Seed($s) { $h = 17; foreach ($ch in $s.ToCharArray()) { $h = ($h * 31 + [int]$ch) % 2147483647 }; [int]$h }

# Glyph (or text) as a path, scaled and centered in a box.
function GlyphPath($text, $cx, $cy, $w, $h, $family = $emoji) {
  $p = [System.Drawing.Drawing2D.GraphicsPath]::new()
  $p.AddString($text, $family, 0, 200, [System.Drawing.PointF]::new(0, 0), [System.Drawing.StringFormat]::GenericTypographic)
  $b = $p.GetBounds()
  $s = [math]::Min($w / $b.Width, $h / $b.Height)
  $m = [System.Drawing.Drawing2D.Matrix]::new()
  $m.Translate($cx, $cy); $m.Scale($s, $s); $m.Translate(-($b.X + $b.Width / 2), -($b.Y + $b.Height / 2))
  $p.Transform($m); $p
}
function Canvas($w, $h) {
  $bmp = [System.Drawing.Bitmap]::new($w, $h, [System.Drawing.Imaging.PixelFormat]::Format32bppArgb)
  $g = [System.Drawing.Graphics]::FromImage($bmp)
  $g.SmoothingMode = 'AntiAlias'; $g.InterpolationMode = 'HighQualityBicubic'; $g.PixelOffsetMode = 'HighQuality'; $g.CompositingQuality = 'HighQuality'
  $g.TextRenderingHint = 'AntiAliasGridFit'
  , @($bmp, $g)
}
function Save($bmp, $path) {
  $dir = Split-Path $path; if (-not (Test-Path $dir)) { New-Item -ItemType Directory -Force $dir | Out-Null }
  $bmp.Save($path, [System.Drawing.Imaging.ImageFormat]::Png)
}
function Resized($bmp, $w, $h) {
  $c = Canvas $w $h; $c[1].DrawImage($bmp, 0, 0, $w, $h); $c[1].Dispose(); $c[0]
}
function RoundRect($x, $y, $w, $h, $r) {
  $p = [System.Drawing.Drawing2D.GraphicsPath]::new()
  $p.AddArc($x, $y, $r * 2, $r * 2, 180, 90); $p.AddArc($x + $w - $r * 2, $y, $r * 2, $r * 2, 270, 90)
  $p.AddArc($x + $w - $r * 2, $y + $h - $r * 2, $r * 2, $r * 2, 0, 90); $p.AddArc($x, $y + $h - $r * 2, $r * 2, $r * 2, 90, 90)
  $p.CloseFigure(); $p
}
# White glyph, thick dark outline, soft drop shadow.
function DrawIcon($g, $path, $fill, $stroke, $strokeW, $shadow = 0) {
  if ($shadow -gt 0) {
    $sp = $path.Clone(); $m = [System.Drawing.Drawing2D.Matrix]::new(); $m.Translate($shadow, $shadow); $sp.Transform($m)
    $pen = [System.Drawing.Pen]::new((C '#000000' 90), $strokeW * 1.6); $pen.LineJoin = 'Round'
    $g.DrawPath($pen, $sp); $g.FillPath([System.Drawing.SolidBrush]::new((C '#000000' 90)), $sp)
  }
  $pen = [System.Drawing.Pen]::new($stroke, $strokeW); $pen.LineJoin = 'Round'
  $g.DrawPath($pen, $path)
  $g.FillPath([System.Drawing.SolidBrush]::new($fill), $path)
}

. (Join-Path $here 'icons.ps1')

$brotherColor = @{ Daniel = C '#1f9fb4'; David = C '#3e9a4c'; Joshua = C '#7d4fc9'; Tim = C '#c8552f' }
$brotherGlyph = @{ Daniel = '🤓'; David = '💰'; Joshua = '🎸'; Tim = '📐' }

function IdMap($file) {
  $map = @{}
  foreach ($m in [regex]::Matches((Get-Content (Join-Path $loc $file) -Raw), '"SPIREBROTHERS-([A-Z0-9_]+)\.title"')) {
    $id = $m.Groups[1].Value; $map[$id.Replace('_', '').ToLowerInvariant()] = $id.ToLowerInvariant()
  }
  $map
}
function Glyphs($file) {
  $map = @{}
  foreach ($line in Get-Content (Join-Path $here $file) -Encoding utf8) {
    if (-not $line.Trim() -or $line.StartsWith('#')) { continue }
    $parts = $line.Split(' '); $map[$parts[0]] = @{ G = $parts[1]; Debuff = $parts.Count -gt 2; Badge = $(if ($parts.Count -gt 2) { $parts[2] } else { '' }) }
  }
  $map
}

# ---------- Cards ----------
function CardArt($brother, $type, $glyph, $name, $badge = '') {
  $W = 1000; $H = 760
  $rnd = [System.Random]::new((Seed $name))
  $c = Canvas $W $H; $bmp = $c[0]; $g = $c[1]
  $base = $brotherColor[$brother]
  $base = switch ($type) { 'Attack' { Mix $base (C '#b3261e') 0.35 } 'Power' { Mix $base (C '#e8b931') 0.25 } default { $base } }
  $top = Mix $base (C '#ffffff') 0.25; $bot = Mix $base (C '#000000') 0.55
  $g.FillRectangle([System.Drawing.Drawing2D.LinearGradientBrush]::new([System.Drawing.Point]::new(0, 0), [System.Drawing.Point]::new(0, $H), $top, $bot), 0, 0, $W, $H)

  $cx = $W / 2; $cy = $H / 2 + 10
  $light = C '#ffffff' 38
  switch ($type) {
    'Power' {   # sunburst
      $n = 18; $rot = $rnd.NextDouble() * 20
      for ($i = 0; $i -lt $n; $i++) {
        $a1 = ($i * 360 / $n + $rot) * [math]::PI / 180; $a2 = (($i + 0.5) * 360 / $n + $rot) * [math]::PI / 180
        $pts = [System.Drawing.PointF[]]@([System.Drawing.PointF]::new($cx, $cy),
          [System.Drawing.PointF]::new($cx + 1200 * [math]::Cos($a1), $cy + 1200 * [math]::Sin($a1)),
          [System.Drawing.PointF]::new($cx + 1200 * [math]::Cos($a2), $cy + 1200 * [math]::Sin($a2)))
        $g.FillPolygon([System.Drawing.SolidBrush]::new($light), $pts)
      }
    }
    'Attack' {  # speed slashes
      $pen = [System.Drawing.Pen]::new((C '#ffffff' 45), 1); $pen.StartCap = 'Round'; $pen.EndCap = 'Round'
      for ($i = 0; $i -lt 14; $i++) {
        $pen.Width = 6 + $rnd.Next(22); $x = $rnd.Next(-200, $W); $y = $rnd.Next(0, $H); $len = 150 + $rnd.Next(350)
        $g.DrawLine($pen, $x, $y, $x + $len, $y - $len * 0.55)
      }
    }
    default {   # soft rings
      for ($r = 120; $r -lt 900; $r += 90) {
        $g.DrawEllipse([System.Drawing.Pen]::new((C '#ffffff' 28), 18), $cx - $r, $cy - $r, $r * 2, $r * 2)
      }
    }
  }
  # Glow behind the icon
  $glow = [System.Drawing.Drawing2D.GraphicsPath]::new(); $glow.AddEllipse($cx - 330, $cy - 330, 660, 660)
  $pgb = [System.Drawing.Drawing2D.PathGradientBrush]::new($glow); $pgb.CenterColor = C '#ffffff' 110; $pgb.SurroundColors = @((C '#ffffff' 0))
  $g.FillPath($pgb, $glow)
  # Cheesy sparkles
  for ($i = 0; $i -lt 7; $i++) {
    $sx = $rnd.Next(40, $W - 40); $sy = $rnd.Next(40, $H - 40)
    if ([math]::Abs($sx - $cx) -lt 300 -and [math]::Abs($sy - $cy) -lt 280) { continue }
    $sp = GlyphPath '✦' $sx $sy (30 + $rnd.Next(40)) (30 + $rnd.Next(40)) $bold
    $g.FillPath([System.Drawing.SolidBrush]::new((C '#ffffff' 150)), $sp)
  }
  if ($glyph.StartsWith('@') -and $CustomIcons[$glyph.Substring(1)]) {
    DrawParts $g (& $CustomIcons[$glyph.Substring(1)]) (Mix $base (C '#000000') 0.75)
  }
  else {
    $path = GlyphPath $glyph $cx $cy 520 480
    DrawIcon $g $path (C '#ffffff') (Mix $base (C '#000000') 0.75) 26 14
  }
  # Vignette
  $v = [System.Drawing.Drawing2D.GraphicsPath]::new(); $v.AddEllipse(-250, -220, $W + 500, $H + 440)
  $vb = [System.Drawing.Drawing2D.PathGradientBrush]::new($v); $vb.CenterColor = C '#000000' 0; $vb.SurroundColors = @((C '#000000' 150)); $vb.FocusScales = [System.Drawing.PointF]::new(0.6, 0.6)
  $g.FillRectangle($vb, 0, 0, $W, $H)
  # Reference badge: the game or hobby this card nods to, in a white disc in the bottom-right corner
  if ($badge) {
    $bx = $W - 150; $by = $H - 150; $br = 118
    $g.FillEllipse([System.Drawing.SolidBrush]::new((C '#000000' 90)), $bx - $br + 10, $by - $br + 12, $br * 2, $br * 2)
    $g.FillEllipse([System.Drawing.SolidBrush]::new((C '#ffffff')), $bx - $br, $by - $br, $br * 2, $br * 2)
    $g.DrawEllipse([System.Drawing.Pen]::new((Mix $base (C '#000000') 0.75), 16), $bx - $br, $by - $br, $br * 2, $br * 2)
    $bp = GlyphPath $badge $bx $by 150 150
    $pen = [System.Drawing.Pen]::new((C '#ffffff'), 10); $pen.LineJoin = 'Round'; $g.DrawPath($pen, $bp)
    $g.FillPath([System.Drawing.SolidBrush]::new((Mix $base (C '#000000') 0.55)), $bp)
  }
  $g.Dispose(); $bmp
}

$cardIds = IdMap 'cards.json'; $cardGlyphs = Glyphs 'glyphs.txt'
$cardCount = 0
foreach ($brother in $(if ($Section) { @() } else { 'Daniel', 'David', 'Joshua', 'Tim' })) {
  foreach ($f in Get-ChildItem (Join-Path $proj "SpireBrothersCode\Cards\$brother") -Filter *.cs) {
    $cls = $f.BaseName
    if ($Preview -and $cardCount -ge 12) { break }
    $id = $cardIds[$cls.ToLowerInvariant()]; if (-not $id) { Write-Warning "no card id for $cls"; continue }
    $type = [regex]::Match((Get-Content $f.FullName -Raw), 'CardType\.(\w+)').Groups[1].Value
    if ($Only -and $cls -ne $Only) { continue }
    $gl = $cardGlyphs[$cls]; $glyph = if ($gl) { $gl.G } else { Write-Warning "no glyph for $cls"; $brotherGlyph[$brother] }
    $big = CardArt $brother $type $glyph $cls $(if ($gl) { $gl.Badge } else { '' })
    Save $big (Join-Path $out "card_portraits\big\$id.png")
    $small = Resized $big 250 190; Save $small (Join-Path $out "card_portraits\$id.png")
    $big.Dispose(); $small.Dispose(); $cardCount++
  }
}
"cards: $cardCount"
if ($CardsOnly -or ($Only -and -not $Section)) { return }

# ---------- Relics ----------
$relicGlyphs = @{ and_you_know_what = '💬'; trusty_multimeter = '📟'; rubber_chicken_with_a_pulley = '🐔'; never_paid_a_mechanic = '🚗'
  grapefruit = '🍊'; monkey_phrasebook = '📕'; stone_monkey_head = '🗿'; monkey_wrench = '🔧'; old_wallet = '👛'; well_worn_guitar = '🎸'; family_minivan = '🚐'
  well_technically = '☝'; overstuffed_wallet = '💰'; signature_guitar = '🎸'; fifteen_passenger_van = '🚌' }
$relicColors = @{ grapefruit = '#ff8a5c'; well_worn_guitar = '#d9a25b'; old_wallet = '#9b6a3c'; family_minivan = '#6fa8dc'; stone_monkey_head = '#9aa3a8'
  rubber_chicken_with_a_pulley = '#ffe066'; monkey_phrasebook = '#e06666'; trusty_multimeter = '#f6b26b'; monkey_wrench = '#c0c7cc'; never_paid_a_mechanic = '#e06666'; and_you_know_what = '#8ecae6'
  well_technically = '#ffd966'; overstuffed_wallet = '#6aa84f'; signature_guitar = '#b36bff'; fifteen_passenger_van = '#4a86c8' }
# Solid silhouette of a line-art glyph: everything not reachable from the border.
function Silhouette($path, $size, $strokeW, $color) {
  $c = Canvas $size $size
  $pen = [System.Drawing.Pen]::new((C '#000000'), $strokeW); $pen.LineJoin = 'Round'
  $c[1].DrawPath($pen, $path); $c[1].FillPath([System.Drawing.Brushes]::Black, $path); $c[1].Dispose()
  $rect = [System.Drawing.Rectangle]::new(0, 0, $size, $size)
  $data = $c[0].LockBits($rect, 'ReadWrite', [System.Drawing.Imaging.PixelFormat]::Format32bppArgb)
  $px = [byte[]]::new($size * $size * 4); [System.Runtime.InteropServices.Marshal]::Copy($data.Scan0, $px, 0, $px.Length)
  $outside = [bool[]]::new($size * $size); $q = [System.Collections.Generic.Queue[int]]::new()
  for ($i = 0; $i -lt $size; $i++) { foreach ($s in $i, (($size - 1) * $size + $i), ($i * $size), ($i * $size + $size - 1)) { if (-not $outside[$s] -and $px[$s * 4 + 3] -lt 128) { $outside[$s] = $true; $q.Enqueue($s) } } }
  while ($q.Count) {
    $s = $q.Dequeue(); $x = $s % $size; $y = [math]::Floor($s / $size)
    foreach ($n in $(if ($x -gt 0) { $s - 1 }), $(if ($x -lt $size - 1) { $s + 1 }), $(if ($y -gt 0) { $s - $size }), $(if ($y -lt $size - 1) { $s + $size })) {
      if ($null -ne $n -and -not $outside[$n] -and $px[$n * 4 + 3] -lt 128) { $outside[$n] = $true; $q.Enqueue($n) }
    }
  }
  for ($s = 0; $s -lt $outside.Length; $s++) {
    $a = if ($outside[$s]) { $px[$s * 4 + 3] } else { 255 }
    $px[$s * 4] = $color.B; $px[$s * 4 + 1] = $color.G; $px[$s * 4 + 2] = $color.R; $px[$s * 4 + 3] = $a
  }
  [System.Runtime.InteropServices.Marshal]::Copy($px, 0, $data.Scan0, $px.Length); $c[0].UnlockBits($data)
  $c[0]
}
foreach ($k in $relicGlyphs.Keys) {
  if ($Section -eq 'powers' -or ($Only -and $k -ne $Only)) { continue }
  $size = 256; $pad = $size * 0.12; $sw = $size * 0.06
  $path = GlyphPath $relicGlyphs[$k] ($size / 2) ($size / 2) ($size - $pad * 2) ($size - $pad * 2)
  $c = Canvas $size $size; $g = $c[1]
  $shadow = Silhouette $path $size $sw (C '#000000'); $g.DrawImage($shadow, [System.Drawing.Rectangle]::new(6, 7, $size, $size), 0, 0, $size, $size, 'Pixel', (& { $ia = [System.Drawing.Imaging.ImageAttributes]::new(); $cm = [System.Drawing.Imaging.ColorMatrix]::new(); $cm.Matrix33 = 0.35; $ia.SetColorMatrix($cm); $ia }))
  $col = C $relicColors[$k]; $sil = Silhouette $path $size $sw (Mix $col (C "#ffffff") 0.6); $g.DrawImage($sil, 0, 0)
  $pen = [System.Drawing.Pen]::new((C '#1b1b1b'), $sw); $pen.LineJoin = 'Round'
  $g.DrawPath($pen, $path); $g.FillPath([System.Drawing.SolidBrush]::new($col), $path)
  $g.Dispose()
  Save $c[0] (Join-Path $out "relics\big\$k.png")
  $small = Resized $c[0] 94 94; Save $small (Join-Path $out "relics\$k.png"); $small.Dispose()
  $white = Silhouette $path $size ($sw * 1.6) (C '#ffffff'); $small = Resized $white 94 94
  Save $small (Join-Path $out "relics\${k}_outline.png"); $small.Dispose(); $white.Dispose()
  $c[0].Dispose(); $sil.Dispose(); $shadow.Dispose()
}
"relics: $($relicGlyphs.Count)"
if ($Section -eq 'relics') { return }

# ---------- Powers ----------
$powerIds = IdMap 'powers.json'; $powerGlyphs = Glyphs 'power_glyphs.txt'; $pc = 0
foreach ($f in Get-ChildItem (Join-Path $proj 'SpireBrothersCode\Powers') -Filter '*Power.cs') {
  $cls = $f.BaseName; if ($cls -eq 'BrothersPower' -or ($Only -and $cls -ne $Only)) { continue }
  $short = $cls.Substring(0, $cls.Length - 5)
  $id = $powerIds[$cls.ToLowerInvariant()]; if (-not $id) { Write-Warning "no power id for $cls"; continue }
  $gl = $powerGlyphs[$short]; if (-not $gl) { Write-Warning "no glyph for power $short"; continue }
  $fill = if ($gl.Badge -like '#*') { C $gl.Badge } elseif ($gl.Debuff) { C '#ff6b5e' } else { Hsv ((Seed $short) % 1000 / 1000.0) 0.45 1.0 }
  $size = 256; $pad = $size * 0.08; $sw = $size * 0.07
  $path = GlyphPath $gl.G ($size / 2) ($size / 2) ($size - $pad * 2) ($size - $pad * 2)
  $c = Canvas $size $size; $g = $c[1]
  $sil = Silhouette $path $size $sw (Mix $fill (C "#ffffff") 0.65); $g.DrawImage($sil, 0, 0)
  $pen = [System.Drawing.Pen]::new((C '#141414'), $sw); $pen.LineJoin = 'Round'
  $g.DrawPath($pen, $path); $g.FillPath([System.Drawing.SolidBrush]::new($fill), $path); $g.Dispose()
  Save $c[0] (Join-Path $out "powers\big\$id.png")
  $small = Resized $c[0] 64 64; Save $small (Join-Path $out "powers\$id.png")
  $small.Dispose(); $sil.Dispose(); $c[0].Dispose()
  $pc++
}
"powers: $pc"
if ($Section) { return }

# ---------- Potions ----------
$potions = @{ grog = @('#7ddc3a', '☠'); ook_ook_eek = @('#ffd43b', '🍌'); monkey_business = @('#a0522d', '🐒') }
foreach ($k in $potions.Keys) {
  $S = 256
  $bottle = [System.Drawing.Drawing2D.GraphicsPath]::new()
  $bottle.AddEllipse(48, 88, 160, 150); $bottle.AddRectangle([System.Drawing.RectangleF]::new(100, 36, 56, 70))
  $region = [System.Drawing.Region]::new($bottle)
  $c = Canvas $S $S; $g = $c[1]
  $g.DrawPath([System.Drawing.Pen]::new((C '#1b1b1b'), 16), $bottle)
  $g.FillPath([System.Drawing.SolidBrush]::new((C '#dff3ff' 200)), $bottle)
  $g.SetClip($region, [System.Drawing.Drawing2D.CombineMode]::Replace); $g.FillRectangle([System.Drawing.SolidBrush]::new((C $potions[$k][0])), 0, 128, $S, $S); $g.ResetClip()
  $g.FillRectangle([System.Drawing.SolidBrush]::new((C '#8b5a2b')), 94, 22, 68, 26)
  $g.FillEllipse([System.Drawing.SolidBrush]::new((C '#ffffff' 140)), 72, 110, 26, 46)
  $icon = GlyphPath $potions[$k][1] 128 182 70 60
  DrawIcon $g $icon (C '#ffffff') (C '#1b1b1b') 8
  Save $c[0] (Join-Path $out "potions\$k.png"); $g.Dispose(); $c[0].Dispose()
  $c = Canvas $S $S
  $c[1].DrawPath([System.Drawing.Pen]::new((C '#ffffff'), 16), $bottle); $c[1].FillPath([System.Drawing.Brushes]::White, $bottle)
  $c[1].FillRectangle([System.Drawing.Brushes]::White, 94, 22, 68, 26)
  Save $c[0] (Join-Path $out "potions\outline\$k.png"); $c[1].Dispose(); $c[0].Dispose()
}
"potions: $($potions.Count)"

# ---------- Character select, locked, map marker ----------
foreach ($b in $brotherColor.Keys) {
  $col = $brotherColor[$b]; $dir = Join-Path $out "characters\$($b.ToLower())"
  foreach ($locked in $false, $true) {
    $c = Canvas 132 195; $g = $c[1]
    $top = if ($locked) { C '#3a3a3a' } else { Mix $col (C '#ffffff') 0.3 }
    $bot = if ($locked) { C '#151515' } else { Mix $col (C '#000000') 0.6 }
    $g.FillRectangle([System.Drawing.Drawing2D.LinearGradientBrush]::new([System.Drawing.Point]::new(0, 0), [System.Drawing.Point]::new(0, 195), $top, $bot), 0, 0, 132, 195)
    for ($r = 20; $r -lt 200; $r += 22) { $g.DrawEllipse([System.Drawing.Pen]::new((C '#ffffff' 22), 6), 66 - $r, 80 - $r, $r * 2, $r * 2) }
    $path = GlyphPath $brotherGlyph[$b] 66 80 100 100
    if ($locked) { $g.FillPath([System.Drawing.SolidBrush]::new((C '#000000')), $path) }
    else { DrawIcon $g $path (C '#ffffff') (Mix $col (C '#000000') 0.75) 7 3 }
    $name = GlyphPath $b.ToUpper() 66 168 112 24 $bold
    $pen = [System.Drawing.Pen]::new((C '#000000'), 5); $pen.LineJoin = 'Round'; $g.DrawPath($pen, $name)
    $g.FillPath([System.Drawing.SolidBrush]::new($(if ($locked) { C '#666666' } else { C '#ffe9a8' })), $name)
    Save $c[0] (Join-Path $dir $(if ($locked) { 'select_locked.png' } else { 'select.png' })); $g.Dispose(); $c[0].Dispose()
  }
  $c = Canvas 128 128; $g = $c[1]
  $g.FillEllipse([System.Drawing.SolidBrush]::new((C '#1b1b1b')), 4, 4, 120, 120)
  $g.FillEllipse([System.Drawing.Drawing2D.LinearGradientBrush]::new([System.Drawing.Point]::new(0, 10), [System.Drawing.Point]::new(0, 118), (Mix $col (C '#ffffff') 0.3), (Mix $col (C '#000000') 0.4)), 12, 12, 104, 104)
  $path = GlyphPath $brotherGlyph[$b] 64 64 66 66
  DrawIcon $g $path (C '#ffffff') (C '#1b1b1b') 6
  Save $c[0] (Join-Path $dir 'map_marker.png'); $g.Dispose(); $c[0].Dispose()
}
"characters: 4"

# ---------- Tim's Ages ----------
$ages = @(@('DARK', '🌑', '#5b5b66'), @('FEUDAL', '⚔', '#8a6a3b'), @('CASTLE', '🏰', '#4f7cac'), @('IMPERIAL', '👑', '#d4a017'))
for ($i = 0; $i -lt 4; $i++) {
  $c = Canvas 192 144; $g = $c[1]; $col = C $ages[$i][2]
  $shape = RoundRect 6 6 180 132 26
  $g.FillPath([System.Drawing.Drawing2D.LinearGradientBrush]::new([System.Drawing.Point]::new(0, 6), [System.Drawing.Point]::new(0, 138), (Mix $col (C '#ffffff') 0.35), (Mix $col (C '#000000') 0.45)), $shape)
  $g.DrawPath([System.Drawing.Pen]::new((C '#1b1b1b'), 8), $shape)
  $g.DrawPath([System.Drawing.Pen]::new((C '#ffe9a8' 160), 3), (RoundRect 14 14 164 116 20))
  $icon = GlyphPath $ages[$i][1] 96 58 74 66
  DrawIcon $g $icon (C '#ffffff') (C '#1b1b1b') 6 3
  $t = GlyphPath $ages[$i][0] 96 114 130 22 $bold
  $pen = [System.Drawing.Pen]::new((C '#1b1b1b'), 5); $pen.LineJoin = 'Round'; $g.DrawPath($pen, $t); $g.FillPath([System.Drawing.SolidBrush]::new((C '#ffe9a8')), $t)
  Save $c[0] (Join-Path $out "ages\age$i.png"); $g.Dispose(); $c[0].Dispose()
}
"ages: 4"
