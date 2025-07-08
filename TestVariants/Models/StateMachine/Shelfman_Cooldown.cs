using TestVariants.Behaviours.Characters;
using UnityEngine;

namespace TestVariants.Models.StateMachine;

public class Shelfman_Cooldown(Shelfman shelfman) : Shelfman_StateBase(shelfman)
{
    private float timer = 15f;

    public override void Enter()
    {
        base.Enter();

        npc.Navigator.SetSpeed(30f);
        npc.Navigator.maxSpeed = 30f;

        ChangeNavigationState(new NavigationState_WanderRandom(npc, 0));
    }

    public override void Update()
    {
        base.Update();

        timer -= Time.deltaTime * npc.TimeScale;

        if (timer <= 0f) npc.behaviorStateMachine.ChangeState(new Shelfman_Wander(npc));
    }
}
