using TestVariants.Models.StateMachine;
using UnityEngine;

namespace TestVariants.Behaviours.Characters;

public class Canny : NPC, IEntityTrigger
{
    private AudioManager audMan;

    public override void Initialize()
    {
        base.Initialize();

        spriteRenderer[0].sprite = TestPlugin.Instance.assetMan.Get<Sprite>("CannedTestNPC");
        spriteRenderer[0].transform.position += Vector3.down * spriteRenderer[0].transform.position.y + Vector3.up * 5f;
        spriteRenderer[0].transform.localScale *= 10f;

        audMan = GetComponent<PropagatedAudioManager>();
        audMan.overrideSubtitleColor = false;

        behaviorStateMachine.ChangeState(new Canny_Uncanned(this));
    }

    public void PlaySound()
    {
        audMan.PlaySingle(TestPlugin.Instance.assetMan.Get<SoundObject>("CannedTestPush"));
    }
}
