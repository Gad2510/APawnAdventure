using UnityEngine;
using Betadron.Pawn;

namespace Betadron.Pawn.NPC
{
    public class Herbivore : NPC
    {


        public override void AutomaticActions()
        {
            EndPhase = false;
            base.AutomaticActions();
        }
    }
}
