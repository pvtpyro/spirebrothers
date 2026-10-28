using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using SpireBrothers.SpireBrothersCode.Mechanics;

namespace SpireBrothers.SpireBrothersCode.Powers;

/// <summary>
/// Home Sweet Home (Ancient). The Mastiff bites at the start of Huffman's turn as well as the end. The Metal for each
/// bite is given in Mastiff.Attack, so it counts every bite (Resisting Arrest, Loyal Companion, cards).
/// </summary>
public class HomeSweetHomePower : BrothersPower
{
    public const int MetalPerBite = 1;

    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Single;

    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        if (player.Creature != Owner) return;
        Flash();
        await Mastiff.Attack(choiceContext, player);
    }
}
