using System;
using System.Collections.Generic;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.Config;
using Terraria.ModLoader.IO;

namespace NPCKillCounter;

public class NPCKillCounterSystem : ModSystem
{
    // 包含所有MOD的NPC记录,同步时只会同步当前已有的NPC
    internal static DefaultDictionary<string, int> Count; // NPC.killCount用于旗帜计算,不能修改其值.

    public override void Load()
    {
        IL.Terraria.NPC.NPCLoot += il => // 单人客户端/服务端,同步过早,只能通过修改83号包来实现
        {
            var ilCursor = new ILCursor(il);
            ilCursor.GotoNext(MoveType.After, i => i.MatchCall(typeof(NPCLoader), nameof(NPCLoader.OnKill)));
            ilCursor.Emit(OpCodes.Ldarg_0); // NPC
            ilCursor.EmitDelegate<Action<NPC>>(npc => { Count[new NPCDefinition(npc.type).ToString()]++; });
        }; // 死亡计数
        IL.Terraria.MessageBuffer.GetData += il => // 发送全部的死亡计数
        {
            var ilCursor = new ILCursor(il);
            ilCursor.GotoNext(MoveType.Before, i => i.MatchLdcI4(289));
            ilCursor.GotoNext(MoveType.Before, i => i.MatchLdcI4(670));
            ilCursor.Remove();
            ilCursor.Emit(OpCodes.Ldc_I4, 289);
        };
        On.Terraria.NPC.CountKillForBannersAndDropThem += (invoke, npc) => // 在NPCLoot中执行
        {
            if (Main.netMode == NetmodeID.Server)
            {
                var mp = Mod.GetPacket();
                mp.Write(npc.type);
                mp.Write(Count[new NPCDefinition(npc.type).ToString()]);
                mp.Send();
            }

            invoke(npc);
        };
    }

    public override void OnWorldLoad()
    {
        Count = new DefaultDictionary<string, int>();
    }

    public override void OnWorldUnload()
    {
        Count = null;
    }

    public override void SaveWorldData(TagCompound tag)
    {
        if (Count == null)
        {
            return;
        }

        var data = new List<string>();
        foreach (var (name, count) in Count)
        {
            data.Add($"{name}: {count}");
        }

        tag.Set(nameof(Count), data);
    }

    public override void LoadWorldData(TagCompound tag)
    {
        if (tag.ContainsKey(nameof(Count)))
        {
            tag.Get<List<string>>(nameof(Count)).ForEach(obj =>
            {
                var item = obj.Split(": ", 2);
                if (item.Length != 2 || !int.TryParse(item[1], out var count))
                {
                    return;
                }

                Count[item[0]] = count;
            });
        }
    }

    public override bool HijackSendData(int whoAmI, int msgType, int remoteClient, int ignoreClient, NetworkText text, int number, float number2, float number3, float number4, int number5, int number6, int number7)
    {
        // 只有服务端才会发送NPCKillCountDeathTally
        if (msgType == MessageID.NPCKillCountDeathTally && Netplay.Clients[remoteClient].State == 3)
        {
            var mp = Mod.GetPacket(); // 全体死亡计数在初次进入世界时同步
            mp.Write(number);
            mp.Write(Count[new NPCDefinition(number).ToString()]);
            mp.Send(remoteClient);
        }

        return false;
    }
}