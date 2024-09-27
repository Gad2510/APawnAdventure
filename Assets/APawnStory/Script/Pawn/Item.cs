using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Betadron.Pawn
{
    public class Item : Pawn
    {
        [SerializeField]
        private int itemID;

        public override object OnSelect()
        {
            gameObject.SetActive(false);
            return itemID;
        }

    }
}