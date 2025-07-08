using TestVariants.Behaviours.Characters;

namespace TestVariants.Models.StateMachine;

public class Testholder_StateBase(Testholder npc) : NpcState(npc)
{
    protected new Testholder npc = npc;
}
