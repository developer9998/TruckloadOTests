using TestVariants.Behaviours.Characters;
using UnityEngine;

namespace TestVariants.Models.StateMachine;

public class Bluxam_Cooldown(Bluxam bluxam) : Bluxam_StateBase(bluxam)
{
    private bool said = false;

    private float timer = 0f;

    public override void Enter()
    {
        base.Enter();
        npc.Navigator.SetSpeed(18f);
        npc.Navigator.maxSpeed = 18f;

        ChangeNavigationState(new NavigationState_WanderRandom(npc, 0));

        timer = 0f;
    }

    public override void Update()
    {
        base.Update();

        timer += Time.deltaTime;

        if (timer >= 30f) npc.behaviorStateMachine.ChangeState(new Bluxam_Wander(npc));

        if (timer >= 3.5f & !said)
        {
            said = true;
            npc.SayTheLine(4);
        }
    }
}
