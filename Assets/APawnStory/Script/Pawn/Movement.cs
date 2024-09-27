using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Betadron.Managers;
namespace Betadron.Pawn
{
    public class Movement : MonoBehaviour
    {
        private Character scp_char;
        private GameModeGameplay gm_gamemode;

        private void Start()
        {
            gm_gamemode = ((GameModeGameplay)GameManager.gm_gamemode);
            scp_char = gameObject.GetComponent<Character>();


        }
        public void UpdateStamina(Vector3 _steps)
        {
            Vector3 dif = _steps - transform.position;
            int stepMove = (int)Mathf.Abs(dif.x) + (int)Mathf.Abs(dif.z);
            scp_char.CharacterStats.UpdateStamina(stepMove);
            scp_char.IsMovable = scp_char.CharacterStats.int_movement > 0;
        }

        public void MoveCharacter(Vector3 _steps)
        {
            UpdateStamina(_steps);
            //TEMPORAL
            _steps.y = transform.position.y;
            transform.position = _steps;
            //Actualiza coordenadas
            scp_char.UpdateSelected(0);
            gm_gamemode.TilesManager.HideTiles();
        }


    }
}