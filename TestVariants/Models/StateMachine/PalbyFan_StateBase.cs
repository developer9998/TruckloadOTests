using TestVariants.Behaviours.Characters;

namespace TestVariants.Models.StateMachine;

public class PalbyFan_StateBase(PalbyFan npc) : NpcState(npc)
{
    protected new PalbyFan npc = npc;
}
