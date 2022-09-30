using System;
using System.IO;
using Terraria.ModLoader;
using Terraria.ModLoader.Config;

namespace NPCKillCounter;

public class NPCKillCounter : Mod
{
    public override object Call(params object[] args)
    {
        NPCDefinition GetNPC(object obj)
        {
            return obj switch
            {
                int i => new NPCDefinition(i),
                string i => new NPCDefinition(i),
                NPCDefinition i => i,
                _ => throw new ArgumentException("Call Parameter error")
            };
        }

        switch ((string)args[0]) // 注意调用的是客户端/服务端
        {
            case "Count":
            {
                var npc = GetNPC(args[1]);
                return ModContent.GetInstance<NPCKillCounterSystem>().Count[npc.ToString()];
            }
            case "Modify":
            {
                var npc = GetNPC(args[1]);
                return ModContent.GetInstance<NPCKillCounterSystem>().Count[npc.ToString()] = Convert.ToInt32(args[2]);
            }
            default:
                throw new ArgumentException("Call Parameter error");
        }
    }

    public override void HandlePacket(BinaryReader bin, int plr)
    {
        ModContent.GetInstance<NPCKillCounterSystem>().Count[new NPCDefinition(bin.ReadInt32()).ToString()] = bin.ReadInt32();
    }
}