using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

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

    //Origen de los tiles en el mundo
    private readonly Vector3 v3_origin = new Vector3(0.5f, 0.2f, 0.5f);

    //Referencia de los tiles en el mapa organizados por sus cordenadas
    public Dictionary<Vector2Int,Tile> Map { get; private set; }

    // Start is called before the first frame update
    void Start()
    {
        //Inicializa el diccionario
        Map = new Dictionary<Vector2Int, Tile>();
        //Obtiene referencia del prefab
        obj_tilePrefab = Resources.Load("Prefabs/MovementTile");
        //TESTING
        GenerateMap(mapSize);
        
    }

    // Update is called once per frame
    void Update()
    {
        //TESTING
        foreach(Tile t in Map.Values)
        {
            t.SetTempeture(Formula(t.v2i_coord));
        }
    }
    //CRUD
    //Create
    //Genera el mapa
    private void GenerateMap(int _mapSize)
    {
        for(int x = 0; x < _mapSize; x++)
        {
            for(int y = 0; y < _mapSize; y++)
            {
                //Instancea un tile del mapa en las coordenadas
                Tile go = (Instantiate(obj_tilePrefab, gameObject.transform) as GameObject).GetComponent<Tile>();
                Vector2Int coord = new(x, y);
                //Se añade al map
                Map.Add(coord, go);
                //Se le da su posicion con respecto al origen
                go.transform.position = v3_origin+(Vector3.right*x)+(Vector3.forward*y);

                go.SetCoordinate(coord);
                go.SetTempeture(Formula(coord));
            }
        }
    }
    //Update
    //Actualiza el tile para que reaccione acorde a las interacciones (Hover mouse, player range, etc)
    public MeshRenderer UpdateTile(Vector2Int _pos,Color _col, bool _enable=true)
    {
        if (!Map.ContainsKey(_pos))
            return null;

        Map[_pos].enabled = _enable;

        Map[_pos].mr_renderer.material.color = _col;

        return Map[_pos].mr_renderer;
    }
    public Vector2Int GetTileCoordinate(MeshRenderer _tile)
    {
        Vector2Int coord= Map.FirstOrDefault((x) => x.Value == _tile).Key;
        return coord;
    }
    //Funcion Objetivo a evaluar con el tiempo
    private float Formula(Vector2 coord)
    {
        //TESTING
        float t = Time.time/10;

        coord = new Vector2(Mathf.Sin((coord.x) + t), Mathf.Cos((coord.y) + t));
        float exp = Mathf.Exp(((Mathf.Pow(coord.x , 2)) + (Mathf.Pow(coord.y ,2))) / (2 * Mathf.Pow(sigma ,2)));
        float trian =  Mathf.Cos((coord.y*fy)) * Mathf.Sin((coord.x*fx));
        return (exp * trian)*5;
    }
}
