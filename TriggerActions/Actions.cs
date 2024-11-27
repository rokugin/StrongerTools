using StardewValley.Delegates;
using StardewValley;
using StardewModdingAPI;
using Microsoft.Xna.Framework;
using StardewValley.Triggers;

namespace StrongerTools.TriggerActions;

public class Actions {

    public static void RegisterTriggerActions() {
        TriggerActionManager.RegisterAction("rokugin.PlayerHealth", ChangePlayerHealth);
        TriggerActionManager.RegisterAction("rokugin.PlayerStamina", ChangePlayerStamina);
        TriggerActionManager.RegisterAction("rokugin.FixHealth", RevalidatePlayerHealth);
        TriggerActionManager.RegisterAction("rokugin.PassOut", ChangePassingOut);
    }

    private static bool ChangePassingOut(string[] args, TriggerActionContext context, out string error) {
        if (ArgUtility.TryGetBool(args, 1, out bool value, out error)) {
            ModEntry.PassOut = value;
            return true;
        }
        return false;
    }

    public static bool ChangePlayerHealth(string[] args, TriggerActionContext context, out string error) {
        if (ArgUtility.TryGet(args, 1, out string amount, out error, allowBlank: false)) {
            Farmer player = Game1.player;
            switch (amount) {
                case "full":
                    player.health = Game1.player.maxHealth;
                    break;
                case "half":
                    player.health = Game1.player.maxHealth / 2;
                    break;
                case "kill":
                    player.health = 0;
                    break;
            }

            if (int.TryParse(amount, out int healthAmount)) {
                if (player.health + healthAmount <= 0) {
                    player.health = 1;
                } else if (player.health + healthAmount > player.maxHealth) {
                    player.health = player.maxHealth;
                } else {
                    player.health += healthAmount;
                }
            }
            ModEntry.SMonitor.Log($"\nPlayer health changed, new value: {player.health}\n", LogLevel.Info);
            return true;
        }
        return false;
    }

    public static bool ChangePlayerStamina(string[] args, TriggerActionContext context, out string error) {
        if (ArgUtility.TryGet(args, 1, out string amount, out error, allowBlank: false)) {
            Farmer player = Game1.player;
            switch (amount) {
                case "full":
                    player.stamina = Game1.player.MaxStamina;
                    break;
                case "half":
                    player.stamina = Game1.player.MaxStamina / 2;
                    break;
                case "empty":
                    player.stamina = 0;
                    break;
            }

            if (int.TryParse(amount, out int staminaAmount)) {
                if (player.stamina + staminaAmount <= 0) {
                    player.stamina = 1;
                } else if (player.stamina + staminaAmount > player.MaxStamina) {
                    player.stamina = player.MaxStamina;
                } else {
                    player.stamina += staminaAmount;
                }
            }
            ModEntry.SMonitor.Log($"\nPlayer stamina changed, new value: {player.stamina}\n", LogLevel.Info);
            return true;
        }
        return false;
    }

    public static bool RevalidatePlayerHealth(string[] args, TriggerActionContext context, out string error) {
        if (ArgUtility.TryGet(args, 0, out string value, out error, allowBlank: true)) {
            RevalidateHealth(Game1.player);
        }
        return false;
    }

    public static void RevalidateHealth(Farmer farmer) {
        int expected_max_health = 100;
        if (farmer.mailReceived.Contains("qiCave")) {
            expected_max_health += 25;
        }
        for (int i = 1; i <= farmer.GetUnmodifiedSkillLevel(4); i++) {
            if (!farmer.newLevels.Contains(new Point(4, i)) && i != 5 && i != 10) {
                expected_max_health += 5;
            }
        }
        if (farmer.professions.Contains(24)) {
            expected_max_health += 15;
        }
        if (farmer.professions.Contains(27)) {
            expected_max_health += 25;
        }
        if (farmer.maxHealth != expected_max_health) {
            ModEntry.SMonitor.Log("Max health not expected value, adjusting.", LogLevel.Warn);
            farmer.maxHealth = expected_max_health;
            farmer.health = farmer.maxHealth;
        }
    }

}