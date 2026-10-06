# pixelgen: the brothers' pixel-art bodies

A small C# program that draws every combat animation frame (64 x 80) for all four brothers. Each brother is built
from simple shapes on a shared skeleton (`Rig.cs`), and each animation is a list of poses (`Brothers.cs`).

- `Rig.cs`: the skeleton and shared body parts (head, beard, hair, arms, legs, torso outline). The bodies are drawn
  three-quarters turned to the right, like the vanilla characters.
- `Brothers.cs`: each brother's clothes, props, and animation poses (David's sunglasses are in `DavidArt.DrawHeadExtras`).
- `Canvas.cs`: the drawing and PNG code.
- `Backgrounds.cs`: the character select backgrounds (480 x 270, one scene per brother inspired by his favorite
  games, with him standing on the right). Saved as `out/<name>/select_bg.png`, previews in `out/select_bg_<name>_preview.png`.
- `Program.cs`: renders every frame into `out/<name>/frames/`, plus preview sheets (`out/preview_<name>.png`,
  `out/lineup.png`, `out/heads.png`, and `out/campfire.png` / `out/campfire_frames.png` for the rest-site loop).
- Animations: `idle`, `attack`, `cast`, `hit`, `dead` (combat) and `rest` (sitting on a log at rest sites: Daniel
  tinkers with his multimeter, David flips a coin, Joshua strums, Tim toasts a marshmallow until it catches fire).
- `SelectButtons.cs.txt`: renders the character select buttons from the first idle frame. To use it, rename
  `Program.cs` out of the way and rename this file to `Program.cs`. It reads the original buttons in
  `art_originals/select_icons_old/` to keep the name text and writes to `out/select/`; copy `select.png` and
  `select_locked.png` from there into each brother's image folder.

## Running it

Double-click `Render frames.bat` (it opens the `out` folder when it's done), or from this folder:

```
"C:\Program Files\dotnet\dotnet.exe" run -c Release -- out
```

Check the previews in `out/`, then copy `out/<name>/frames/*.png` into
`SpireBrothers/images/characters/<name>/frames/` (and `out/<name>/select_bg.png` into
`SpireBrothers/images/characters/<name>/`) and rebuild the mod.

The main mod project ignores this folder (`<Compile Remove="art_originals/**" />` in `SpireBrothers.csproj`), and
Godot ignores it because of the `.gdignore` in `art_originals/`.

A copy also lives on the Desktop in `pixelgen`. Edit either one, but copy changes back into
`art_originals/pixelgen/` in the project so they are saved with it.
