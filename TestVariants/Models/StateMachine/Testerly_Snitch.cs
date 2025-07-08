using TestVariants.Behaviours.Characters;
using UnityEngine;

namespace TestVariants.Models.StateMachine;

public class Testerly_Snitch(Testerly Testerly) : Testerly_StateBase(Testerly)
{
    private float runningtime = 0f;

    private bool alertframe = false;

    private bool running = true;

    private bool chasing = false;

    private float frametimer = 0f;

    private float callouttimer = 10f;

    private int frame = 0;

    public override void Enter()
    {
        base.Enter();

        npc.Navigator.SetSpeed(30f);
        npc.Navigator.maxSpeed = 30f;

        ChangeNavigationState(new NavigationState_WanderRandom(npc, 74));

        frametimer = 2f;
        frame = 1;
        npc.SayLine(2);
        npc.spriteRotation.sprites = npc.runCycleASprites;
        npc.spriteRotation.enabled = true;

        Cell cell = npc.ec.CellFromPosition(npc.transform.position);
        npc.transform.position = cell.CenterWorldPosition;
    }

    private void NextFrame()
    {
        frame++;
        if (frame == 5) frame = 1;

        switch (frame)
        {
            case 1:
                npc.spriteRotation.sprites = npc.runCycleASprites;
                break;
            case 2:
            case 4:
                npc.SayLine(5);
                npc.spriteRotation.sprites = npc.runCycleBSprites;
                break;
            default:
                npc.spriteRotation.sprites = npc.runCycleCSprites;
                break;
        }
    }

    public override void DestinationEmpty()
    {
        base.DestinationEmpty();

        chasing = false;
        ChangeNavigationState(new NavigationState_WanderRandom(npc, 74));
    }

    public override void Update()
    {
        if (running)
        {
            if (runningtime >= 20f & !chasing)
            {
                runningtime = 0f;
                ChangeNavigationState(new NavigationState_TargetPosition(npc, 74, Object.FindObjectOfType<Principal>().transform.position));
            }

            runningtime += Time.deltaTime;
            frametimer -= Time.deltaTime * 10f;

            if (frametimer <= 0f)
            {
                frametimer = 1f;
                NextFrame();
            }

            base.Update();

            callouttimer -= Time.deltaTime;
            if (callouttimer <= 0f)
            {
                callouttimer = Random.Range(5f, 15f);
                npc.SayLine(3);
            }

            foreach (NPC npc in npc.ec.Npcs)
            {
                Vector3 relativePosition = npc.transform.position - this.npc.transform.position;
                Vector3 normalized = relativePosition.normalized;
                float magnitude = relativePosition.magnitude;

                if (!Physics.Raycast(this.npc.transform.position + Vector3.up, normalized, out RaycastHit raycastHit, magnitude, this.npc.ec.Players[0].pc.ClickLayers, QueryTriggerInteraction.Ignore) && magnitude <= 9999f & npc.gameObject.TryGetComponent(out Principal principal))
                {
                    if (chasing)
                    {
                        currentNavigationState.UpdatePosition(npc.transform.position);
                        if ((npc.transform.position - this.npc.gameObject.transform.position).magnitude <= 7f)
                        {
                            this.npc.spriteRotation.enabled = false;
                            running = false;
                            this.npc.spriteRenderer[0].sprite = TestPlugin.Instance.assetMan.Get<Sprite>("Testerly_Alert_0");
                            this.npc.Snitcharoo(principal);
                            this.npc.SayLine(4);
                        }
                    }
                    else ChangeNavigationState(new NavigationState_TargetPosition(this.npc, 74, npc.transform.position));

                    chasing = true;
                    break;
                }
            }
            return;
        }

        npc.Navigator.SetSpeed(0f);
        npc.Navigator.maxSpeed = 0f;

        frametimer -= Time.deltaTime * 10f;
        if (frametimer <= 0f)
        {
            frametimer = 1f;
            alertframe = !alertframe;
            npc.spriteRenderer[0].sprite = alertframe ? TestPlugin.Instance.assetMan.Get<Sprite>("Testerly_Alert_1") : TestPlugin.Instance.assetMan.Get<Sprite>("Testerly_Alert_0");
        }
    }

}
