using StardewValley;

namespace StrongerTools.Patches;

static class FarmPatch {
    
    public static bool AddCrows_Prefix(Farm __instance) {
        return !__instance.buildings.Any(static building => building.buildingType.Value == "");
    }

}