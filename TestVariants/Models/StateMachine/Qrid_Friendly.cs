using TestVariants.Behaviours.Characters;

namespace TestVariants.Models.StateMachine;

public class Qrid_Friendly(Qrid Qrid) : Qrid_StateBase(Qrid)
{
    public override void Enter()
    {
        base.Enter();

        npc.Navigator.SetSpeed(19f);
        npc.Navigator.maxSpeed = 22f;

        ChangeNavigationState(new NavigationState_TargetPlayer(npc, 0, npc.transform.position));
    }

    public override void Update()
    {
        base.Update();

        currentNavigationState.UpdatePosition(npc.ec.Players[0].transform.position);

        if ((npc.ec.Players[0].transform.position - npc.transform.position).magnitude <= 20f)
        {
            npc.Navigator.SetSpeed(0f);
            npc.Navigator.maxSpeed = 0f;
            return;
        }

        npc.Navigator.SetSpeed(19f);
        npc.Navigator.maxSpeed = 22f;

        if ((npc.ec.Players[0].transform.position - npc.transform.position).magnitude >= 32f)
        {
            bool isNotFriends = false;

            foreach (NPC npc in npc.ec.Npcs)
            {
                if (npc.gameObject != this.npc.gameObject & npc.gameObject.GetComponent<ActivityModifier>() && (npc.transform.position - this.npc.transform.position).magnitude <= 5f)
                {
                    isNotFriends = true;
                    this.npc.ImMadAtNPCNow(npc);
                }
            }

            if (!isNotFriends) npc.ImMadNow();
        }
    }
}
