using System.Collections.Generic;
using UnityEngine;
using Betadron.Interfaces;
using Betadron.Managers;
using Betadron.Pawn;
using System.Linq;
using System.Collections;

namespace Betadron.Pawn.NPC
{
    public class NPC : Character
    {

        protected List<Vector2Int> lst_bestPath;
        protected IInteractable ifc_item2Colect;

        private SpawnerManager scp_spwManager;

        protected override void Start()
        {
            base.Start();
            scp_spwManager = ((GameModeGameplay)GameManager.gm_gamemode).SpawnManager;

        }

        //Busca el mojor spot para enocntrar comida
        //TESTING : Obtinene la unica fruta en el campo para seguirla
        protected Vector2Int RememberBestSpot()
        {
            //FORMULA 
            return Vector2Int.zero;
        }

        //Deteccion de enemigos y alimento
        protected virtual void Detect()
        {
            //TESTING
            ifc_item2Colect= scp_spwManager.GetAllItems()[0];

            GetBestPath();
            
        }

        //Implementa algoritmo A*
        protected void GetBestPath()
        {
            //Selecciona Ubicacion destino
            Vector2Int goal=(ifc_item2Colect != null)? ifc_item2Colect.Coordinates : RememberBestSpot();
            INagavable selfTile = ((GameModeGameplay)GameManager.gm_gamemode).MapManager.GetTile4Coordinate(Coordinates);
            List<INagavable> visited = new List<INagavable>();
            List<INagavable> unknow = new List<INagavable>();

            Debug.Log($"init :{selfTile.Coordinates} | goal: {goal}");
            //ExplorePath(ref path, visited,unknow, visited.First,goal, 300);
            INagavable path= ExplorePath(visited, unknow,selfTile, goal, 300);
            
            StartCoroutine(CheckPath(path));
        }

        private IEnumerator CheckPath(INagavable _path)
        {
            INagavable tile = _path;
            while (tile!= null)
            {
                ((MeshRenderer)tile.OnSelect()).enabled = true;
                yield return new WaitForSeconds(0.5f);
                tile = tile.Conected;

            }
        }

        private INagavable ExplorePath( List<INagavable> _visited,List<INagavable> _unknown,INagavable  _tile, Vector2Int _goal, int iteration)
        {

            INagavable currentTile = _tile;
            Vector2Int f = currentTile.GetNavegationCost(currentTile, _goal);
            Debug.Log($"{iteration} Explote tile: {_tile.Name} | Cost F= {f} ");
            if (f.x <= 0 || iteration <= 0) // Condicion de salida
                return _tile;
            //Se agrega a la lista de visitados
            _visited.Add(_tile);
            if(_unknown.Contains(_tile))
                _unknown.Remove(_tile);
            //Asigna valores de a cual es su origen
            //Cual es su costo de navegacion de ese origen
            _tile.Neighbors.ForEach(x =>
            {
                Vector2Int values = currentTile.GetNavegationCost(x, _goal);
                if (!x.navValues.HasValue || values.x < x.navValues.Value.x)
                {
                    x.Conected = currentTile;
                    x.navValues = values;
                }

                if (!_unknown.Contains(x) && !_visited.Contains(x))
                    _unknown.Add(x);

            });
            
            //Reoganiza desconocidos por los mejores valores
            _unknown = _unknown.OrderBy(x => currentTile.GetNavegationCost(x, _goal).y).
                ThenBy(x => currentTile.GetNavegationCost(x, _goal).x).ToList();

            _unknown.ForEach(n => print($"<color=yellow> Unknow location {n.Name} | values: {n.navValues}"));

            _tile = _unknown.First();
            Debug.Log($"<color=green> Best solution {_tile.Name} | {currentTile.GetNavegationCost(_tile, _goal)}");
            return ExplorePath(_visited, _unknown,_tile, _goal, iteration - 1);
        }
        //Original
        /*private int ExplorePath(ref List<INagavable> _path, LinkedList<INagavable> _visited, List<INagavable> _unknow, LinkedListNode<INagavable> _tile, Vector2Int _goal, int iteration)
        {

            INagavable currentTile = _tile.Value;
            int f = currentTile.GetNavegationCost(currentTile, _goal).x;
            Debug.Log($"{iteration} Explote tile: {_tile.Value.Name} | Cost F= {f} ");
            if (f <= 0 || iteration <= 0) // Condicion de salida
                return 1;

            //Debug.Log("Add neighbors to unkonw");
            //Agraga vecinos a lista de vecinos desconocidos
            List<INagavable> newLoactions = currentTile.Neighbors.Where(x => !_visited.Contains(x)).ToList();
            _unknow.AddRange(newLoactions);
            foreach (INagavable n in _unknow)
            {
                Debug.Log($"<color=yellow> Unknow location {n.Name}  | {currentTile.GetNavegationCost(n, _goal)}");
            }

            //Encontrar le que tenga el menor valor F
            //Si ahi mas de uno el que tenga mejor valor G
            // Verifica que no este contenido en los ya visitados
            INagavable best = _unknow.OrderBy(x => currentTile.GetNavegationCost(x, _goal).y).
                ThenBy(x => currentTile.GetNavegationCost(x, _goal).x).
                First();
            Debug.Log($"<color=green> Best solution {best.Name} | {currentTile.GetNavegationCost(best, _goal)}");
            //Removemos de la lista de los que no han sido visitados 
            _unknow.Remove(best);
            //Se agrega a los ya visitados
            LinkedListNode<INagavable> next = _visited.AddFirst(best);
            //xplora el best
            int value = ExplorePath(ref _path, _visited, _unknow, next, _goal, iteration - 1);
            //Agrega best si su valor es 1 lo que significa que llego a su objetivo
            if (value == 1)
            {
                Debug.Log($"Tile: {currentTile.Name} added to path");
                _path.Add(best);
            }

            return value;
        }*/

        public override void AutomaticActions()
        {
            Detect();
        }
    }
}
