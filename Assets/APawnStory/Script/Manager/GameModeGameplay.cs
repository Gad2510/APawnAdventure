using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameModeGameplay : GameMode
{
    public enum GameState
    {
        Dispaching,
        Select,
        Moving,
        Attacking,
        none
    }

    public delegate void OnAction (IInteractable _char,Stats _stats);
    public delegate void OnReturn (GameState _phase);

    public OnReturn UndoAction;
    public OnAction SelectCharacter;

    public TilesManager TilesManager { private set; get; }
    public MapManager MapManager { private set; get; }
    public CombatManager CombatManager { private set; get; }
    public IInteractable Target { get; set; }
    public Stats Stats { get; set; }
    public GameState Phase { get; set; }

    public ItemRecord ItemsRecords { get; private set; }

    // Start is called before the first frame update
    void Awake()
    {
        TilesManager = gameObject.AddComponent<TilesManager>();
        MapManager = gameObject.AddComponent<MapManager>();
        CombatManager = gameObject.AddComponent<CombatManager>();
        Phase = GameState.none;

        ItemsRecords =Resources.Load<ItemRecord>("ScriptableObjects/ItemRecord");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UndoLastAction()
    {
        if (Phase == GameState.none)
            return;

        UndoAction(Phase);

        switch (Phase)
        {
            case GameState.Select:
                Phase = GameState.none;
                break;
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
