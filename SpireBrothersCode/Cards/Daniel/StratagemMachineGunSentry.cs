using BaseLib.Cards.Variables;
using BaseLib.Commands;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using SpireBrothers.SpireBrothersCode.Mechanics;
using SpireBrothers.SpireBrothersCode.Powers;

namespace SpireBrothers.SpireBrothersCode.Cards.Daniel;

/// <summary>Helldivers 2. Costs 0. Requires Hands, Hands, Logic this turn. At the end of your turn, deal 3 damage to a random enemy 3 times.</summary>
public class StratagemMachineGunSentry() : DanielCard(0, CardType.Power, CardRarity.Uncommon, TargetType.Self)
{
    protected override IReadOnlyList<CardKeyword> StratagemCombo => [BrotherKeywords.Hands, BrotherKeywords.Hands, BrotherKeywords.Logic];
    public override IEnumerable<CardKeyword> CanonicalKeywords => [BrotherKeywords.Stratagem];
    protected override IEnumerable<DynamicVar> CanonicalVars => [new PowerVar<MachineGunSentryPower>(3), new DamageVar(MachineGunSentryPower.DamagePerShot, ValueProp.Unpowered)];

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        await CommonActions.ApplySelf<MachineGunSentryPower>(ctx, this);
    }

    protected override void OnUpgrade() => DynamicVars["MachineGunSentryPower"].UpgradeValueBy(1);
}
