using TestVariants.Behaviours.Characters;

namespace TestVariants.Models.StateMachine;

public class Bluxam_Chase(Bluxam bluxam) : Bluxam_StateBase(bluxam)
{
    private bool didThing = false;

    public override void Enter()
    {
        base.Enter();

        npc.Navigator.SetSpeed(26f);
        npc.Navigator.maxSpeed = 26f;

        ChangeNavigationState(new NavigationState_TargetPlayer(npc, 74, npc.ec.Players[0].transform.position));
    }

    public override void DestinationEmpty()
    {
        base.DestinationEmpty();

        npc.behaviorStateMachine.ChangeState(new Bluxam_Wander(npc));
    }

    public override void PlayerInSight(PlayerManager player)
    {
        base.PlayerInSight(player);

        currentNavigationState.UpdatePosition(player.transform.position);

        if (npc.ec.CellFromPosition(player.transform.position).room.category == RoomCategory.Class)
        {
            npc.behaviorStateMachine.ChangeState(new Bluxam_Wander(npc));
            npc.SayTheLine(2);
        }

        if ((npc.transform.position - player.transform.position).magnitude <= 5f & !didThing & !npc.Navigator.Entity.Squished)
        {
            didThing = true;
            npc.behaviorStateMachine.ChangeState(new Bluxam_Cooldown(npc));
            npc.HelpPlayer(player);
            npc.SayTheLine(3);
        }
    }

    public override void PlayerSighted(PlayerManager player)
    {
        base.PlayerSighted(player);

        ChangeNavigationState(new NavigationState_TargetPlayer(npc, 74, npc.ec.Players[0].transform.position));
    }
}
