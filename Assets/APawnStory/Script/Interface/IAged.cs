using UnityEngine;

namespace Betadron.Interfaces
{
    public interface IAged
    {
        int Life { get;}
        void Born();
        void Aged();
        void Die();
    }
}
