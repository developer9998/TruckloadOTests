using TestVariants.Behaviours;
using TestVariants.Behaviours.Characters;
using UnityEngine;

namespace TestVariants.Models.StateMachine;

public class PalbyFan_Angered(PalbyFan palbyfan) : PalbyFan_StateBase(palbyfan)
{
    private bool chase = false;

    public override void Enter()
    {
        base.Enter();

        npc.Navigator.maxSpeed = 30f;
        npc.Navigator.SetSpeed(12f);

        ChangeNavigationState(new NavigationState_WanderRandom(npc, 74));
    }

    public override void Hear(GameObject source, Vector3 position, int value)
    {
        base.Hear(source, position, value);

        if (!npc.looker.PlayerInSight()) ChangeNavigationState(new NavigationState_TargetPosition(npc, 74, position));
    }

    public override void Update()
    {
        base.Update();

        float angle = npc.Navigator.NextPoint != npc.transform.position
            ? Mathf.DeltaAngle(npc.transform.eulerAngles.y, Mathf.Atan2(npc.Navigator.NextPoint.x - npc.transform.position.x, npc.Navigator.NextPoint.z - npc.transform.position.z) * 57.29578f)
            : 0f;

        if (Mathf.Abs(angle) >= 45f & npc.Navigator.speed >= 20f)
        {
            npc.Navigator.maxSpeed = 30f;
            npc.Navigator.SetSpeed(12f);
            npc.audMan.PlaySingle(TestPlugin.Instance.assetMan.Get<SoundObject>("Bang"));
        }

        foreach (NPC npc in npc.ec.Npcs)
        {
            if ((npc.transform.position - this.npc.transform.position).magnitude <= 5f & npc.gameObject != this.npc.gameObject & !this.npc.punching & !npc.gameObject.GetComponent<Dizziness>())
            {
                this.npc.Punch();
            }
        }
    }

    public override void DestinationEmpty()
    {
        base.DestinationEmpty();

        ChangeNavigationState(new NavigationState_WanderRandom(npc, 74));
        chase = true;
    }

    public override void PlayerInSight(PlayerManager player)
    {
        base.PlayerInSight(player);

        if (!chase)
        {
            chase = true;
            ChangeNavigationState(new NavigationState_TargetPlayer(npc, 74, player.transform.position));
        }

        currentNavigationState.UpdatePosition(player.transform.position);

        if ((npc.transform.position - player.transform.position).magnitude <= 5f & !npc.gameObject.GetComponent<Entity>().Squished & !npc.punching)
        {
            npc.Punch();
        }
    }

    public override void PlayerSighted(PlayerManager player)
    {
        base.PlayerSighted(player);

        chase = true;
        ChangeNavigationState(new NavigationState_TargetPlayer(npc, 74, player.transform.position));
    }
}
