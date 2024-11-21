using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Betadron.Managers;
using Betadron.Interfaces;
namespace Betadron.Pawn
{
    public class Item : Pawn,IColectable,IAged
    {
        [SerializeField]
        private int itemID;
        [SerializeField]
        private bool isEdible;
        [SerializeField]
        private bool isMeat;

        private ItemManager scp_spawnMagr;
        private MeshRenderer mr_renderer;
        private int maxLife=2;
        private int life = 0;
        public int Life { get=>life; private set=>life=value; }
        public int ID => itemID;
        public bool IsEdible => isEdible;
        public bool IsMeat => isMeat;
        private void Awake()
        {
            scp_spawnMagr = ((GameModeGameplay)GameManager.gm_gamemode).SpawnManager;
            mr_renderer = GetComponent<MeshRenderer>();
        }
        protected override void Start()
        {

            scp_spawnMagr.SetItemAtributes(this);
            base.Start();
        }
        //Regresa ID del objeto para cargar info en el UI
        public override object OnSelect()
        {
            return itemID;
        }

        public override void OnCreateElement()
        {
            mr_renderer.enabled=true;
            scp_spawnMagr.Add2AgedItems(this);
            Born();
        }
        //Actualiza posicion del objeto, revisa si el objeto mandado es un vector 3
        public override void UpdateSelected(object var)
        {
            if(var is Vector3)
                transform.position=(Vector3)var;

            base.UpdateSelected(var);
        }
        //Desactiva Objeto
        public override void OnDestroyElement()
        {
            scp_spawnMagr.RemoveItem(this);
            scp_spawnMagr.RemoveFromAgedItems(this);
            mr_renderer.enabled=false;
        }
        public void Born()
        {
            life = maxLife; 
        }
        public void Aged()
        {
            life--;
            if (life <= 0)
                Die();
        }
        public void Die()
        {
            OnDestroyElement();
        }
    }
}