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

            IInteractable item=Instantiate(scp_catalog.GetItemRecord(_ID).obj_reference) as IInteractable;

            item.UpdateSelected(MapFunctions.GetLocation4Coord(loc));
        }

        private Vector2Int GetRandomLocation()
        {
            return scp_map.GetRandomTile().Coordinates;
        }
        //Read
        //Busca todos los objetos cerca del rango enviado
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
        
    }
}
