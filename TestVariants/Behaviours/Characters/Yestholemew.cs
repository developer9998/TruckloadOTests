using MTM101BaldAPI.Reflection;
using System.Collections.Generic;
using System.Linq;
using TestVariants.Models.StateMachine;
using UnityEngine;

namespace TestVariants.Behaviours.Characters;

public class Yestholemew : NPC, IEntityTrigger
{
    private AudioManager audMan;

    public bool ScreamingAnim;

    private Vector3 pos;

    private bool WAITFORENTER;

    public RoomController myHide;

    public bool First = true;

    public int sprt = 1;

    public override void Initialize()
    {
        base.Initialize();

        spriteRenderer[0].sprite = TestPlugin.Instance.assetMan.Get<Sprite>("YestholemewIdle");
        spriteRenderer[0].transform.position += Vector3.down * spriteRenderer[0].transform.position.y + Vector3.up * 5f;
        spriteRenderer[0].transform.localScale *= 10f;

        audMan = GetComponent<PropagatedAudioManager>();
        audMan.ReflectionSetVariable("overrideSubtitleColor", false);

        navigator.SetRoomAvoidance(false);
        navigator.SetSpeed(48f);
        navigator.maxSpeed = 48f;

        behaviorStateMachine.ChangeState(new Yestholemew_Hide(this));

        Invoke(nameof(TestiporterPatch), 0.1f);
        Invoke(nameof(Sprtchange), 0.068f);
    }

    private void TestiporterPatch()
    {
        behaviorStateMachine.ChangeState(new Yestholemew_Hide(this));
    }

    private void Sprtchange()
    {
        sprt++;
        if (sprt == 5) sprt = 1;

        spriteRenderer[0].sprite = TestPlugin.Instance.assetMan.Get<Sprite>(ScreamingAnim ? ("YestholomewScream" + sprt.ToString()) : "YestholomewIdle");
        Invoke(nameof(Sprtchange), 0.068f);
    }

    public void Scare()
    {
        ScreamingAnim = true;

        audMan.PlaySingle(TestPlugin.Instance.assetMan.Get<SoundObject>("Yestholomew_Scream" + Random.Range(1, 6).ToString()));
        Invoke(nameof(RUN), 1f);
        ec.MakeNoise(transform.position, 126);
    }

    public override void VirtualUpdate()
    {
        if (WAITFORENTER & (transform.position - pos).magnitude <= 6f)
        {
            WAITFORENTER = false;

            behaviorStateMachine.ChangeState(new Yestholemew_Hide(this));
            audMan.PlaySingle(TestPlugin.Instance.assetMan.Get<SoundObject>("Yestholomew_LockedInRoom"));
            Invoke(nameof(CalmDown), 30f);
        }
    }

    private void CalmDown()
    {
        ScreamingAnim = false;

        audMan.audioDevice.Stop();
    }

    private void RUN()
    {
        List<RoomController> classes = [.. ec.rooms.Where(room => room.category == RoomCategory.Class)];

        if (classes.Count == 0)
        {
            behaviorStateMachine.ChangeNavigationState(new NavigationState_WanderRandom(this, 74));
            return;
        }

        if (classes.Contains(myHide)) classes.Remove(myHide);

        RoomController randomClass = classes[Random.Range(0, classes.Count)];
        pos = ec.RealRoomMid(randomClass);
        behaviorStateMachine.ChangeNavigationState(new NavigationState_TargetPosition(this, 74, pos));

        WAITFORENTER = true;
    }
}
