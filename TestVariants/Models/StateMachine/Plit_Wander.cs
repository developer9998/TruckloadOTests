using TestVariants.Behaviours.Characters;
using UnityEngine;

namespace TestVariants.Models.StateMachine;

public class Plit_Wander(Plit plit) : Plit_StateBase(plit)
{
    private float timer = 0f;

    public override void Enter()
    {
        base.Enter();

        npc.Navigator.SetSpeed(12f);
        npc.Navigator.maxSpeed = 12f;

        ChangeNavigationState(new NavigationState_WanderRandom(npc, 0));
    }

    public override void Update()
    {
        base.Update();

        timer -= Time.deltaTime;

        if (timer <= 0f)
        {
            timer = Random.Range(10f, 20f);
            npc.SayTheLine(0);
        }

        foreach (NPC npc in npc.ec.Npcs)
        {
            if (npc?.Navigator?.Entity?.ExternalActivity is not ActivityModifier activityMod) continue;

            Vector3 relativePosition = npc.transform.position - this.npc.transform.position;
            Vector3 normalized = relativePosition.normalized;
            float magnitude = relativePosition.magnitude;

            if (!Physics.Raycast(this.npc.transform.position + Vector3.up, normalized, out RaycastHit raycastHit, magnitude, this.npc.ec.Players[0].pc.ClickLayers, QueryTriggerInteraction.Ignore) && magnitude <= 45f & npc.gameObject != this.npc.gameObject)
            {
                this.npc.LungeAt(activityMod);
                break;
            }
        }
    }

    public override void PlayerInSight(PlayerManager player)
    {
        base.PlayerInSight(player);

        if ((npc.transform.position - player.transform.position).magnitude <= 45f) npc.LungeAt(player.gameObject.GetComponent<ActivityModifier>());
    }
}
