using TestVariants.Behaviours.Characters;

namespace TestVariants.Models.StateMachine;

public class Qrid_Wander(Qrid Qrid) : Qrid_StateBase(Qrid)
{
    public override void Enter()
    {
        base.Enter();

        npc.Navigator.SetSpeed(15f);
        npc.Navigator.maxSpeed = 15f;

        ChangeNavigationState(new NavigationState_WanderRandom(npc, 0));
    }

    public override void PlayerInSight(PlayerManager player)
    {
        base.PlayerInSight(player);

        if (!npc.onCooldown && !player.Tagged && (player.transform.position - npc.transform.position).sqrMagnitude < (20f * 20f)) npc.Befriend();
    }
}
