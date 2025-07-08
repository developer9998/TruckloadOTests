using TestVariants.Models.StateMachine;
using UnityEngine;

namespace TestVariants.Behaviours.Characters;

public class TheBestOne : NPC, IEntityTrigger
{
    private MovementModifier moveModPlayer;

    private MovementModifier moveModSelf;

    public PlayerManager yellower;

    private AudioManager audMan;

    private bool yellowing = false;

    public bool cooldown = false;

    public override void Initialize()
    {
        base.Initialize();

        spriteRenderer[0].sprite = TestPlugin.Instance.assetMan.Get<Sprite>("TheBestOne");
        spriteRenderer[0].transform.position += Vector3.down * spriteRenderer[0].transform.position.y + Vector3.up * 5f;
        spriteRenderer[0].transform.localScale *= 10f;

        audMan = GetComponent<PropagatedAudioManager>();
        audMan.overrideSubtitleColor = false;

        navigator.SetRoomAvoidance(val: false);
        navigator.SetSpeed(22f);
        navigator.maxSpeed = 22f;

        behaviorStateMachine.ChangeState(new TheBestOne_Wander(this));
    }

    public void SayTheLine(bool Wizz)
    {
        audMan.audioDevice.Stop();
        audMan.PlaySingle(TestPlugin.Instance.assetMan.Get<SoundObject>(Wizz ? "TheBestOne_Yellowing" : ("TheBestOne_Yellow_" + Random.Range(1, 6))));
    }

    public void Yellow(PlayerManager player)
    {
        behaviorStateMachine.ChangeState(new TheBestOne_Yellow(this));
        yellower = player;
        SayTheLine(true);
        moveModPlayer = new MovementModifier(Vector3.zero, 0f);
        moveModSelf = new MovementModifier(Vector3.zero, 0f);
        yellower.plm.Entity.ExternalActivity.moveMods.Add(moveModPlayer);
        Navigator.Entity.ExternalActivity.moveMods.Add(moveModSelf);
        yellowing = true;
        Invoke(nameof(QuitYellowin), 20f);
    }

    public void QuitYellowin()
    {
        if (yellowing)
        {
            audMan.audioDevice.Stop();
            cooldown = true;
            yellowing = false;
            yellower.plm.Entity.ExternalActivity.moveMods.Remove(moveModPlayer);
            Navigator.Entity.ExternalActivity.moveMods.Remove(moveModSelf);
            behaviorStateMachine.ChangeState(new TheBestOne_Wander(this));
            Invoke(nameof(Warmup), 20f);
        }
    }

    private void Warmup()
    {
        cooldown = false;
    }
}
