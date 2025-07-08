using TestVariants.Behaviours.Characters;
using UnityEngine;

namespace TestVariants.Models.StateMachine;

public class Testerly_Idle(Testerly Testerly) : Testerly_StateBase(Testerly)
{
    private int newframe = 0;

    private float newtimer = 0f;

    private Vector3 typingPos;

    private bool isTyping = false;

    private float frametimer = 0f;

    private float checktimer = 0f;

    private float timer = 0f;

    public override void Enter()
    {
        base.Enter();

        npc.Navigator.SetSpeed(0f);
        npc.Navigator.maxSpeed = 0f;

        ChangeNavigationState(new NavigationState_DoNothing(npc, 74));

        timer = 5f;
        frametimer = 2f;
        checktimer = -1f;
        newtimer = 2f;
        npc.spriteRotation.enabled = false;
        npc.spriteRenderer[0].sprite = TestPlugin.Instance.assetMan.Get<Sprite>("Testerly_NewLaptop_0");
    }

    private void NextFrame()
    {
        newframe++;
        npc.spriteRenderer[0].sprite = TestPlugin.Instance.assetMan.Get<Sprite>("Testerly_NewLaptop_" + newframe);
    }

    public override void Update()
    {
        if (newframe < 5)
        {
            newtimer -= Time.deltaTime * 10f;

            if (newtimer <= 0f)
            {
                if (newframe == 4)
                {
                    npc.Shutup();
                    npc.TypeLoop();
                    typingPos = npc.transform.position;
                    isTyping = true;
                }

                newtimer = 2f;
                NextFrame();
            }
        }

        base.Update();

        timer += -Time.deltaTime;
        frametimer += -Time.deltaTime * 10f;

        if (frametimer <= 0f & isTyping)
        {
            frametimer = 1f;
            npc.spriteRenderer[0].sprite = TestPlugin.Instance.assetMan.Get<Sprite>("Testerly_Idle_" + Random.Range(0, 5).ToString());
        }

        if (timer <= 0f & isTyping)
        {
            timer = Random.Range(0f, 15f);
            frametimer = 5f;
            npc.spriteRenderer[0].sprite = TestPlugin.Instance.assetMan.Get<Sprite>("Testerly_Idle_Peak");
        }

        if ((npc.transform.position - typingPos).magnitude > 5f & isTyping)
        {
            Drop(npc.gameObject);
        }

        if (isTyping)
        {
            foreach (NPC npc in npc.ec.Npcs)
            {
                if (npc?.Navigator?.Entity is Entity entity && (npc.transform.position - this.npc.transform.position).magnitude <= 5f & entity.Velocity.magnitude / Time.deltaTime > 1f)
                {
                    Drop(npc.gameObject);
                    break;
                }
            }

            if ((npc.ec.Players[0].transform.position - npc.transform.position).magnitude <= 5f & npc.ec.Players[0].plm.Entity.Velocity.magnitude / Time.deltaTime > 1f)
                Drop(npc.ec.Players[0].gameObject);

            return;
        }

        if (checktimer > 0f)
        {
            checktimer += -Time.deltaTime;
            if (checktimer <= 0f)
            {
                checktimer = -1f;
                npc.SayLine(1);
                npc.transform.LookAt(npc.laptopEnt.transform.position);
                npc.Navigator.Entity.AddForce(new Force(npc.transform.forward, 42f, -42f));
                npc.spriteRotation.sprites = npc.distraughtSprites;
                npc.spriteRotation.enabled = true;
                npc.GetReady();
            }
        }
    }

    private void Drop(GameObject toBlame)
    {
        npc.Navigator.Entity.AddForce(new Force(-npc.transform.forward, 10f, -10f));
        checktimer = 2f;
        npc.Shutup();
        isTyping = false;
        npc.spriteRenderer[0].sprite = TestPlugin.Instance.assetMan.Get<Sprite>("Testerly_Drop");
        npc.SayLine(6);
        npc.SayLine(0);
        npc.Drop();
        npc.toBlame = toBlame.GetComponent<Principal>() || toBlame.GetComponent<Canny>() ? npc.gameObject : toBlame;
    }
}
