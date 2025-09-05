using System.ComponentModel;
using TestVariants.Behaviours.Characters;
using UnityEngine;

namespace TestVariants.Models.StateMachine;

public class ThroneTest_Wander(ThroneTest ThroneTest) : ThroneTest_StateBase(ThroneTest)
{
    private int combo = 0;

    private float combotimer = 0f;

    private float timer = 0f;

    public override void Enter()
    {
        base.Enter();

        npc.Navigator.SetSpeed(5f);
        npc.Navigator.maxSpeed = 100f;

        ChangeNavigationState(new NavigationState_WanderRandom(npc, 0));

        timer = 0f;
    }

    public static SpriteRenderer CreateSpriteRender(string name, bool billboarded, Transform baseObject)
    {
        GameObject spriteObject = new(name);
        spriteObject.transform.parent = baseObject;
        spriteObject.transform.position = baseObject.position;
        spriteObject.AddComponent<SpriteRenderer>();
        SpriteRenderer component = spriteObject.GetComponent<SpriteRenderer>();
        component.material = TestPlugin.FindResourceOfName<Material>(billboarded ? "SpriteStandard_Billboard" : "SpriteWithFog_Forward_NoBillboard", null);
        return component;
    }

    public override void Update()
    {
        base.Update();

        float angle = npc.Navigator.NextPoint != npc.transform.position
            ? Mathf.DeltaAngle(npc.transform.eulerAngles.y, Mathf.Atan2(npc.Navigator.NextPoint.x - npc.transform.position.x, npc.Navigator.NextPoint.z - npc.transform.position.z) * 57.29578f)
            : 0f;

        if (Mathf.Abs(angle) >= 45f & npc.Navigator.speed >= 20f)
        {
            if (Physics.Raycast(npc.transform.position, npc.transform.forward, out RaycastHit hit, 7.5f, 2097152, QueryTriggerInteraction.Collide) && hit.transform.CompareTag("Window"))
            {
                hit.transform.GetComponent<Window>().Break(true);
            }

            foreach (Window window in Object.FindObjectsOfType<Window>())
            {
                if ((window.transform.position - npc.transform.position).magnitude <= 7.5f) window.Break(false);
            }

            npc.Navigator.maxSpeed = 100f;
            npc.Navigator.SetSpeed(5f);
            npc.SayLine(3, 0);

            GameObject hitMarker = new();
            hitMarker.transform.position = npc.transform.position + npc.transform.forward * 3.5f;
            SpriteRenderer spriteRenderer = CreateSpriteRender("Zap!", true, hitMarker.transform);
            spriteRenderer.sprite = TestPlugin.Instance.assetMan.Get<Sprite>("ThroneTest_Hitmarker");
            spriteRenderer.transform.localScale *= 25f;
            Object.Destroy(hitMarker, 1f);
        }

        timer -= Time.deltaTime;
        combotimer -= Time.deltaTime;

        if (combotimer <= 0f)
        {
            if (combo > 2)
            {
                npc.Shutit();
                npc.SayLine(7, 0);
            }
            combo = 0;
        }

        if (timer <= 0f)
        {
            timer = 20f;
            npc.SayLine(0, Random.Range(1, 5));
        }
    }

    public override void OnStateTriggerEnter(Collider other, bool validCollision)
    {
        base.OnStateTriggerEnter(other, validCollision);

        if (validCollision && other.TryGetComponent(out Entity entity) & npc.Navigator.speed >= 20f && !entity.squished)
        {
            npc.SetGuilty();
            GameObject gameObject = new();
            gameObject.transform.position = npc.transform.position + npc.transform.forward * 3.5f;
            SpriteRenderer spriteRenderer = CreateSpriteRender("Zap!", true, gameObject.transform);
            spriteRenderer.sprite = TestPlugin.Instance.assetMan.Get<Sprite>("ThroneTest_Hitmarker");
            spriteRenderer.transform.localScale *= 25f;
            Object.Destroy(gameObject, 1f);
            entity.Squish(15f);

            if (combo < 5)
            {
                if (combo > 0)
                {
                    npc.SayLine(4, combo);
                    if (combo == 4) npc.SayLine(5, 0);
                }
                else npc.SayLine(2, 0);
            }
            else if (combo == 5)
            {
                npc.Shutit();
                npc.SayLine(6, 0);
            }
            else npc.SayLine(4, Random.Range(1, 5));

            npc.SayLine(1, Random.Range(1, 5));
            combo++;
            combotimer = 10f;
        }
    }
}
