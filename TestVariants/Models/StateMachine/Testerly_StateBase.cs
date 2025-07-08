using TestVariants.Behaviours.Characters;

namespace TestVariants.Models.StateMachine;

public class Testerly_StateBase(Testerly npc) : NpcState(npc)
{
    protected new Testerly npc = npc;
}
