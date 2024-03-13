using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : Pawn
{
    [SerializeField]
    private int itemID;

    public int Collect()
    {
        gameObject.SetActive(false);
        return itemID;
    }

}
