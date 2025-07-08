using System.Collections.Generic;
using TestVariants.Models.StateMachine;
using UnityEngine;

namespace TestVariants.Behaviours.Characters;

public class Gummin : NPC, IEntityTrigger
{
    public int timr = 0;

    public bool running = false;

    private AudioManager audMan;

    public override void Initialize()
    {
        base.Initialize();

        spriteRenderer[0].sprite = TestPlugin.Instance.assetMan.Get<Sprite>("Gummin_Idle");
        spriteRenderer[0].transform.position += Vector3.down * spriteRenderer[0].transform.position.y + Vector3.up * 5f;
        spriteRenderer[0].transform.localScale *= 10f;

        audMan = GetComponent<PropagatedAudioManager>();
        audMan.overrideSubtitleColor = false;

        navigator.SetRoomAvoidance(val: true);
        navigator.SetSpeed(20f);
        navigator.maxSpeed = 20f;

        behaviorStateMachine.ChangeState(new Gummin_Wander(this));
        Invoke(nameof(RunStepA), 0.12f);
    }

    private void RunStepA()
    {
        if (running)
        {
            spriteRenderer[0].sprite = TestPlugin.Instance.assetMan.Get<Sprite>("Gummin_Run1");
        }
        Invoke(nameof(RunStepB), 0.12f);
    }

    private void RunStepB()
    {
        if (running)
        {
            spriteRenderer[0].sprite = TestPlugin.Instance.assetMan.Get<Sprite>("Gummin_Run2");
        }
        Invoke(nameof(RunStepA), 0.12f);
    }

    public void Give()
    {
        running = false;
        Navigator.Entity.SetFrozen(true);
        behaviorStateMachine.ChangeState(new Gummin_Wander(this));
        SayLine(ec.Players[0].itm.InventoryFull() ? 2 : 1);
        ec.Players[0].itm.AddItem(TestPlugin.Instance.assetMan.Get<ItemObject>("GumminChunk"));
        spriteRenderer[0].sprite = TestPlugin.Instance.assetMan.Get<Sprite>("Gummin_Give1");
        Invoke(nameof(GiveAgain), 0.12f);
        Invoke(nameof(NoGive), 0.24f);
        audMan.PlaySingle(TestPlugin.Instance.assetMan.Get<SoundObject>("GumminChunk_Splat"));

        TestPlugin.Instance.Logger.LogInfo("The Gummin has gave the player a Gummin Chunk");
    }

    private void GiveAgain()
    {
        spriteRenderer[0].sprite = TestPlugin.Instance.assetMan.Get<Sprite>("Gummin_Give2");
    }

    private void NoGive()
    {
        spriteRenderer[0].sprite = TestPlugin.Instance.assetMan.Get<Sprite>("Gummin_Idle");
        Navigator.Entity.SetFrozen(false);
    }

    public void SayLine(int c)
    {
        switch (c)
        {
            case 0:
                audMan.PlaySingle(TestPlugin.Instance.assetMan.Get<SoundObject>("Gummin_Wantsome"));
                break;
            case 1:
                audMan.PlaySingle(TestPlugin.Instance.assetMan.Get<SoundObject>("Gummin_HereGo"));
                break;
            case 2:
                audMan.PlaySingle(TestPlugin.Instance.assetMan.Get<SoundObject>("Gummin_TakingandGiving"));
                break;
        }
    }

    public override void VirtualUpdate()
    {
        base.VirtualUpdate();
    }
}
