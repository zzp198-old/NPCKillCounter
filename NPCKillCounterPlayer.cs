using System;
using System.Collections.Generic;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;
using Terraria.ModLoader.Config;
using Terraria.ModLoader.IO;

namespace NPCKillCounter;

public class NPCKillCounterPlayer : ModPlayer
{
    internal static int Hit;
    internal DefaultDictionary<string, int> Count = new(); // 和角色绑定,不是静态字段但是不是同步

    public override void Load()
    {
        IL.Terraria.Player.OnKillNPC += il => // 单人本地客户端/多人本地客户端.
        {
            var ilCursor = new ILCursor(il);
            ilCursor.Emit(OpCodes.Ldarg_1); // NPCKillAttempt
            ilCursor.Emit(OpCodes.Ldfld, typeof(NPCKillAttempt).GetField(nameof(NPCKillAttempt.npc))); // NPCKillAttempt
            ilCursor.EmitDelegate<Action<NPC>>(npc => { Count[new NPCDefinition(npc.type).ToString()]++; });
        };
    }

    public override void SaveData(TagCompound tag)
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

    public override void LoadData(TagCompound tag)
    {
        Count = new DefaultDictionary<string, int>();
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

    public override void OnHitNPC(Item item, NPC target, int damage, float knockback, bool crit)
    {
        Hit = target.type;
    }

    public override void OnHitNPCWithProj(Projectile proj, NPC target, int damage, float knockback, bool crit)
    {
        Hit = target.type;
    }
}