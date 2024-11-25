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

        private bool is_processing=false;
        private int int_turn=0;
        private int currentTurn = 0;
        private int int_numTurns=1;

        

        public float EvaluationTime { get; private set; }
        public int TurnToEvaluate { get => int_numTurns; set => int_numTurns = value; }

        // Start is called before the first frame update
        protected override void Awake()
        {
            PythonIDE.LoadPath();
            PythonIDE.CreatePythonIDE();
            IsPlayerTurn = true;
            EvaluationTime = Time.deltaTime / 4;
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
            SpawnManager.SpawnItems();
            Phase = CharacterTurn.StartTurn;
            is_processing = false;
            //StartTurn();

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
            if (Phase == CharacterTurn.StartTurn)
                return;

            UndoAction(Phase);
            //Cambia el estado al anterior
            switch (Phase)
            {
                case CharacterTurn.Select:
                    Phase = CharacterTurn.StartTurn;
                    break;
                //Ambas regresan al menu de seleccion de accion de caracter
                case CharacterTurn.Attacking:
                case CharacterTurn.Moving:
                    Phase = CharacterTurn.Select;
                    break;
                default:
                    Phase = CharacterTurn.StartTurn;
                    break;
            }
        }
        //Inicia simulaicon por el numero de turnos designados
        public void StartSimulation()
        {
            if (is_processing)
            {
                print("IN OTHER PHASE - "+Phase);
                return;
            }
                
            print("Start Simulation");
            currentTurn = 0;
            StartTurn();
        }

        public void StartTurn()
        {
            Phase = CharacterTurn.StartTurn;
            if (currentTurn >= TurnToEvaluate)
            {
                return;
            }
                
            print("Start Turn");
            is_processing = true;
            int_turn++;
            MapFunctions.GameTime+= EvaluationTime;
            print(MapFunctions.GameTime);
            //player start input
            IsPlayerTurn = true;
            CharacterManager.InitCharactersTurn();
            PlayerTurn();
        }
        public void PlayerTurn()
        {
            Phase = CharacterTurn.none;
            print("Player turn");
            IsPlayerTurn = false;
            EnemyTurn();
        }
        public void EnemyTurn()
        {
            Phase = CharacterTurn.none;
            print("Enemy turn");
            int status=CharacterManager.ExecuteNPCActions();
            print($"Enemis status {status}");
            if (status == 0)
            {
                EndTurn();
            }
        }
        public void EndTurn()
        {
            Phase = CharacterTurn.EndTurn;
            print("EndTurn turn");
            currentTurn++;
            if(SpawnManager.agedItems!=null)
                SpawnManager.agedItems.Invoke();
            SpawnManager.SpawnItems();
            StartCoroutine(DelayNextTurn());
                
        }

        private IEnumerator DelayNextTurn()
        {
            yield return new WaitForSeconds(0.1f);
            is_processing = false;
            StartTurn();
        }
    }
}