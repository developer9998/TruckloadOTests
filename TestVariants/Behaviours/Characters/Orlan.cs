using TestVariants.Models.StateMachine;
using UnityEngine;

namespace TestVariants.Behaviours.Characters;

public class Orlan : NPC, IEntityTrigger
{
    public bool stealing = false;

    public bool rich = false;

    private AudioManager audMan;

    public PlayerManager stealingFrom;

    public override void Initialize()
    {
        base.Initialize();

        spriteRenderer[0].sprite = TestPlugin.Instance.assetMan.Get<Sprite>("Orlan");
        spriteRenderer[0].transform.position += Vector3.down * spriteRenderer[0].transform.position.y + Vector3.up * 5f;
        spriteRenderer[0].transform.localScale *= 10f;

        audMan = GetComponent<PropagatedAudioManager>();
        audMan.overrideSubtitleColor = false;

        navigator.SetRoomAvoidance(false);
        navigator.SetSpeed(18f);
        navigator.maxSpeed = 18f;

        behaviorStateMachine.ChangeState(new Orlan_Wander(this));
    }

    public void SayTheLine()
    {
        switch (Random.Range(0, 3))
        {
            case 0:
                audMan.PlaySingle(TestPlugin.Instance.assetMan.Get<SoundObject>("Orlan7_Laugh"));
                break;
            case 1:
                audMan.PlaySingle(TestPlugin.Instance.assetMan.Get<SoundObject>("Orlan7_AintGettingBack"));
                break;
            default:
                audMan.PlaySingle(TestPlugin.Instance.assetMan.Get<SoundObject>("Orlan7_BelongToMe"));
                break;
        }
    }

    public void Scurry()
    {
        behaviorStateMachine.ChangeState(new Orlan_Scurry(this));

        if (stealing) SayTheLine();
        stealing = false;
    }

    public void BeginStealing()
    {
        if (Singleton<CoreGameManager>.Instance.GetPoints(stealingFrom.playerNumber) < 50)
        {
            audMan.PlaySingle(TestPlugin.Instance.assetMan.Get<SoundObject>("Orlan7_Poor"));
            behaviorStateMachine.ChangeState(new Orlan_Scurry(this));
            return;
        }

        rich = Singleton<CoreGameManager>.Instance.GetPoints(stealingFrom.playerNumber) >= 750;
        if (!stealing)
        {
            stealing = true;
            Steal();
        }
    }

    private void Steal()
    {
        if (stealing)
        {
            TestPlugin.Instance.Logger.LogInfo("Orlan stealing from player");

            if (Singleton<CoreGameManager>.Instance.GetPoints(stealingFrom.playerNumber) < 50 & !rich)
            {
                TestPlugin.Instance.Logger.LogInfo("Orlan scurrying, player has 50 or less points");
                Scurry();
                return;
            }

            if ((stealingFrom.transform.position - transform.position).magnitude >= 10f)
            {
                TestPlugin.Instance.Logger.LogInfo("Orlan out of range from player");
                stealing = false;
                return;
            }

            audMan.PlaySingle(TestPlugin.Instance.assetMan.Get<SoundObject>("Orlan7_Snatch"));
            Singleton<CoreGameManager>.Instance.AddPoints(-50, stealingFrom.playerNumber, true);
            TestPlugin.Instance.Logger.LogInfo("Orlan stole 50 points");
            Invoke(nameof(Steal), 0.3f);
        }
    }
}
