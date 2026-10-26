using BaseLib.Cards.Variables;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using SpireBrothers.SpireBrothersCode.Mechanics;
using SpireBrothers.SpireBrothersCode.Orbs;

namespace SpireBrothers.SpireBrothersCode.Cards.Joshua;

/// <summary>Piano. Song. Deal 3 damage 2 times. Each play speeds up the tempo: +1 hit for the rest of this combat.</summary>
public class Metronome() : JoshuaCard(1, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
{
    public const int HitsPerPlay = 1;
    public override IEnumerable<CardKeyword> CanonicalKeywords => [BrotherKeywords.Song];
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(3, ValueProp.Move), new RepeatVar(2)];

    // Hits added this combat. Combat cards are fresh copies, so this starts at 0 each fight.
    private int _tempo;

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        await CommonActions.CardAttack(this, play, DynamicVars.Repeat.IntValue, vfx: "vfx/vfx_attack_blunt").Execute(ctx);
        _tempo += HitsPerPlay;
        DynamicVars.Repeat.BaseValue += HitsPerPlay;
    }

    // Growing number: printed hits dimmed, current hits in green.
    protected override void AddExtraArgsToDescription(LocString description)
    {
        base.AddExtraArgsToDescription(description);
        description.Add("TempoGrown", _tempo > 0);
        description.Add("TempoStart", DynamicVars.Repeat.BaseValue - _tempo);
    }

    protected override void OnUpgrade() => DynamicVars.Damage.UpgradeValueBy(1);
}
