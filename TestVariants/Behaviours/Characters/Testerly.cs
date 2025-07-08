using System.Collections.Generic;
using TestVariants.Behaviours.Items;
using TestVariants.Models.StateMachine;
using UnityEngine;

namespace TestVariants.Behaviours.Characters;

public class Testerly : NPC, IEntityTrigger
{
    public AudioManager stepAudMan;

    public SpriteRotation spriteRotation;

    public Sprite[] distraughtSprites;

    public Sprite[] runCycleASprites;

    public Sprite[] runCycleBSprites;

    public Sprite[] runCycleCSprites;

    public Entity laptopEnt;

    public GameObject toBlame;

    private AudioManager audMan;

    public override void Initialize()
    {
        base.Initialize();

        distraughtSprites =
        [
            TestPlugin.Instance.assetMan.Get<Sprite>("Testerly_Distraught_0"),
            TestPlugin.Instance.assetMan.Get<Sprite>("Testerly_Distraught_7"),
            TestPlugin.Instance.assetMan.Get<Sprite>("Testerly_Distraught_6"),
            TestPlugin.Instance.assetMan.Get<Sprite>("Testerly_Distraught_5"),
            TestPlugin.Instance.assetMan.Get<Sprite>("Testerly_Distraught_4"),
            TestPlugin.Instance.assetMan.Get<Sprite>("Testerly_Distraught_3"),
            TestPlugin.Instance.assetMan.Get<Sprite>("Testerly_Distraught_2"),
            TestPlugin.Instance.assetMan.Get<Sprite>("Testerly_Distraught_1")
        ];

        runCycleASprites =
        [
            TestPlugin.Instance.assetMan.Get<Sprite>("TesterlyRunCycle1_0"),
            TestPlugin.Instance.assetMan.Get<Sprite>("TesterlyRunCycle1_7"),
            TestPlugin.Instance.assetMan.Get<Sprite>("TesterlyRunCycle1_6"),
            TestPlugin.Instance.assetMan.Get<Sprite>("TesterlyRunCycle1_5"),
            TestPlugin.Instance.assetMan.Get<Sprite>("TesterlyRunCycle1_4"),
            TestPlugin.Instance.assetMan.Get<Sprite>("TesterlyRunCycle1_3"),
            TestPlugin.Instance.assetMan.Get<Sprite>("TesterlyRunCycle1_2"),
            TestPlugin.Instance.assetMan.Get<Sprite>("TesterlyRunCycle1_1")
        ];

        runCycleBSprites =
        [
            TestPlugin.Instance.assetMan.Get<Sprite>("TesterlyRunCycle2_0"),
            TestPlugin.Instance.assetMan.Get<Sprite>("TesterlyRunCycle2_7"),
            TestPlugin.Instance.assetMan.Get<Sprite>("TesterlyRunCycle2_6"),
            TestPlugin.Instance.assetMan.Get<Sprite>("TesterlyRunCycle2_5"),
            TestPlugin.Instance.assetMan.Get<Sprite>("TesterlyRunCycle2_4"),
            TestPlugin.Instance.assetMan.Get<Sprite>("TesterlyRunCycle2_3"),
            TestPlugin.Instance.assetMan.Get<Sprite>("TesterlyRunCycle2_2"),
            TestPlugin.Instance.assetMan.Get<Sprite>("TesterlyRunCycle2_1")
        ];

        runCycleCSprites =
        [
            TestPlugin.Instance.assetMan.Get<Sprite>("TesterlyRunCycle3_0"),
            TestPlugin.Instance.assetMan.Get<Sprite>("TesterlyRunCycle3_7"),
            TestPlugin.Instance.assetMan.Get<Sprite>("TesterlyRunCycle3_6"),
            TestPlugin.Instance.assetMan.Get<Sprite>("TesterlyRunCycle3_5"),
            TestPlugin.Instance.assetMan.Get<Sprite>("TesterlyRunCycle3_4"),
            TestPlugin.Instance.assetMan.Get<Sprite>("TesterlyRunCycle3_3"),
            TestPlugin.Instance.assetMan.Get<Sprite>("TesterlyRunCycle3_2"),
            TestPlugin.Instance.assetMan.Get<Sprite>("TesterlyRunCycle3_1")
        ];

        spriteRenderer[0].transform.position += Vector3.down * spriteRenderer[0].transform.position.y + Vector3.up * 4f;
        spriteRenderer[0].transform.localScale *= 8f;
        spriteRotation = spriteRenderer[0].gameObject.AddComponent<SpriteRotation>();
        spriteRotation.sprites = distraughtSprites;
        spriteRotation.spriteRenderer = spriteRenderer[0];
        spriteRotation.offset = 6;
        spriteRotation.enabled = false;
        spriteRenderer[0].sprite = TestPlugin.Instance.assetMan.Get<Sprite>("Testerly_Idle_0");

        audMan = GetComponent<PropagatedAudioManager>();
        audMan.overrideSubtitleColor = false;

        GameObject stepAudioObject = new();
        stepAudioObject.transform.SetParent(transform);
        stepAudioObject.transform.position = transform.position;
        stepAudMan = stepAudioObject.AddComponent<PropagatedAudioManager>();

        GameObject propagatorObject = new("Propagator");
        propagatorObject.transform.parent = stepAudioObject.transform;
        propagatorObject.transform.position = stepAudioObject.transform.position;
        propagatorObject.AddComponent<AudioSource>();

        AudioSource component = propagatorObject.GetComponent<AudioSource>();
        component.playOnAwake = false;
        component.loop = false;

        stepAudMan.audioDevice = component;
        stepAudMan.overrideSubtitleColor = false;
        stepAudMan.gameObject.GetComponent<PropagatedAudioManager>().maxDistance = audMan.gameObject.GetComponent<PropagatedAudioManager>().maxDistance;
        // stepAudMan.gameObject.GetComponent<PropagatedAudioManager>().minDistance = audMan.gameObject.GetComponent<PropagatedAudioManager>().minDistance;
        stepAudMan.Invoke("VirtualAwake", 0.1f);

        navigator.SetRoomAvoidance(val: true);
        navigator.SetSpeed(20f);
        navigator.maxSpeed = 20f;

        behaviorStateMachine.ChangeState(new Testerly_Idle(this));
    }

    public void Drop()
    {
        TestPlugin.Instance.Logger.LogInfo("Testerly laptop dropped");

        ITM_Laptop iTM_Laptop = Instantiate((ITM_Laptop)TestPlugin.Instance.assetMan.Get<ItemObject>("Laptop").item);
        Vector3 origin = transform.position - Vector3.up * transform.position.y + Vector3.up * 5f;
        Vector3 direction = transform.forward;
        origin += direction * 2f;
        iTM_Laptop.Prop(ec, origin, direction);
        laptopEnt = iTM_Laptop.entity;
    }

    public void SayLine(int c)
    {
        switch(c)
        {
            case 5:
                stepAudMan.PlaySingle(TestPlugin.Instance.assetMan.Get<SoundObject>("Testerly_Run" + Random.Range(1, 3)));
                break;
            case 0:
                audMan.PlaySingle(TestPlugin.Instance.assetMan.Get<SoundObject>("Testerly_Gasp"));
                break;
            case 1:
                audMan.PlaySingle(TestPlugin.Instance.assetMan.Get<SoundObject>("Testerly_MyLaptopBroken"));
                break;
            case 2:
                List<string> soundNames = ["GonnaRegret", "Snitching"];
                audMan.PlaySingle(TestPlugin.Instance.assetMan.Get<SoundObject>("Testerly_" + soundNames[Random.Range(0, 2)]));
                break;
            case 3:
                audMan.PlaySingle(TestPlugin.Instance.assetMan.Get<SoundObject>("Testerly_SeekRandom" + Random.Range(1, 3)));
                break;
            case 4:
                audMan.PlaySingle(TestPlugin.Instance.assetMan.Get<SoundObject>("Testerly_AlertPrincipal"));
                break;
            case 6:
                audMan.PlaySingle(TestPlugin.Instance.assetMan.Get<SoundObject>("Testerly_Bump"));
                break;
        }
    }

    public void Snitcharoo(Principal p)
    {
        TestPlugin.Instance.Logger.LogInfo($"Testerly sniching to Principal: {p.gameObject.name}");

        Invoke(nameof(GetBack), 15f);

        if (toBlame.TryGetComponent(out PlayerManager pm))
        {
            p.targetedPlayer = pm;
            p.behaviorStateMachine.ChangeState(new Principal_ChasingPlayer(p, pm));
            p.Scold("Bullying");
        }
        else
        {
            p.behaviorStateMachine.ChangeState(new Principal_ChasingNpc(p, toBlame.GetComponent<NPC>()));
            p.Scold("Bullying");
        }
    }

    public void GetReady()
    {
        if (toBlame == gameObject)
        {
            Invoke(nameof(GetBack), 30f);
            return;
        }

        Invoke(nameof(Snitch), 5f);
    }

    private void Snitch()
    {
        behaviorStateMachine.ChangeState(new Testerly_Snitch(this));
    }

    private void GetBack()
    {
        TestPlugin.Instance.Logger.LogInfo("Testerly returning to idle state");

        Cell cell = ec.CellFromPosition(transform.position);
        transform.position = cell.CenterWorldPosition;
        behaviorStateMachine.ChangeState(new Testerly_Idle(this));
    }

    public void Shutup()
    {
        audMan.audioDevice.Stop();
        audMan.FlushQueue(true);
    }

    public void TypeLoop()
    {
        Shutup();
        audMan.QueueAudio(TestPlugin.Instance.assetMan.Get<SoundObject>("Testerly_Keyboard"));
        audMan.SetLoop(true);
    }

    public override void VirtualUpdate()
    {
        base.VirtualUpdate();
    }
}
