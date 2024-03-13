using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MapManager : MonoBehaviour
{

    public Object obj_tilePrefab;


    private readonly Vector3 v3_origin = new Vector3(0.5f, 0, 0.5f);

    public Dictionary<Vector2Int,MeshRenderer> Map { get; private set; }

    // Start is called before the first frame update
    void Start()
    {
        Map = new Dictionary<Vector2Int, MeshRenderer>();
        obj_tilePrefab = Resources.Load("Prefabs/MovementTile");
        GenerateMap(20);
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void GenerateMap(int _mapSize)
    {
        int middle = Mathf.FloorToInt(_mapSize / 2);
        for(int x = 0; x < _mapSize; x++)
        {
            for(int y = 0; y < _mapSize; y++)
            {
                GameObject go = Instantiate(obj_tilePrefab, gameObject.transform) as GameObject;
                Map.Add(new Vector2Int(x, y), go.GetComponent<MeshRenderer>());
                go.transform.position = new Vector3(v3_origin.x+x, 0.2f, v3_origin.z+y);
                go.tag = TilesManager.UNTAGGED;
            }
        }
    }

    public MeshRenderer UpdateTile(Vector2Int _pos,Color _col, bool _enable=true)
    {
        if (!Map.ContainsKey(_pos))
            return null;

        Map[_pos].enabled = _enable;

        Map[_pos].material.color = _col;

        return Map[_pos];
    }
    public Vector2Int GetTileCoordinate(MeshRenderer _tile)
    {
        Vector2Int coord= Map.FirstOrDefault((x) => x.Value == _tile).Key;
        return coord;
    }
    
}
