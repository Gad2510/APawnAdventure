using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Betadron.Objects;
namespace Betadron.Managers
{
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
            Instance.LoadGameReferences();
            //Caraga el gamemode
            Instance.LoadGameMode();
            //Hace que el objeto no se destruya entre cada escena
            DontDestroyOnLoad(go);
        }
        private LevelRecord lv_record;
        private LevelDef lv_definition;
        public LevelDef LevelInfo { get => lv_definition; private set => lv_definition = value; }

        private LoadSceneManager lv_manager;
        public LoadSceneManager LoadSceneManager { get => lv_manager; private set => lv_manager = value; }

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
        //Carga referencias usadas en el juego
        private void LoadGameReferences()
        {
            Instance.LoadSceneManager = gameObject.AddComponent<LoadSceneManager>();
            Instance.lv_record = Resources.Load<LevelRecord>("ScriptableObjects/Levels");
        }

        private void LoadGameMode()
        {
            string sceneName= Instance.LoadSceneManager.GetSceneName();
            Instance.LevelInfo = Instance.lv_record.GetLevelRecord(sceneName);
            Modes lv_mode = Instance.LevelInfo.gameMode;
            GameMode gm;
            switch (lv_mode)
            {
                case Modes.Game:
                    {
                        gm = gameObject.AddComponent<GameModeGameplay>();
                        break;
                    }
                default:
                    {
                        gm = gameObject.AddComponent<GameModeGameplay>();
                        break;
                    }
            }
            //Carga el GameMode de la escena
            gm_gamemode = gm;
        }


    }
}