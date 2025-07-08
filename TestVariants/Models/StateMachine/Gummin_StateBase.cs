using TestVariants.Behaviours.Characters;

namespace TestVariants.Models.StateMachine;

public class Gummin_StateBase(Gummin npc) : NpcState(npc)
{
    protected new Gummin npc = npc;
}
