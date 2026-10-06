# pixelgen: the brothers' pixel-art bodies

A small C# program that draws every combat animation frame (64 x 80) for all four brothers. Each brother is built
from simple shapes on a shared skeleton (`Rig.cs`), and each animation is a list of poses (`Brothers.cs`).

- `Rig.cs`: the skeleton and shared body parts (head, beard, hair, arms, legs, torso outline). The bodies are drawn
  three-quarters turned to the right, like the vanilla characters.
- `Brothers.cs`: each brother's clothes, props, and animation poses (David's sunglasses are in `DavidArt.DrawHeadExtras`).
- `Canvas.cs`: the drawing and PNG code.
- `Program.cs`: renders every frame into `out/<name>/frames/`, plus preview sheets (`out/preview_<name>.png`,
  `out/lineup.png`, `out/heads.png`).
- `SelectButtons.cs.txt`: renders the character select buttons from the first idle frame. To use it, rename
  `Program.cs` out of the way and rename this file to `Program.cs`. It reads the current `select.png` files to keep
  the name text.

## Running it

From this folder:

```
"C:\Program Files\dotnet\dotnet.exe" run -c Release -- out
```

Check the previews in `out/`, then copy `out/<name>/frames/*.png` into
`SpireBrothers/images/characters/<name>/frames/` and rebuild the mod.

The main mod project ignores this folder (`<Compile Remove="art_originals/**" />` in `SpireBrothers.csproj`), and
Godot ignores it because of the `.gdignore` in `art_originals/`.
