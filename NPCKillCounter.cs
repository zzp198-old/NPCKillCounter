using System;
using System.IO;
using Terraria.ModLoader;
using Terraria.ModLoader.Config;

namespace NPCKillCounter;

public class NPCKillCounter : Mod
{
    public override object Call(params object[] args)
    {
        return (string)args[0] switch // 注意调用的是那个客户端还是服务端
        {
            "PlayerCount" => NPCKillCounterPlayer.Count,
            "SystemCount" => NPCKillCounterSystem.Count,
            _ => throw new ArgumentException("Call Parameter error")
        };
    }

    public override void HandlePacket(BinaryReader bin, int plr)
    {
        NPCKillCounterSystem.Count[new NPCDefinition(bin.ReadInt32()).ToString()] = bin.ReadInt32();
    }
}