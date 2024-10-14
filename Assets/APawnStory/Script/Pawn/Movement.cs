using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Betadron.Managers;
using Betadron.Interfaces;
using Betadron.Struct;
namespace Betadron.Pawn
{
    public class Movement : MonoBehaviour
    {
        protected Character scp_char;
        protected GameModeGameplay gm_gamemode;

        private Vector3 v3_goal;

        private const float MOVEMENT_SPEED = 1f;
        private const float MOVEMENT_TIME = 1f;
        private void Start()
        {
            gm_gamemode = ((GameModeGameplay)GameManager.gm_gamemode);
            scp_char = gameObject.GetComponent<Character>();
        }
        //Movimiento por teleportacion
        private void UpdateStamina(Vector3 _steps)
        {
            Vector3 dif = _steps - transform.position;
            int stepMove = (int)Mathf.Abs(dif.x) + (int)Mathf.Abs(dif.z);
            scp_char.CharacterStats.UpdateMovement(stepMove);
            scp_char.IsMovable = scp_char.CharacterStats.int_movement > 0;
        }
        //Moviminto por pasos
        private void UpdateStamina()
        {
            scp_char.CharacterStats.UpdateMovement(1);
            scp_char.IsMovable = scp_char.CharacterStats.int_movement > 0;
        }
        public void MoveCharacter(Vector3 _goal,bool teleport=false)
        {
            v3_goal = _goal;
            //Depende tipo de movimiento como se ejecutara
            if (teleport)
            {
                Telerport();
            }
            else
            {
                InitMovement();
            }
            
        }
        protected virtual void Telerport()
        {
            UpdateStamina(v3_goal);
            v3_goal.y = transform.position.y;
            transform.position = v3_goal;
            EndMovement();
        }
        protected virtual void InitMovement()
        {
            INagavable selfTile = gm_gamemode.MapManager.GetTile4Coordinate(scp_char.Coordinates);
            selfTile.Conected = null;
            Vector2Int goal=MapFunctions.GetCoordn4Location(v3_goal);
            Stack<INagavable> path = GetBestPath(selfTile, goal);
            StartCoroutine(StartMovement(path));
            //StartCoroutine(CheckPath(path));
        }
        //Empieza moviemiento de el personaje
        private IEnumerator StartMovement(Stack<INagavable> path)
        {
            
            while (path.Count>0)
            {
                INagavable nextTile = path.Pop();
                (nextTile.OnSelect() as MeshRenderer).enabled = true;

                Vector3 current = transform.position;
                Vector3 goal = MapFunctions.GetLocation4Coord(nextTile.Coordinates);
                UpdateStamina();
                float counter=0;
                while(counter< MOVEMENT_TIME)
                {
                    counter += Time.deltaTime*MOVEMENT_SPEED;
                    transform.position = Vector3.Lerp(current, goal, counter / MOVEMENT_TIME);

                    yield return null;
                }
                transform.position = goal;
            }
            EndMovement();
        }
        protected virtual void EndMovement()
        {
            //Actualiza coordenadas
            scp_char.UpdateSelected(0);
            gm_gamemode.TilesManager.HideTiles();
        }
        //Implementa algoritmo A*
        private Stack<INagavable> GetBestPath(INagavable _selfTile,Vector2Int _goal)
        {
            Stack<INagavable> visited = new Stack<INagavable>();
            List<INagavable> unknow = new List<INagavable>();

            Debug.Log($"init :{_selfTile.Coordinates} | goal: {_goal}");
            //ExplorePath(ref path, visited,unknow, visited.First,goal, 300);
            INagavable path = ExploreTiles(visited, unknow, _selfTile, _goal, 100);
            Debug.Log("Order Path");
            visited= OrderMovementStack(path);
            Debug.Log("End Order Path");
            return visited;
        }
        private Stack<INagavable> OrderMovementStack(INagavable _endTile)
        {
            Stack<INagavable> path=new Stack<INagavable>();
            INagavable tile = _endTile;
            int counter = 0;
            while (tile != null && counter <= scp_char.CharacterStats.int_movement)
            {
                path.Push(tile);
                tile = tile.Conected;
                counter++;
            }
            return path;
        }
        private INagavable ExploreTiles(Stack<INagavable> _visited, List<INagavable> _unknown, INagavable _tile, Vector2Int _goal, int iteration)
        {

            INagavable currentTile = _tile;
            Vector2Int f = currentTile.GetNavegationCost(currentTile, _goal);
            Debug.Log($"{iteration} Explote tile: {_tile.Name} | Cost F= {f} ");
            if (f.x <= 0 || iteration <= 0) // Condicion de salida
                return _tile;
            //Se agrega a la lista de visitados
            _visited.Push(_tile);
            if (_unknown.Contains(_tile))
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

            _tile = _unknown.First();
            Debug.Log($"<color=green> Best solution {_tile.Name} | {currentTile.GetNavegationCost(_tile, _goal)}");
            return ExploreTiles(_visited, _unknown, _tile, _goal, iteration - 1);
        }
        //Usado para revisar el camino usado
        //TESTING 
        private IEnumerator CheckPath(Stack<INagavable> _path)
        {
            INagavable tile = _path.Last();
            while (tile != null)
            {
                ((MeshRenderer)tile.OnSelect()).enabled = true;
                yield return new WaitForSeconds(0.5f);
                tile = tile.Conected;

            }
        }
    }
}