using StardewValley.Objects.Trinkets;
using StardewValley.Objects;
using StardewValley.Tools;
using StardewValley;

namespace StrongerTools.Patches;

static class ItemPatch {

    public static void CanBeShipped_Postfix(Item __instance, ref bool __result) {
        switch (__instance) {
            case Ring:
            case Trinket:
            case Boots:
            case Clothing:
            case MeleeWeapon:
                __result = true;
                break;
        }
    }

}