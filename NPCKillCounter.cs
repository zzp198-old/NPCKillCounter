using System;
using System.IO;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.Config;

namespace NPCKillCounter;

public class NPCKillCounter : Mod
{
    public override object Call(params object[] args)
    {
        switch ((string)args[0]) // 注意调用的是客户端还是服务端
        {
            case "PlayerCount":
            {
                return Main.player[(int)args[1]].GetModPlayer<NPCKillCounterPlayer>().Count;
            }
            case "SystemCount":
            {
                return ModContent.GetInstance<NPCKillCounterSystem>().Count;
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