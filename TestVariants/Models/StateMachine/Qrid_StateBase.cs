using TestVariants.Behaviours.Characters;

namespace TestVariants.Models.StateMachine;

public class Qrid_StateBase(Qrid npc) : NpcState(npc)
{
    protected new Qrid npc = npc;
}
