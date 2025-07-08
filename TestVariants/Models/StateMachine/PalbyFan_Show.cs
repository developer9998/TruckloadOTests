using TestVariants.Behaviours.Characters;
using UnityEngine;

namespace TestVariants.Models.StateMachine;

public class PalbyFan_Show(PalbyFan palbyfan) : PalbyFan_StateBase(palbyfan)
{
    private bool didthing = false;

    // private bool tog = false;

    private float timr = 0f;

    public override void Enter()
    {
        base.Enter();
        npc.Navigator.SetSpeed(25f);
        npc.Navigator.maxSpeed = 25f;
        ChangeNavigationState(new NavigationState_DoNothing(npc, 74));
        npc.walkSpeed = 2f;
    }

    public override void Update()
    {
        base.Update();

        if (didthing) return;

        PlayerManager playerManager = npc.ec.Players[0];

        if ((playerManager.transform.position - npc.transform.position).magnitude <= 12f)
        {
            // tog == true: 
            // ChangeNavigationState(new NavigationState_DoNothing(npc, 74));

            if (npc.looker.IsVisible & npc.looker.PlayerInSight())
            {
                timr += Time.deltaTime;
                if (timr > 10f)
                {
                    didthing = true;
                    npc.Please();
                }
            }
            return;
        }

        if ((playerManager.transform.position - npc.transform.position).magnitude >= 35f)
        {
            didthing = true;
            ChangeNavigationState(new NavigationState_DoNothing(npc, 74));
            npc.Anger();
        }

        // tog == false:
        ChangeNavigationState(new NavigationState_TargetPlayer(npc, 74, playerManager.transform.position));
    }
}
