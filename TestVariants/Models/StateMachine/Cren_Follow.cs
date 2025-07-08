using TestVariants.Behaviours.Characters;
using UnityEngine;

namespace TestVariants.Models.StateMachine;

public class Cren_Follow(Cren cren) : Cren_StateBase(cren)
{
    private PlayerManager player;

    private float cooldown = 1f;

    private bool onCooldown = false;

    public override void Enter()
    {
        base.Enter();

        npc.Navigator.SetSpeed(13f);
        npc.Navigator.maxSpeed = 13f;
        player = npc.ec.Players[0];

        ChangeNavigationState(new NavigationState_TargetPlayer(npc, 0, player.transform.position));
    }

    public override void Update()
    {
        base.Update();

        if (player == null) player = npc.ec.Players[0];

        if (cooldown <= 0f)
        {
            currentNavigationState.UpdatePosition(player.transform.position);
        }
        else if (onCooldown & cooldown <= 1f)
        {
            onCooldown = false;
            ChangeNavigationState(new NavigationState_TargetPlayer(npc, 0, player.transform.position));
        }

        cooldown -= Time.deltaTime * npc.TimeScale;
    }

    public override void PlayerInSight(PlayerManager player)
    {
        base.PlayerInSight(player);

        if ((player.transform.position - npc.transform.position).magnitude <= 45f & cooldown <= 0f)
        {
            cooldown = 15f;
            npc.Spit();
            ChangeNavigationState(new NavigationState_WanderRandom(npc, 0));
            onCooldown = true;
        }
    }
}
