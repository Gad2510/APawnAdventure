using UnityEngine;

namespace Betadron.Interfaces
{
    public interface IPlayable
    {

        bool IsControllable { get; set; }
        bool IsMovable { get; set; }
        bool CanAttack { get; set; }
        bool EndPhase { get; set; }
        Vector2Int GetCoordinates();

        public void StartTurn();
        public void AutomaticActions();
    }
}