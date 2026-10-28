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

namespace SpireBrothers.SpireBrothersCode.Cards.Huffman;

/// <summary>Total Annihilation. At the end of your turn, deal 3 damage to a random enemy. Build 4: 6 instead.</summary>
public class NanolatheTurret() : HuffmanCard(1, CardType.Power, CardRarity.Uncommon, TargetType.Self)
{
    protected override int BuildCost => 4;
    public override IEnumerable<CardKeyword> CanonicalKeywords => [BrotherKeywords.Build];
    protected override IEnumerable<DynamicVar> CanonicalVars => [new PowerVar<NanolatheTurretPower>(3), new DynamicVar("BuildDamage", 6)];

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        decimal damage = await Build(ctx) ? DynamicVars["BuildDamage"].BaseValue : DynamicVars["NanolatheTurretPower"].BaseValue;
        await PowerCmd.Apply<NanolatheTurretPower>(ctx, Owner.Creature, damage, Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars["NanolatheTurretPower"].UpgradeValueBy(1);
        DynamicVars["BuildDamage"].UpgradeValueBy(2);
    }
}
