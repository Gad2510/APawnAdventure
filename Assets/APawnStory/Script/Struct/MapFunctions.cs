using UnityEngine;

namespace Betadron.Struct
{
    public class MapFunctions
    {
        //Variables funcion objetivo
        private static float sigma = 25;
        private static float fx = 29.4f;
        private static float fy = 39.3f;

        public static float Sigma { get=>sigma; set=>sigma=value; }
        public static float FX { get=>fx; set=>fx=value; }
        public static float FY { get=>fy; set=>fy=value; }

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

        //Funcion Objetivo a evaluar con el tiempo
        public static float Formula(Vector2 coord)
        {
            //TESTING
            float t = Time.time / 10;

            coord = new Vector2(Mathf.Sin((coord.x) + t), Mathf.Cos((coord.y) + t));
            float exp = Mathf.Exp(((Mathf.Pow(coord.x, 2)) + (Mathf.Pow(coord.y, 2))) / (2 * Mathf.Pow(Sigma, 2)));
            float trian = Mathf.Cos((coord.y * FY)) * Mathf.Sin((coord.x * FX));
            return (exp * trian);
        }
    }
}
