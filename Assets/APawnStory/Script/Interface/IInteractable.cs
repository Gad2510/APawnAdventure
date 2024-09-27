using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInteractable
{
    public Vector2Int Coordinates { get; set; }

    public void OnSelect();
}
