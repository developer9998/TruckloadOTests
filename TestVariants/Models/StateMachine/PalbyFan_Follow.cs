using TestVariants.Behaviours.Characters;

namespace TestVariants.Models.StateMachine;

public class PalbyFan_Follow(PalbyFan palbyfan) : PalbyFan_StateBase(palbyfan)
{
    private bool didThing = false;

    public override void Enter()
    {
        base.Enter();

        npc.Navigator.SetSpeed(21f);
        npc.Navigator.maxSpeed = 21f;

        ChangeNavigationState(new NavigationState_TargetPlayer(npc, 0, npc.ec.Players[0].transform.position));
        npc.walkSpeed = 1.5f;

    }

    public override void Update()
    {
        base.Update();
    }

    public override void DestinationEmpty()
    {
        base.DestinationEmpty();

        npc.behaviorStateMachine.ChangeState(new PalbyFan_Wander(npc));
    }

    public override void PlayerInSight(PlayerManager player)
    {
        base.PlayerInSight(player);

        currentNavigationState.UpdatePosition(player.transform.position);

        if ((npc.transform.position - player.transform.position).magnitude <= 10f & !didThing & !npc.gameObject.GetComponent<Entity>().Squished)
        {
            didThing = true;
            npc.ShowPalby();
        }
    }

    public override void PlayerSighted(PlayerManager player)
    {
        base.PlayerSighted(player);

        ChangeNavigationState(new NavigationState_TargetPlayer(npc, 0, npc.ec.Players[0].transform.position));
    }
}
