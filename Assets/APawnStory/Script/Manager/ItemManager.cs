using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Betadron.Interfaces;
using Betadron.Objects;
using Betadron.Struct;

namespace Betadron.Managers
{
    public class ItemManager : MonoBehaviour
    {
        /*
         Clase para crear comida y el empleo de energia en la escena
         */
        public delegate void AgedItems();
        public AgedItems agedItems;

        private MapManager scp_map;
        private FoodCatalog scp_catalog;

        private List<IColectable> lst_listItems;
        private List<IColectable> lst_notUsed;

        public void InitManager(MapManager _map)
        {
            scp_catalog = Resources.Load<FoodCatalog>("ScriptableObjects/Catalog");
            lst_listItems = new List<IColectable>();
            lst_notUsed = new List<IColectable>();
            scp_map = _map;
        }
        //CRUD
        //Create
        public void CreateItem(int _ID, Vector2Int? _coord= null)
        {
            Vector2Int loc = (_coord != null) ? (Vector2Int)_coord : GetRandomLocation();
            //Busca uno que no este en uso para reactivar
            IColectable item = GetItemNotUsed(_ID);
            //Crea un nuevo item con el ID asignado
            if (item == null)
                item = (Instantiate(scp_catalog.GetItemRecord(_ID).obj_reference) as GameObject).GetComponent<IColectable>();
            else
                lst_notUsed.Remove(item);

            item.UpdateSelected(MapFunctions.GetLocation4Coord(loc));
            item.OnCreateElement();

            lst_listItems.Add(item);
        }

        private Vector2Int GetRandomLocation()
        {
            List<Vector2Int> excluded = lst_listItems.Select((x) => x.Coordinates).ToList();
            INagavable t = scp_map.GetRandomTile(excluded);
            return t.Coordinates;
        }
        private Vector2Int GetRandomLocationInRange(Vector2Int _coord, int _range)
        {
            List<Vector2Int> excluded = lst_listItems.Select((x) => x.Coordinates).ToList();
            INagavable t = scp_map.GetRandomTileInRange(_coord, _range, excluded);

            if (t == null)
                return GetRandomLocation();

            return t.Coordinates;
        }
        public void SpawnItems()
        {
            int[] spawnRates = { 10, 5, 3 };
            //Exploracion superficial
            PythonIDE.Population = 5;
            PythonIDE.Cicles = 10;

            int mapSize = GameManager.Instance.LevelInfo.size;
            PythonIDE.MaxXValue = (mapSize / 2);
            PythonIDE.MinXValue = -(mapSize / 2);
            PythonIDE.MaxYValue = (mapSize / 2);
            PythonIDE.MinYValue = -(mapSize / 2);

            PythonIDE.LoadPythonIDE();

            List<PythonRef> spawnCandidates = new List<PythonRef>();
            //Obtener referencias 
            for(int i =0; i<3; i++)
            {
                PythonRef codeRef = PythonIDE.ExecuteIDE();
                spawnCandidates.Add(codeRef);
            }

            spawnCandidates=spawnCandidates.OrderBy((x) => x.Value).ToList();
            //Spawn items en area 
            for (int i = 0; i < spawnCandidates.Count; i++)
            {
                Vector2Int c = new((int)spawnCandidates[i].coord[0], (int)spawnCandidates[i].coord[1]);
                for(int a=0;a<spawnRates[i];a++)
                    CreateItem(0, GetRandomLocationInRange(c, (3 - i) * 3));
            }
            //Spawn en locaciones aleatorias
            for (int i = 0; i < 5; i++)
            {
                CreateItem(0);
            }
        }


        //Read
        //Busca todos los objetos cerca del rango enviado
        private IColectable GetItemNotUsed(int _ID)
        {
            IColectable item = null;

            item = lst_notUsed.FirstOrDefault((x) => x.ID == _ID);
            
            return item;
        }
        public List<IColectable> GetItemByDistance(Vector2Int _coord, int _range)
        {
            List<IColectable> objects = null;
            if (lst_listItems.Any((x)=> Vector2Int.Distance(x.Coordinates,_coord)< _range)){
                objects= lst_listItems.Where((x) => Vector2Int.Distance(x.Coordinates, _coord) < _range).ToList();
            }
            return objects;
        }
        public IColectable GetItemInLocation(Vector2Int _coord)
        {
            return lst_listItems.FirstOrDefault(x => x.Coordinates == _coord);
        }
        public List<IColectable> GetAllItems()
        {
            return lst_listItems;
        }

        public List<IColectable> GetAllItemsInRange(Vector2Int _coord, int _range)
        {
            List<IColectable> lookfor = lst_listItems.Where((x) => Vector2Int.Distance(x.Coordinates, _coord) <= _range).ToList();
            return lookfor;
        }
        //Update
        public void SetItemAtributes(IColectable _item)
        {
            bool isRegister = lst_listItems.Any(x => x.Coordinates == _item.Coordinates);

            if (isRegister)
            {
                IColectable item = lst_listItems.First(x => x.Coordinates == _item.Coordinates);
                int i = lst_listItems.IndexOf(item);
                lst_listItems[i] = _item;
            }
            else
            {
                lst_listItems.Add(_item);
            }
        }

        //Delete
        //Remoueve Items y regresa el numero actual de Items disponibles
        public int RemoveInCoord(Vector2Int _coord)
        {
            IColectable item = lst_listItems.FirstOrDefault((x => x.Coordinates == _coord));
            int count = -1;
            if (item != null)
                count=RemoveItem(item);

            return count;
        }
        public int RemoveItem(IColectable _item)
        {
            lst_listItems.Remove(_item);
            lst_notUsed.Add(_item);
            return lst_listItems.Count;
        }
        public void Add2AgedItems(IAged _item)
        {
            agedItems += _item.Aged;
        }
        public void RemoveFromAgedItems(IAged _item)
        {
            agedItems -= _item.Aged;
        }
        
    }
}
