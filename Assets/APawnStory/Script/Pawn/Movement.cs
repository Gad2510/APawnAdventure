using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    private GameModeGameplay gm_gamemode;
    private void Start()
    {
        gm_gamemode=((GameModeGameplay)GameManager.gm_gamemode);
        
    }


    public void MoveCharacter(Vector3 _steps) 
    {
        _steps.y = transform.position.y;
        transform.position =_steps;
        gm_gamemode.TilesManager.HideTiles();
    }


}
