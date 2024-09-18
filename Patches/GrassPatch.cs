using StardewValley;

namespace StrongerTools.Patches;

static class GrassPatch {

    public static void DoCollisionAction_Postfix() {
        Farmer player = Game1.player;
        if (player.stats.Get("Book_Grass") != 0) {
            player.temporarySpeedBuff = 0;
        }
    }

}