using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Betadron.Interfaces;
using Betadron.Struct;
namespace Betadron.Pawn
{
    public class Tile : MonoBehaviour, INagavable
    {
        /*
         Clase de los tiles del mapa 
         */
        //Color de los tiles 
        [SerializeField]
        private Gradient g_heatMap;

        public float f_tempeture;
        private MeshRenderer mr_renderer;

        public Vector2Int? navValues { get; set; }
        public INagavable Conected { get; set; }

        private Vector2Int v2i_coord;
        
        public Vector2Int Coordinates
        {
            get { return v2i_coord; }
            set
            {
                //Actualiza posicion
                transform.position = MapFunctions.GetLocation4Coord(value);
                v2i_coord = value;

                Name = v2i_coord.ToString();
            }
        }
        public string Name { get; set; }

        private bool hasObstacle=false;
        public bool HasObstacle { 
            get { return hasObstacle; }  
            set 
            {
                if (value)
                {
                    //Se elimina de la lista de los demas vecinos
                    Neighbors.ForEach(x => x.Neighbors.Remove(this));
                    hasObstacle = value;
                }
                else
                {
                    //Cambia su valor a falso
                    hasObstacle = value;
                    //Se registra en todos sus vecinos
                    Neighbors.ForEach(x => x.Neighbors.Add(this));
                }
            } 
        }
        private List<INagavable> neighbors;
        public List<INagavable> Neighbors {
            get 
            {
                if (HasObstacle)
                    return null;
                return neighbors;
            }
            private set
            {
                neighbors = value;
            }
        }

        private void Awake()
        {
            navValues = null;
            Conected = null;
            Neighbors = new List<INagavable>();
            mr_renderer = GetComponent<MeshRenderer>();
            gameObject.tag = "Untagged";
            mr_renderer.enabled = false;
        }
        //Cuando sea seleccionado regresa referencia al renderer
        public object OnSelect()
        {
            return mr_renderer;
        }
        //Assigna un valor a la temperatura
        public void UpdateSelected(object var)
        {
            f_tempeture = Mathf.Clamp01((float)var);
           // mr_renderer.material.color = g_heatMap.Evaluate(f_tempeture);
        }

        
        public Vector2Int GetNavegationCost(INagavable _other, Vector2Int _goal)
        {
            
            float H = Vector2Int.Distance(Coordinates, _other.Coordinates) *10;
            float G = Vector2Int.Distance(_other.Coordinates,_goal)*10;
            int F =(int)(H + G);
            return (Vector2Int.right * F) + (Vector2Int.up * (int)G);
        }

        public void AddNeighbor(INagavable _other)
        {
            Neighbors.Add(_other);
            _other.Neighbors.Add(this);
        }

        public void RemoveNeighbor(INagavable _other)
        {
            Neighbors.Remove(_other);
            _other.RemoveNeighbor(this); 
        }
    }
}