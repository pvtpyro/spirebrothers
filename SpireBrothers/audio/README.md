# Brother sound effects

Drop sound files in this folder with these exact names, then rebuild (`dotnet publish -c Release`).
Each one plays automatically once it's here. Until then, the game uses a built-in sound or stays silent.
`.ogg` or `.wav` both work (if both exist, `.ogg` wins).

| File name | When it plays | Until then |
|---|---|---|
| `daniel_wired` | Daniel's first Wired effect each turn | game's lightning zap |
| `daniel_barrage` | Daniel plays Orbital Barrage | silent |
| `david_gold` | David gains Gold during combat | game's coin sound |
| `david_rant` | David's 3rd Rant in a turn | silent |
| `joshua_chorus` | Joshua plays any Chorus card | silent |
| `joshua_heal` | Joshua's heal-everyone cards and Honey Lemon Tea | game's heal sound |
| `tim_kids` | Tim's Kids attack | silent |
| `tim_age_up` | Tim Ages Up | silent |

Tips for recording:
- Keep each clip short: under 2 seconds, ideally under 1. Trim the silence at the start so it feels instant.
- A phone voice memo is fine. Free tools like Audacity can trim the clip and export it as `.ogg`.
- Keep the volume moderate. The game's SFX volume slider applies on top.
- The same sound won't replay within 0.4 seconds, so quick repeats don't stack up.
