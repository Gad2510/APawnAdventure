using UnityEngine;
using Betadron.Interfaces;
using Betadron.Managers;

namespace Betadron.Pawn
{
    public class Obstacle : Pawn, IAged
    {
        public int Life { get ; private set ; }

        protected override void Start()
        {
            base.Start();
            print(Coordinates);
            Born();
        }

        public void Aged()
        {
            return;
        }

        public void Born()
        {
            ((GameModeGameplay)GameManager.gm_gamemode).CharacterManager.AddObject(this);
        }

        public void Die()
        {
            ((GameModeGameplay)GameManager.gm_gamemode).CharacterManager.RemoveObject(this);
        }
    }
}
