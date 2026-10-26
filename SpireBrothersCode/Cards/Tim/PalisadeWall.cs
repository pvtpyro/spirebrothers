using MegaCrit.Sts2.Core.Localization;
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

/// <summary>
/// Age of Empires. Gain 7 Block, +3 per Age. The wall follows the real tech tree (David pointed out nobody builds a
/// wooden wall in Castle Age): Palisade Wall in the Dark Age, Stone Wall in Feudal, Fortified Wall from Castle on.
/// </summary>
public class PalisadeWall() : TimCard(1, CardType.Skill, CardRarity.Common, TargetType.Self)
{
    protected override int? AgeBonusAt => Ages.Feudal;
    public override bool GainsBlock => true;
    public override IEnumerable<CardKeyword> CanonicalKeywords => [BrotherKeywords.Age];
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new CalculationBaseVar(7),
        new CalculationExtraVar(3),
        new CalculatedBlockVar(ValueProp.Move).WithMultiplier(AgesPast)
    ];

    private static decimal AgesPast(CardModel card, Creature? _) => Ages.Get(card.Owner);

    // The canonical card (compendium, card rewards before pickup) has no owner, so it keeps the Dark Age name.
    private int Age => !IsMutable || Owner == null ? Ages.Dark : Ages.Get(Owner);

    public override string Title
    {
        get
        {
            try
            {
                string key = Age switch
                {
                    >= Ages.Castle => "fortifiedTitle",
                    Ages.Feudal => "stoneTitle",
                    _ => null!
                };
                if (key == null) return base.Title;
                // Swap just the wall name, so whatever the game adds to the title (like the upgrade "+") stays.
                string palisade = new LocString("cards", $"{Id.Entry}.title").GetFormattedText();
                string wall = new LocString("cards", $"{Id.Entry}.{key}").GetFormattedText();
                return base.Title.Replace(palisade, wall);
            }
            catch (Exception e)
            {
                MainFile.Logger.Error($"Palisade Wall title failed: {e}");
                return base.Title;
            }
        }
    }

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        await CommonActions.CardBlock(this, play);
    }

    protected override void OnUpgrade() => DynamicVars.CalculationBase.UpgradeValueBy(3);
}
