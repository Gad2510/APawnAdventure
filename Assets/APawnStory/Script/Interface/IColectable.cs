using UnityEngine;

namespace Betadron.Interfaces
{
    public interface IColectable:IInteractable
    {
        int ID { get; }

        bool IsEdible { get; }
        bool IsMeat { get; }

    }
}
