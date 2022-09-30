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

        switch ((string)args[0])
        {
            case "Count":
            {
                var npc = GetNPC(args[1]);
                return NPCKillCounterSystem.Count[npc.ToString()];
            }
            case "Modify":
            {
                var npc = GetNPC(args[1]);
                return NPCKillCounterSystem.Count[npc.ToString()] = Convert.ToInt32(args[2]);
            }
            default:
                throw new ArgumentException("Call Parameter error");
        }
    }

    public override void HandlePacket(BinaryReader bin, int plr)
    {
        var type = bin.ReadInt32();
        var count = bin.ReadInt32();
        NPCKillCounterSystem.Count[new NPCDefinition(type).ToString()] = count;
        Console.WriteLine($"{new NPCDefinition(type)} {NPCKillCounterSystem.Count[new NPCDefinition(type).ToString()]}");
    }
}