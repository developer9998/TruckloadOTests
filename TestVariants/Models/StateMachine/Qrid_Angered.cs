using TestVariants.Behaviours.Characters;
using UnityEngine;

namespace TestVariants.Models.StateMachine;

public class Qrid_Angered(Qrid Qrid) : Qrid_StateBase(Qrid)
{
    private MovementModifier movemod;

    private bool teleported = false;

    private int OldLayer;

    private float timer = 0f;

    private bool dragging = false;

    public override void Enter()
    {
        base.Enter();

        npc.Navigator.SetSpeed(0f);
        npc.Navigator.maxSpeed = 0f;

        ChangeNavigationState(new NavigationState_TargetPlayer(npc, 0, npc.transform.position));
    }

    public override void Update()
    {
        base.Update();

        if (!dragging)
        {
            if (npc.Navigator.maxSpeed != 0f)
            {
                npc.audMan.pitchModifier += Time.deltaTime / 18f;
                currentNavigationState.UpdatePosition(npc.ec.Players[0].transform.position);
                Vector3 relativePosition = npc.transform.position - npc.ec.Players[0].transform.position;
                if (relativePosition.magnitude <= 5f)
                {
                    ChangeNavigationState(new NavigationState_WanderRandom(npc, 0));
                    movemod = new MovementModifier(Vector3.zero, 0f);
                    dragging = true;
                    npc.audMan.pitchModifier = 1f;
                    OldLayer = npc.spriteRenderer[0].gameObject.layer;
                    npc.spriteRenderer[0].gameObject.layer = 29;
                    npc.ec.Players[0].plm.am.moveMods.Clear();
                    npc.ec.Players[0].plm.am.moveMods.Add(movemod);
                }
            }
            return;
        }

        if (npc.ec.Players[0].plm.am.moveMods.Count > 1)
        {
            npc.ec.Players[0].plm.am.moveMods.Clear();
            npc.ec.Players[0].plm.am.moveMods.Add(movemod);
        }

        if (timer > -50f)
        {
            npc.Navigator.SetSpeed(30f);
            npc.Navigator.maxSpeed = 30f;
            npc.spriteBase.transform.Rotate(0f, Time.deltaTime * npc.TimeScale * 180f, 0f);
            npc.spriteBase.transform.position = npc.transform.position + npc.spriteoffset + npc.spriteBase.transform.forward * 8f;
            timer += Time.deltaTime * npc.TimeScale;
            npc.ec.Players[0].transform.position = npc.transform.position;
        }

        if (timer >= 5f)
        {
            timer = 0f;

            if (teleported)
            {
                npc.spriteBase.transform.position = npc.transform.position + npc.spriteoffset;
                npc.spriteRenderer[0].gameObject.layer = OldLayer;
                timer = -99f;
                npc.ec.Players[0].plm.Entity.AddForce(new Force(npc.transform.forward, 80f, -60f));
                npc.ec.Players[0].plm.am.moveMods.Remove(movemod);
                teleported = false;
                npc.StopBeingMad();
                return;
            }

            npc.transform.position = npc.TeleportPosition();
            teleported = true;
        }
    }
}
