using TestVariants.Behaviours.Items;
using TestVariants.Models.StateMachine;
using UnityEngine;

namespace TestVariants.Behaviours.Characters;

public class Cren : NPC, IEntityTrigger
{
    private AudioManager audMan;

    private bool animating = false;

    private float stepTimer = 1f;

    public Vector3 spawnPoint;

    private int stepStage = 0;

    public override void Initialize()
    {
        base.Initialize();

        spriteRenderer[0].sprite = TestPlugin.Instance.assetMan.Get<Sprite>("CrenInbetween");
        spriteRenderer[0].transform.position += Vector3.down * spriteRenderer[0].transform.position.y + Vector3.up * 5f;
        spriteRenderer[0].transform.localScale *= 10f;

        audMan = GetComponent<PropagatedAudioManager>();
        audMan.overrideSubtitleColor = false;

        navigator.SetRoomAvoidance(false);
        navigator.SetSpeed(13f);
        navigator.maxSpeed = 13f;

        behaviorStateMachine.ChangeState(new Cren_Follow(this));

        spawnPoint = transform.position;
    }

    public override void VirtualUpdate()
    {
        base.VirtualUpdate();

        stepTimer += -1f * TimeScale * (gameObject.GetComponent<Entity>().Velocity.magnitude / 9f);
        if (animating) stepTimer = 1f;

        if (stepTimer <= 0f & !Singleton<CoreGameManager>.Instance.paused)
        {
            stepTimer = 1f;
            stepStage = (stepStage + 1) % 4;

            switch (stepStage)
            {
                case 0:
                    spriteRenderer[0].sprite = TestPlugin.Instance.assetMan.Get<Sprite>("CrenInbetween");
                    audMan.PlaySingle(TestPlugin.Instance.assetMan.Get<SoundObject>("Cren_Walk2"));
                    break;
                case 1:
                    spriteRenderer[0].sprite = TestPlugin.Instance.assetMan.Get<Sprite>("CrenWalk1");
                    break;
                case 2:
                    spriteRenderer[0].sprite = TestPlugin.Instance.assetMan.Get<Sprite>("CrenInbetween");
                    audMan.PlaySingle(TestPlugin.Instance.assetMan.Get<SoundObject>("Cren_Walk1"));
                    break;
                case 3:
                    spriteRenderer[0].sprite = TestPlugin.Instance.assetMan.Get<Sprite>("CrenWalk2");
                    break;
            }
        }
    }

    public void Spit()
    {
        TestPlugin.Instance.Logger.LogInfo("Cren is charing up spit attack");

        audMan.PlaySingle(TestPlugin.Instance.assetMan.Get<SoundObject>("Cren_Spit"));
        animating = true;
        spriteRenderer[0].sprite = TestPlugin.Instance.assetMan.Get<Sprite>("CrenSpitPrepare1");
        Invoke(nameof(SpitPartTwo), 0.1f);
        Invoke(nameof(RealSpit), 0.3f);
    }

    private void SpitPartTwo()
    {
        spriteRenderer[0].sprite = TestPlugin.Instance.assetMan.Get<Sprite>("CrenSpitPrepare2");
    }

    private void RealSpit()
    {
        SetGuilt(1f, "Drinking");
        ITM_BubblerumSoda itm_BubblerumSoda = Instantiate((ITM_BubblerumSoda)TestPlugin.Instance.assetMan.Get<ItemObject>("Bubblerum").item);
        Vector3 origin = transform.position - Vector3.up * transform.position.y + Vector3.up * 5f;
        Vector3 direction = Directions.DirFromVector3(ec.Players[0].transform.position - transform.position, 45f).ToVector3();
        origin += direction * 4f;
        itm_BubblerumSoda.Prop(ec, origin, direction);
        spriteRenderer[0].sprite = TestPlugin.Instance.assetMan.Get<Sprite>("CrenSpitOut2");
        Invoke(nameof(SpitPartThree), 0.12f);
        Invoke(nameof(SpitPartFour), 0.2f);

        TestPlugin.Instance.Logger.LogInfo("Cren has spat bubblerum");
    }

    private void SpitPartThree()
    {
        spriteRenderer[0].sprite = TestPlugin.Instance.assetMan.Get<Sprite>("CrenSpitOut1");
    }

    private void SpitPartFour()
    {
        spriteRenderer[0].sprite = TestPlugin.Instance.assetMan.Get<Sprite>("CrenInbetween");
        animating = false;
    }

    public void Activate()
    {
        TestPlugin.Instance.Logger.LogInfo("Cren has activated");

        behaviorStateMachine.ChangeState(new Cren_Follow(this));
        spriteRenderer[0].enabled = true;
    }

    public void Run()
    {
        Navigator.Entity.SetFrozen(true);
        behaviorStateMachine.ChangeState(new Cren_Runback(this));
        audMan.PlaySingle(TestPlugin.Instance.assetMan.Get<SoundObject>("Cren_RunAway"));
        Invoke(nameof(TrulyRun), 1f);
    }

    private void TrulyRun()
    {
        TestPlugin.Instance.Logger.LogInfo("Cren is dashing away from incident area");

        Entity entity = Navigator.Entity;
        entity.ExternalActivity.moveMods.Clear();
        audMan.PlaySingle(TestPlugin.Instance.assetMan.Get<SoundObject>("Cren_DashSfx"));
        entity.SetFrozen(false);
    }
}
