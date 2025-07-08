using TestVariants.Behaviours.Characters;

namespace TestVariants.Models.StateMachine;

public class Cren_Runback(Cren cren) : Cren_StateBase(cren)
{
    public override void Enter()
    {
        base.Enter();

        npc.Navigator.SetSpeed(75f);
        npc.Navigator.maxSpeed = 75f;

        ChangeNavigationState(new NavigationState_TargetPosition(npc, 0, npc.spawnPoint));
    }

    public override void Update()
    {
        base.Update();
    }

    public override void DestinationEmpty()
    {
        base.DestinationEmpty();

        npc.behaviorStateMachine.ChangeState(new Cren_Inactive(npc));
    }
}
