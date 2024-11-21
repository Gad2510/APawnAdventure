using System;
using UnityEngine;
using UnityEngine.UI;
using Betadron.Managers;

namespace Betadron.UI
{
    public class UI_Simulation : MonoBehaviour
    {
        [SerializeField]
        private Button btn_StartSimulation;

        private GameModeGameplay gm_gamemode;
        public int NumTurnos
        {
            get
            {
                if (gm_gamemode != null)
                {
                    return gm_gamemode.TurnToEvaluate;
                }

                return 0;
            }
            set
            {
                if (gm_gamemode != null)
                {
                    print("Assign value");
                    gm_gamemode.TurnToEvaluate=value;
                }
            }
        }
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            gm_gamemode = (GameManager.gm_gamemode as GameModeGameplay);

            btn_StartSimulation.onClick.AddListener(() => gm_gamemode.StartSimulation());
        }

        public void SetTurns(string value)
        {
            NumTurnos = Convert.ToInt32(value);
        }

    }
}
