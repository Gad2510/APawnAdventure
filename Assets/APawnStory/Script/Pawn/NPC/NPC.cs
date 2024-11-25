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
        protected Vector2Int? v2i_bestSpot;

        public Vector2Int testingItem;
        public Vector2Int testingSpot;

        protected ItemManager scp_itemManager;
        protected GameModeGameplay gm_gamemode;


        public override bool EndPhase
        {
            get => base.EndPhase;
            set
            {
                base.EndPhase = value;
                if (value && !gm_gamemode.IsPlayerTurn)
                    gm_gamemode.EnemyTurn();
            }
        }

        protected override void Start()
        {
            v2i_bestSpot = null;
            gm_gamemode = ((GameModeGameplay)GameManager.gm_gamemode);
            scp_itemManager = gm_gamemode.SpawnManager;
            base.Start();

        }

        protected abstract void CollectItem(IColectable _item);

        //Busca el mojor spot para enocntrar comida
        //TESTING : Obtinene la unica fruta en el campo para seguirla
        protected Vector2Int RememberBestSpot()
        {
            //Checa si tiene un espacio ya registrado en la memoria
            if (v2i_bestSpot != null)
                return (Vector2Int)v2i_bestSpot;

            print("Remembering spot");
            PythonIDE.Cicles = 100;
            PythonIDE.Population = 10;
            PythonIDE.MinXValue = Coordinates.x - (int)(detectArea * 3);
            PythonIDE.MaxXValue = Coordinates.x + (int)(detectArea * 3);
            PythonIDE.MinYValue = Coordinates.y - (int)(detectArea * 3);
            PythonIDE.MaxYValue = Coordinates.y + (int)(detectArea * 3);

            PythonIDE.LoadPythonIDE(gm_gamemode.EvaluationTime * 3);
            PythonRef codeRef = PythonIDE.ExecuteIDE();

            return new((int)codeRef.coord[0], (int)codeRef.coord[1]);
        }

        //Deteccion de enemigos y alimento
        protected virtual Vector3 Detect()
        {
            List<IColectable> itemsInRange = scp_itemManager.GetAllItemsInRange(Coordinates, detectArea);
            if (itemsInRange.Count != 0)
            {
                itemsInRange = itemsInRange.OrderByDescending((x) => Vector2Int.Distance(x.Coordinates, Coordinates)).ToList();
            }
            ifc_item2Colect = (itemsInRange.Count == 0) ? null : itemsInRange[0];
            //Selecciona Ubicacion destino
            Vector2Int goal = (ifc_item2Colect != null) ? ifc_item2Colect.Coordinates : RememberBestSpot();
            return MapFunctions.GetLocation4Coord(goal);

        }

        public override void UpdateSelected(object var)
        {
            base.UpdateSelected(var);
        }



        public override void AutomaticActions()
        {
            EndPhase = false;
            //Default su mismo lugar
            Vector3 goal = transform.position;

            if (ifc_item2Colect == null)
            {
                //No tiene donde llegar
                goal = Detect();
            }
            else
            {
                goal = MapFunctions.GetLocation4Coord(ifc_item2Colect.Coordinates);
            }
            MoveComp.MoveCharacter(goal);

        }

        public virtual void ReachDestination()
        {
            if (Coordinates == v2i_bestSpot)
                v2i_bestSpot = null;

            testingItem = (ifc_item2Colect != null) ? ifc_item2Colect.Coordinates : Vector2Int.zero;
            testingSpot = (v2i_bestSpot != null) ? (Vector2Int)v2i_bestSpot : Vector2Int.zero;
        }
    }
}
