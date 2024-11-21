using System.Collections.Generic;
using UnityEngine;
using Betadron.Interfaces;
using Betadron.Managers;
using Betadron.Pawn;
using Betadron.Struct;
using System.Linq;
using System.Collections;

namespace Betadron.Pawn.NPC
{
    public abstract class NPC : Character
    {
        [SerializeField]
        protected int detectArea = 3;

        protected Stack<INagavable> lst_bestPath;
        protected IInteractable ifc_item2Colect;

        protected ItemManager scp_itemManager;
        protected GameModeGameplay gm_gamemode;


        public override bool EndPhase { get => base.EndPhase;
            set { 
                base.EndPhase = value;
                if (value && !gm_gamemode.IsPlayerTurn)
                    gm_gamemode.EnemyTurn();
            } 
        }

        protected override void Start()
        {
            gm_gamemode = ((GameModeGameplay)GameManager.gm_gamemode);
            scp_itemManager = gm_gamemode.SpawnManager;
            base.Start();

        }

        public abstract void ReachDestination();
        protected abstract void CollectItem(IColectable _item);

        //Busca el mojor spot para enocntrar comida
        //TESTING : Obtinene la unica fruta en el campo para seguirla
        protected Vector2Int RememberBestSpot()
        {
            PythonIDE.Cicles = 100;
            PythonIDE.Population = 10;
            PythonIDE.MinXValue = Coordinates.x - (int)(detectArea * 1.5);
            PythonIDE.MaxXValue = Coordinates.x + (int)(detectArea * 1.5);
            PythonIDE.MinYValue = Coordinates.y - (int)(detectArea * 1.5);
            PythonIDE.MaxYValue = Coordinates.y + (int)(detectArea * 1.5);

            PythonIDE.LoadPythonIDE(gm_gamemode.EvaluationTime*3);
            PythonRef codeRef= PythonIDE.ExecuteIDE();

            return new((int)codeRef.coord[0], (int)codeRef.coord[1]);
        }

        //Deteccion de enemigos y alimento
        protected virtual void Detect()
        {
            List<IColectable> itemsInRange= scp_itemManager.GetAllItemsInRange(Coordinates, detectArea);
            if (itemsInRange.Count != 0)
            {
                itemsInRange = itemsInRange.OrderByDescending((x) => Vector2Int.Distance(x.Coordinates, Coordinates)).ToList();
            }
            ifc_item2Colect = (itemsInRange.Count == 0) ? null : itemsInRange[0];
            //Selecciona Ubicacion destino
            Vector2Int goal = (ifc_item2Colect != null) ? ifc_item2Colect.Coordinates : RememberBestSpot();
            MoveComp.MoveCharacter(Vector3.right * goal.x+ Vector3.forward* goal.y);
            
        }

        public override void UpdateSelected(object var)
        {
            base.UpdateSelected(var);
        }

        

        public override void AutomaticActions()
        {
            EndPhase = false;
            Detect();
        }
    }
}
