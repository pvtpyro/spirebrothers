using BaseLib.Cards.Variables;
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

namespace SpireBrothers.SpireBrothersCode.Cards.Joshua;

/// <summary>Retired 2026-10-26 (Joshua isn't a support anymore); kept so saved runs that have it still load. Was: Terraria. ALL players heal 4. Exhaust.</summary>
public class HeartCrystal() : JoshuaCard(1, CardType.Skill, CardRarity.Basic, TargetType.Self), IGroupHealCard
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];
    protected override IEnumerable<DynamicVar> CanonicalVars => [new HealVar(4)];

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        foreach (var player in AllPlayers) await CreatureCmd.Heal(player, DynamicVars.Heal.BaseValue);
    }

    protected override void OnUpgrade() => DynamicVars.Heal.UpgradeValueBy(2);
}
