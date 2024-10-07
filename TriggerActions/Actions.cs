using StardewValley.Delegates;
using StardewValley;
using StardewModdingAPI;

namespace StrongerTools.TriggerActions;

public class Actions {

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

}