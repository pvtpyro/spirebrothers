using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using SpireBrothers.SpireBrothersCode.Mechanics;
using Brothers = SpireBrothers.SpireBrothersCode.Character;

namespace SpireBrothers.SpireBrothersCode.Cards.Tim;

/// <summary>
/// Duo with Daniel: a LAN party that never ends (Script runs it again next turn). ALL players gain 3 Block and draw
/// 1 card next turn.
/// </summary>
public class LanParty() : TimCard(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self), IDuoCard
{
    public Type Partner => typeof(Brothers.Daniel);
    public override bool GainsBlock => true;
    public override IEnumerable<CardKeyword> CanonicalKeywords => [BrotherKeywords.Duo, BrotherKeywords.Script];
    protected override IEnumerable<DynamicVar> CanonicalVars => [new BlockVar(3, ValueProp.Move), new CardsVar(1)];

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        foreach (var player in AllPlayers)
        {
            await CreatureCmd.GainBlock(player, DynamicVars.Block, play);
            await PowerCmd.Apply<DrawCardsNextTurnPower>(ctx, player, DynamicVars.Cards.BaseValue, Owner.Creature, this);
        }
    }

    protected override void OnUpgrade() => DynamicVars.Block.UpgradeValueBy(2);
}
