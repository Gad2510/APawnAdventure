using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pawn : MonoBehaviour, IInteractable
{
    public bool IsMovable { get; set; }
    public bool CanAttack { get; set; }
    public Vector2Int Coordinates { set; get; }

    protected virtual void Start()
    {
        InitObject();
        Coordinates = new Vector2Int((int)transform.position.x, (int)transform.position.z);
    }

    public void InitObject()
    {
        ((GameModeGameplay)GameManager.gm_gamemode).CombatManager.AddCharacter(this);
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
