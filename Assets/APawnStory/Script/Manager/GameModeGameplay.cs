using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Betadron.Interfaces;
using Betadron.Objects;
using Betadron.Struct;

namespace Betadron.Managers
{
    public class GameModeGameplay : GameMode
    {
        //Gamemode del gameplay
        //Controla como funcionan los turnos y todos las acciones que puede hacer el jugador

        //Estados que hay por cada turno
        public enum CharacterTurn
        {
            StartTurn,
            Select, //Elemento seleccionado
            Moving, //Selecciona opcion de mover personaje
            Attacking, //Selecciona opcion de ataque
            EndTurn,
            none
        }
        //Definicion Delegates que se ejecutan en puntos clave del programa 
        //OnAction:- Acciones a ejecutar cuando se interactua con un objeto interactuable
        public delegate void OnAction(IPlayable _char, Stats _stats);
        //OnReturn:- Acciones que se ejecutan al retroceder en los menus o acciones
        public delegate void OnReturn(CharacterTurn _phase);

        //Variables:
        public OnReturn UndoAction; //Conjunto de acciones que se usan al regresar 
        public OnAction SelectCharacter; //Conjunto de acciones cuando se selecciona un caracter

        public bool IsPlayerTurn { get; set; } 

        public TilesManager TilesManager { private set; get; }
        public MapManager MapManager { private set; get; }
        public CharactersManager CharacterManager { private set; get; }
        public ItemManager SpawnManager { private set; get; }
        public IPlayable Target { get; set; }
        public Stats Stats { get; set; }
        public CharacterTurn Phase { get; set; }

        public ItemRecord ItemsRecords { get; private set; }
        public GameObject UI_Characters { get; set; }

        // Start is called before the first frame update
        protected override void Awake()
        {
            //Inicializar managers
            TilesManager = gameObject.AddComponent<TilesManager>();
            MapManager = gameObject.AddComponent<MapManager>();
            CharacterManager = gameObject.AddComponent<CharactersManager>();
            SpawnManager = gameObject.AddComponent<ItemManager>();
            SpawnManager.InitManager(MapManager);
            //Asignar estado inicial del juego
            Phase = CharacterTurn.none;

            ItemsRecords = Resources.Load<ItemRecord>("ScriptableObjects/ItemRecord");

            base.Awake();
        }

        protected void Start()
        {
            //Ejecuta el inicio de turno un frame mas tarde para que termine que cargar las referencias de escena
            Invoke(nameof(StartTurn),Time.deltaTime);
            
        }
        //PENDIENTE
        protected override void LoadUI()
        {
            Object prefb = Resources.Load<GameObject>("Prefabs/UICharacters");
            UI_Characters=Instantiate(prefb)as GameObject;
            return;
        }

        //Se ejecuta al retroceder en el juego
        public void UndoLastAction()
        {
            //Si NO hay un estado asginado
            if (Phase == CharacterTurn.none)
                return;

            UndoAction(Phase);
            //Cambia el estado al anterior
            switch (Phase)
            {
                case CharacterTurn.Select:
                    Phase = CharacterTurn.none;
                    break;
                //Ambas regresan al menu de seleccion de accion de caracter
                case CharacterTurn.Attacking:
                case CharacterTurn.Moving:
                    Phase = CharacterTurn.Select;
                    break;
                default:
                    Phase = CharacterTurn.none;
                    break;
            }
        }
        public void StartTurn()
        {
            print("Start Turn");
            //player start input
            IsPlayerTurn = true;
            CharacterManager.InitCharactersTurn();
            PlayerTurn();
        }
        public void PlayerTurn()
        {
            print("Player turn");
            IsPlayerTurn = false;
            EnemyTurn();
        }
        public void EnemyTurn()
        {
            print("Enemy turn");
            CharacterManager.ExecuteNPCActions();
        }
        public void EndTurn()
        {
            print("EndTurn turn");
            //StartTurn();
        }
    }
}