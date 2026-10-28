using BaseLib.Abstracts;
using BaseLib.Utils;
using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.RestSite;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using SpireBrothers.SpireBrothersCode.Cards.John;

namespace SpireBrothers.SpireBrothersCode.Mechanics;

/// <summary>
/// John's talents. At rest sites he gets a Stimm Lab option next to Rest and Smith: he picks 1 of 3 random talents,
/// and it lasts the rest of the run (saved per player, like Tim's Age). Taking it uses up the rest site.
/// The talents themselves act in JohnTracker, Shroud.HpLoss, JohnCard.GrindTimes, the Cartel Special, and the Flame Altar.
/// </summary>
public static class StimmLab
{
    public enum Talent { Potency, FastActing, ToxicPayload, ShroudLungs, ExtendedRelease, ThickSkin, EfficientGrind, PackMule }

    public const int PotencyBlock = 2;
    public const int ToxicPayloadToxin = 2;
    public const int ThickSkinMaxHp = 4;
    public const int PackMuleBlock = 3;
    public const int Choices = 3;

    private static readonly SavedSpireField<Player, int>[] Ranks =
        Enum.GetValues<Talent>().Select(t => new SavedSpireField<Player, int>(() => 0, $"SpireBrothersTalent{t}")).ToArray();

    public static int MaxRank(Talent t) => t is Talent.FastActing or Talent.ShroudLungs or Talent.EfficientGrind ? 1 : 3;

    public static int Rank(Player? player, Talent t) => player == null ? 0 : Ranks[(int)t].Get(player);

    public static List<Talent> Available(Player player) => Enum.GetValues<Talent>().Where(t => Rank(player, t) < MaxRank(t)).ToList();

    /// <summary>
    /// The talents offered at this rest site. Seeded from the run and the floor (never the run RNG, which would shift
    /// every other random roll), so every player in co-op sees the same 3 for John.
    /// </summary>
    public static List<Talent> Offer(Player player)
    {
        var run = player.RunState;
        int seed = unchecked((int)run.Rng.CombatTargets.Seed * 31 + run.TotalFloor * 7919 + (int)player.NetId * 104729);
        var rng = new Random(seed);
        return Available(player).OrderBy(_ => rng.Next()).Take(Choices).ToList();
    }

    public static async Task Learn(Player player, Talent t)
    {
        Ranks[(int)t].Set(player, Rank(player, t) + 1);
        if (t == Talent.ThickSkin) await CreatureCmd.GainMaxHp(player.Creature, ThickSkinMaxHp);
    }

    /// <summary>His learned talents, one line each with the rank ("Potency 2/3"), for the Talents icon and the Stimm Lab button.</summary>
    public static List<string> LearnedLines(Player? player) =>
        Enum.GetValues<Talent>().Where(t => Rank(player, t) > 0).Select(t =>
        {
            var title = new LocString("cards", CardFor(t).Id.Entry + ".title").GetFormattedText();
            return $"[gold]{title}[/gold] {Rank(player, t)}/{MaxRank(t)}";
        }).ToList();

    public static int TotalRanks(Player? player) => Enum.GetValues<Talent>().Sum(t => Rank(player, t));

    public static CardModel CardFor(Talent t) => t switch
    {
        Talent.Potency => ModelDb.Card<TalentPotency>(),
        Talent.FastActing => ModelDb.Card<TalentFastActing>(),
        Talent.ToxicPayload => ModelDb.Card<TalentToxicPayload>(),
        Talent.ShroudLungs => ModelDb.Card<TalentShroudLungs>(),
        Talent.ExtendedRelease => ModelDb.Card<TalentExtendedRelease>(),
        Talent.ThickSkin => ModelDb.Card<TalentThickSkin>(),
        Talent.EfficientGrind => ModelDb.Card<TalentEfficientGrind>(),
        _ => ModelDb.Card<TalentPackMule>(),
    };

    /// <summary>The rest site button. Drop images/ui/stimm_lab.png in for a custom icon; until then it borrows Smith's.</summary>
    public static readonly string IconPath = $"{MainFile.ResPath}/images/ui/stimm_lab.png";
    public static string FallbackIconPath => ImageHelper.GetImagePath("ui/rest_site/option_smith.png");
    public static string ActiveIconPath => ResourceLoader.Exists(IconPath) ? IconPath : FallbackIconPath;
}

/// <summary>The Stimm Lab button at John's rest sites.</summary>
public class StimmLabRestSiteOption(Player owner) : RestSiteOption(owner)
{
    // Loc keys: rest_site_ui OPTION_SPIREBROTHERS_STIMM_LAB.name / .description / .descriptionDisabled.
    public override string OptionId => "SPIREBROTHERS_STIMM_LAB";

    public override bool IsEnabled => StimmLab.Available(Owner).Count > 0;

    // Lists what he already knows, so he can plan his next pick.
    public override LocString Description
    {
        get
        {
            var text = new LocString("rest_site_ui", $"OPTION_{OptionId}.{(IsEnabled ? "description" : "descriptionDisabled")}");
            var learned = StimmLab.LearnedLines(Owner);
            var heading = new LocString("rest_site_ui", $"OPTION_{OptionId}.learned").GetFormattedText();
            text.Add("Learned", learned.Count == 0 ? "" : $"\n\n{heading}\n{string.Join("\n", learned)}");
            return text;
        }
    }

    public override IEnumerable<string> AssetPaths => [StimmLab.ActiveIconPath];

    public override async Task<bool> OnSelect()
    {
        var offers = StimmLab.Offer(Owner);
        if (offers.Count == 0) return false;
        var cards = offers.Select(t => Owner.RunState.CreateCard(StimmLab.CardFor(t), Owner)).ToList();
        var picked = await CardSelectCmd.FromChooseACardScreen(new BlockingPlayerChoiceContext(), cards, Owner, canSkip: true);
        if (picked is not TalentCard talent) return false;
        await StimmLab.Learn(Owner, talent.Talent);
        return true;
    }
}

/// <summary>Adds the Stimm Lab to John's rest sites. A run-hook singleton, since rest sites are outside combat.</summary>
public class JohnRunTracker() : CustomSingletonModel(HookType.Run)
{
    public override bool TryModifyRestSiteOptions(Player player, ICollection<RestSiteOption> options)
    {
        if (player.Character is not SpireBrothers.SpireBrothersCode.Character.John) return false;
        options.Add(new StimmLabRestSiteOption(player));
        return true;
    }
}

/// <summary>The rest site option's icon path is private and points into the game's own folder, so ours is swapped in here.</summary>
[HarmonyPatch(typeof(RestSiteOption), nameof(RestSiteOption.Icon), MethodType.Getter)]
public static class StimmLabIconPatch
{
    public static bool Prefix(RestSiteOption __instance, ref Texture2D __result)
    {
        if (__instance is not StimmLabRestSiteOption) return true;
        __result = GD.Load<Texture2D>(StimmLab.ActiveIconPath);
        return false;
    }
}
