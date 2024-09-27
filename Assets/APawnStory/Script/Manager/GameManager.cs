using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    //Singleton : - Referencia global de la clase Gamemanager para usar en el proyecto
    public static GameManager Instance;
    //Variable statica para que todo el proyecto tenga acceso al gamemode 
    public static GameMode gm_gamemode
    {
        set;
        get;
    }
    
    [RuntimeInitializeOnLoadMethod]
    public static void Initialized()
    {
        //Revisa si esta es la primera vez que se ejecuta 
        if (Instance != null)
            return;

        //Función que se ejecuta al iniciar cualquier escena
        //Crea un objeto vacio al cual le agrega el comopnente de GameManager
        GameObject go = new GameObject("Manager");
        Instance = go.AddComponent<GameManager>();
        //Caraga el gamemode
        Instance.LoadGameMode();
        //Hace que el objeto no se destruya entre cada escena
        DontDestroyOnLoad(go);
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    private void LoadGameMode()
    {
        //Caraga el GameMode de la escena
        //-Como solo hay uno se carga el default
        gm_gamemode = gameObject.AddComponent<GameModeGameplay>();
    }

    
}
