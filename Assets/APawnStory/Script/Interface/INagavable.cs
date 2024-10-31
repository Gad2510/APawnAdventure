using System;
using System.Collections.Generic;
using UnityEngine;

namespace Betadron.Interfaces
{
    public interface INagavable:IInteractable
    {
        string Name { get; set; }
        bool HasObstacle { get; set; }
        Vector2Int? navValues { get; set; }
        int PathSize { get; set; }
        INagavable Conected { get; set; }
        List<INagavable> Neighbors { get; }
        /// <summary>Calcula el costo de navegacion</summary>
        /// <returns> x : valor de F(suma de costo total y vecino) || y: valor de G (costo entre puntos)</returns>
        Vector2Int GetNavegationCost(INagavable _other, Vector2Int _goal);

        void AddNeighbor(INagavable _other);

        void RemoveNeighbor(INagavable _other);
    }
}
