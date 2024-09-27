using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : Pawn,IPlayable
{
    private Movement scp_move;
    private Health scp_health;
    private Inventory scp_inventory;

    [SerializeField]
    private Stats s_stats;

    public bool IsControllable { get; set; }
    public bool IsMovable { get; set; }
    public bool CanAttack { get; set; }

    public Stats CharacterStats
    {
        get
        {
            return s_stats;
        }
    }

    protected override void Start()
    {
        IsMovable = true;
        scp_move = gameObject.GetComponent<Movement>();
        scp_health = gameObject.GetComponent<Health>();
        scp_inventory = gameObject.GetComponent<Inventory>();
        scp_health.InitHealthBar(s_stats.getHealthPorcentage());
        TurnInit();
        base.Start();
    }

    public void TurnInit()
    {
        s_stats.int_movement = s_stats.int_maxMovement;
        s_stats.int_stamina = s_stats.int_maxStamina;
        CanAttack = true;
    }

    public override void ChangeCordinates(Vector3 _steps)
    {
        Vector3 dif =  _steps-transform.position;
        int stepMove = (int)Mathf.Abs(dif.x) + (int)Mathf.Abs(dif.z);
        s_stats.int_movement -= stepMove;
        scp_move.MoveCharacter(_steps);
        IsMovable = s_stats.int_movement>0;
        base.ChangeCordinates(_steps);
    }

    public void GetDamage(Character _attacker)
    {
        this.s_stats.int_health -= _attacker.s_stats.attack;
        scp_health.GetDamage(this.s_stats.getHealthPorcentage());
    }

    public void AddItem(int _item)
    {
        scp_inventory.AddItem(_item);
    }
}
