using TestVariants.Behaviours.Characters;

namespace TestVariants.Models.StateMachine;

public class AJOPI_StateBase(AJOPI npc) : NpcState(npc)
{
    protected new AJOPI npc = npc;
}
