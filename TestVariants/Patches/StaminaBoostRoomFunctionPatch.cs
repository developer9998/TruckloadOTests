using HarmonyLib;
using UnityEngine;
using Random = System.Random;

namespace TestVariants.Patches;

[HarmonyPatch(typeof(RoomFunction), nameof(RoomFunction.OnGenerationFinished)), HarmonyWrapSafe, HarmonyPriority(Priority.LowerThanNormal)]
internal class StaminaBoostRoomFunctionPatch
{
    private static void Postfix(ref RoomController ___room)
    {
        Random random = new(Singleton<CoreGameManager>.Instance.Seed());

        if (___room.functionObject.GetComponent<StaminaBoostRoomFunction>() || ___room.functionObject.GetComponent<FieldTripBaseRoomFunction>() & random.Next(0, 20) == 0 & ___room.transform.Find("Really Evil Bubblerum") == null)
        {
            GameObject evilBubblerum = new("Really Evil Bubblerum");
            GameObject spriteObject = new("Sprite");
            spriteObject.transform.parent = evilBubblerum.transform;
            spriteObject.transform.position = evilBubblerum.transform.position;
            spriteObject.AddComponent<SpriteRenderer>();
            SpriteRenderer spriteRenderer = spriteObject.GetComponent<SpriteRenderer>();
            spriteRenderer.material = TestPlugin.FindResourceOfName<Material>("SpriteStandard_Billboard", null);
            spriteRenderer.sprite = TestPlugin.Instance.assetMan.Get<Sprite>("ReallyEvilBubblerum");
            spriteRenderer.gameObject.layer = 9;
            Vector3 position = ___room.ec.RealRoomMid(___room) + Vector3.up * 60f;
            evilBubblerum.transform.position = position;
            evilBubblerum.transform.SetParent(___room.transform);
        }

        if (___room.functionObject.GetComponent<StaminaBoostRoomFunction>() & ___room.transform.Find("HeadplumTree") == null && random.Next(0, 100) < 25)
        {
            GameObject headplumTree = new("HeadplumTree");
            GameObject spriteObject = new("Sprite");
            spriteObject.transform.parent = headplumTree.transform;
            spriteObject.transform.position = headplumTree.transform.position;
            spriteObject.AddComponent<SpriteRenderer>();
            SpriteRenderer spriteRenderer = spriteObject.GetComponent<SpriteRenderer>();
            spriteRenderer.material = TestPlugin.FindResourceOfName<Material>("SpriteStandard_Billboard", null);
            spriteRenderer.sprite = TestPlugin.Instance.assetMan.Get<Sprite>("HeadplumTree");
            spriteRenderer.gameObject.layer = 9;
            int index = random.Next(0, ___room.TileCount);
            Cell cell = ___room.cells[index];
            Vector3 floorWorldPosition = cell.FloorWorldPosition;
            headplumTree.transform.position = floorWorldPosition + Vector3.up * 10f;
            headplumTree.AddComponent<CapsuleCollider>();
            headplumTree.GetComponent<CapsuleCollider>().radius = 3f;
            headplumTree.GetComponent<CapsuleCollider>().height = 25f;
            headplumTree.AddComponent<HeadplumTree>();
            headplumTree.transform.SetParent(___room.transform);
        }
    }
}
