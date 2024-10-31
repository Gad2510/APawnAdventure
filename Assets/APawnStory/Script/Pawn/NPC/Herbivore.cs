using UnityEngine;
using Betadron.Pawn;
using Betadron.Interfaces;

namespace Betadron.Pawn.NPC
{
    public class Herbivore : NPC
    {
        public override void ReachDestination()
        {
            IColectable item= scp_itemManager.GetItemInLocation(Coordinates);
            if(item!= null && item.IsEdible && !item.IsMeat)
            {
                CollectItem(item);
            }
            else
            {
                EndPhase = true;
            }
        }

        protected override void CollectItem(IColectable _item)
        {
            _item.OnDestroyElement();
            //Aun se puede mover busca nuevo item
            if (IsMovable) Detect();
            //No; termina turno
            else EndPhase = true;
        }

        public override void AutomaticActions()
        {
            base.AutomaticActions();
        }
    }
}
