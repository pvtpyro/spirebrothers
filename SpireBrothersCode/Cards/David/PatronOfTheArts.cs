using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using SpireBrothers.SpireBrothersCode.Mechanics;
using Brothers = SpireBrothers.SpireBrothersCode.Character;

namespace SpireBrothers.SpireBrothersCode.Cards.David;

/// <summary>
/// Duo with Joshua: David spending money on someone is a big deal. Lose 15 Gold. Joshua gains 3 Verses and ALL players
/// gain 6 Block. Exact: don't lose the Gold.
/// </summary>
public class PatronOfTheArts() : DavidCard(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self), IDuoCard
{
    public Type Partner => typeof(Brothers.Joshua);
    public override bool GainsBlock => true;
    public override IEnumerable<CardKeyword> CanonicalKeywords => [BrotherKeywords.Duo, BrotherKeywords.Exact];
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new DynamicVar("Gold", 15), new DynamicVar("Verses", 3), new BlockVar(6, ValueProp.Move)];

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        int exact = await Exact.Check(ctx, this);
        if (exact == 0) await LoseGold(DynamicVars["Gold"].IntValue);
        foreach (var joshua in DuoCards.Partners(CombatState, Partner))
            await Verses.Gain(ctx, joshua, DynamicVars["Verses"].BaseValue, this);
        foreach (var player in CombatState!.Players.Where(p => p.Creature.IsAlive).ToList())
            await CreatureCmd.GainBlock(player.Creature, DynamicVars.Block, play);
    }

    protected override void OnUpgrade() => DynamicVars.Block.UpgradeValueBy(3);
}
