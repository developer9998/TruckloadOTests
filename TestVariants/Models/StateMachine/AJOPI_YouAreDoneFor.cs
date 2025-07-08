using TestVariants.Behaviours.Characters;
using UnityEngine;

namespace TestVariants.Models.StateMachine;

public class AJOPI_YouAreDoneFor(AJOPI ajopi) : AJOPI_StateBase(ajopi)
{
    private float speed = 0.1f;

    private bool colorSwapped = false;

    private bool didThing = false;

    public override void Enter()
    {
        base.Enter();

        speed = 0.1f;
        npc.Navigator.SetSpeed(speed);
        npc.Navigator.maxSpeed = speed;

        ChangeNavigationState(new NavigationState_TargetPlayer(npc, 74, npc.ec.Players[0].transform.position));

        for (int i = 0; i < 5; i++)
        {
            npc.ec.FlickerLights(true);
        }
    }

    public override void OnStateTriggerEnter(Collider other)
    {
        base.OnStateTriggerEnter(other);

        if (npc.Navigator.passableObstacles.Contains(PassableObstacle.Window) && other.CompareTag("Window")) other.GetComponent<Window>().Break(false);
    }

    public override void Update()
    {
        base.Update();

        ChangeNavigationState(new NavigationState_TargetPlayer(npc, 74, npc.ec.Players[0].transform.position));

        speed += Time.deltaTime;
        npc.Navigator.SetSpeed(speed);
        npc.Navigator.maxSpeed = speed;

        if (speed >= 40f & !colorSwapped)
        {
            colorSwapped = true;
            npc.BecomeCoolRed();
        }

        if ((npc.transform.position - npc.ec.Players[0].transform.position).magnitude <= 5f & !didThing)
        {
            didThing = true;
            npc.Blind();
            for (int i = 0; i < 5; i++)
            {
                npc.ec.FlickerLights(false);
            }
        }

        if (!npc.Navigator.passableObstacles.Contains(PassableObstacle.Window))
        {
            npc.Navigator.passableObstacles.Add(PassableObstacle.Window);
            npc.Navigator.CheckPath();
        }
    }
}
