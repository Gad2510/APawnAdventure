using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour, IInteractable
{
    /*
     Clase de los tiles del mapa 
     */
    //Color de los tiles 
    [SerializeField]
    private Gradient g_heatMap;

    public float f_tempeture;
    public MeshRenderer mr_renderer;
    public Vector2Int v2i_coord;
    public Vector2Int Coordinates { get; set; }

    
    private void Awake()
    {
        mr_renderer = GetComponent<MeshRenderer>();
        gameObject.tag = TilesManager.UNTAGGED;
    }
    //Asignar temperatura para mostrar en pantalla
    public void SetTempeture(float amount)
    {
        f_tempeture = Mathf.Clamp01(amount);
        mr_renderer.material.color = g_heatMap.Evaluate(f_tempeture);
    }
    //Asignar coorendas a este tile
    public void SetCoordinate(Vector2Int coord)
    {
        v2i_coord = coord;
    }

    public void OnSelect()
    {
        return;
    }
}
