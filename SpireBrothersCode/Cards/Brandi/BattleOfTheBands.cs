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
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Combat;

namespace SpireBrothers.SpireBrothersCode.Cards.Brandi;

/// <summary>Duo with Joshua: her metal against his ballads. Deal 9 damage. Joshua gains 2 Verses.</summary>
public class BattleOfTheBands() : BrandiCard(1, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy), IDuoCard
{
    public Type Partner => typeof(SpireBrothers.SpireBrothersCode.Character.Joshua);
    public override IEnumerable<CardKeyword> CanonicalKeywords => [BrotherKeywords.Duo];
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(9, ValueProp.Move), new DynamicVar("Verses", 2)];

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        await CommonActions.CardAttack(this, play, vfx: "vfx/vfx_heavy_blunt").Execute(ctx);
        foreach (var joshua in DuoCards.Partners(CombatState, Partner).Where(p => p.Creature.IsAlive))
            await Verses.Gain(ctx, joshua, DynamicVars["Verses"].BaseValue, this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(3);
        DynamicVars["Verses"].UpgradeValueBy(1);
    }
}
