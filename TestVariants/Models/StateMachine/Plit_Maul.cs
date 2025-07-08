using TestVariants.Behaviours.Characters;
using UnityEngine;

namespace TestVariants.Models.StateMachine;

public class Plit_Maul(Plit plit) : Plit_StateBase(plit)
{
    private MovementModifier moveModSelf, moveModOther;

    private float maultime = 0f;

    public override void Enter()
    {
        base.Enter();

        npc.Navigator.SetSpeed(0f);
        npc.Navigator.maxSpeed = 0f;

        ChangeNavigationState(new NavigationState_DoNothing(npc, 0));

        moveModSelf = new MovementModifier(npc.transform.forward * 28f, 0f);
        moveModOther = new MovementModifier(npc.transform.forward * 28f, 0f);
        npc.Navigator.Entity.ExternalActivity.moveMods.Add(moveModSelf);
        npc.mauling.moveMods.Add(moveModOther);
        maultime = 0f;
    }

    public override void Update()
    {
        base.Update();

        maultime += Time.deltaTime * npc.TimeScale;
        npc.spriteBase.transform.position = npc.transform.position + npc.spriteOffset + new Vector3(Random.Range(-3f, 3f), Random.Range(0f, 6f), Random.Range(-3f, 3f));

        if (npc.Navigator.Entity.Velocity.magnitude * 200f <= 22f || (npc.transform.position - npc.mauling.transform.position).magnitude >= 30f)
        {
            if (((npc.transform.position - npc.mauling.transform.position).magnitude >= 30f || maultime >= 3f) & npc.Navigator.Entity.Velocity.magnitude * 200f <= 15f)
            {
                npc.Navigator.Entity.ExternalActivity.moveMods.Remove(moveModSelf);
                npc.mauling.moveMods.Remove(moveModOther);
                npc.StopMaul(false);
                return;
            }

            Physics.Raycast(npc.transform.position + Vector3.up, npc.Navigator.Entity.Velocity.normalized, out RaycastHit raycastHit, 25f, npc.ec.Players[0].pc.ClickLayers, QueryTriggerInteraction.Ignore);
            npc.transform.LookAt(npc.transform.position + Vector3.Reflect(npc.Navigator.Entity.Velocity.normalized, raycastHit.normal));
            moveModSelf.movementAddend = npc.transform.forward * 28f;
            moveModOther.movementAddend = npc.transform.forward * 28f;
        }

        npc.transform.position = npc.mauling.transform.position;
    }
}
