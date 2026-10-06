param($Root, $Out, $Pattern = '*.png', [int]$Cell = 160, [int]$Cols = 8, [int]$Max = 64)
Add-Type -AssemblyName System.Drawing
$files = Get-ChildItem $Root -Recurse -Filter $Pattern | Select-Object -First $Max
$rows = [math]::Ceiling($files.Count / $Cols)
$bmp = [System.Drawing.Bitmap]::new($Cols * $Cell, $rows * $Cell)
$g = [System.Drawing.Graphics]::FromImage($bmp); $g.Clear([System.Drawing.Color]::FromArgb(255, 70, 60, 80)); $g.InterpolationMode = 'HighQualityBicubic'
$i = 0
foreach ($f in $files) {
  $im = [System.Drawing.Image]::FromFile($f.FullName)
  $s = [math]::Min(($Cell - 8) / $im.Width, ($Cell - 8) / $im.Height)
  $w = $im.Width * $s; $h = $im.Height * $s
  $g.DrawImage($im, ($i % $Cols) * $Cell + ($Cell - $w) / 2, [math]::Floor($i / $Cols) * $Cell + ($Cell - $h) / 2, $w, $h)
  $im.Dispose(); $i++
}
$bmp.Save($Out)
