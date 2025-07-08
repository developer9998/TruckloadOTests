using System.Collections.Generic;
using TestVariants.Models.StateMachine;
using UnityEngine;

namespace TestVariants.Behaviours.Characters;

public class Qrid : NPC, IEntityTrigger
{
    public NPC targetedNPC;

    public bool onCooldown = false;

    public AudioManager audMan;

    public Vector3 spriteoffset;

    public override void Initialize()
    {
        base.Initialize();

        spriteRenderer[0].sprite = TestPlugin.Instance.assetMan.Get<Sprite>("Qrid_Docile");
        spriteRenderer[0].transform.position += Vector3.down * spriteRenderer[0].transform.position.y + Vector3.up * 5f;
        spriteRenderer[0].transform.localScale *= 10f;
        spriteoffset = spriteRenderer[0].transform.position - transform.position;

        audMan = GetComponent<PropagatedAudioManager>();
        audMan.audioDevice.gameObject.AddComponent<AudioDistortionFilter>().distortionLevel = 0.5f;
        audMan.overrideSubtitleColor = false;

        navigator.SetRoomAvoidance(val: true);
        navigator.SetSpeed(15f);
        navigator.maxSpeed = 15f;

        behaviorStateMachine.ChangeState(new Qrid_Wander(this));
    }

    public void Befriend()
    {
        audMan.PlaySingle(TestPlugin.Instance.assetMan.Get<SoundObject>("Qrid_Friend" + Random.Range(1, 3)));
        behaviorStateMachine.ChangeState(new Qrid_Friendly(this));
    }

    public void ImMadNow()
    {
        spriteRenderer[0].sprite = TestPlugin.Instance.assetMan.Get<Sprite>("Qrid_Hostile");
        behaviorStateMachine.ChangeState(new Qrid_Angered(this));
        audMan.PlaySingle(TestPlugin.Instance.assetMan.Get<SoundObject>("Qrid_Anger" + Random.Range(1, 4)));
        Invoke(nameof(Rev), 3f);
    }

    public void ImMadAtNPCNow(NPC npc)
    {
        targetedNPC = npc;
        spriteRenderer[0].sprite = TestPlugin.Instance.assetMan.Get<Sprite>("Qrid_Hostile");
        behaviorStateMachine.ChangeState(new Qrid_AngeredNPC(this));
        audMan.PlaySingle(TestPlugin.Instance.assetMan.Get<SoundObject>("Qrid_NPC" + Random.Range(1, 4)));
        Invoke(nameof(Rev), 3f);
    }

    public void StopBeingMad()
    {
        audMan.FlushQueue(true);
        audMan.audioDevice.Stop();
        audMan.PlaySingle(TestPlugin.Instance.assetMan.Get<SoundObject>("Qrid_Leave" + Random.Range(1, 4)));
        onCooldown = true;
        behaviorStateMachine.ChangeState(new Qrid_Wander(this));
        spriteRenderer[0].sprite = TestPlugin.Instance.assetMan.Get<Sprite>("Qrid_Docile");
        Invoke(nameof(NotCool), 90f);
    }

    public void StopBeingMadNPC()
    {
        audMan.FlushQueue(true);
        audMan.audioDevice.Stop();
        audMan.PlaySingle(TestPlugin.Instance.assetMan.Get<SoundObject>("Qrid_NPCLeave" + Random.Range(1, 4)));
        onCooldown = true;
        behaviorStateMachine.ChangeState(new Qrid_Wander(this));
        spriteRenderer[0].sprite = TestPlugin.Instance.assetMan.Get<Sprite>("Qrid_Docile");
        Invoke(nameof(NotCool), 90f);
    }

    private void NotCool()
    {
        onCooldown = false;
    }

    private void Rev()
    {
        audMan.PlaySingle(TestPlugin.Instance.assetMan.Get<SoundObject>("Qrid_Intro"));
        audMan.QueueAudio(TestPlugin.Instance.assetMan.Get<SoundObject>("Qrid_Loop"));
        audMan.SetLoop(val: true);
        Navigator.maxSpeed = 30f;
    }

    public Vector3 TeleportPosition()
    {
        List<IntVector2> list = [];

        foreach (Elevator elevator in ec.elevators)
        {
            if (elevator.IsOpen)
            {
                list.Add(elevator.Door.position);
            }
        }

        if (list.Count <= 0)
        {
            foreach (Elevator elevator in ec.elevators)
            {
                list.Add(elevator.Door.position);
            }
        }

        bool useConstantElement = false;

        List<Cell> path = [];
        IntVector2 position = list[Random.Range(0, list.Count)];
        int pathCount = 0;

        if (list.Count > 0)
        {
            while (!useConstantElement && pathCount < 32)
            {
                ec.FindPath(ec.CellFromPosition(position), ec.mainHall.TileAtIndex(Random.Range(0, ec.mainHall.TileCount)), PathType.Nav, out path, out var success);
                if (success && path.Count > 15)
                {
                    useConstantElement = true;
                    path = [.. path];
                }
                pathCount++;
            }
        }

        return useConstantElement ? path[12].CenterWorldPosition : transform.position;
    }
}
