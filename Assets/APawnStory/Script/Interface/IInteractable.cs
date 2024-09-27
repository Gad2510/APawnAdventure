using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Betadron.Interfaces
{
    public interface IInteractable
    {
        public Vector2Int Coordinates { get; set; }
        public object OnSelect();
        public void UpdateSelected(object var);

        
    }
}