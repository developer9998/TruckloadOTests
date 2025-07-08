using TestVariants.Behaviours.Characters;

namespace TestVariants.Models.StateMachine;

public class DaSketch_StateBase(DaSketch npc) : NpcState(npc)
{
    protected new DaSketch npc = npc;
}
