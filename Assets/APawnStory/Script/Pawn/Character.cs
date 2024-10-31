using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Betadron.Managers;
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
        public Movement MoveComp { get; private set; }
        //Componente de vida (UI)
        public Health HealthComp { get; private set; }
        //Comopoennte de inventario (drop para enemigos)
        public Inventory InventoryComp { private set; get; }

        [SerializeField]
        private Stats s_stats;

        public bool IsControllable
        {
            get { return s_stats.isPlayer; }
            set
            {
                s_stats.isPlayer = value;
            }
        }
        public bool IsMovable { get; set; }
        public bool CanAttack { get; set; }
        public virtual bool EndPhase { get; set; }
        public Stats CharacterStats => s_stats;

        protected override void Start()
        {
            OnCreateElement();
             
            base.Start();
            
            MoveComp = gameObject.GetComponent<Movement>();
            HealthComp = gameObject.GetComponent<Health>();
            InventoryComp = gameObject.GetComponent<Inventory>();
        }
        public override void OnCreateElement()
        {
            s_stats.int_health = s_stats.int_maxHealth;
            ((GameModeGameplay)GameManager.gm_gamemode).CharacterManager.AddCharacter(this);
            IsMovable = true;
        }
        public override void OnDestroyElement()
        {
            ((GameModeGameplay)GameManager.gm_gamemode).CharacterManager.RemoveCharacter(this);
        }

        //Inicio de turno se reinician stats
        public void StartTurn()
        {
            Debug.Log("Start turn for", gameObject);
            EndPhase = false;
            s_stats.int_movement = s_stats.int_maxMovement;
            s_stats.int_stamina = s_stats.int_maxStamina;
            CanAttack = true;
        }

        public void UpdateHealth(int amount)
        {
            s_stats.int_health -= amount;
            s_stats.int_health = Mathf.Clamp(s_stats.int_health, 0, s_stats.int_maxHealth);
        }

        public void UpdateMovement(int amount)
        {
            s_stats.int_movement -= amount;
            s_stats.int_movement = Mathf.Clamp(s_stats.int_movement, 0, s_stats.int_maxMovement);
            IsMovable = s_stats.int_movement > 0;
        }

        public void UpdateStamina(int amount)
        {
            s_stats.int_stamina -= amount;
            s_stats.int_stamina = Mathf.Clamp(s_stats.int_stamina, 0, s_stats.int_maxStamina);
        }
        public Vector2Int GetCoordinates()
        {
            return Coordinates;
        }
        //Funcion de todos los caracteres cuando son seleccionados 
        //regresa el componente de vida para usar en el combate
        public override object OnSelect()
        {
            return HealthComp;
        }

        public virtual void AutomaticActions()
        {
            return;
        }
    }
}