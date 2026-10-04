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

namespace SpireBrothers.SpireBrothersCode.Cards.Tim;

/// <summary>Splinter Cell. Can only be played if you've played 3+ cards this turn. Deal 12 damage to ALL enemies.</summary>
public class MarkAndExecute() : TimCard(1, CardType.Attack, CardRarity.Rare, TargetType.AllEnemies)
{
    public const int CardsNeeded = 3;
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(12, ValueProp.Move)];

    protected override bool IsPlayable
    {
        get
        {
            if (!base.IsPlayable) return false;
            try
            {
                return !IsInCombat || Owner?.PlayerCombatState == null || TimTracker.CardsThisTurn(Owner) >= CardsNeeded;
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
        await CommonActions.CardAttack(this, play, vfx: "vfx/vfx_attack_slash").Execute(ctx);
    }

    protected override void OnUpgrade() => DynamicVars.Damage.UpgradeValueBy(4);
}
