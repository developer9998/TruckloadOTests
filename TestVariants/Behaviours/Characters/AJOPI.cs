using TestVariants.Models.StateMachine;
using UnityEngine;

namespace TestVariants.Behaviours.Characters;

public class AJOPI : NPC, IEntityTrigger
{
    private AudioManager audMan;

    private Fog fog;

    public override void Initialize()
    {
        base.Initialize();

        spriteRenderer[0].sprite = TestPlugin.Instance.assetMan.Get<Sprite>("AJOPI9Calm");
        spriteRenderer[0].transform.position += Vector3.down * spriteRenderer[0].transform.position.y + Vector3.up * 5f;
        spriteRenderer[0].transform.localScale *= 10f;

        audMan = GetComponent<PropagatedAudioManager>();
        audMan.overrideSubtitleColor = false;

        navigator.SetRoomAvoidance(true);
        navigator.SetSpeed(2f);
        navigator.maxSpeed = 2f;

        behaviorStateMachine.ChangeState(new AJOPI_Wander(this));
    }

    public void BeginRev()
    {
        TestPlugin.Instance.Logger.LogInfo("AJOPI is charging up scream");

        audMan.PlaySingle(TestPlugin.Instance.assetMan.Get<SoundObject>("ajopi9Begin"));
        audMan.QueueAudio(TestPlugin.Instance.assetMan.Get<SoundObject>("ajopi9Loop"));
        audMan.SetLoop(true);
        spriteRenderer[0].sprite = TestPlugin.Instance.assetMan.Get<Sprite>("AJOPI9Start1");
        Invoke(nameof(OpenWide), 1.5f);
        Invoke(nameof(OpenWider), 3f);
        Invoke(nameof(OpenWidest), 5f);
    }

    private void OpenWide()
    {
        spriteRenderer[0].sprite = TestPlugin.Instance.assetMan.Get<Sprite>("AJOPI9Start2");
    }

    private void OpenWider()
    {
        spriteRenderer[0].sprite = TestPlugin.Instance.assetMan.Get<Sprite>("AJOPI9Start3");
    }

    private void OpenWidest()
    {
        spriteRenderer[0].sprite = TestPlugin.Instance.assetMan.Get<Sprite>("AJOPI9Scream");
    }

    public void BecomeCoolRed()
    {
        spriteRenderer[0].sprite = TestPlugin.Instance.assetMan.Get<Sprite>("AJOPI9Red");
    }

    public void Blind()
    {
        transform.position = new Vector3(-9999f, 0f, -9999f);
        audMan.FlushQueue(true);
        audMan.SetLoop(false);
        audMan.audioDevice.Stop();
        gameObject.GetComponent<Entity>().SetFrozen(true);
        fog = new Fog
        {
            color = new Color(0.08f, 0.15f, 0f),
            maxDist = 45f,
            startDist = 0f,
            priority = 99,
            strength = 9f
        };
        Singleton<CoreGameManager>.Instance.audMan.PlaySingle(TestPlugin.Instance.assetMan.Get<SoundObject>("ajopi9Blind"));
        ec.AddFog(fog);
        Invoke(nameof(Over), 300f);

        TestPlugin.Instance.Logger.LogInfo("AJOPI has blinded player");
    }

    private void Over()
    {
        TestPlugin.Instance.Logger.LogInfo("AJOPI blindness has worn off");

        ec.RemoveFog(fog);
        Despawn();
    }
}
