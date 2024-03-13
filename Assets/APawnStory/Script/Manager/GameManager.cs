using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public static GameMode gm_gamemode
    {
        set;
        get;
    }

    [RuntimeInitializeOnLoadMethod]
    public static void Initialized()
    {
        if (Instance != null)
            return;

        //Start the manager in any scene
        GameObject go = new GameObject("Manager");
        Instance = go.AddComponent<GameManager>();
        Instance.LoadGameMode();
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
        //Load the gamemode into the scene
        gm_gamemode = gameObject.AddComponent<GameModeGameplay>();
    }

    
}
