using System.Collections;
using TestVariants.Models.StateMachine;
using UnityEngine;

namespace TestVariants.Behaviours.Characters;

public class PalbyFan : NPC, IEntityTrigger
{
    public float walkSpeed = 1.25f;

    public bool cooldown = false;

    public AudioManager audMan;

    private bool animating = false;

    private bool talkanim = false;

    public bool punching;

    private bool eyewide = false;

    private bool madtalk = false;

    private int frame = 0;

    private float animTimer = 0f;

    public override void Initialize()
    {
        base.Initialize();

        spriteRenderer[0].sprite = TestPlugin.Instance.assetMan.Get<Sprite>("PalbyFan_Mad0");
        spriteRenderer[0].transform.position += Vector3.down * spriteRenderer[0].transform.position.y + Vector3.up * 5f;
        spriteRenderer[0].transform.localScale *= 10f;

        audMan = GetComponent<PropagatedAudioManager>();
        audMan.overrideSubtitleColor = false;

        navigator.SetRoomAvoidance(val: false);
        navigator.SetSpeed(18f);
        navigator.maxSpeed = 18f;

        behaviorStateMachine.ChangeState(new PalbyFan_Wander(this));

        animating = false;
    }

    public override void VirtualUpdate()
    {
        base.VirtualUpdate();

        if (!animating)
        {
            animTimer -= Time.deltaTime * walkSpeed * 2f;
            if (animTimer <= 0f)
            {
                animTimer = 1f;
                frame++;
                switch (frame)
                {
                    case 1:
                        spriteRenderer[0].sprite = TestPlugin.Instance.assetMan.Get<Sprite>("PalbyFan_Run0");
                        break;
                    case 2:
                        spriteRenderer[0].sprite = TestPlugin.Instance.assetMan.Get<Sprite>("PalbyFan_Mad0");
                        break;
                    case 3:
                        spriteRenderer[0].sprite = TestPlugin.Instance.assetMan.Get<Sprite>("PalbyFan_Run1");
                        break;
                    case 4:
                        spriteRenderer[0].sprite = TestPlugin.Instance.assetMan.Get<Sprite>("PalbyFan_Mad0");
                        frame = 0;
                        break;
                }
            }
            return;
        }

        if (talkanim)
        {
            if (eyewide)
            {
                spriteRenderer[0].sprite = TestPlugin.Instance.assetMan.Get<Sprite>("PalbyFan_Show2");
                return;
            }

            animTimer -= Time.deltaTime * 5f;
            if (animTimer <= 0f)
            {
                animTimer = 1f;
                frame++;

                if (frame == 1)
                {
                    spriteRenderer[0].sprite = TestPlugin.Instance.assetMan.Get<Sprite>("PalbyFan_Show0");
                    return;
                }

                frame = 0;
                spriteRenderer[0].sprite = TestPlugin.Instance.assetMan.Get<Sprite>("PalbyFan_Show1");
            }

            return;
        }

        if (!madtalk) return;

        animTimer -= Time.deltaTime * 5f;
        if (animTimer <= 0f)
        {
            animTimer = 1f;
            frame++;

            if (frame == 1)
            {
                spriteRenderer[0].sprite = TestPlugin.Instance.assetMan.Get<Sprite>("PalbyFan_Mad0");
                return;
            }

            frame = 0;
            spriteRenderer[0].sprite = TestPlugin.Instance.assetMan.Get<Sprite>("PalbyFan_Mad1");
        }
    }

    public void ShowPalby()
    {
        TestPlugin.Instance.Logger.LogInfo("PalbyFan is showing Palby");
        talkanim = true;
        LookAnim();
        SayTheLine(0);
        behaviorStateMachine.ChangeState(new PalbyFan_Show(this));
    }

    public void Shutup()
    {
        audMan.audioDevice.Stop();
        audMan.FlushQueue(true);
    }

    public void LookAnim()
    {
        talkanim = true;
        animating = true;
        eyewide = false;
        Invoke(nameof(WideEyed), 0.67f);
        Invoke(nameof(WideEyed), 1.51f);
        Invoke(nameof(WideEyed), 2.59f);
        Invoke(nameof(WideEyed), 3.51f);
        Invoke(nameof(WideEyed), 4.63f);
        Invoke(nameof(WideEyed), 5.62f);
        Invoke(nameof(WideEyed), 6.03f);
        Invoke(nameof(WideEyed), 6.87f);
        Invoke(nameof(LoopLookAnim), 7.385f);
    }

    public void Please()
    {
        TestPlugin.Instance.Logger.LogInfo("PalbyFan recognises the player likes Palby");
        talkanim = false;
        animating = false;
        cooldown = true;
        Shutup();
        behaviorStateMachine.ChangeState(new PalbyFan_Wander(this));
        Invoke(nameof(NoCooldown), 20f);
        Singleton<CoreGameManager>.Instance.AddPoints(25, ec.Players[0].playerNumber, true);
    }

    private void NoCooldown()
    {
        cooldown = false;
    }

    public void Anger()
    {
        TestPlugin.Instance.Logger.LogInfo("PalbyFan is angry, surely the player must not like Palby");
        Shutup();
        talkanim = false;
        animating = true;
        SayTheLine(1);
        madtalk = true;
        Invoke(nameof(StartChasing), 3.365f);
    }

    private void StartChasing()
    {
        madtalk = false;
        animating = false;
        talkanim = false;
        punching = false;
        behaviorStateMachine.ChangeState(new PalbyFan_Angered(this));
    }

    private void LoopLookAnim()
    {
        if (talkanim)
        {
            LookAnim();
        }
    }

    private void WideEyed()
    {
        eyewide = true;
        Invoke(nameof(NotWideEyed), 0.225f);
    }

    private void NotWideEyed()
    {
        eyewide = false;
    }

    public void SayTheLine(int c)
    {
        switch (c)
        {
            case 0:
                audMan.QueueAudio(TestPlugin.Instance.assetMan.Get<SoundObject>("PalbyFan_Look"));
                audMan.SetLoop(val: true);
                break;
            case 1:
                audMan.PlaySingle(TestPlugin.Instance.assetMan.Get<SoundObject>("PalbyFan_UhOh"));
                break;
            case 2:
                audMan.PlaySingle(TestPlugin.Instance.assetMan.Get<SoundObject>("PalbyFan_AfterPunch"));
                break;
        }
    }

    public void Punch()
    {
        TestPlugin.Instance.Logger.LogInfo("PalbyFan charing up punch");
        Navigator.SetSpeed(12f);
        punching = true;
        animating = true;
        Navigator.Entity.SetFrozen(true);
        spriteRenderer[0].sprite = TestPlugin.Instance.assetMan.Get<Sprite>("PalbyFan_Punch0");
        Invoke(nameof(RealPunch), 0.25f);
    }

    public IEnumerator RemoveMod(ActivityModifier am, MovementModifier mm)
    {
        yield return new WaitForSeconds(10f);
        am.moveMods.Remove(mm);
    }

    private void RealPunch()
    {
        bool playPunchSound = false;

        foreach (NPC npc in ec.Npcs)
        {
            if (npc.Navigator?.Entity?.ExternalActivity is ActivityModifier activityModifier && activityModifier)
            {
                if ((npc.transform.position - transform.position).magnitude <= 10f & npc.gameObject != gameObject)
                {
                    TestPlugin.Instance.Logger.LogInfo($"PalbyFan punched {npc.gameObject.name}/{npc.character}");
                    playPunchSound = true;
                    StartCoroutine(RemoveMod(activityModifier, new MovementModifier(Vector3.zero, 0.25f)));
                    npc.Navigator.Entity.AddForce(new Force(transform.forward, 50f, -40f));
                    npc.gameObject.AddComponent<Dizziness>().StartDizziness();
                }
            }
        }

        if ((ec.Players[0].transform.position - transform.position).magnitude <= 10f)
        {
            TestPlugin.Instance.Logger.LogInfo("PalbyFan punched local player");
            playPunchSound = true;
            ec.Players[0].plm.Entity.AddForce(new Force(transform.forward, 50f, -40f));
            ec.Players[0].gameObject.AddComponent<Dizziness>().StartDizziness();
            Invoke(nameof(Happy), 0.49f);
        }

        if (playPunchSound) audMan.PlaySingle(TestPlugin.Instance.assetMan.Get<SoundObject>("PalbyFan_Punch"));

        spriteRenderer[0].sprite = TestPlugin.Instance.assetMan.Get<Sprite>("PalbyFan_Punch1");
        Invoke(nameof(StopPunch), 0.5f);
    }

    private void Happy()
    {
        TestPlugin.Instance.Logger.LogInfo("PalbyFan returning to wander state after punch");
        SayTheLine(2);
        cooldown = true;
        behaviorStateMachine.ChangeState(new PalbyFan_Wander(this));
        Invoke(nameof(NoCooldown), 30f);
    }

    private void StopPunch()
    {
        animating = false;
        Navigator.Entity.SetFrozen(false);
        punching = false;
    }
}
