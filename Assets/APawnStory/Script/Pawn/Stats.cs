using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public struct Stats
{
    public string str_name;

    public int int_maxHealth;
    public int int_health;

    public int int_maxStamina;
    public int int_stamina;

    public int int_maxMovement;
    public int int_movement;

    public int attack;
    public int defence;
    public int dexerity;

    public float getHealthPorcentage()
    {
        return (float)int_health / (float)int_maxHealth;
    }
}
