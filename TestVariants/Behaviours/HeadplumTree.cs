using TestVariants;
using TestVariants.Behaviours.Items;
using UnityEngine;

public class HeadplumTree : MonoBehaviour
{
    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("GrapplingHook"))
        {
            Vector2 insideUnitCircle;

            for (int i = 0; i < Random.Range(2, 5); i++)
            {
                ITM_Headplum itm_Headplum = Instantiate((ITM_Headplum)TestPlugin.Instance.assetMan.Get<ItemObject>("Headplum").item);
                insideUnitCircle = Random.insideUnitCircle;
                itm_Headplum.Prop(Singleton<BaseGameManager>.Instance.Ec, transform.position - 5f * Vector3.down + new Vector3(insideUnitCircle.x, 0f, insideUnitCircle.y) * Random.Range(5f, 15f));
            }

            Pickup pickup = Singleton<BaseGameManager>.Instance.Ec.CreateItem(Singleton<BaseGameManager>.Instance.Ec.rooms[0], TestPlugin.Instance.assetMan.Get<ItemObject>("Headplum"), Vector2.zero);
            insideUnitCircle = Random.insideUnitCircle;
            pickup.transform.position = transform.position - 5f * Vector3.up + new Vector3(insideUnitCircle.x, 0f, insideUnitCircle.y) * Random.Range(5f, 15f);
        }
    }
}
