using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GameMode : MonoBehaviour
{
    //Clase encargada de controles de juego asi como el game flow
    //Esta solo contiene cualquier tipo de funcion que se requiera compartir entre modos:
    //  -Empezar la escena
    //  -Cargar UI 

    protected virtual void Awake()
    {
        StartScene();
    }
    //Carga elementos de la escena
    private void StartScene()
    {
        LoadUI();
    }
    //Carga el UI
    protected abstract void LoadUI();

}
