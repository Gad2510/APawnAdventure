using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Betadron.Interfaces;
using Betadron.Struct;
namespace Betadron.Managers
{
    public enum Direction
    {
        TL=1<<0,
        TC=1<<1,
        TR=1<<2,
        CL=1<<3,
        C =1<<4,
        CR=1<<5,
        DL=1<<6,
        DC=1<<7,
        DR=1<<8,

        Top=TL|TC|TR,
        CenterH=C|CL|CR,
        Down=DR|DC|DL,
        Rigth=TR|CR|DR,
        Left=TL|CL|DL,
        CenterV=TC|C|DC
    }

    public class MapManager : MonoBehaviour
    {
        /*
        MapManager: Clase encargada de crear y manejar las actualizaciones en el mapa
        */

        

        //Referencia el prefab del tile para instanciar
        public Object obj_tilePrefab;



        //Variables generacion
        public List<TileBatch> testing;
        private const int MAX_BATCH_SIZE = 12;
        private int batchSize = 12;
        private Dictionary<Direction,TileBatch> dic_map;

        //Referencia de los tiles en el mapa organizados por sus cordenadas
        private List<INagavable> Map { get; set; }

        // Start is called before the first frame update
        void Awake()
        {
            //Inicializa el diccionario
            Map = new List<INagavable>();
            //Obtiene referencia del prefab
            obj_tilePrefab = Resources.Load("Prefabs/MovementTile");
            int mapSize=GameManager.Instance.LevelInfo.size;
            float middle = mapSize / 2;
            dic_map = new Dictionary<Direction,TileBatch>() 
            {
                { Direction.TL,new TileBatch(Direction.TL,middle)},
                { Direction.TC,new TileBatch(Direction.TC,middle)},
                { Direction.TR,new TileBatch(Direction.TR,middle)},
                { Direction.CL,new TileBatch(Direction.CL,middle)},
                { Direction.C ,new TileBatch(Direction.C ,middle)},
                { Direction.CR,new TileBatch(Direction.CR,middle)},
                { Direction.DL,new TileBatch(Direction.DL,middle)},
                { Direction.DC,new TileBatch(Direction.DC,middle)},
                { Direction.DR,new TileBatch(Direction.DR,middle)},
            };
            testing = dic_map.Values.ToList();
            batchSize = (Mathf.FloorToInt(mapSize / 3) < MAX_BATCH_SIZE) ? (int)Mathf.Ceil(mapSize / 3) : batchSize;

            GenerateMap(mapSize);

            foreach (TileBatch b in dic_map.Values)
            {
                print($"{b.en_dir} : {b.v2_pivot}");
            }
        }

        private void Update()
        {
            foreach(TileBatch b in dic_map.Values)
            {
                b.UpdateBatch();
            }
        }

        //CRUD
        //Create
        //Genera el mapa
        private void GenerateMap(int _mapSize)
        {
            int middle = _mapSize / 2;
            INagavable lastTile = null;
            for (int x = -middle; x < _mapSize-middle; x++)
            {
                for (int y = -middle; y < _mapSize - middle; y++)
                {
                    int index = (Mathf.FloorToInt((y + middle) / batchSize)+1 * Mathf.FloorToInt((x + middle) / batchSize)+1)-1;
                    Direction d = dic_map.Keys.ToList()[index];
                    //Instancea un tile del mapa en las coordenadas
                    INagavable go = (Instantiate(obj_tilePrefab, gameObject.transform) as GameObject).GetComponent<INagavable>();
                    Vector2Int coord= (Vector2Int.right * x) + (Vector2Int.up * y);
                    //Se añade al map
                    Map.Add(go);
                    dic_map[d].AddTile(go);
                    go.Coordinates = coord;

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
        //Agrega el satatus de obstaculo a un tile
        public void UpdateObstacleTile(Vector2Int _coord, bool _status=true)
        {
            INagavable target = GetTile4Coordinate(_coord);
            target.HasObstacle = _status;
        }
        public void UpdatePlayerPos(Vector3 _plPos)
        {

        }
        //Read
         //Obtiene la referencia de las coordenas por el mesh renderer del objeto
        public Vector2Int GetCoordinate4Tile(MeshRenderer _tile)
        {
            Vector2Int coord = Map.FirstOrDefault((x) => (MeshRenderer)x.OnSelect() == _tile).Coordinates;
            return coord;
        }
        //Obtiene la referencia de el navegable por la coordenada seleccionada
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
       
    }
    public struct Batch_R_Tile
    {
        public Vector2Int coord;
        public INagavable tile;
    }

    [SerializeField]
    public struct TileBatch
    {
        public static TileBatch Center;
        public static float Step;

        public Direction en_dir;
        public Vector2 v2_pivot;
        public Vector2 v2_centerRel;
        public List<Batch_R_Tile> lst_batch;

        public TileBatch(Direction _dir,float _middle)
        {
            v2_pivot = Vector2.zero;
            v2_centerRel = Vector2.zero;
            lst_batch = new List<Batch_R_Tile>();
            en_dir = _dir;

            if (_dir == Direction.C)
            {
                Center = this;
                Step = _middle;
            }
                
            v2_centerRel = CalculatePivotDir(_dir);
            v2_pivot = v2_centerRel * _middle;
        }

        public Vector2 CalculatePivotDir(Direction _dir)
        {
            Vector2 pivot = Vector2.zero;
            if ((Direction.Top & _dir) > 0)
            {
                pivot += Vector2.up;
            }
            else if ((Direction.Down & _dir) > 0)
            {
                pivot += Vector2.down;
            }

            if ((Direction.Left & _dir) > 0)
            {
                pivot += Vector2.right;
            }
            else if ((Direction.Rigth & _dir) > 0)
            {
                pivot += Vector2.left;
            }

            return pivot;
        }
        
        public void UpdateBatch()
        {
            foreach(Batch_R_Tile n in lst_batch)
            {
                n.tile.UpdateSelected(MapFunctions.Formula(n.tile.Coordinates));
            }
        }

        public void AddTile(INagavable _tile)
        {
            Batch_R_Tile rel= new Batch_R_Tile();
            rel.tile = _tile;
            int x = (_tile.Coordinates.x < v2_pivot.x) ? Mathf.CeilToInt(v2_pivot.x) : Mathf.FloorToInt(v2_pivot.x);
            int y = (_tile.Coordinates.y < v2_pivot.y) ? Mathf.CeilToInt(v2_pivot.y) : Mathf.FloorToInt(v2_pivot.y);
            rel.coord =new Vector2Int(_tile.Coordinates.x - x, _tile.Coordinates.y - y);
            lst_batch.Add(rel);
        }

        public void MoveBatch()
        {
            //Actualiza posicion con su relacion con el centro
            v2_pivot = Center.v2_pivot + v2_centerRel;
            foreach(Batch_R_Tile t in lst_batch)
            {
                Vector2 newpos = v2_pivot;
                newpos.x += t.coord.x;
                newpos.y += t.coord.y;
                t.tile.Coordinates =Vector2Int.RoundToInt(newpos);
            }

        }
    }
}