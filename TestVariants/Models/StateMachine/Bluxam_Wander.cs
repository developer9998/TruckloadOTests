using TestVariants.Behaviours.Characters;

namespace TestVariants.Models.StateMachine;

public class Bluxam_Wander(Bluxam bluxam) : Bluxam_StateBase(bluxam)
{
    public override void Enter()
    {
        base.Enter();

        npc.Navigator.SetSpeed(18f);
        npc.Navigator.maxSpeed = 18f;

        ChangeNavigationState(new NavigationState_WanderRandom(npc, 0));
    }

    public override void Update()
    {
        base.Update();
    }

    public override void PlayerInSight(PlayerManager player)
    {
        base.PlayerInSight(player);

        if (npc.ec.CellFromPosition(player.transform.position).room.category != RoomCategory.Class && player.plm.running && !player.Tagged)
        {
            npc.behaviorStateMachine.ChangeState(new Bluxam_Chase(npc));
            npc.SayTheLine(1);
        }
    }
}
