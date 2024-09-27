using UnityEngine;

public interface IPlayable
{
    bool IsControllable { get; set; }
    bool IsMovable { get; set; }
    bool CanAttack { get; set; } 
}
