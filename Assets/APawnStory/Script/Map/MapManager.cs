using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    public Object obj_tilePrefab;
    public float sigma = 25;
    public float fx = 29.4f;
    public float fy = 39.3f;


    private readonly Vector3 v3_origin = new Vector3(0.5f, 0, 0.5f);

    public Dictionary<Vector2Int,Tile> Map { get; private set; }

    // Start is called before the first frame update
    void Start()
    {
        Map = new Dictionary<Vector2Int, Tile>();
        obj_tilePrefab = Resources.Load("Prefabs/MovementTile");
        GenerateMap(20);
        
    }

    // Update is called once per frame
    void Update()
    {
        foreach(Tile t in Map.Values)
        {
            t.SetTempeture(Formula(t.v2i_coord));
        }
    }

    private void GenerateMap(int _mapSize)
    {
        int middle = Mathf.FloorToInt(_mapSize / 2);
        for(int x = 0; x < _mapSize; x++)
        {
            for(int y = 0; y < _mapSize; y++)
            {
                Tile go = (Instantiate(obj_tilePrefab, gameObject.transform) as GameObject).GetComponent<Tile>();
                Vector2Int coord = new Vector2Int(x, y);
                Map.Add(coord, go);
                go.transform.position = new Vector3(v3_origin.x+x, 0.2f, v3_origin.z+y);
                go.tag = TilesManager.UNTAGGED;

                go.SetCoordinate(coord);
                go.SetTempeture(Formula(coord));
            }
        }
    }

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
    
    private float Formula(Vector2 coord)
    {
        //Onda de Gabor Bidimensional
        /*float exp = Mathf.Exp(-((Mathf.Pow(coord.x , 2)) + (Mathf.Pow(coord.y ,2))) / (2 * Mathf.Pow(sigma ,2)));
        float trian = Mathf.Cos(2 * Mathf.PI * frecuency * ((coord.x * Mathf.Cos(orientation)) + (coord.y * Mathf.Sin(orientation))) + fase);

        return exp * trian;*/

        //Ruido aleatorio
        float t = Time.time/10;

        //coord = new Vector2(coord.x+t, coord.y+t);
        coord = new Vector2(Mathf.Sin((coord.x) + t), Mathf.Cos((coord.y) + t));
        float exp = Mathf.Exp(((Mathf.Pow(coord.x , 2)) + (Mathf.Pow(coord.y ,2))) / (2 * Mathf.Pow(sigma ,2)));
        float trian =  Mathf.Cos((coord.y*fy)) * Mathf.Sin((coord.x*fx));
        return (exp * trian)*5;
    }
}
