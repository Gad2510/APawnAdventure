using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TilesManager : MonoBehaviour
{
    private List<MeshRenderer> lst_tiles;

    private MeshRenderer mr_selectedTile;
    private Tuple<Color,bool> t_lastState;

    private bool[,] mtx_bitmap;
    private const int int_bitmapZise=25;
    private int int_amountTiles;

    public Texture2D attackMap;

    private GameModeGameplay gm_gamemode;

    private const string TILE_TAG = "Tile";
    public static string UNTAGGED = "Untagged";
    // Start is called before the first frame update
    void Start()
    {
        attackMap = Resources.Load<Texture2D>("Textures/AtaqueSencillo");

        mtx_bitmap = new bool[int_bitmapZise, int_bitmapZise];
        lst_tiles = new List<MeshRenderer>();

        gm_gamemode = ((GameModeGameplay)GameManager.gm_gamemode);

        gm_gamemode.UndoAction += UndoAction;
    }

    private void OnDestroy()
    {
        gm_gamemode.UndoAction -= UndoAction;
    }

    public void SetCursorTile(MeshRenderer _tile)
    {
        if (mr_selectedTile != null)
        {
            mr_selectedTile.material.color =t_lastState.Item1;
            mr_selectedTile.enabled =t_lastState.Item2;
        }

        mr_selectedTile = _tile;
        t_lastState =Tuple.Create(mr_selectedTile.material.color,mr_selectedTile.enabled);
        mr_selectedTile.enabled = true;
        mr_selectedTile.material.color = Color.red;


    }

    public void SetMovementTiles(Vector2Int _origin, int _size)
    {
        //Calculate spaces to create area
        int_amountTiles = 0;
        int middle = (int)Mathf.Ceil(int_bitmapZise / 2);
        CalculateBitMap(ref mtx_bitmap, _size, Vector2Int.one * middle);

        //Set tiles in place and set them active
        Vector2Int offset = Vector2Int.one;

        for (int y = 0; y < int_bitmapZise && int_amountTiles > 0; y++)
        {
            for(int x=0;x< int_bitmapZise && int_amountTiles>0; x++)
            {
                if (mtx_bitmap[x, y])
                {
                    offset.x = _origin.x + x-middle;
                    offset.y = _origin.y + y-middle;
                    SetTileToType(offset, Color.blue);
                    mtx_bitmap[x, y] = false;
                }
            }
        }
    }

    public void SetAttackTiles(Vector2Int _origin)
    {
        int middleX = (int)Mathf.Ceil((attackMap.width-1) / 2);

        for (int y = 0; y < attackMap.height; y++)
        {
            for (int x = 0; x < attackMap.width; x++)
            {
                Color pixel=attackMap.GetPixel(x, y);
                if (pixel == Color.black)
                {
                    Vector2Int pos = new Vector2Int(x - middleX, y - middleX) + _origin;
                    SetTileToType(pos, Color.yellow, UNTAGGED);
                    gm_gamemode.CombatManager.AddCharacterInRange(pos);
                }
            }
        }
    }

    private void SetTileToType(Vector2Int _coord, Color _color, string _tag=TILE_TAG)
    {
        MeshRenderer mr = gm_gamemode.MapManager.UpdateTile(_coord, _color);
        if (mr != null)
        {
            lst_tiles.Add(mr);
            mr.gameObject.tag = _tag;
        }
    }

    public void CalculateBitMap(ref bool [,] _map,int _size, Vector2Int _origin)
    {
        for(int i = 1; i <=_size; i++)
        {
            //Generate the consecutive side to form a cross
            _map[_origin.x - i, _origin.y] = true;
            _map[_origin.x + i, _origin.y] = true;
            _map[_origin.x, _origin.y - i] = true;
            _map[_origin.x, _origin.y + i] = true;
            //Fill the rest of the gaps
            for (int x = 1; x <i; x++)
            {
                _map[_origin.x + x, _origin.y+(i-x)] = true;
                _map[_origin.x - x, _origin.y+(i-x)] = true;
                _map[_origin.x + (i-x), _origin.y - x] = true;
                _map[_origin.x - (i-x), _origin.y - x] = true;

                int_amountTiles+=4;
            }
            int_amountTiles += 4;
        }
    }

    public void HideTiles()
    {
        if (lst_tiles==null || lst_tiles.Count == 0)
            return;

        foreach(MeshRenderer c in lst_tiles)
        {
            c.gameObject.tag = UNTAGGED;
            c.enabled=false;
        }

        lst_tiles.Clear();
    }

    public void UndoAction(GameModeGameplay.GameState _phase)
    {
        if (_phase != GameModeGameplay.GameState.Moving && _phase!=GameModeGameplay.GameState.Attacking)
            return;

        HideTiles();
    }
}
