using TestVariants.Behaviours.Characters;
using UnityEngine;

namespace TestVariants.Models.StateMachine;

public class Canny_Uncanned(Canny canny) : Canny_StateBase(canny)
{
    private Transform closest;

    private MovementModifier movemoda;

    private MovementModifier movemodb;

    private bool pushing = false;

    private float timer = 0f;

    public override void Enter()
    {
        base.Enter();

        npc.Navigator.SetSpeed(30f);
        npc.Navigator.maxSpeed = 30f;
        closest = npc.transform;

        ChangeNavigationState(new NavigationState_TargetPosition(npc, 74, closest.position));
    }

    public override void Update()
    {
        base.Update();

        float distance = 99999f;
        closest = null;

        foreach (NPC npc in npc.ec.Npcs)
        {
            if (npc.transform != this.npc.transform && (this.npc.transform.position - npc.transform.position).magnitude <= distance)
            {
                distance = (this.npc.transform.position - npc.transform.position).magnitude;
                closest = npc.transform;
            }
        }

        if (closest != null && !pushing)
        {
            currentNavigationState.UpdatePosition(closest.position);
        }

        if ((npc.transform.position - closest.position).magnitude <= 5f & !pushing)
        {
            pushing = true;
            npc.spriteRenderer[0].sprite = TestPlugin.Instance.assetMan.Get<Sprite>("CannedTestNPCSplat");
            npc.spriteRenderer[0].transform.localScale *= 2f;
            movemoda = new MovementModifier(npc.transform.forward * 32f, 0f);
            movemodb = new MovementModifier(npc.transform.forward * 32f, 0f);
            npc.gameObject.GetComponent<ActivityModifier>().moveMods.Add(movemoda);
            closest.gameObject.GetComponent<ActivityModifier>().moveMods.Add(movemodb);
            npc.PlaySound();
            ChangeNavigationState(new NavigationState_DoNothing(npc, 74));
        }

        if (pushing)
        {
            timer += Time.deltaTime;

            if (timer >= 7f)
            {
                timer = -99f;

                if (npc.Navigator.Entity.ExternalActivity.moveMods.Contains(movemoda))
                    npc.Navigator.Entity.ExternalActivity.moveMods.Remove(movemoda);

                if (closest.TryGetComponent(out ActivityModifier activityMod) && activityMod.moveMods.Contains(movemodb))
                    activityMod.moveMods.Remove(movemodb);

                npc.Despawn();
            }
        }
    }
}
