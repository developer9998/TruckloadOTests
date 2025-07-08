using HarmonyLib;
using TestVariants.Behaviours;
using UnityEngine;

namespace TestVariants.Patches
{
    [HarmonyPatch(typeof(RoomFunction))]
    internal class StoreRoomFunctionPatch
    {
        [HarmonyPatch(nameof(RoomFunction.OnPlayerEnter)), HarmonyPostfix]
        private static void PlayerEnterPatch()
        {
            ItemManager itemManager = CoreGameManager.Instance?.GetPlayer(0)?.itm ?? Object.FindObjectOfType<ItemManager>();
            itemManager.disables = 0;
            itemManager.disabled = false;
        }

        [HarmonyPatch(nameof(RoomFunction.OnGenerationFinished)), HarmonyPostfix]
        private static void GenerationFinishPatch(ref RoomController ___room)
        {
            if (___room.ec.notebooks.Count == 0 & ___room.transform.Find("TestFolderPoster") == null & ___room.functionObject.GetComponent<StoreRoomFunction>())
            {
                Cell cell = ___room.ec.CellFromPosition(new Vector3(302.8242f, 5f, 103.5992f));
                ItemObject item = TestPlugin.Instance.assetMan.Get<ItemObject>("TestFolder");
                Pickup pickup = ___room.ec.CreateItem(___room.ec.rooms[0], item, Vector2.zero);

                pickup.transform.position = cell.CenterWorldPosition;
                pickup.price = 100;
                pickup.showDescription = true;
                pickup.free = false;

                GameObject poster = new("TestFolderPoster");
                TestFolderShopThing testFolderShopThing = poster.AddComponent<TestFolderShopThing>();
                pickup.OnItemPurchased += testFolderShopThing.OnItemPurchased;
                pickup.OnItemDenied += testFolderShopThing.OnItemDenied;
                pickup.OnItemCollected += testFolderShopThing.OnItemCollected;
                poster.AddComponent<SpriteRenderer>().sprite = TestPlugin.Instance.assetMan.Get<Sprite>("TestFolderPoster");
                poster.transform.localScale *= 10f;
                poster.transform.position = cell.CenterWorldPosition + new Vector3(-4.9f, 0f, 10f);
                poster.transform.LookAt(cell.CenterWorldPosition + new Vector3(0f, 0f, 10f));
                poster.GetComponent<SpriteRenderer>().flipX = true;
                poster.transform.SetParent(___room.transform);

                GameObject vase = new("TestFolderVase");
                vase.AddComponent<SpriteRenderer>().sprite = TestPlugin.Instance.assetMan.Get<Sprite>("vase");
                vase.transform.position = new Vector3(304.75f, -4.5f, 105.25f);
                vase.GetComponent<SpriteRenderer>().material = TestPlugin.FindResourceOfName<Material>("SpriteStandard_Billboard", null);
                vase.transform.SetParent(___room.transform);
                vase.gameObject.layer = 9;
            }
        }
    }
}
