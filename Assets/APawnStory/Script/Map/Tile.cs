using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    [SerializeField]
    private Gradient g_heatMap;

    private float f_maxT=5;
    public float f_tempeture;
    public MeshRenderer mr_renderer;
    public Vector2Int v2i_coord;

    private void Awake()
    {
        mr_renderer = GetComponent<MeshRenderer>();

        gameObject.tag = TilesManager.UNTAGGED;
    }

    public void SetTempeture(float amount)
    {
        f_tempeture = Mathf.Clamp(amount,-f_maxT,f_maxT);
        mr_renderer.material.color = g_heatMap.Evaluate(f_tempeture/f_maxT);
    }

    public void SetCoordinate(Vector2Int coord)
    {
        v2i_coord = coord;
    }
}
