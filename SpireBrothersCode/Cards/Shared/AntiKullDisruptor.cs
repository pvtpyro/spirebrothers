using BaseLib.Abstracts;
using BaseLib.Extensions;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.ValueProps;
using SpireBrothers.SpireBrothersCode.Extensions;
using SpireBrothers.SpireBrothersCode.Powers;

namespace SpireBrothers.SpireBrothersCode.Cards.Shared;

/// <summary>
/// Token: in the Anubis fight one player, picked at random, starts with it (HalfAscendedPower). The weapon SG-1 built
/// to stop the Kull: strips Kull Armor from every enemy, then deals damage to ALL. Retain, Exhaust. Shared (the token
/// pool) because the hero can be anyone.
/// </summary>
[Pool(typeof(TokenCardPool))]
public class AntiKullDisruptor() : CustomCardModel(0, CardType.Attack, CardRarity.Token, TargetType.AllEnemies)
{
    public override string CustomPortraitPath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".BigCardImagePath();
    public override string PortraitPath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".CardImagePath();
    public override string BetaPortraitPath => $"beta/{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".CardImagePath();

    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Retain, CardKeyword.Exhaust];
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(6, ValueProp.Move)];
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<KullArmorPower>()];

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        foreach (var enemy in CombatState?.Enemies.Where(e => e.IsAlive).ToList() ?? [])
            if (enemy.GetPower<KullArmorPower>() is { } armor) await PowerCmd.Remove(armor);
        await CommonActions.CardAttack(this, play, vfx: "vfx/vfx_attack_slash").Execute(ctx);
    }

    protected override void OnUpgrade() => DynamicVars.Damage.UpgradeValueBy(3);
}
