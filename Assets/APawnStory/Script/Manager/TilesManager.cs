using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace  Betadron.Managers
{
    public class TilesManager : MonoBehaviour
    {
        /*
         * Clase encargada de actualizar los tiles (visualizacion )
         */
        //Lista de tiles seleccionados o cambiados para regresar a estado original
        private List<MeshRenderer> lst_selectedTiles;
        //Meshrenderer del tile especifico seleccionado por el jugador
        private MeshRenderer mr_selectedTile;
        //Estado anterior del tile seleccionado por el jugador 
        private Tuple<Color, bool> t_lastState;

        private bool[,] mtx_bitmap;
        private const int int_bitmapZise = 25;
        private int int_amountTiles;

        public Texture2D attackMap;
        //Referencia al game mode
        private GameModeGameplay gm_gamemode;

        private const string TILE_TAG = "Tile";
        public static string UNTAGGED = "Untagged";
        // Start is called before the first frame update
        void Start()
        {
            attackMap = Resources.Load<Texture2D>("Textures/AtaqueSencillo");

            mtx_bitmap = new bool[int_bitmapZise, int_bitmapZise];
            lst_selectedTiles = new List<MeshRenderer>();

            gm_gamemode = ((GameModeGameplay)GameManager.gm_gamemode);

            gm_gamemode.UndoAction += UndoAction;
        }

        private void OnDestroy()
        {
            gm_gamemode.UndoAction -= UndoAction;
        }
        //Cambia de color el tile que el jugador esta señalando con el mouse
        public void SetCursorTile(MeshRenderer _tile)
        {
            //Revisa si ya habia uno seleccionado para regresarlo a su estado previo
            if (mr_selectedTile != null)
            {
                mr_selectedTile.material.color = t_lastState.Item1;
                mr_selectedTile.enabled = t_lastState.Item2;
            }
            //Guarda la referencia del tile seleccionado y le asigna su color
            mr_selectedTile = _tile;
            t_lastState = Tuple.Create(mr_selectedTile.material.color, mr_selectedTile.enabled);
            mr_selectedTile.enabled = true;
            mr_selectedTile.material.color = Color.red;


        }
        //Cambia de color los tiles disponibles para el movimineto
        public void SetMovementTiles(Vector2Int _origin, int _size)
        {
            //Calcula los tiles que cambiaran 
            int_amountTiles = 0;
            int middle = (int)Mathf.Ceil(int_bitmapZise / 2);
            mtx_bitmap = CalculateBitMap(_size, Vector2Int.one * middle);

            //Actva los tiles y les asigna su color indicado
            Vector2Int offset = Vector2Int.one;

            for (int y = 0; y < int_bitmapZise && int_amountTiles > 0; y++)
            {
                for (int x = 0; x < int_bitmapZise && int_amountTiles > 0; x++)
                {
                    if (mtx_bitmap[x, y])
                    {
                        offset.x = _origin.x + x - middle;
                        offset.y = _origin.y + y - middle;
                        SetTileToType(offset, Color.blue);
                        mtx_bitmap[x, y] = false;
                    }
                }
            }
        }
        //Activa los tiles para la accion de ataque o seleccion
        public void SetAttackTiles(Vector2Int _origin)
        {
            int middleX = (int)Mathf.Ceil((attackMap.width - 1) / 2);

            for (int y = 0; y < attackMap.height; y++)
            {
                for (int x = 0; x < attackMap.width; x++)
                {
                    Color pixel = attackMap.GetPixel(x, y);
                    if (pixel == Color.black)
                    {
                        Vector2Int pos = new Vector2Int(x - middleX, y - middleX) + _origin;
                        SetTileToType(pos, Color.yellow, UNTAGGED);
                        gm_gamemode.CharacterManager.AddCharacterInRange(pos);
                    }
                }
            }
        }
        //Cambia el tipo de tile para seleccion
        private void SetTileToType(Vector2Int _coord, Color _color, string _tag = TILE_TAG)
        {
            MeshRenderer mr = gm_gamemode.MapManager.UpdateTile(_coord, _color);
            if (mr != null)
            {
                lst_selectedTiles.Add(mr);
                mr.gameObject.tag = _tag;
            }
        }
        //Calcula 
        public bool[,] CalculateBitMap(int _size, Vector2Int _origin)
        {
            bool[,] map = new bool[int_bitmapZise, int_bitmapZise];
            for (int i = 1; i <= _size; i++)
            {
                //Generate the consecutive side to form a cross
                map[_origin.x - i, _origin.y] = true;
                map[_origin.x + i, _origin.y] = true;
                map[_origin.x, _origin.y - i] = true;
                map[_origin.x, _origin.y + i] = true;
                //Fill the rest of the gaps
                for (int x = 1; x < i; x++)
                {
                    map[_origin.x + x, _origin.y + (i - x)] = true;
                    map[_origin.x - x, _origin.y + (i - x)] = true;
                    map[_origin.x + (i - x), _origin.y - x] = true;
                    map[_origin.x - (i - x), _origin.y - x] = true;

                    int_amountTiles += 4;
                }
                int_amountTiles += 4;
            }
            return map;
        }
        //Regresa los tiles a su estado original 
        public void HideTiles()
        {
            if (lst_selectedTiles == null || lst_selectedTiles.Count == 0)
                return;

            foreach (MeshRenderer c in lst_selectedTiles)
            {
                c.gameObject.tag = UNTAGGED;
                c.enabled = false;
            }

            lst_selectedTiles.Clear();
        }
        //Accion a realizar cuando se regresa a una accion anterior
        public void UndoAction(GameModeGameplay.CharacterTurn _phase)
        {
            if (_phase != GameModeGameplay.CharacterTurn.Moving && _phase != GameModeGameplay.CharacterTurn.Attacking)
                return;

            HideTiles();
        }
    }
}