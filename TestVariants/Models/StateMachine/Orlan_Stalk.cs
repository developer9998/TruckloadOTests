using TestVariants.Behaviours.Characters;

namespace TestVariants.Models.StateMachine;

public class Orlan_Stalk(Orlan orlan) : Orlan_StateBase(orlan)
{
    public override void Enter()
    {
        base.Enter();

        npc.Navigator.SetSpeed(80f);
        npc.Navigator.maxSpeed = 80f;

        ChangeNavigationState(new NavigationState_TargetPlayer(npc, 0, npc.stealingFrom.transform.position));
    }

    public override void Update()
    {
        base.Update();

        currentNavigationState.UpdatePosition(npc.stealingFrom.transform.position - npc.stealingFrom.transform.forward * 5f);
        if (npc.looker.IsVisible & npc.looker.PlayerInSight())
        {
            if (!npc.stealing)
            {
                npc.Navigator.SetSpeed(0f);
                npc.Navigator.maxSpeed = 0f;
            }

            if ((npc.transform.position - npc.stealingFrom.transform.position).magnitude <= 10f) npc.Scurry();
            return;
        }

        npc.Navigator.SetSpeed(40f);
        npc.Navigator.maxSpeed = 40f;

        if ((npc.transform.position - (npc.stealingFrom.transform.position - npc.stealingFrom.transform.forward * 5f)).magnitude <= 5f)
        {
            npc.BeginStealing();
        }
    }
}
