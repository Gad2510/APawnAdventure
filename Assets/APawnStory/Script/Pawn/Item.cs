using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Betadron.Managers;
namespace Betadron.Pawn
{
    public class Item : Pawn
    {
        [SerializeField]
        private int itemID;

        protected override void Start()
        {
            ((GameModeGameplay)GameManager.gm_gamemode).SpawnManager.SetItemAtributes(this);
            base.Start();
        }

        public override object OnSelect()
        {
            gameObject.SetActive(false);
            return itemID;
        }
        //Actualiza posicion del objeto, revisa si el objeto mandado es un vector 3
        public override void UpdateSelected(object var)
        {
            if(var is Vector3)
                transform.position=(Vector3)var;

            base.UpdateSelected(var);
            print($"Item coord {Coordinates}");
        }

    }
}