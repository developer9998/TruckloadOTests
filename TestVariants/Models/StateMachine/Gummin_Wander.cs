using TestVariants.Behaviours.Characters;
using UnityEngine;

namespace TestVariants.Models.StateMachine;

public class Gummin_Wander(Gummin Gummin) : Gummin_StateBase(Gummin)
{
    private bool didThing = false;

    private float timer = 0f;

    public override void Enter()
    {
        base.Enter();

        npc.Navigator.SetSpeed(20f);
        npc.Navigator.maxSpeed = 20f;

        ChangeNavigationState(new NavigationState_WanderRandom(npc, 0));

        npc.timr += 30;
        timer = npc.timr;
    }

    public override void Update()
    {
        base.Update();

        timer += -Time.deltaTime;
    }

    public override void DestinationEmpty()
    {
        base.DestinationEmpty();

        npc.running = false;
        npc.Navigator.SetSpeed(20f);
        npc.Navigator.maxSpeed = 20f;

        ChangeNavigationState(new NavigationState_WanderRandom(npc, 0));
    }

    public override void PlayerInSight(PlayerManager player)
    {
        base.PlayerInSight(player);

        if (timer <= 0f)
        {
            currentNavigationState.UpdatePosition(player.transform.position);
            if ((npc.transform.position - player.transform.position).magnitude <= 5f & !didThing & !npc.Navigator.Entity.Squished)
            {
                didThing = true;
                npc.Give();
            }
        }
    }

    public override void PlayerSighted(PlayerManager player)
    {
        base.PlayerSighted(player);

        if (timer <= 0f)
        {
            npc.Navigator.SetSpeed(30f);
            npc.Navigator.maxSpeed = 40f;
            npc.SayLine(0);
            npc.running = true;
            ChangeNavigationState(new NavigationState_TargetPlayer(npc, 74, npc.ec.Players[0].transform.position));
        }
    }
}
