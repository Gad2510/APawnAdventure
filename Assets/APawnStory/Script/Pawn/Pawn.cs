using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Betadron.Interfaces;
using Betadron.Managers;
using Betadron.Struct;
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
        //Reactiva el objeto en caso de haber sido desabilitado
        public virtual void UpdateSelected(object var)
        {
            Coordinates = MapFunctions.GetCoordn4Location(transform.position);
            gameObject.SetActive(true);
        }

        public virtual void OnCreateElement()
        {
            return;
        }

        public virtual void OnDestroyElement()
        {
            return;
        }
    }
}