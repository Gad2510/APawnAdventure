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
        public Dictionary<Vector2Int, IInteractable> Map { get; private set; }

        // Start is called before the first frame update
        void Start()
        {
            //Inicializa el diccionario
            Map = new Dictionary<Vector2Int, IInteractable>();
            //Obtiene referencia del prefab
            obj_tilePrefab = Resources.Load("Prefabs/MovementTile");
            //TESTING
            GenerateMap(mapSize);

        }

        // Update is called once per frame
        void Update()
        {
            //TESTING
            foreach (IInteractable t in Map.Values)
            {
                t.UpdateSelected(Formula(t.Coordinates));
            }
        }
        //CRUD
        //Create
        //Genera el mapa
        private void GenerateMap(int _mapSize)
        {
            for (int x = 0; x < _mapSize; x++)
            {
                for (int y = 0; y < _mapSize; y++)
                {
                    //Instancea un tile del mapa en las coordenadas
                    IInteractable go = (Instantiate(obj_tilePrefab, gameObject.transform) as GameObject).GetComponent<IInteractable>();
                    Vector2Int coord= (Vector2Int.right * x) + (Vector2Int.up * y);
                    //Se añade al map
                    Map.Add(coord, go);

                    go.Coordinates = coord;
                    go.UpdateSelected(Formula(coord));
                }
            }
        }
        //Update
        //Actualiza el tile para que reaccione acorde a las interacciones (Hover mouse, player range, etc)
        public MeshRenderer UpdateTile(Vector2Int _pos, Color _col, bool _enable = true)
        {
            if (!Map.ContainsKey(_pos))
                return null;

            MeshRenderer msr = (MeshRenderer)Map[_pos].OnSelect();
            msr.enabled = _enable;
            msr.material.color = _col;

            return msr;
        }//Read
         //Obtiene la referencia de las coordenas por el mesh renderer del objeto
        public Vector2Int GetTileCoordinate(MeshRenderer _tile)
        {
            Vector2Int coord = Map.FirstOrDefault((x) => (MeshRenderer)x.Value.OnSelect() == _tile).Key;
            return coord;
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