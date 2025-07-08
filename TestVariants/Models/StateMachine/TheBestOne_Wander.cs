using TestVariants.Behaviours.Characters;

namespace TestVariants.Models.StateMachine;

public class TheBestOne_Wander(TheBestOne thebestone) : TheBestOne_StateBase(thebestone)
{
    private bool didThing = false;

    private bool chasing = false;

    public override void Enter()
    {
        base.Enter();

        npc.Navigator.SetSpeed(20f);
        npc.Navigator.maxSpeed = 20f;

        ChangeNavigationState(new NavigationState_WanderRandom(npc, 0));
    }

    public override void DestinationEmpty()
    {
        base.DestinationEmpty();

        npc.Navigator.SetSpeed(20f);
        npc.Navigator.maxSpeed = 20f;
        chasing = false;

        ChangeNavigationState(new NavigationState_WanderRandom(npc, 0));
    }

    public override void PlayerInSight(PlayerManager player)
    {
        base.PlayerInSight(player);

        if (!npc.cooldown)
        {
            if (!chasing)
            {
                chasing = true;
                ChangeNavigationState(new NavigationState_TargetPlayer(npc, 0, npc.ec.Players[0].transform.position));
                npc.SayTheLine(false);
                npc.Navigator.SetSpeed(26f);
                npc.Navigator.maxSpeed = 26f;
            }

            currentNavigationState.UpdatePosition(player.transform.position);
            if ((npc.transform.position - player.transform.position).magnitude <= 10f & !didThing & !npc.Navigator.Entity.Squished)
            {
                didThing = true;
                npc.Yellow(player);
            }
        }
    }

    public override void PlayerSighted(PlayerManager player)
    {
        if (!npc.cooldown)
        {
            chasing = true;
            base.PlayerSighted(player);
            ChangeNavigationState(new NavigationState_TargetPlayer(npc, 0, npc.ec.Players[0].transform.position));
            npc.SayTheLine(false);
            npc.Navigator.SetSpeed(26f);
            npc.Navigator.maxSpeed = 26f;
        }
    }
}
