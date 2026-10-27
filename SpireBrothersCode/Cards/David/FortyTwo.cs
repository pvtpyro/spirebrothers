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

namespace SpireBrothers.SpireBrothersCode.Cards.David;

/// <summary>
/// The answer to life, the universe, and everything (he's still working out the question). Rant. Can only be played
/// if you've played 2+ Rants this turn. Deal 42 damage. Exhaust.
/// </summary>
public class FortyTwo() : DavidCard(2, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [BrotherKeywords.Rant, CardKeyword.Exhaust];
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(42, ValueProp.Move), new DynamicVar("RantsNeeded", 2)];

    protected override bool IsPlayable
    {
        get
        {
            if (!base.IsPlayable) return false;
            try
            {
                return !IsMutable || Owner?.PlayerCombatState == null || Rants >= DynamicVars["RantsNeeded"].IntValue;
            }
            catch (Exception e)
            {
                MainFile.Logger.Error($"Playable check failed on {GetType().Name}: {e}");
                return false;
            }
        }
    }

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        await CommonActions.CardAttack(this, play, vfx: "vfx/vfx_heavy_blunt").Execute(ctx);
    }

    protected override void OnUpgrade() => EnergyCost.UpgradeBy(-1);
}
