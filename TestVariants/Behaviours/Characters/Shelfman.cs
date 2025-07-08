using System.Collections;
using TestVariants.Models.StateMachine;
using UnityEngine;
using Random = UnityEngine.Random;

namespace TestVariants.Behaviours.Characters;

public class Shelfman : NPC, IEntityTrigger
{
    private readonly MovementModifier mainMoveMod = new(Vector3.zero, 0f);

    private MovementModifier meanMoveSelf, meanMoveTestholder;

    private Testholder testholde;

    private bool Meaning = false;

    private AudioManager audMan;

    public PlayerManager asked;

    public override void Initialize()
    {
        base.Initialize();
        spriteRenderer[0].sprite = TestPlugin.Instance.assetMan.Get<Sprite>("Shelfman");
        spriteRenderer[0].transform.position += Vector3.down * spriteRenderer[0].transform.position.y + Vector3.up * 5f;
        spriteRenderer[0].transform.localScale *= 10f;

        audMan = GetComponent<PropagatedAudioManager>();
        audMan.overrideSubtitleColor = false;

        navigator.SetRoomAvoidance(val: false);
        navigator.SetSpeed(25f);
        navigator.maxSpeed = 25f;

        behaviorStateMachine.ChangeState(new Shelfman_Wander(this));
    }

    private IEnumerator TurnPlayer(PlayerManager player, float speed)
    {
        float time = 0.5f;
        player.plm.am.moveMods.Add(mainMoveMod);
        while (time > 0f)
        {
            Vector3 vector = Vector3.RotateTowards(player.transform.forward.ZeroOutY(), (transform.position.ZeroOutY() - player.transform.position.ZeroOutY()).normalized, Time.deltaTime * 2f * 3.1415927f * speed, 0f);
            Debug.DrawRay(player.transform.position, vector, Color.yellow);
            player.transform.rotation = Quaternion.LookRotation(vector, Vector3.up);
            time -= Time.deltaTime;
            yield return null;
        }
        player.plm.am.moveMods.Remove(mainMoveMod);
        yield break;
    }

    public override void VirtualUpdate()
    {
        base.VirtualUpdate();
        foreach (NPC npc in ec.Npcs)
        {
            if (npc.gameObject.TryGetComponent(out Testholder testholder) & (npc.transform.position - transform.position).magnitude <= 5f & !Meaning)
            {
                Meaning = true;
                YouSuck(testholder);
            }
        }
    }

    private void YouSuck(Testholder testholder)
    {
        meanMoveSelf = new MovementModifier(Vector3.zero, 0f);
        meanMoveTestholder = new MovementModifier(Vector3.zero, 0f);
        gameObject.GetComponent<ActivityModifier>().moveMods.Add(meanMoveSelf);
        testholder.gameObject.GetComponent<ActivityModifier>().moveMods.Add(meanMoveTestholder);
        audMan.PlaySingle(TestPlugin.Instance.assetMan.Get<SoundObject>("Shelfman_Testholder"));
        testholder.Sadden();
        testholde = testholder;
        Invoke(nameof(MoveOnFrom), 1.5f);
        Invoke(nameof(Unmeaning), 10f);
    }

    private void MoveOnFrom()
    {
        gameObject.GetComponent<ActivityModifier>().moveMods.Remove(meanMoveSelf);
        testholde.gameObject.GetComponent<ActivityModifier>().moveMods.Remove(meanMoveTestholder);
    }

    private void Unmeaning()
    {
        Meaning = false;
    }

    public void SayTheLine(bool huh)
    {
        audMan.PlaySingle(TestPlugin.Instance.assetMan.Get<SoundObject>(huh ? "Shelfman_Huh" + Random.Range(1, 6) : "Shelfman_Question"));
    }

    public void AskQuestion(PlayerManager asker)
    {
        asked = asker;
        StartCoroutine(TurnPlayer(asked, 1f));
        SayTheLine(huh: false);
        Invoke(nameof(Run), 1.127f);
    }

    private void Run()
    {
        behaviorStateMachine.ChangeState(new Shelfman_Cooldown(this));
    }
}
