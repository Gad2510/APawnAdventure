using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class UI_Gameplay : MonoBehaviour
{
    [SerializeField]
    private Button btn_StartMovement;
    [SerializeField]
    private Button btn_Attack;

    [SerializeField]
    private GameObject go_charMenu;

    private GameModeGameplay gm_gamemode;
    // Start is called before the first frame update
    void Start()
    {
        gm_gamemode= ((GameModeGameplay)GameManager.gm_gamemode);

        gm_gamemode.SelectCharacter += OpenCharacterMenu;


        gm_gamemode.UndoAction += CloseCharacterMenu;

        btn_StartMovement.onClick.AddListener(() =>
        {
            if (gm_gamemode.Target != null)
            {
                gm_gamemode.TilesManager.SetMovementTiles(gm_gamemode.Target.Coordinates, gm_gamemode.Stats.int_movement);
                go_charMenu.SetActive(false);
                gm_gamemode.Phase = GameModeGameplay.CharacterTurn.Moving;
            }
        });

        btn_Attack.onClick.AddListener(() =>
        {
            if (gm_gamemode.Target != null)
            {
                gm_gamemode.CombatManager.CleanCharacterInRange();
                gm_gamemode.TilesManager.SetAttackTiles(gm_gamemode.Target.Coordinates);
                go_charMenu.SetActive(false);
                gm_gamemode.Phase = GameModeGameplay.CharacterTurn.Attacking;
            }
        });
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnDestroy()
    {
        gm_gamemode.SelectCharacter -= OpenCharacterMenu;


        gm_gamemode.UndoAction -= CloseCharacterMenu;
    }
    public void OpenCharacterMenu(IPlayable _char, Stats _stats)
    {
        //Activate Character Menu
        go_charMenu.SetActive(true);
        //Set state of buttons
        btn_StartMovement.interactable = _char.IsMovable;
        btn_Attack.interactable = _char.CanAttack;
        //Update Stats in the gamemode
        gm_gamemode.Target = _char;
        gm_gamemode.Stats = _stats;

    }

    public void CloseCharacterMenu(GameModeGameplay.CharacterTurn _phase)
    {
        if (_phase == GameModeGameplay.CharacterTurn.Moving || _phase == GameModeGameplay.CharacterTurn.Attacking)
            go_charMenu.SetActive(true);
        else if (_phase == GameModeGameplay.CharacterTurn.Select)
            go_charMenu.SetActive(false);
    }
}
