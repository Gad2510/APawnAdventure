using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Betadron.Interfaces;
namespace Betadron.Pawn
{
    public class Tile : MonoBehaviour, IInteractable
    {
        /*
         Clase de los tiles del mapa 
         */
        //Color de los tiles 

        //Origen de los tiles en el mundo
        private readonly Vector3 v3_origin = new Vector3(0.5f, 0.2f, 0.5f);

        [SerializeField]
        private Gradient g_heatMap;

        public float f_tempeture;
        private MeshRenderer mr_renderer;
        private Vector2Int v2i_coord;
       
        public Vector2Int Coordinates
        {
            get { return v2i_coord; }
            set
            {
                //Actualiza posicion
                transform.position= v3_origin+ Vector3.right*value.x + Vector3.forward*value.y;
                v2i_coord = value;
            }
        }

        private void Awake()
        {
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
    }
}