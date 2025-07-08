using TestVariants.Models.StateMachine;
using UnityEngine;

namespace TestVariants.Behaviours.Characters;

public class Testholder : NPC, IEntityTrigger
{
    private AudioManager audMan;

    public override void Initialize()
    {
        base.Initialize();

        spriteRenderer[0].sprite = TestPlugin.Instance.assetMan.Get<Sprite>("TestholderMain");
        spriteRenderer[0].transform.position += Vector3.down * spriteRenderer[0].transform.position.y + Vector3.up * 5f;
        spriteRenderer[0].transform.localScale *= 10f;

        audMan = GetComponent<PropagatedAudioManager>();
        audMan.overrideSubtitleColor = false;

        navigator.SetRoomAvoidance(val: false);
        navigator.SetSpeed(17f);
        navigator.maxSpeed = 17f;

        behaviorStateMachine.ChangeState(new Testholder_Wander(this));
    }

    public void SayTheLine(bool huh)
    {
        audMan.PlaySingle(TestPlugin.Instance.assetMan.Get<SoundObject>("Testholder9_Idle" + Random.Range(1, 6)));
    }

    public void Sadden()
    {
        Invoke(nameof(Testholde), 0.75f);
        Invoke(nameof(RNine), 10f);
    }

    private void RNine()
    {
        spriteRenderer[0].sprite = TestPlugin.Instance.assetMan.Get<Sprite>("TestholderMain");
    }

    private void Testholde()
    {
        spriteRenderer[0].sprite = TestPlugin.Instance.assetMan.Get<Sprite>("TestholderFrown");
    }
}
