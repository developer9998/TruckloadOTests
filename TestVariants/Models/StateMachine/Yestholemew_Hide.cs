using TestVariants.Behaviours.Characters;

namespace TestVariants.Models.StateMachine;

public class Yestholemew_Hide(Yestholemew yestholemew) : Yestholemew_StateBase(yestholemew)
{
    private bool scared = false;

    public override void Enter()
    {
        base.Enter();

        npc.ScreamingAnim = false;
        npc.Navigator.SetSpeed(48f);
        npc.Navigator.maxSpeed = 48f;

        ChangeNavigationState(new NavigationState_DoNothing(npc, 74));

        npc.myHide = npc.ec.CellFromPosition(npc.transform.position).room;

        if (npc.First) return;
        foreach (Door door in npc.myHide.doors)
        {
            door.LockTimed(30f);
        }
    }

    public override void Update()
    {
        base.Update();

        if (npc.ec.CellFromPosition(npc.ec.Players[0].transform.position).room == npc.myHide & !scared)
        {
            scared = true;
            npc.Scare();
            npc.First = false;
        }
    }
}
