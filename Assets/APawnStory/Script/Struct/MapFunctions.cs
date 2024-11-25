using System.Collections.Generic;
using UnityEngine;

namespace Betadron.Struct
{
    public enum Direction
    {
        None=0,
        C = 1 << 0,
        U = 1 << 1,
        D = 1 << 2,
        L = 1 << 3,
        R = 1 << 4,

        CU = C | U,
        CD = C | D,
        CR = C | R,
        CL = C | L,
        LU = L | U,
        LD = L | D,
        RU = R | U,
        RD = R | D,

        Horizontal= L|R,
        Vertical=U|D
    }
    public class MapFunctions
    {
        //Variables funcion objetivo
        private static float sigma = 50f;
        private static float fx = 24f;
        private static float fy = 26f;
        private static float gameTime = 0f;

        public static float Sigma { get=>sigma; set=>sigma=value; }
        public static float FX { get=>fx; set=>fx=value; }
        public static float FY { get=>fy; set=>fy=value; }
        public static float GameTime { get => gameTime; set => gameTime = value; }

        //Funciones generales usadas para el mapa
        private static readonly Vector3 v3_origin = new Vector3(0.5f, 0.2f, 0.5f);

        public static Dictionary<Vector2, Direction> vector2dir = new Dictionary<Vector2, Direction>
        {
            {Vector2.zero ,Direction.C},
            {Vector2.left ,Direction.L},
            {Vector2.right,Direction.R},
            {Vector2.down ,Direction.D},
            {Vector2.up   ,Direction.U}
        };

        //Convierte la cordenada en el mapa por una posicion en 3D
        public static Vector3 GetLocation4Coord(Vector2Int _coord)
        {
            Vector3 newLoc= v3_origin + Vector3.right * _coord.x + Vector3.forward * _coord.y;
            return newLoc;
        }
        //Convierte posicion 3D en las coordenas del objeto en el grid
        public static Vector2Int GetCoordn4Location(Vector3 _coord)
        {
            Vector2Int newLoc = new((int)_coord.x, (int)_coord.z);
            return newLoc;
        }
        //Regresa el vector multiplicado de cada uno de sus ejes
        public static Vector3 MultiplyVectors(Vector3 _v1, Vector3 _v2)
        {
            return new Vector3(_v1.x*_v2.x, _v1.y * _v2.y, _v1.z * _v2.z);
        }
        public static float CalculateNormalizeDistance(Vector3 _v)
        {
            Vector3 normal = Vector3.Normalize(_v);
            return Vector3.Distance(Vector3.zero,MultiplyVectors(_v, normal));
        }
        //Calcula la direccion del pivote con relaicon a la direccion dada
        public static Vector2 CalculateDir(Direction _dir)
        {
            Vector2 dir = Vector2.zero;
            if ((Direction.U & _dir) > 0)
            {
                dir += Vector2.up;
            }
            else if ((Direction.D & _dir) > 0)
            {
                dir += Vector2.down;
            }

            if ((Direction.L & _dir) > 0)
            {
                dir += Vector2.left;
            }
            else if ((Direction.R & _dir) > 0)
            {
                dir += Vector2.right;
            }

            return dir;
        }
        //Calcula la direccion del pivote con relaicon a la direccion dada
        public static Direction CalculateDir(Vector2 _dir)
        {
            Vector2 vh = (Vector2.right*_dir.x).normalized;
            Vector2 vv = (Vector2.up *_dir.y).normalized;

            Direction h = vector2dir[vh];
            Direction v = vector2dir[vv];

            return h|v;
        }
        //Funcion Objetivo a evaluar con el tiempo
        public static float Formula(Vector2 coord)
        {
            //TESTING
            //float t = Time.time / 10;
            float t = gameTime;
            float result = 0;
            
            coord = coord+(Vector2.one*t);
            result = EvaluateAxis(coord.x,0)+ EvaluateAxis(coord.y,1);
            return result;
        }

        private static float EvaluateAxis(float _axis, float index)
        {
            float exp = 0;
            float trian = 0;
            float baseM = index % 2;
            switch (baseM)
            {
                case 0:
                    exp = Mathf.Pow(_axis, 3);
                    trian = 10 * (Mathf.Cos(2 * Mathf.PI * _axis * FY) + Mathf.Sin(_axis * FX));
                    return (exp * trian) / (2 * Mathf.Pow(Sigma, 2));
                case 1:
                    exp = Mathf.Pow(_axis, 3);
                    trian = 10 * (Mathf.Sin(2 * Mathf.PI * _axis * FX) + Mathf.Cos(_axis * FY));
                    return (exp * trian) / (2 * Mathf.Pow(Sigma, 2));
                default:
                    return 0;
            }

            
        }
    }
}
