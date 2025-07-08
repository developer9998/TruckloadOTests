using TestVariants.Behaviours.Characters;
using UnityEngine;

namespace TestVariants.Models.StateMachine;

public class Orlan_Scurry(Orlan orlan) : Orlan_StateBase(orlan)
{
    private float cooldown = 20f;

    public override void Enter()
    {
        base.Enter();

        npc.Navigator.SetSpeed(60f);
        npc.Navigator.maxSpeed = 60f;

        ChangeNavigationState(new NavigationState_WanderFlee(npc, 0, npc.stealingFrom.DijkstraMap));
    }

    public override void Update()
    {
        base.Update();

        cooldown += 0f - Time.deltaTime;
        if (cooldown <= 0f) npc.behaviorStateMachine.ChangeState(new Orlan_Wander(npc));
    }
}
