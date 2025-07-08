using TestVariants.Models.StateMachine;
using UnityEngine;

namespace TestVariants.Behaviours.Characters;
public class DaSketch : NPC, IEntityTrigger
{
    private bool animating = false;

    private AudioManager audMan;

    public override void Initialize()
    {
        base.Initialize();

        spriteRenderer[0].sprite = TestPlugin.Instance.assetMan.Get<Sprite>("DaSketch_Idle");
        spriteRenderer[0].transform.position += Vector3.down * spriteRenderer[0].transform.position.y + Vector3.up * 5f;
        spriteRenderer[0].transform.localScale *= 10f;

        audMan = GetComponent<PropagatedAudioManager>();
        audMan.overrideSubtitleColor = false;

        navigator.SetRoomAvoidance(val: false);
        navigator.SetSpeed(32f);
        navigator.maxSpeed = 32f;

        behaviorStateMachine.ChangeState(new DaSketch_Hide(this));
    }

    public void PopOut()
    {
        TestPlugin.Instance.Logger.LogInfo("Da Sketch has emerged from a pile of papers");

        audMan.PlaySingle(TestPlugin.Instance.assetMan.Get<SoundObject>("DaSketch_Spawn"));
        gameObject.GetComponent<Entity>().SetFrozen(value: false);
        animating = true;
        spriteRenderer[0].sprite = TestPlugin.Instance.assetMan.Get<Sprite>("DaSketch_Idle");
        Invoke(nameof(Run), 3f);
        Invoke(nameof(QuitPanicking), 18f);
    }

    private void Run()
    {
        TestPlugin.Instance.Logger.LogInfo("Da Sketch has began to run with alert");

        ec.MakeNoise(transform.position, 100);
        audMan.QueueAudio(TestPlugin.Instance.assetMan.Get<SoundObject>("DaSketch_Alert"));
        audMan.SetLoop(true);
        behaviorStateMachine.ChangeNavigationState(new NavigationState_WanderFlee(this, 0, ec.Players[0].DijkstraMap));
        PanicA();
    }

    private void QuitPanicking()
    {
        TestPlugin.Instance.Logger.LogInfo("Da Sketch is now back to hiding");

        animating = false;
        audMan.audioDevice.Stop();
        audMan.FlushQueue(true);
        behaviorStateMachine.ChangeState(new DaSketch_Hide(this));
    }

    private void PanicA()
    {
        if (animating)
        {
            spriteRenderer[0].sprite = TestPlugin.Instance.assetMan.Get<Sprite>("DaSketch_Alert0");
            Invoke(nameof(PanicB), 0.1f);
            Invoke(nameof(PanicC), 0.2f);
            Invoke(nameof(PanicB), 0.3f);
            Invoke(nameof(PanicA), 0.4f);
        }
    }

    private void PanicB()
    {
        if (animating)
        {
            spriteRenderer[0].sprite = TestPlugin.Instance.assetMan.Get<Sprite>("DaSketch_Alert1");
        }
    }

    private void PanicC()
    {
        if (animating)
        {
            spriteRenderer[0].sprite = TestPlugin.Instance.assetMan.Get<Sprite>("DaSketch_Alert2");
        }
    }
}
