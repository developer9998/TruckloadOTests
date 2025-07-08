using System.Collections.Generic;
using TestVariants.Models.StateMachine;
using UnityEngine;

namespace TestVariants.Behaviours.Characters;

public class Bluxam : NPC, IEntityTrigger
{
    private AudioManager audMan;

    public override void Initialize()
    {
        base.Initialize();

        spriteRenderer[0].sprite = TestPlugin.Instance.assetMan.Get<Sprite>("Bluxam");
        spriteRenderer[0].transform.position += Vector3.down * spriteRenderer[0].transform.position.y + Vector3.up * 5f;
        spriteRenderer[0].transform.localScale *= 10f;

        audMan = GetComponent<PropagatedAudioManager>();
        audMan.overrideSubtitleColor = false;

        navigator.SetRoomAvoidance(val: false);
        navigator.SetSpeed(18f);
        navigator.maxSpeed = 18f;

        behaviorStateMachine.ChangeState(new Bluxam_Wander(this));
    }

    public void SayTheLine(int cat)
    {
        switch(cat)
        {
            case 1:
                audMan.PlaySingle(TestPlugin.Instance.assetMan.Get<SoundObject>(Random.Range(0, 2) == 0 ? "Bluxam_GonnaBeLate" : "Bluxam_MindIfHelp"));
                break;
            case 2:
                audMan.PlaySingle(TestPlugin.Instance.assetMan.Get<SoundObject>(Random.Range(0, 2) == 0 ? "Bluxam_OhOk" : "Bluxam_Nevermind"));
                break;
            case 3:
                audMan.PlaySingle(TestPlugin.Instance.assetMan.Get<SoundObject>("Bluxam_HereWeAre"));
                break;
            case 4:
                audMan.PlaySingle(TestPlugin.Instance.assetMan.Get<SoundObject>("Bluxam_ThankLater"));
                break;
        }
    }

    public void HelpPlayer(PlayerManager player)
    {
        TestPlugin.Instance.Logger.LogInfo("Bluxam now assisting player");

        List<Vector3> list = [];

        foreach (RoomController room in ec.rooms)
        {
            if (room.category == RoomCategory.Class)
            {
                list.Add(room.doors[0].aTile.CenterWorldPosition);
            }
        }

        if (list.Count > 0)
        {
            Vector3 classPosition = list[Random.Range(0, list.Count)];
            player.Teleport(classPosition);
            transform.position = classPosition;
        }
    }
}
