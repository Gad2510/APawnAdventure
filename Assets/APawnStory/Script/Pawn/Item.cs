using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Betadron.Managers;
using Betadron.Interfaces;
namespace Betadron.Pawn
{
    public class Item : Pawn,IColectable
    {
        [SerializeField]
        private int itemID;
        [SerializeField]
        private bool isEdible;
        [SerializeField]
        private bool isMeat;

        private ItemManager scp_spawnMagr;

        public int ID => itemID;
        public bool IsEdible => isEdible;
        public bool IsMeat => isMeat;

        protected override void Start()
        {
            scp_spawnMagr = ((GameModeGameplay)GameManager.gm_gamemode).SpawnManager;
            scp_spawnMagr.SetItemAtributes(this);
            base.Start();
        }
        //Regresa ID del objeto para cargar info en el UI
        public override object OnSelect()
        {
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
        //Desactiva Objeto
        public override void OnDestroyElement()
        {
            scp_spawnMagr.RemoveItem(this);
            gameObject.SetActive(false);
        }
    }
}