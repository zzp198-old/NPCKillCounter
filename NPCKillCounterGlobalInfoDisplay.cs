using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.Config;

namespace NPCKillCounter;

public class NPCKillCounterGlobalInfoDisplay : GlobalInfoDisplay
{
    public override void ModifyDisplayValue(InfoDisplay currentDisplay, ref string displayValue)
    {
        if (currentDisplay is not TallyCounterInfoDisplay)
        {
            return;
        }

        var npc = new NPC(); // 重新设置NPC,防止NPC死亡后无法继续显示
        npc.SetDefaults(NPCKillCounterPlayer.Hit);

        if (npc.type == 0) // NPC的名字不能为None
        {
            displayValue = Language.GetTextValue("GameUI.NoKillCount");
            return;
        }

        var name = Main.keyState.IsKeyDown(Main.FavoriteKey) ? NPCID.Search.GetName(npc.type) : npc.GivenOrTypeName;
        var count = NPCKillCounterSystem.Count[new NPCDefinition(npc.type).ToString()];
        displayValue = $"{name}: {count}";
    }
}