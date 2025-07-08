using TestVariants.Behaviours.Characters;
using UnityEngine;

namespace TestVariants.Models.StateMachine;

public class Testholder_Wander(Testholder testholder) : Testholder_StateBase(testholder)
{
    private float timer = 0f;

    public override void Enter()
    {
        base.Enter();

        npc.Navigator.SetSpeed(17f);
        npc.Navigator.maxSpeed = 17f;

        ChangeNavigationState(new NavigationState_WanderRandom(npc, 0));
    }

    public override void Update()
    {
        base.Update();

        timer += 0f - Time.deltaTime;
        if (timer <= 0f)
        {
            timer = Random.Range(5f, 30f);
            npc.SayTheLine(huh: true);
        }
    }
}
