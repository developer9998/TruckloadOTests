using TestVariants.Behaviours.Characters;

namespace TestVariants.Models.StateMachine;

public class AJOPI_Wander(AJOPI ajopi) : AJOPI_StateBase(ajopi)
{
    public override void Enter()
    {
        base.Enter();

        npc.Navigator.SetSpeed(2f);
        npc.Navigator.maxSpeed = 2f;

        ChangeNavigationState(new NavigationState_WanderRandom(npc, 0));
    }

    public override void PlayerInSight(PlayerManager player)
    {
        base.PlayerInSight(player);

        if (npc.looker.IsVisible)
        {
            npc.behaviorStateMachine.ChangeState(new AJOPI_YouAreDoneFor(npc));
            npc.BeginRev();
        }
    }
}
