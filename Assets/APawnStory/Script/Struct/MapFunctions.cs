using UnityEngine;

namespace Betadron.Struct
{
    public class MapFunctions
    {
        //Funciones generales usadas para el mapa
        private static readonly Vector3 v3_origin = new Vector3(0.5f, 0.2f, 0.5f);

        //Convierte la cordenada en el mapa por una posicion en 3D
        public static Vector3 GetLocation4Coord(Vector2Int _coord)
        {
            Vector3 newLoc= v3_origin + Vector3.right * _coord.x + Vector3.forward * _coord.y;
            return newLoc;
        }
        //Convierte posicion 3D en las coordenas del objeto en el grid
        public static Vector2Int GetCoordn4Location(Vector3 _coord)
        {
            Vector2Int newLoc = new Vector2Int((int)_coord.x, (int)_coord.z);
            return newLoc;
        }
    }
}
