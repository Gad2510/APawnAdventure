using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Betadron.Interfaces;
using Betadron.Struct;
namespace Betadron.Pawn
{
    public class Character : Pawn, IPlayable
    {

        /*
         Clase padre de todos los elementos que puedan tomar acciones
         */
        //Componente de movimiento
        public Movement Movement { get; private set; }
        //Componente de vida (UI)
        public Health Health { get; private set; }
        //Comopoennte de inventario (drop para enemigos)
        public Inventory Inventory { private set; get; }

        [SerializeField]
        private Stats s_stats;

        public bool IsControllable { get; set; }
        public bool IsMovable { get; set; }
        public bool CanAttack { get; set; }

        public Stats CharacterStats
        {
            get
            {
                return s_stats;
            }
        }

        protected override void Start()
        {
            IsMovable = true;
            Movement = gameObject.GetComponent<Movement>();
            Health = gameObject.GetComponent<Health>();
            Inventory = gameObject.GetComponent<Inventory>();
            TurnInit();
            base.Start();
        }
        //Inicio de turno se reinician stats
        public void TurnInit()
        {
            s_stats.int_movement = s_stats.int_maxMovement;
            s_stats.int_stamina = s_stats.int_maxStamina;
            CanAttack = true;
        }

        public Vector2Int GetCoordinates()
        {
            return Coordinates;
        }
        //Funcion de todos los caracteres cuando son seleccionados 
        //regresa el componente de vida para usar en el combate
        public override object OnSelect()
        {
            return Health;
        }
    }
}