using System.Collections.Generic;
using System.Linq;
using TestVariants.Behaviours;
using TestVariants.Behaviours.Characters;
using UnityEngine;

namespace TestVariants.Models.StateMachine;

public class DaSketch_Hide(DaSketch sketch) : DaSketch_StateBase(sketch)
{
    private Transform myPaper;

    private Sprite olSprite;

    private float timer = 0f;

    public override void Enter()
    {
        base.Enter();

        npc.Navigator.SetSpeed(40f);
        npc.Navigator.maxSpeed = 40f;

        ChangeNavigationState(new NavigationState_DoNothing(npc, 0));

        SpriteRenderer[] spriteRenderers = Object.FindObjectsOfType<SpriteRenderer>();

        if (spriteRenderers == null || spriteRenderers.Length == 0)
        {
            npc.Despawn();
            return;
        }

        List<Transform> transforms = [.. spriteRenderers
            .Select(renderer => renderer.gameObject)
            .Where(gameObject => gameObject.name == "Decor_Papers(Clone)" || gameObject.name == "Decor_Notebooks(Clone)")
            .Select(gameObject => gameObject.transform)];

        if (transforms.Count == 0)
        {
            npc.Despawn();
            return;
        }

        myPaper = transforms[Random.Range(0, transforms.Count)];
        olSprite = myPaper.gameObject.GetComponent<SpriteRenderer>().sprite;
        myPaper.gameObject.GetComponent<SpriteRenderer>().sprite = TestPlugin.Instance.assetMan.Get<Sprite>("DaSketch_PaperSpawn_0");
        npc.transform.position = myPaper.transform.position;
        npc.spriteRenderer[0].sprite = null;
        npc.gameObject.GetComponent<Entity>().SetFrozen(value: true);
    }

    public override void Update()
    {
        base.Update();

        if (npc.ec.CellFromPosition(npc.ec.Players[0].transform.position).room == npc.ec.CellFromPosition(npc.transform.position).room)
        {
            timer += Time.deltaTime * npc.TimeScale;
            if (timer >= 8f)
            {
                timer = -99f;
                myPaper.gameObject.GetComponent<SpriteRenderer>().sprite = olSprite;
                myPaper.gameObject.AddComponent<ParticleScript>();
                Object.Destroy(myPaper.gameObject.GetComponent<ParticleScript>(), 5f);
                npc.PopOut();
            }
            return;
        }

        timer += (0f - Time.deltaTime) * npc.TimeScale;
        if (timer <= 0f & timer >= -50f)
        {
            timer = 0f;
        }
    }
}
