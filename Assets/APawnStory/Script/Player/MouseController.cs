using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseController : MonoBehaviour
{
    private Vector2Int v2_currentPos;

    private GameModeGameplay gm_gamemode;

    private Character scp_charRef;

    public const int MAP_LAYER=1<<3;
    private void Start()
    {
        gm_gamemode = ((GameModeGameplay)GameManager.gm_gamemode);
    }

    // Update is called once per frame
    void Update()
    {
        UpdateCursorPos();
        PerformAction();
        UndoAction();
    }
    private void UpdateCursorPos()
    {
        Ray cam2world = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(cam2world, out RaycastHit hit,float.PositiveInfinity, MAP_LAYER))
        {
            MeshRenderer tile = hit.collider.GetComponent<MeshRenderer>();
            gm_gamemode.TilesManager.SetCursorTile(tile);
            v2_currentPos=gm_gamemode.MapManager.GetTileCoordinate(tile);
        }
    }
    private void PerformAction()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray cam2world = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(cam2world, out RaycastHit hit))
            {
                OnClickTile(hit.collider);
                OnClickCharacter(hit.collider);
                OnClickItem(hit.collider);
            }
        }
    }
    private void UndoAction()
    {
        if (Input.GetMouseButtonDown(1))
        {
            gm_gamemode.UndoLastAction();
        }
    }
    private void OnClickTile(Collider _other)
    {
        //Get the tile to move character
        if (scp_charRef != null && _other.CompareTag("Tile"))
        {
            if (gm_gamemode.Phase == GameModeGameplay.CharacterTurn.Moving)
                scp_charRef.ChangeCordinates(_other.transform.position);

            gm_gamemode.Phase = GameModeGameplay.CharacterTurn.none;
        }
    }
    private void OnClickCharacter(Collider _other)
    {
        if (_other.CompareTag("Character"))
        {
            Character hitCharacter = _other.GetComponent<Character>();
            //Get the character
            if (gm_gamemode.Phase == GameModeGameplay.CharacterTurn.none)
            {
                scp_charRef = hitCharacter;
                gm_gamemode.SelectCharacter.Invoke(scp_charRef, scp_charRef.CharacterStats);
                gm_gamemode.Phase = GameModeGameplay.CharacterTurn.Select;
            }
            //Check if its posible to attack
            else if (gm_gamemode.Phase == GameModeGameplay.CharacterTurn.Attacking &&  //Check status of gamemode
                gm_gamemode.CombatManager.LookForCharacter(hitCharacter.Coordinates)) //Check if character is on range
            {
                scp_charRef.CanAttack = false;
                hitCharacter.GetDamage(scp_charRef);
                gm_gamemode.TilesManager.HideTiles();
                gm_gamemode.Phase = GameModeGameplay.CharacterTurn.none;
            }

        }
    }

    private void OnClickItem(Collider _other)
    {
        if (_other.gameObject.CompareTag("Item"))
        {
            Item hitItem = _other.GetComponent<Item>();
            if (gm_gamemode.Phase == GameModeGameplay.CharacterTurn.Attacking &&  //Check status of gamemode
                    gm_gamemode.CombatManager.LookForCharacter(hitItem.Coordinates)) //Check if item is on range
            {
                scp_charRef.AddItem(hitItem.Collect());
                gm_gamemode.TilesManager.HideTiles();
                gm_gamemode.Phase = GameModeGameplay.CharacterTurn.none;
            }
        }
        
    }


}
