using StardewModdingAPI;
using StardewValley.Tools;
using StardewValley;
using HarmonyLib;
using StardewValley.Triggers;
using StardewModdingAPI.Events;
using StardewValley.TerrainFeatures;
using StrongerTools.Patches;
using StrongerTools.TriggerActions;

namespace StrongerTools;

public class ModEntry : Mod {

    public static IMonitor SMonitor = null!;

    bool hardwareCursor = false;

    public override void Entry(IModHelper helper) {
        SMonitor = Monitor;

        helper.Events.GameLoop.GameLaunched += OnGameLaunched;
        helper.Events.Input.ButtonsChanged += OnButtonsChanged;
        helper.Events.GameLoop.DayStarted += OnDayStarted;
        helper.Events.GameLoop.UpdateTicked += OnUpdateTicked;
        helper.Events.GameLoop.ReturnedToTitle += OnReturnedToTitle;

        var harmony = new Harmony(ModManifest.UniqueID);
        //prefixes
        harmony.Patch(
            original: AccessTools.Method(typeof(Farm), nameof(Farm.addCrows)),
            prefix: new HarmonyMethod(typeof(FarmPatch), nameof(FarmPatch.AddCrows_Prefix))
        );
        //postfixes
        harmony.Patch(
            original: AccessTools.Method(typeof(Item), nameof(Item.canBeShipped)),
            postfix: new HarmonyMethod(typeof(ItemPatch), nameof(ItemPatch.CanBeShipped_Postfix))
        );
        harmony.Patch(
            original: AccessTools.Method(typeof(Grass), nameof(Grass.doCollisionAction)),
            postfix: new HarmonyMethod(typeof(GrassPatch), nameof(GrassPatch.DoCollisionAction_Postfix))
        );
        //transpilers
        harmony.Patch(
            original: AccessTools.Method(typeof(Game1), "drawHUD"),
            transpiler: new HarmonyMethod(typeof(Game1Patch), nameof(Game1Patch.DrawHUD_Transpiler))
        );
    }

    private void OnGameLaunched(object? sender, GameLaunchedEventArgs e) {
        EnableHardwareCursor();
        RegisterTriggerActions();
    }

    private void OnReturnedToTitle(object? sender, ReturnedToTitleEventArgs e) {
        hardwareCursor = false;
    }

    private void OnUpdateTicked(object? sender, UpdateTickedEventArgs e) {
        if (hardwareCursor) return;
        EnableHardwareCursor();
    }

    private void OnDayStarted(object? sender, DayStartedEventArgs e) {
        Game1.netWorldState.Value.canDriveYourselfToday.Value = true;
    }

    private void OnButtonsChanged(object? sender, ButtonsChangedEventArgs e) {
        if (!Context.IsPlayerFree) return;

        if (e.Pressed.Any(button => button.IsUseToolButton())
            && Game1.player.CurrentTool is Item item) {
            if (item is Pickaxe pickaxe) {
                switch (pickaxe.ItemId) {
                    case "GoldPickaxe":
                        if (pickaxe.additionalPower.Value < 3) pickaxe.additionalPower.Value = 3;
                        break;
                    case "IridiumPickaxe":
                        if (pickaxe.additionalPower.Value < 10) pickaxe.additionalPower.Value = 10;
                        break;
                }
                //Monitor.Log($"{pickaxe.Name} current additional power: +{pickaxe.additionalPower.Value}", LogLevel.Info);
            }
        }
    }

    void EnableHardwareCursor() {
        Game1.options.hardwareCursor = true;
        hardwareCursor = true;
    }

    private void RegisterTriggerActions() {
        TriggerActionManager.RegisterAction("rokugin.PlayerHealth", Actions.ChangePlayerHealth);
        TriggerActionManager.RegisterAction("rokugin.PlayerStamina", Actions.ChangePlayerStamina);
    }

}