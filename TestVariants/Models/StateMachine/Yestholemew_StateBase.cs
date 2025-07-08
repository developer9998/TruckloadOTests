using TestVariants.Behaviours.Characters;

namespace TestVariants.Models.StateMachine;

public class Yestholemew_StateBase(Yestholemew npc) : NpcState(npc)
{
    protected new Yestholemew npc = npc;
}
