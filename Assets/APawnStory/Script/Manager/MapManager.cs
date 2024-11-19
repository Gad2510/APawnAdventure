using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Betadron.Interfaces;
using Betadron.Struct;
namespace Betadron.Managers
{
    

    public class MapManager : MonoBehaviour
    {
        /*
        MapManager: Clase encargada de crear y manejar las actualizaciones en el mapa
        */

        //Referencia el prefab del tile para instanciar
        public Object obj_tilePrefab;

        //Variables generacion
        private const int MAX_BATCH_SIZE = 15;
        private int batchSize = 15;
        private List<TileBatch> lst_map;

        //Referencia de los tiles en el mapa organizados por sus cordenadas
        private List<INagavable> Map { get; set; }

        public float sigma = 5f;
        public float fx = 24f;
        public float fy = 26f;
        // Start is called before the first frame update
        void Awake()
        {
            //Inicializa el diccionario
            Map = new List<INagavable>();
            //Obtiene referencia del prefab
            obj_tilePrefab = Resources.Load("Prefabs/MovementTile");
            int mapSize=GameManager.Instance.LevelInfo.size;
            this.batchSize = (Mathf.FloorToInt(mapSize / 3) < MAX_BATCH_SIZE) ? (int)Mathf.Ceil(mapSize / 3) : this.batchSize;
            lst_map = new List<TileBatch>() 
            {
                new TileBatch(Direction.LU,batchSize),
                new TileBatch(Direction.CU,batchSize),
                new TileBatch(Direction.RU,batchSize),
                new TileBatch(Direction.CL,batchSize),
                new TileBatch(Direction.C ,batchSize),
                new TileBatch(Direction.CR,batchSize),
                new TileBatch(Direction.LD,batchSize),
                new TileBatch(Direction.CD,batchSize),
                new TileBatch(Direction.RD,batchSize),
            };

            GenerateMap(mapSize);

            foreach (TileBatch b in lst_map)
            {
                print($"{b.en_dir} : {b.v2_pivot}");
                print(b.lst_batch.Count);
            }
        }

        private void Update()
        {
            foreach(TileBatch b in lst_map)
            {
                MapFunctions.FX = fx;
                MapFunctions.FY = fy;
                MapFunctions.Sigma = sigma;
                b.UpdateItems();
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
                    //Instancea un tile del mapa en las coordenadas
                    INagavable go = (Instantiate(obj_tilePrefab, gameObject.transform) as GameObject).GetComponent<INagavable>();
                    Vector2Int coord = (Vector2Int.right * x) + (Vector2Int.up * y);
                    //Se añade al map
                    Map.Add(go);
                    go.Coordinates = coord;

                    TileBatch batch= lst_map.First((x) => !x.HasPassBoundary(go.Coordinates));
                    if(batch.en_dir!= Direction.None)
                    {
                        batch.AddTile(go);
                    }
                    

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
        public Vector3 UpdatePlayerPos(Vector3 _plPos)
        {
            //Ha pasado la barrera de el pivote actual (central)
            if (!TileBatch.HasPassBoundary(_plPos))
                return TileBatch.Center.v2_pivot;

            Vector2 v_dir = TileBatch.GetPlayerDirection(_plPos);
            Direction cdir = MapFunctions.CalculateDir(v_dir * -1) & (~Direction.C);
            Direction dir = MapFunctions.CalculateDir(v_dir);
            TileBatch newCenter = lst_map.First((x) => x.en_dir == dir);
            dir = dir & (~Direction.C);
            TileBatch.UpdateCenter(newCenter);
            print($"<color=green>{_plPos}|{v_dir}|{dir}|{cdir}|{newCenter.v2_pivot}");
            //if (Mathf.Log((int)cdir,2) % 1 >0)
            //Actualizar central
            foreach (TileBatch tb in lst_map)
            {
                Direction conterPart = Direction.None;
                //Actualiza posicion de linea opuesta
                //Actualiza posocion
                if ((tb.en_dir & cdir) > 0)
                {
                    print($"<color=red>Dir: {tb.en_dir} |Pivot: {tb.v2_pivot} | Move on one {cdir}  from {dir}");
                    conterPart = (tb.en_dir & (~cdir)) | dir;
                    tb.UpdateBatch(conterPart);
                    tb.MoveBatch();
                    
                }
                //Actulizar a linea de direccion
                else if ((tb.en_dir & dir) > 0)
                {
                    print($"<color=blue> Dir: {tb.en_dir} |Pivot: {tb.v2_pivot} | Move on one {dir}  from {Direction.C}");
                    conterPart = (tb.en_dir & (~dir)) | Direction.C;
                    tb.UpdateBatch(conterPart);

                }
                //Actualiza linea del centro
                else
                {
                    print($"Center Dir: {tb.en_dir} |Pivot: {tb.v2_pivot} | Move on one {Direction.C}  from {cdir}");
                    conterPart = tb.en_dir;
                    if (tb.en_dir!=Direction.C)
                        conterPart = (conterPart & (~Direction.C));

                    conterPart = conterPart | cdir;

                    tb.UpdateBatch(conterPart);

                }

                print($"From C: {TileBatch.Center.v2_pivot}|{tb.en_dir} | pivot : {tb.v2_pivot}");
            }
            foreach (TileBatch tb in lst_map) {
                print($"<color=yellow>{tb.en_dir} | pivot : {tb.v2_pivot}");
            }
            print($"<color=green>{TileBatch.Center.en_dir} | pivot : {TileBatch.Center.v2_pivot}");

            return TileBatch.Center.v2_pivot;
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
    [SerializeField]
    public class Batch_R_Tile
    {
        public Vector2Int coord;
        public INagavable tile;
    }

    [SerializeField]
    public class TileBatch
    {
        public static TileBatch Center;
        public static float Step;

        public Direction en_dir;
        public Vector2 v2_pivot;
        public Vector2 v2_centerRel;
        public List<Batch_R_Tile> lst_batch;


        public TileBatch(Direction _dir,float _batchDistance)
        {
            v2_pivot = Vector2.zero;
            v2_centerRel = Vector2.zero;
            lst_batch = new List<Batch_R_Tile>();
            en_dir = _dir;

            if (_dir == Direction.C)
            {
                Center = this;
                Step = _batchDistance;
            }
                
            v2_centerRel = MapFunctions.CalculateDir(_dir);
            v2_pivot = v2_centerRel * _batchDistance;
        }
        //Revisa si se han pasado los limites del bathc
        public static bool HasPassBoundary(Vector3 _pos)
        {
            Vector3 relativePos = _pos;
            relativePos.x -= Center.v2_pivot.x;
            relativePos.z -= Center.v2_pivot.y;
            float d = Step / 2;
            return (relativePos.x > d || relativePos.x <d) ||
                (relativePos.z > d || relativePos.z < d);
        }
        public bool HasPassBoundary(Vector2Int _pos)
        {
            float d =Step/ 2;
            return (_pos.x > v2_pivot.x+d || _pos.x < v2_pivot.x - d) || 
                (_pos.y > v2_pivot.y+d || _pos.y < v2_pivot.y - d);
        }
        public static Vector2 GetPlayerDirection(Vector3 _pos)
        {
            Vector2 boundery = Vector2.one;
            Vector3 relativePos = _pos;
            relativePos.x -= Center.v2_pivot.x;
            relativePos.z -= Center.v2_pivot.y;
            float d = Step / 2;
            boundery.x = (Mathf.Abs(relativePos.x) > d) ? Mathf.Sign(relativePos.x) : 0f;
            boundery.y = (Mathf.Abs(relativePos.z) > d) ? Mathf.Sign(relativePos.z) : 0f;
            return boundery;
        }
        //Actualiza el centro con un nuevo batch
        public static void UpdateCenter(TileBatch _newCenter)
        {
            Center = _newCenter;
        }
        public void UpdateBatch(Direction _dir)
        {
            en_dir =_dir;
            v2_centerRel = MapFunctions.CalculateDir(_dir);
        }
        //Actauliza el valor de la formula por batch
        public void UpdateItems()
        {
            foreach(Batch_R_Tile n in lst_batch)
            {
                n.tile.UpdateSelected(MapFunctions.Formula(n.tile.Coordinates));
            }
        }
        //Agraga un tile a el batch con el que trabajara
        public void AddTile(INagavable _tile)
        {
            int x = (_tile.Coordinates.x < v2_pivot.x) ? Mathf.CeilToInt(v2_pivot.x) : Mathf.FloorToInt(v2_pivot.x);
            int y = (_tile.Coordinates.y < v2_pivot.y) ? Mathf.CeilToInt(v2_pivot.y) : Mathf.FloorToInt(v2_pivot.y);
            Batch_R_Tile rel = new Batch_R_Tile
            {
                tile = _tile,
                coord = new Vector2Int(_tile.Coordinates.x - x, _tile.Coordinates.y - y)
            };
            lst_batch.Add(rel);
        }
        //Camiba posicion del batch con respecto al pivote del batch
        //El pivote es un conceto no visible en el escenario solo una coordenada
        public void MoveBatch()
        {
            //Actualiza posicion con su relacion con el centro
            v2_pivot = Center.v2_pivot + (v2_centerRel*Step);
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