using HarmonyLib;

namespace TestVariants.Patches
{
    [HarmonyPatch(typeof(Entity), nameof(Entity.UpdateAllEntities)), HarmonyWrapSafe, HarmonyPriority(Priority.HigherThanNormal)]
    internal class EntityPatch
    {
        public static bool Prefix()
        {
            bool areEntitiesSame = true;

            for (int i = 0; i < Entity.allEntities.Count; i++)
            {
                Entity entity = Entity.allEntities[i];
                if (entity == null || !entity)
                {
                    Entity.allEntities.RemoveAt(i);
                    i--;
                    areEntitiesSame = false;
                }
            }

            return areEntitiesSame;
        }
    }
}
