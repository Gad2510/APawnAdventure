using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Betadron.Interfaces;
using Betadron.Managers;
namespace Betadron.Pawn
{
    public class Pawn : MonoBehaviour, IInteractable
    {
        /*
         Clase base de todos los elementos interactuables del juego
         */
        public Vector2Int Coordinates { set; get; }

        protected virtual void Start()
        {
            UpdateSelected(0);
        }

        public virtual object OnSelect()
        {
            return 0;
        }

        
        //Actualiza las cordenas actuales del objeto 
        public void UpdateSelected(object var)
        {
            Coordinates = new Vector2Int((int)transform.position.x, (int)transform.position.z);
        }
    }
}