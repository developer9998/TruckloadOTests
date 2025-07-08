using HarmonyLib;
using UnityEngine;

namespace TestVariants.Patches;

[HarmonyPatch(typeof(FloodEvent), nameof(FloodEvent.ChangeUnderwateState))]
internal class FloodEventPatch
{
    public static void Postfix(bool underwater)
    {
        if (!underwater) return;

        ItemManager itemManager = CoreGameManager.Instance?.GetPlayer(0)?.itm ?? Object.FindObjectOfType<ItemManager>();
        ItemObject[] items = itemManager.items;

        for (int i = 0; i < items.Length; i++)
        {
            ItemObject itemObject = items[i];
            if (itemObject.nameKey == "ITM_GumminChunk")
            {
                itemManager.RemoveItem(i);
            }
        }
    }
}
