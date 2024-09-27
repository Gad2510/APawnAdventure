using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pawn : MonoBehaviour, IInteractable
{
    public Vector2Int Coordinates { set; get; }

    protected void Awake()
    {
        ((GameModeGameplay)GameManager.gm_gamemode).CombatManager.AddCharacter(this);
    }
    protected virtual void Start()
    {
        ChangeCordinates(Vector3.zero);

    }

    public virtual void OnSelect()
    {
        return;
    }

    private void OnDisable()
    {
        ((GameModeGameplay)GameManager.gm_gamemode).CombatManager.RemoveCharacter(this);
    }

    public virtual void ChangeCordinates(Vector3 _steps)
    {
        Coordinates = new Vector2Int((int)transform.position.x, (int)transform.position.z);
    }
}
