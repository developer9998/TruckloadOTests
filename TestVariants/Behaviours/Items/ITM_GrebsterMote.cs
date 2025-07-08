using TestVariants.Behaviours.Characters;
using UnityEngine;

namespace TestVariants.Behaviours.Items;

public class ITM_GrebsterMote : Item
{
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

    public override bool Use(PlayerManager pm)
    {
        Singleton<CoreGameManager>.Instance.audMan.PlaySingle(TestPlugin.Instance.assetMan.Get<SoundObject>("Grebstermote_Use"));
        Vector3 point = pm.transform.position + Singleton<CoreGameManager>.Instance.GetCamera(pm.playerNumber).transform.forward * 100f;
        if (Physics.Raycast(pm.transform.position, Singleton<CoreGameManager>.Instance.GetCamera(pm.playerNumber).transform.forward, out RaycastHit raycastHit, 9999f, pm.pc.ClickLayers)) point = raycastHit.point;

        GameObject zap = new();
        zap.transform.position = point;
        SpriteRenderer spriteRenderer = CreateSpriteRender("Zap!", true, zap.transform);
        spriteRenderer.sprite = TestPlugin.Instance.assetMan.Get<Sprite>("ZAP");
        Destroy(zap, 1f);

        foreach (Plit plit in FindObjectsOfType<Plit>())
        {
            bool inRange = (plit.transform.position - pm.transform.position).magnitude <= 10f & plit.mauling != null;
            if (inRange)
            {
                TestPlugin.Instance.Logger.LogInfo("Grebstermote interaction with Plit (the lunge and maul one)");
                plit.Stun();
                return false;
            }
        }

        bool hasUsed = false;

        foreach (Pickup pickup in FindObjectsOfType<Pickup>())
        {
            bool inRange = (pickup.transform.position - point).magnitude <= 3f;
            if (inRange)
            {
                TestPlugin.Instance.Logger.LogInfo($"Grebstermote interaction with pickup {pickup.gameObject.name}");

                if (pm.itm.InventoryFull())
                {
                    pm.itm.SetItem(pm.itm.nothing, pm.itm.selectedItem);
                    pickup.Clicked(pm.playerNumber);
                    return false;
                }

                pickup.Clicked(pm.playerNumber);
                hasUsed = true;
            }
        }

        foreach (GameButtonBase gameButtonBase in FindObjectsOfType<GameButtonBase>())
        {
            bool inRange = (gameButtonBase.transform.position - point).magnitude <= 10f;
            if (inRange)
            {
                TestPlugin.Instance.Logger.LogInfo($"Grebstermote interaction with button {gameButtonBase.gameObject.name}");
                gameButtonBase.Clicked(pm.playerNumber);
                hasUsed = true;
            }
        }

        foreach (Notebook notebook in FindObjectsOfType<Notebook>())
        {
            bool inRange = (notebook.transform.position - point).magnitude <= 3f & notebook.icon.spriteRenderer.enabled;
            if (inRange)
            {
                TestPlugin.Instance.Logger.LogInfo($"Grebstermote interaction with notebook {notebook.gameObject.name}");
                notebook.Clicked(pm.playerNumber);
                hasUsed = true;
            }
        }

        return hasUsed;
    }
}
