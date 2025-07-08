using TestVariants.Behaviours.Characters;

namespace TestVariants.Models.StateMachine;

public class Cren_StateBase(Cren npc) : NpcState(npc)
{
    protected new Cren npc = npc;
}
