using Terraria;
using Terraria.ModLoader;

namespace NPCKillCounter;

public class NPCKillCounterPlayer : ModPlayer
{
    internal static int Hit;

    public override void OnHitNPC(Item item, NPC target, int damage, float knockback, bool crit)
    {
        Hit = target.type;
    }

    public override void OnHitNPCWithProj(Projectile proj, NPC target, int damage, float knockback, bool crit)
    {
        Hit = target.type;
    }
}