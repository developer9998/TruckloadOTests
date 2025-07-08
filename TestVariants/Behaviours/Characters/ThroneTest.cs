using System.Collections.Generic;
using TestVariants.Models.StateMachine;
using UnityEngine;

namespace TestVariants.Behaviours.Characters;

public class ThroneTest : NPC, IEntityTrigger
{
    private AudioManager audMan;

    public override void Initialize()
    {
        base.Initialize();

        spriteRenderer[0].sprite = TestPlugin.Instance.assetMan.Get<Sprite>("ThroneTest_0");

        Transform transform = spriteRenderer[0].transform;
        transform.position += Vector3.down * spriteRenderer[0].transform.position.y + Vector3.up * 4f;
        Transform transform2 = spriteRenderer[0].transform;
        transform2.localScale *= 8f;

        Sprite[] sprites =
        [
            TestPlugin.Instance.assetMan.Get<Sprite>("ThroneTest_0"),
            TestPlugin.Instance.assetMan.Get<Sprite>("ThroneTest_7"),
            TestPlugin.Instance.assetMan.Get<Sprite>("ThroneTest_6"),
            TestPlugin.Instance.assetMan.Get<Sprite>("ThroneTest_5"),
            TestPlugin.Instance.assetMan.Get<Sprite>("ThroneTest_4"),
            TestPlugin.Instance.assetMan.Get<Sprite>("ThroneTest_3"),
            TestPlugin.Instance.assetMan.Get<Sprite>("ThroneTest_2"),
            TestPlugin.Instance.assetMan.Get<Sprite>("ThroneTest_1")
        ];

        SpriteRotation sPRITERTOATE = spriteRenderer[0].gameObject.AddComponent<SpriteRotation>();
        sPRITERTOATE.sprites = sprites;
        sPRITERTOATE.spriteRenderer = spriteRenderer[0];
        sPRITERTOATE.offset = 6;

        audMan = GetComponent<PropagatedAudioManager>();
        audMan.overrideSubtitleColor = false;

        navigator.SetRoomAvoidance(true);
        navigator.SetSpeed(20f);
        navigator.maxSpeed = 20f;

        behaviorStateMachine.ChangeState(new ThroneTest_Wander(this));
    }

    public void SetGuilty()
    {
        SetGuilt(1f, "Bullying");
    }

    public void Shutit()
    {
        audMan.audioDevice.Stop();
    }

    public void SayLine(int c, int cc)
    {
        switch (c)
        {
            case 0:
                audMan.PlaySingle(TestPlugin.Instance.assetMan.Get<SoundObject>("ThroneTest_Idle_" + cc));
                break;
            case 1:
                audMan.PlaySingle(TestPlugin.Instance.assetMan.Get<SoundObject>("ThroneTest_RunOverRandom_" + cc));
                break;
            case 2:
                audMan.PlaySingle(TestPlugin.Instance.assetMan.Get<SoundObject>("ThroneTest_Hit"));
                break;
            case 3:
                audMan.PlaySingle(TestPlugin.Instance.assetMan.Get<SoundObject>("ThroneTest_Bang"));
                break;
            case 4:
                audMan.PlaySingle(TestPlugin.Instance.assetMan.Get<SoundObject>("ThroneTest_Combo" + cc));
                break;
            case 5:
                audMan.PlaySingle(TestPlugin.Instance.assetMan.Get<SoundObject>("ThroneTest_Combo5Buildup"));
                break;
            case 6:
                audMan.PlaySingle(TestPlugin.Instance.assetMan.Get<SoundObject>("ThroneTest_Combo5Successful"));
                break;
            case 7:
                audMan.PlaySingle(TestPlugin.Instance.assetMan.Get<SoundObject>("ThroneTest_ComboLost"));
                break;
        }
    }
}
