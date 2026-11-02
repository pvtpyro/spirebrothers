using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Nodes.Cards;
using MegaCrit.Sts2.Core.Nodes.Cards.Holders;
using MegaCrit.Sts2.Core.Nodes.CommonUi;
using MegaCrit.Sts2.Core.Nodes.Screens.CardLibrary;

namespace SpireBrothers.SpireBrothersCode.Cards;

/// <summary>
/// A "Titan Side" tickbox in the Compendium, under the game's "Upgrades" one (a copy of it): ticked, Danny's cards show
/// their Titan side there (DannyCard.ShowTitanInLibrary). It only touches cards with no owner, so a run's deck and
/// combat are never affected, and it unticks every time the Compendium opens.
/// </summary>
[HarmonyPatch(typeof(NCardLibrary))]
public static class TitanLibraryToggle
{
    private const string NodeName = "SpireBrothersTitanSide";

    private static NLibraryStatTickbox? Upgrades(NCardLibrary library) =>
        AccessTools.Field(typeof(NCardLibrary), "_viewUpgrades").GetValue(library) as NLibraryStatTickbox;

    private static NCardGrid? Grid(NCardLibrary library) =>
        AccessTools.Field(typeof(NCardLibrary), "_grid").GetValue(library) as NCardGrid;

    [HarmonyPatch(nameof(NCardLibrary._Ready))]
    [HarmonyPostfix]
    private static void AfterReady(NCardLibrary __instance)
    {
        try
        {
            if (Upgrades(__instance) is not { } upgrades || upgrades.GetParent() is not { } parent) return;
            var titan = (NLibraryStatTickbox)upgrades.Duplicate();
            titan.Name = NodeName;
            parent.AddChild(titan);
            parent.MoveChild(titan, upgrades.GetIndex() + 1);
            // Inside a layout container it lines up by itself; otherwise put it just under Upgrades.
            if (parent is not Container) titan.Position = upgrades.Position + new Vector2(0, upgrades.Size.Y + 6);
            titan.SetLabel(new LocString("card_keywords", "SPIREBROTHERS-TITAN.libraryToggle").GetRawText());
            titan.IsTicked = false;
            var library = __instance;
            titan.Connect(NTickbox.SignalName.Toggled, Callable.From<NTickbox>(box => Set(library, box.IsTicked)));
        }
        catch (Exception e)
        {
            MainFile.Logger.Error($"Titan Side tickbox failed: {e}");
        }
    }

    [HarmonyPatch(nameof(NCardLibrary.OnSubmenuOpened))]
    [HarmonyPostfix]
    private static void AfterOpened(NCardLibrary __instance)
    {
        DannyCard.ShowTitanInLibrary = false;
        if (Upgrades(__instance)?.GetParent()?.GetNodeOrNull<NLibraryStatTickbox>(NodeName) is { } titan) titan.IsTicked = false;
    }

    private static void Set(NCardLibrary library, bool on)
    {
        DannyCard.ShowTitanInLibrary = on;
        try
        {
            if (Grid(library) is not { } grid) return;
            foreach (var holder in grid.FindChildren("*", nameof(NGridCardHolder), true, false).OfType<NGridCardHolder>())
            {
                if (holder.CardNode is not { Model: DannyCard } card) continue;
                if (grid.IsShowingUpgrades && card.Model.IsUpgraded) card.ShowUpgradePreview();
                else card.UpdateVisuals(card.DisplayingPile, CardPreviewMode.Normal);
            }
        }
        catch (Exception e)
        {
            MainFile.Logger.Error($"Titan Side refresh failed: {e}");
        }
    }
}
