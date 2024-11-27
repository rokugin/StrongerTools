using StardewModdingAPI;
using StardewValley.Tools;
using StardewValley;
using HarmonyLib;
using StardewModdingAPI.Events;
using StardewValley.TerrainFeatures;
using StrongerTools.Patches;
using StrongerTools.TriggerActions;

namespace StrongerTools;

public class ModEntry : Mod {

    public static IMonitor SMonitor = null!;
    bool hardwareCursor = false;
    public static bool PassOut = false;

    public override void Entry(IModHelper helper) {
        SMonitor = Monitor;

        helper.Events.GameLoop.GameLaunched += OnGameLaunched;
        helper.Events.Input.ButtonsChanged += OnButtonsChanged;
        helper.Events.GameLoop.DayStarted += OnDayStarted;
        helper.Events.GameLoop.UpdateTicked += OnUpdateTicked;
        helper.Events.GameLoop.ReturnedToTitle += OnReturnedToTitle;
        helper.Events.GameLoop.UpdateTicked += OnUpdateTicked;
        helper.Events.GameLoop.TimeChanged += OnTimeChanged;

        var harmony = new Harmony(ModManifest.UniqueID);

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

    private void OnTimeChanged(object? sender, TimeChangedEventArgs e) {
        if (PassOut) return;
        
        if (e.NewTime == 2550) {
            Game1.timeOfDay = 2540;
        }
    }

    private void OnGameLaunched(object? sender, GameLaunchedEventArgs e) {
        EnableHardwareCursor();
        Actions.RegisterTriggerActions();
        Program.enableCheats = true;
    }

    private void OnReturnedToTitle(object? sender, ReturnedToTitleEventArgs e) {
        hardwareCursor = false;
    }

    private void OnUpdateTicked(object? sender, UpdateTickedEventArgs e) {
        if (hardwareCursor == false) {
            EnableHardwareCursor();
        }
    }

    private void OnDayStarted(object? sender, DayStartedEventArgs e) {
        Game1.netWorldState.Value.canDriveYourselfToday.Value = true;
    }

    private void OnButtonsChanged(object? sender, ButtonsChangedEventArgs e) {
        if (!Context.IsPlayerFree) return;

        if (e.Pressed.Any(button => button.IsUseToolButton())
            && Game1.player.CurrentTool is Item item) {
            if (item is Pickaxe pickaxe) {
                if (pickaxe.additionalPower.Value < Game1.player.miningLevel.Value) {
                    pickaxe.additionalPower.Value = Game1.player.miningLevel.Value;
                }
                pickaxe.description =
                    ItemRegistry.GetDataOrErrorItem(pickaxe.QualifiedItemId).Description + $"\n\n+{pickaxe.additionalPower.Value} Power";
            }
            if (item is Axe axe) {
                int desiredPower = Math.Max(0, (int)(Game1.player.foragingLevel.Value / 3));
                if (axe.additionalPower.Value < desiredPower) {
                    axe.additionalPower.Value = desiredPower;
                }
                axe.description =
                        ItemRegistry.GetDataOrErrorItem(axe.QualifiedItemId).Description + $"\n\n+{axe.additionalPower.Value} Power";
            }
        }
    }

    void EnableHardwareCursor() {
        Game1.options.hardwareCursor = true;
        hardwareCursor = true;
    }

}