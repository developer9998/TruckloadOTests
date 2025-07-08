using TestVariants.Behaviours.Characters;

namespace TestVariants.Models.StateMachine;

public class Plit_StateBase(Plit npc) : NpcState(npc)
{
    protected new Plit npc = npc;
}
