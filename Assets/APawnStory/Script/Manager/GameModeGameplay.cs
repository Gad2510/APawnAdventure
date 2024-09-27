using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameModeGameplay : GameMode
{
    //Gamemode del gameplay
    //Controla como funcionan los turnos y todos las acciones que puede hacer el jugador

    //Estados que hay por cada turno
    public enum GameState
    {
        Select, //Elemento seleccionado
        Moving, //Selecciona opcion de mover personaje
        Attacking, //Selecciona opcion de ataque
        none 
    }
    //Definicion Delegates que se ejecutan en puntos clave del programa 
    //OnAction:- Acciones a ejecutar cuando se interactua con un objeto interactuable
    public delegate void OnAction (IInteractable _char,Stats _stats);
    //OnReturn:- Acciones que se ejecutan al retroceder en los menus o acciones
    public delegate void OnReturn (GameState _phase);

    //Variables:
    public OnReturn UndoAction; //Conjunto de acciones que se usan al regresar 
    public OnAction SelectCharacter; //Conjunto de acciones cuando se selecciona un caracter

    public TilesManager TilesManager { private set; get; }
    public MapManager MapManager { private set; get; }
    public CombatManager CombatManager { private set; get; }
    public IInteractable Target { get; set; }
    public Stats Stats { get; set; }
    public GameState Phase { get; set; }

    public ItemRecord ItemsRecords { get; private set; }

    // Start is called before the first frame update
    protected override void Awake()
    {
        //Inicializar managers
        TilesManager = gameObject.AddComponent<TilesManager>();
        MapManager = gameObject.AddComponent<MapManager>();
        CombatManager = gameObject.AddComponent<CombatManager>();
        //Asignar estado inicial del juego
        Phase = GameState.none;

        ItemsRecords =Resources.Load<ItemRecord>("ScriptableObjects/ItemRecord");

        base.Awake();
    }
    //PENDIENTE
    protected override void LoadUI()
    {
        return;
    }

    //Se ejecuta al retroceder en el juego
    public void UndoLastAction()
    {
        //Si NO hay un estado asginado
        if (Phase == GameState.none)
            return;

        UndoAction(Phase);
        //Cambia el estado al anterior
        switch (Phase)
        {
            case GameState.Select:
                Phase = GameState.none;
                break;
            //Ambas regresan al menu de seleccion de accion de caracter
            case GameState.Attacking:
            case GameState.Moving:
                Phase = GameState.Select;
                break;
            default:
                Phase = GameState.none;
                break;
        }
    }

   
}
