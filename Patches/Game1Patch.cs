using HarmonyLib;
using StardewValley;
using System.Reflection.Emit;

namespace StrongerTools.Patches;

static class Game1Patch {

    static bool GetHealthBarVisibilitySetting() {
        return true;
    }

    public static IEnumerable<CodeInstruction> DrawHUD_Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator) {
        var method = AccessTools.Method(typeof(Game1Patch), nameof(GetHealthBarVisibilitySetting));

        var matcher = new CodeMatcher(instructions, generator);

        matcher.MatchStartForward(
            new CodeMatch(OpCodes.Ldc_I4_1),
            new CodeMatch(OpCodes.Stsfld),
            new CodeMatch(OpCodes.Ldc_I4_1),
            new CodeMatch(OpCodes.Stsfld),
            new CodeMatch(OpCodes.Ldc_I4)
        ).ThrowIfNotMatch("Match not found for show health label");

        matcher.CreateLabel(out Label showHealthLabel);

        matcher.MatchStartBackwards(
            new CodeMatch(OpCodes.Call),
            new CodeMatch(OpCodes.Ldfld),
            new CodeMatch(OpCodes.Call),
            new CodeMatch(OpCodes.Ldfld),
            new CodeMatch(OpCodes.Bge)
        ).ThrowIfNotMatch("Match not found for health visibility setting check insertion point.");

        matcher.InsertAndAdvance(
            new CodeInstruction(OpCodes.Call, method),
            new CodeInstruction(OpCodes.Brtrue_S, showHealthLabel)
        );

        return matcher.InstructionEnumeration();
    }

}