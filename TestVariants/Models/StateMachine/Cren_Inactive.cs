using TestVariants.Behaviours.Characters;
using UnityEngine;

namespace TestVariants.Models.StateMachine;

public class Cren_Inactive(Cren cren) : Cren_StateBase(cren)
{
    private float timer = 30f;

    public override void Enter()
    {
        base.Enter();

        npc.Navigator.SetSpeed(0f);
        npc.Navigator.maxSpeed = 0f;

        ChangeNavigationState(new NavigationState_DoNothing(npc, 0));

        timer = 30f;
        npc.gameObject.GetComponent<Entity>().SetFrozen(value: true);
        npc.transform.position = Vector3.zero;
    }

    public override void Update()
    {
        base.Update();

        timer += (0f - Time.deltaTime) * npc.TimeScale;
        npc.spriteRenderer[0].enabled = false;

        if (timer <= 0f)
        {
            timer = 99f;
            npc.gameObject.GetComponent<Entity>().SetFrozen(value: false);
            npc.transform.position = npc.spawnPoint;
            npc.Activate();
        }
    }
}
