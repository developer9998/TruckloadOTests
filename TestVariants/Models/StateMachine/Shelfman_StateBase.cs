using TestVariants.Behaviours.Characters;

namespace TestVariants.Models.StateMachine;

public class Shelfman_StateBase(Shelfman npc) : NpcState(npc)
{
    protected new Shelfman npc = npc;
}
