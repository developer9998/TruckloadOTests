using System.Collections.Generic;
using TestVariants.Models.StateMachine;
using UnityEngine;

namespace TestVariants.Behaviours.Characters;

public class MeMest : NPC, IEntityTrigger
{
    private BoxCollider col;

    private AudioManager audMan;

    private Vector3 blockPos = Vector3.zero;

    private bool db = false;

    private bool blocking = false;

    public override void Initialize()
    {
        base.Initialize();

        spriteRenderer[0].sprite = TestPlugin.Instance.assetMan.Get<Sprite>("MeMestMain");
        spriteRenderer[0].transform.position += Vector3.down * spriteRenderer[0].transform.position.y + Vector3.up * 3f;
        spriteRenderer[0].transform.localScale *= 6f;

        audMan = GetComponent<PropagatedAudioManager>();
        audMan.overrideSubtitleColor = false;

        navigator.SetRoomAvoidance(true);
        navigator.SetSpeed(20f);
        navigator.maxSpeed = 20f;

        behaviorStateMachine.ChangeState(new MeMest_Wander(this));

        col = gameObject.AddComponent<BoxCollider>();
        col.center = Vector3.down * 4f;
        col.size = new Vector3(10f, 2f, 10f);
        col.enabled = false;
    }

    public void SayLine(int c)
    {
        switch (c)
        {
            case 0:
                audMan.PlaySingle(TestPlugin.Instance.assetMan.Get<SoundObject>("MeMest_CannotCross"));
                break;
            case 1:
                audMan.PlaySingle(TestPlugin.Instance.assetMan.Get<SoundObject>("MeMest_Pushed"));
                break;
            case 2:
                audMan.PlaySingle(TestPlugin.Instance.assetMan.Get<SoundObject>("MeMest_Stop"));
                break;
            case 3:
                audMan.PlaySingle(TestPlugin.Instance.assetMan.Get<SoundObject>("MeMest_WTH"));
                break;
            case 4:
                audMan.PlaySingle(TestPlugin.Instance.assetMan.Get<SoundObject>("MeMest_Scream"));
                break;
        }
    }

    public void Block()
    {
        spriteRenderer[0].sprite = TestPlugin.Instance.assetMan.Get<Sprite>("MeMestStop");
        SayLine(2);
        if (!blocking)
        {
            TestPlugin.Instance.Logger.LogInfo("Me Mest is blocking a cell");

            blockPos = transform.position;
            Cell cell = ec.CellFromPosition(transform.position);
            transform.position = cell.CenterWorldPosition;
            for (int i = 0; i < 4; i++)
            {
                Direction direction = (Direction)i;
                if (cell.ConstNavigable(direction)) ec.CellFromPosition(cell.position + direction.ToIntVector2()).Block(direction.GetOpposite(), true);
            }
            col.enabled = true;
            spriteRenderer[0].sprite = TestPlugin.Instance.assetMan.Get<Sprite>("MeMestStop");
            blocking = true;
        }
    }

    public void StopBlock()
    {
        if (blocking)
        {
            TestPlugin.Instance.Logger.LogInfo("Me Mest is removing his previous block on a cell");

            Cell cell = ec.CellFromPosition(transform.position);
            transform.position = cell.CenterWorldPosition;
            for (int i = 0; i < 4; i++)
            {
                Direction direction = (Direction)i;
                if (cell.ConstNavigable(direction)) ec.CellFromPosition(cell.position + direction.ToIntVector2()).Block(direction.GetOpposite(), false);
            }
            col.enabled = false;
            blocking = false;
        }
    }

    private void Idle()
    {
        spriteRenderer[0].sprite = TestPlugin.Instance.assetMan.Get<Sprite>("MeMestMain");
    }

    private void ScreamWrench()
    {
        spriteRenderer[0].sprite = TestPlugin.Instance.assetMan.Get<Sprite>("MeMestShove");
        SayLine(3);
    }

    public override void VirtualUpdate()
    {
        base.VirtualUpdate();

        if (blocking && (transform.position - blockPos).magnitude > 10f)
        {
            if (gameObject.GetComponent<Entity>().Velocity.magnitude >= 100f * Time.deltaTime)
            {
                spriteRenderer[0].sprite = TestPlugin.Instance.assetMan.Get<Sprite>("MeMestWrench");
                SayLine(4);
                Invoke(nameof(ScreamWrench), 1.5f);
                Invoke(nameof(Idle), 5f);
            }
            else
            {
                spriteRenderer[0].sprite = TestPlugin.Instance.assetMan.Get<Sprite>("MeMestShove");
                SayLine(1);
                Invoke(nameof(Idle), 5f);
            }
            StopBlock();
            behaviorStateMachine.ChangeNavigationState(new NavigationState_WanderRandom(this, 0));
        }

        if (blocking && (transform.position - ec.Players[0].transform.position).magnitude <= 8f)
        {
            if (!db)
            {
                db = true;
                SayLine(0);
            }
            return;
        }

        db = false;
    }
}
