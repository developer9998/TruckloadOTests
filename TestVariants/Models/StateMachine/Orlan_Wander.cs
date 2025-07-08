using TestVariants.Behaviours.Characters;

namespace TestVariants.Models.StateMachine;

public class Orlan_Wander(Orlan orlan) : Orlan_StateBase(orlan)
{
    public override void Enter()
    {
        base.Enter();

        npc.Navigator.SetSpeed(10f);
        npc.Navigator.maxSpeed = 10f;

        ChangeNavigationState(new NavigationState_WanderRandom(npc, 0));
    }

    public override void Update()
    {
        base.Update();
    }

    public override void PlayerInSight(PlayerManager player)
    {
        base.PlayerInSight(player);

        if (!player.Tagged)
        {
            npc.stealingFrom = player;
            npc.behaviorStateMachine.ChangeState(new Orlan_Stalk(npc));
        }
    }
}
