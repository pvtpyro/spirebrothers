using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using SpireBrothers.SpireBrothersCode.Powers;

namespace SpireBrothers.SpireBrothersCode.Cards.Daniel;

/// <summary>
/// Wife Aggro ("Walkies"): sometimes he has to take the dog out. Shares its name with Tim's card.
/// Costs 1 (the energy he loses now). Next turn, gain 2 energy and the dog bites a random enemy for 8.
/// </summary>
public class WifeAggroDaniel() : DanielCard(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [BrotherKeywords.Hands];
    protected override IEnumerable<DynamicVar> CanonicalVars => [new EnergyVar(2), new PowerVar<WalkiesPower>(8)];

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        await PowerCmd.Apply<EnergyNextTurnPower>(ctx, Owner.Creature, DynamicVars.Energy.BaseValue, Owner.Creature, this);
        await CommonActions.ApplySelf<WalkiesPower>(ctx, this);
    }

    protected override void OnUpgrade() => DynamicVars["WalkiesPower"].UpgradeValueBy(4);
}
