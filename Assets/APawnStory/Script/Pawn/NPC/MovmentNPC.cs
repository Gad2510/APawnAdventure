using UnityEngine;
using Betadron.Pawn;

namespace Betadron.Pawn.NPC
{
    public class MovmentNPC : Movement
    {
        protected override void EndMovement()
        {
            base.EndMovement();
            (scp_char as NPC).ReachDestination();
        }
    }
}
