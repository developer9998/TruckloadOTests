using HarmonyLib;
using UnityEngine;

namespace TestVariants.Patches;

[HarmonyPatch(typeof(RoomFunction))]
internal class StaminaBoostRoomFunctionPatch
{
    [HarmonyPatch("OnGenerationFinished")]
    [HarmonyPostfix]
    private static void HeadplumTreeGeneration(ref RoomController ___room)
    {
        //IL_0059: Unknown result type (might be due to invalid IL or missing references)
        //IL_005f: Expected O, but got Unknown
        //IL_0064: Unknown result type (might be due to invalid IL or missing references)
        //IL_006a: Expected O, but got Unknown
        //IL_0088: Unknown result type (might be due to invalid IL or missing references)
        //IL_00ee: Unknown result type (might be due to invalid IL or missing references)
        //IL_00f3: Unknown result type (might be due to invalid IL or missing references)
        //IL_00fd: Unknown result type (might be due to invalid IL or missing references)
        //IL_0102: Unknown result type (might be due to invalid IL or missing references)
        //IL_0107: Unknown result type (might be due to invalid IL or missing references)
        //IL_010f: Unknown result type (might be due to invalid IL or missing references)
        //IL_0190: Unknown result type (might be due to invalid IL or missing references)
        //IL_0197: Expected O, but got Unknown
        //IL_019c: Unknown result type (might be due to invalid IL or missing references)
        //IL_01a3: Expected O, but got Unknown
        //IL_01c5: Unknown result type (might be due to invalid IL or missing references)
        //IL_024c: Unknown result type (might be due to invalid IL or missing references)
        //IL_0251: Unknown result type (might be due to invalid IL or missing references)
        //IL_025a: Unknown result type (might be due to invalid IL or missing references)
        //IL_025c: Unknown result type (might be due to invalid IL or missing references)
        //IL_0266: Unknown result type (might be due to invalid IL or missing references)
        //IL_026b: Unknown result type (might be due to invalid IL or missing references)
        bool flag = ___room.functionObject.GetComponent<StaminaBoostRoomFunction>() || ___room.functionObject.GetComponent<FieldTripBaseRoomFunction>() & Random.Range(0, 20) == 0 & ___room.transform.Find("Really Evil Bubblerum") == null;
        if (flag)
        {
            GameObject val = new("Really Evil Bubblerum");
            GameObject val2 = new("Sprite");
            val2.transform.parent = val.transform;
            val2.transform.position = val.transform.position;
            val2.AddComponent<SpriteRenderer>();
            SpriteRenderer component = val2.GetComponent<SpriteRenderer>();
            component.material = TestPlugin.FindResourceOfName<Material>("SpriteStandard_Billboard", null);
            component.sprite = TestPlugin.Instance.assetMan.Get<Sprite>("ReallyEvilBubblerum");
            component.gameObject.layer = 9;
            //((Renderer)component).sortingLayerID = 12;
            Vector3 position = ___room.ec.RealRoomMid(___room) + Vector3.up * 60f;
            val.transform.position = position;
            val.transform.SetParent(___room.transform);
        }

        bool flag2 = ___room.functionObject.GetComponent<StaminaBoostRoomFunction>() & ___room.transform.Find("HeadplumTree") == null;
        if (flag2)
        {
            System.Random random = new(Singleton<CoreGameManager>.Instance.Seed());
            int num = random.Next(0, 100);
            if (num < 25)
            {
                GameObject val3 = new("HeadplumTree");
                GameObject val4 = new("Sprite");
                val4.transform.parent = val3.transform;
                val4.transform.position = val3.transform.position;
                val4.AddComponent<SpriteRenderer>();
                SpriteRenderer component2 = val4.GetComponent<SpriteRenderer>();
                component2.material = TestPlugin.FindResourceOfName<Material>("SpriteStandard_Billboard", null);
                component2.sprite = TestPlugin.Instance.assetMan.Get<Sprite>("HeadplumTree");
                component2.gameObject.layer = 9;
                //((Renderer)component2).sortingLayerID = 12;
                int index = random.Next(0, ___room.TileCount);
                Cell cell = ___room.cells[index];
                Vector3 floorWorldPosition = cell.FloorWorldPosition;
                val3.transform.position = floorWorldPosition + Vector3.up * 10f;
                val3.AddComponent<CapsuleCollider>();
                val3.GetComponent<CapsuleCollider>().radius = 3f;
                val3.GetComponent<CapsuleCollider>().height = 25f;
                val3.AddComponent<HeadplumTree>();
                val3.transform.SetParent(___room.transform);
            }
        }
    }
}
