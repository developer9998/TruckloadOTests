using System.Collections;
using System.Collections.Generic;
using TestVariants.Models.StateMachine;
using UnityEngine;

namespace TestVariants.Behaviours.Characters;

public class Plit : NPC, IEntityTrigger
{
    public Vector3 spriteOffset;

    private bool stunned = false;

    private bool lunging = false;

    public float cooldown = 0f;

    public ActivityModifier mauling;

    private AudioManager audMan;

    public override void Initialize()
    {
        base.Initialize();

        spriteRenderer[0].sprite = TestPlugin.Instance.assetMan.Get<Sprite>("PlitSpriteMain");
        spriteRenderer[0].transform.position += Vector3.down * spriteRenderer[0].transform.position.y + Vector3.up * 5f;
        spriteRenderer[0].transform.localScale *= 10f;
        spriteOffset = spriteBase.transform.position - transform.position;

        audMan = GetComponent<PropagatedAudioManager>();
        audMan.overrideSubtitleColor = false;

        navigator.SetRoomAvoidance(false);
        navigator.SetSpeed(12f);
        navigator.maxSpeed = 12f;

        behaviorStateMachine.ChangeState(new Plit_Wander(this));
    }

    public void SayTheLine(int cat)
    {
        switch (cat)
        {
            case 0:
                audMan.PlaySingle(TestPlugin.Instance.assetMan.Get<SoundObject>("Plit_Idle" + Random.Range(1, 5).ToString()));
                break;
            case 1:
                audMan.PlaySingle(TestPlugin.Instance.assetMan.Get<SoundObject>("Plit_Lunge" + Random.Range(1, 4).ToString()));
                break;
            case 2:
                audMan.QueueAudio(TestPlugin.Instance.assetMan.Get<SoundObject>("Plit_Attack_Loop"));
                audMan.SetLoop(true);
                break;
            case 3:
                audMan.PlaySingle(TestPlugin.Instance.assetMan.Get<SoundObject>("Plit_Stunned"));
                break;
        }
    }

    public void StopMaul(bool zap)
    {
        cooldown = 15f;
        behaviorStateMachine.ChangeState(new Plit_Wander(this));
        spriteBase.transform.position = transform.position + spriteOffset;
        Shutup();
        audMan.PlaySingle(TestPlugin.Instance.assetMan.Get<SoundObject>("Bang"));
        mauling.moveMods.Clear();
        Navigator.Entity.ExternalActivity.moveMods.Clear();
        if (!zap)
        {
            MovementModifier movementModifier = new(Vector3.zero, 0.5f);
            mauling.moveMods.Add(movementModifier);
            StartCoroutine(RemoveMod(mauling, movementModifier));
        }
        mauling = null;
    }

    public void Stun()
    {
        if (mauling != null && mauling)
        {
            spriteRenderer[0].sprite = TestPlugin.Instance.assetMan.Get<Sprite>("PlitSpriteStun");
            gameObject.GetComponent<ActivityModifier>().moveMods.Clear();
            mauling.moveMods.Clear();
            StopMaul(true);
            SayTheLine(3);
            stunned = true;
            Invoke(nameof(Unstun), 10f);
        }
    }

    private void Unstun()
    {
        stunned = false;
        spriteRenderer[0].sprite = TestPlugin.Instance.assetMan.Get<Sprite>("PlitSpriteMain");
        spriteBase.transform.position = transform.position + spriteOffset;
    }

    public IEnumerator RemoveMod(ActivityModifier am, MovementModifier mm)
    {
        yield return new WaitForSeconds(10f);
        am.moveMods.Remove(mm);
        yield break;
    }

    public override void VirtualUpdate()
    {
        cooldown -= Time.deltaTime * TimeScale;

        if (stunned) spriteBase.transform.position = transform.position + spriteOffset + new Vector3(Random.Range(-0.4f, 0.4f), 0f, Random.Range(-0.4f, 0.4f));

        if (lunging)
        {
            List<ActivityModifier> list = [];
            foreach (NPC npc in ec.Npcs)
            {
                list.Add(npc.Navigator.Entity.ExternalActivity);
            }
            list.Add(ec.Players[0].plm.Entity.ExternalActivity);

            foreach (ActivityModifier activityModifier in list)
            {
                if ((activityModifier.transform.position - transform.position).magnitude <= 5f & activityModifier != Navigator.Entity.ExternalActivity)
                {
                    mauling = activityModifier;
                    behaviorStateMachine.ChangeState(new Plit_Maul(this));
                    Shutup();
                    spriteRenderer[0].sprite = TestPlugin.Instance.assetMan.Get<Sprite>("PlitSpriteMain");
                    SayTheLine(2);
                    lunging = false;
                }
            }
        }
    }

    public void Shutup()
    {
        audMan.audioDevice.Stop();
        audMan.FlushQueue(true);
    }

    public void LungeAt(ActivityModifier target)
    {
        if (cooldown <= 0f)
        {
            spriteRenderer[0].sprite = TestPlugin.Instance.assetMan.Get<Sprite>("PlitSpriteLunge");
            behaviorStateMachine.ChangeNavigationState(new NavigationState_DoNothing(this, 0));
            transform.rotation = Quaternion.LookRotation((target.transform.position - transform.position).normalized, Vector3.up);
            gameObject.GetComponent<Entity>().AddForce(new Force(transform.forward, 80f, -45f));
            SayTheLine(1);
            spriteBase.transform.position += Vector3.up * 2.5f;
            lunging = true;
            cooldown = 2f;
            Invoke(nameof(Land), 1.25f);
        }
    }

    private void Land()
    {
        if (lunging)
        {
            spriteRenderer[0].sprite = TestPlugin.Instance.assetMan.Get<Sprite>("PlitSpriteMain");
            lunging = false;
            spriteBase.transform.position = transform.position + spriteOffset;
            behaviorStateMachine.ChangeNavigationState(new NavigationState_WanderRandom(this, 0));
        }
    }
}
