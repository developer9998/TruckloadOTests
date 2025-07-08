using TestVariants.Behaviours.Characters;

namespace TestVariants.Models.StateMachine;

public class ThroneTest_StateBase(ThroneTest npc) : NpcState(npc)
{
    protected new ThroneTest npc = npc;
}
