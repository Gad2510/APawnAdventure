using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInteractable
{
    public bool IsMovable { get; set; }
    public bool CanAttack { get; set; }
    public Vector2Int Coordinates { get; set; }

    public void InitObject();
}
