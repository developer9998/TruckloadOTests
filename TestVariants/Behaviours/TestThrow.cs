using UnityEngine;

namespace TestVariants.Behaviours;

public class TestThrow : MonoBehaviour
{
    public EnvironmentController ec;

    public void Start()
    {
        Destroy(this, 3f);
    }

    public void Update()
    {
        foreach (NPC npc in ec.Npcs)
        {
            if ((npc.transform.position - transform.position).magnitude <= 5f & npc.transform.gameObject != transform.gameObject && !npc.transform.gameObject.GetComponent<TestThrow>())
            {
                npc.gameObject.GetComponent<Entity>().AddForce(new Force(Singleton<CoreGameManager>.Instance.GetCamera(ec.Players[0].playerNumber).camCom.transform.forward, 70f, -50f));
                npc.gameObject.AddComponent<TestThrow>().ec = ec;
            }
        }
    }
}
