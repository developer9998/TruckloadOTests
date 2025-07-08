using TestVariants.Behaviours.Characters;

namespace TestVariants.Models.StateMachine;

public class TheBestOne_StateBase(TheBestOne npc) : NpcState(npc)
{
    protected new TheBestOne npc = npc;
}
