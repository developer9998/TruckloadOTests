using TestVariants.Behaviours.Characters;
using UnityEngine;

namespace TestVariants.Models.StateMachine;

public class MeMest_Wander(MeMest memest) : MeMest_StateBase(memest)
{
    private float timer = 0f;

    private bool tog = false;

    public override void Enter()
    {
        base.Enter();
        npc.Navigator.SetSpeed(20f);
        npc.Navigator.maxSpeed = 20f;
        ChangeNavigationState(new NavigationState_WanderRandom(npc, 0));
        timer = 30f;
    }

    public override void Update()
    {
        base.Update();

        timer -= Time.deltaTime;
        if (timer > 0) return;

        tog = !tog;
        if (tog)
        {
            timer = 10f;
            npc.Block();
            ChangeNavigationState(new NavigationState_DoNothing(npc, 0));
            return;
        }

        Sprite sprite = TestPlugin.Instance.assetMan.Get<Sprite>("MeMestStop");
        if (npc.spriteRenderer[0].sprite == sprite) npc.spriteRenderer[0].sprite = sprite;

        timer = 30f;
        npc.StopBlock();
        ChangeNavigationState(new NavigationState_WanderRandom(npc, 0));
    }
}
