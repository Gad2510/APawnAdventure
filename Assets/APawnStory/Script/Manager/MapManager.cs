using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Betadron.Interfaces;
namespace Betadron.Managers
{
    public class MapManager : MonoBehaviour
    {
        /*
        MapManager: Clase encargada de crear y manejar las actualizaciones en el mapa
        */

        //Referencia el prefab del tile para instanciar
        public Object obj_tilePrefab;

        //Variables funcion objetivo
        public float sigma = 25;
        public float fx = 29.4f;
        public float fy = 39.3f;

        //Variables generacion
        private int mapSize = 20;

        

        //Referencia de los tiles en el mapa organizados por sus cordenadas
        private List<INagavable> Map { get; set; }

        // Start is called before the first frame update
        void Awake()
        {
            //Inicializa el diccionario
            Map = new List<INagavable>();
            //Obtiene referencia del prefab
            obj_tilePrefab = Resources.Load("Prefabs/MovementTile");
            mapSize=GameManager.Instance.LevelInfo.size;
            GenerateMap(mapSize);
        }

        // Update is called once per frame
        void Update()
        {
            //TESTING
            foreach (INagavable t in Map)
            {
                t.UpdateSelected(Formula(t.Coordinates));
            }
        }
        //CRUD
        //Create
        //Genera el mapa
        private void GenerateMap(int _mapSize)
        {
            INagavable lastTile = null;
            for (int x = 0; x < _mapSize; x++)
            {
                for (int y = 0; y < _mapSize; y++)
                {
                    //Instancea un tile del mapa en las coordenadas
                    INagavable go = (Instantiate(obj_tilePrefab, gameObject.transform) as GameObject).GetComponent<INagavable>();
                    Vector2Int coord= (Vector2Int.right * x) + (Vector2Int.up * y);
                    //Se añade al map
                    Map.Add(go);

                    go.Coordinates = coord;
                    go.UpdateSelected(Formula(coord));

                    AddNeighbors(ref lastTile,go, coord);
                }
            }
        }
        //Agrega los vecinos de un mapa cuadrado generado
        private void AddNeighbors(ref INagavable _last,INagavable  _tile, Vector2Int _coord)
        {
            //Es el primer elemento?
            if (_last != null && _coord.x==_last.Coordinates.x)
                _tile.AddNeighbor(_last);
            //Es de la primera fila generada?
            if (_coord.x > 0)
                _tile.AddNeighbor(Map.First(x => x.Coordinates == (_coord + Vector2Int.left)));
            //Guarda referencia para la siguiente iteracion
            _last = _tile;

        }
        //Update
        //Actualiza el tile para que reaccione acorde a las interacciones (Hover mouse, player range, etc)
        public MeshRenderer UpdateTile(Vector2Int _pos, Color _col, bool _enable = true)
        {
            if (!Map.Any((x)=> x.Coordinates==_pos))
                return null;

            MeshRenderer msr = (MeshRenderer)Map.First((x)=>x.Coordinates==_pos).OnSelect();
            msr.enabled = _enable;
            msr.material.color = _col;

            return msr;
        }
        public void UpdateObstacleTile(Vector2Int _coord, bool _status=true)
        {
            INagavable target = GetTile4Coordinate(_coord);
            target.HasObstacle = _status;
        }
        //Read
         //Obtiene la referencia de las coordenas por el mesh renderer del objeto
        public Vector2Int GetCoordinate4Tile(MeshRenderer _tile)
        {
            Vector2Int coord = Map.FirstOrDefault((x) => (MeshRenderer)x.OnSelect() == _tile).Coordinates;
            return coord;
        }
        public INagavable GetTile4Coordinate(Vector2Int _coord)
        {
            INagavable tile = Map.FirstOrDefault((x) => x.Coordinates == _coord);
            return tile;
        }
        public INagavable GetRandomTile()
        {
            int i = Random.Range(0, Map.Count);
            return Map[i];
        }
        //Funcion Objetivo a evaluar con el tiempo
        private float Formula(Vector2 coord)
        {
            //TESTING
            float t = Time.time / 10;

            coord = new Vector2(Mathf.Sin((coord.x) + t), Mathf.Cos((coord.y) + t));
            float exp = Mathf.Exp(((Mathf.Pow(coord.x, 2)) + (Mathf.Pow(coord.y, 2))) / (2 * Mathf.Pow(sigma, 2)));
            float trian = Mathf.Cos((coord.y * fy)) * Mathf.Sin((coord.x * fx));
            return (exp * trian);
        }
    }
}