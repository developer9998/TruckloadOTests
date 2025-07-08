using TestVariants.Behaviours.Characters;

namespace TestVariants.Models.StateMachine;

public class PalbyFan_Wander(PalbyFan palbyfan) : PalbyFan_StateBase(palbyfan)
{
    public override void Enter()
    {
        base.Enter();

        npc.Navigator.SetSpeed(18f);
        npc.Navigator.maxSpeed = 18f;

        ChangeNavigationState(new NavigationState_WanderRandom(npc, 0));

        npc.walkSpeed = 1.25f;
    }

    public override void PlayerInSight(PlayerManager player)
    {
        base.PlayerSighted(player);

        if (!npc.cooldown) npc.behaviorStateMachine.ChangeState(new PalbyFan_Follow(npc));
    }
}
