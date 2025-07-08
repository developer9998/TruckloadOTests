using System.Collections.Generic;
using UnityEngine;

namespace TestVariants.Behaviours.Items;

public class ITM_LuckiestCoin : Item
{
    public override bool Use(PlayerManager pm)
    {
        Vector3 point = pm.transform.position + Singleton<CoreGameManager>.Instance.GetCamera(pm.playerNumber).transform.forward * pm.pc.reach;
        if (Physics.Raycast(pm.transform.position, Singleton<CoreGameManager>.Instance.GetCamera(pm.playerNumber).transform.forward, out RaycastHit raycastHit, pm.pc.reach, pm.pc.ClickLayers)) point = raycastHit.point;

        bool hasAppliedDiscount = false;

        if (FindObjectOfType<StoreRoomFunction>() is StoreRoomFunction storeRoomFunction && storeRoomFunction)
        {
            List<Pickup> pickups = storeRoomFunction.pickups;
            PriceTag[] priceTags = storeRoomFunction.tag;

            foreach (Pickup pickup in FindObjectsOfType<Pickup>())
            {
                if ((pickup.transform.position - point).magnitude <= 3f & pickups.Contains(pickup) & !hasAppliedDiscount)
                {
                    TestPlugin.Instance.Logger.LogInfo($"Luckiest Coin applying discount to pickup {pickup.gameObject.name}");

                    int pickupPrice = pickup.price;
                    float discountToApply = Random.Range(storeRoomFunction.minSaleDiscount, storeRoomFunction.maxSaleDiscount);
                    float reduction = pickupPrice * discountToApply;
                    pickupPrice = Mathf.RoundToInt(reduction - reduction % 10f);

                    priceTags[pickups.IndexOf(pickup)].SetSale(pickup.price, pickupPrice);
                    pickup.price = pickupPrice;
                    hasAppliedDiscount = true;
                    Singleton<CoreGameManager>.Instance.audMan.PlaySingle(TestPlugin.Instance.assetMan.Get<SoundObject>("Lucky"));

                    break;
                }
            }
        }

        return hasAppliedDiscount;
    }
}
