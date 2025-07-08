using TestVariants.Behaviours.Characters;

namespace TestVariants.Models.StateMachine;

public class Shelfman_Chase(Shelfman shelfman) : Shelfman_StateBase(shelfman)
{
    private bool didThing = false;

    public override void Enter()
    {
        base.Enter();

        npc.Navigator.SetSpeed(75f);
        npc.Navigator.maxSpeed = 75f;

        ChangeNavigationState(new NavigationState_TargetPlayer(npc, 0, npc.asked.transform.position));
    }

    public override void DestinationEmpty()
    {
        base.DestinationEmpty();

        npc.behaviorStateMachine.ChangeState(new Shelfman_Wander(npc));
        npc.SayTheLine(true);
    }

    public override void PlayerInSight(PlayerManager player)
    {
        base.PlayerInSight(player);

        currentNavigationState.UpdatePosition(player.transform.position);

        if ((npc.transform.position - player.transform.position).magnitude <= 10f & !didThing & !npc.Navigator.Entity.Squished)
        {
            didThing = true;
            npc.Navigator.SetSpeed(0f);
            npc.Navigator.maxSpeed = 0f;
            npc.AskQuestion(player);
        }
    }

    public override void PlayerSighted(PlayerManager player)
    {
        base.PlayerSighted(player);

        ChangeNavigationState(new NavigationState_TargetPlayer(npc, 0, player.transform.position));
    }
}
