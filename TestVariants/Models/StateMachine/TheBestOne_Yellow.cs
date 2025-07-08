using TestVariants.Behaviours;
using TestVariants.Behaviours.Characters;
using UnityEngine;

namespace TestVariants.Models.StateMachine;

public class TheBestOne_Yellow(TheBestOne thebestone) : TheBestOne_StateBase(thebestone)
{
    private float yellowTimer = 0f;

    private string urine = "Urine";

    public override void Enter()
    {
        base.Enter();

        npc.Navigator.SetSpeed(0f);
        npc.Navigator.maxSpeed = 0f;
        ChangeNavigationState(new NavigationState_DoNothing(npc, 0));

        yellowTimer = 0f;

        if (Random.Range(0, 20) == 0) urine = "UrineRed";
    }

    public override void Update()
    {
        base.Update();
        npc.transform.LookAt(new Vector3(npc.yellower.transform.position.x, npc.transform.position.y, npc.yellower.transform.position.z));

        yellowTimer += -Time.deltaTime;
        if (yellowTimer <= 0f)
        {
            yellowTimer = 0.1f;
            GameObject gameObject = Object.Instantiate(npc.spriteRenderer[0].gameObject, npc.spriteRenderer[0].transform.position - Vector3.up * 2.5f, npc.transform.rotation);
            SpriteRenderer component = gameObject.GetComponent<SpriteRenderer>();
            component.sprite = TestPlugin.Instance.assetMan.Get<Sprite>(urine);
            component.SetSpriteRotation(Random.Range(0, 360));
            component.transform.localScale /= 9f;
            gameObject.AddComponent<YellowParticle>();
            gameObject.transform.SetParent(null);
            Object.Destroy(gameObject, 1.5f);
        }

        if ((npc.transform.position - npc.yellower.transform.position).magnitude >= 25f) npc.QuitYellowin();
    }
}
