using UnityEngine;

namespace Betadron.Interfaces
{
    public interface IAged
    {
        Vector2Int Coordinates { get; set; }
        int Life { get;}
        void Born();
        void Aged();
        void Die();
    }
}
