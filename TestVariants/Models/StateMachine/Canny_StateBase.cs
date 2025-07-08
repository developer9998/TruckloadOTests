using TestVariants.Behaviours.Characters;

namespace TestVariants.Models.StateMachine;

public class Canny_StateBase(Canny npc) : NpcState(npc)
{
    protected new Canny npc = npc;
}
