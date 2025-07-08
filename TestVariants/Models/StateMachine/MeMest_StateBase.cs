using TestVariants.Behaviours.Characters;

namespace TestVariants.Models.StateMachine;

public class MeMest_StateBase(MeMest npc) : NpcState(npc)
{
    protected new MeMest npc = npc;
}
