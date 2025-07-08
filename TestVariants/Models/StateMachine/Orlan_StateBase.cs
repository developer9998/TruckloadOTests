using TestVariants.Behaviours.Characters;

namespace TestVariants.Models.StateMachine;

public class Orlan_StateBase(Orlan npc) : NpcState(npc)
{
    protected new Orlan npc = npc;
}
