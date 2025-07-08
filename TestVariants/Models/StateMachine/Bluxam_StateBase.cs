using TestVariants.Behaviours.Characters;

namespace TestVariants.Models.StateMachine;

public class Bluxam_StateBase(Bluxam npc) : NpcState(npc)
{
    protected new Bluxam npc = npc;
}
