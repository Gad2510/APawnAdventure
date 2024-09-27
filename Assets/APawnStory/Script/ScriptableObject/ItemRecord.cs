using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
namespace Betadron.Objects
{
    [CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/InventoryRecord", order = 1)]
    public class ItemRecord : ScriptableObject
    {
        public List<ItemDef> lst_items;

        public ItemDef GetItemRecord(int _ID)
        {
            return lst_items.FirstOrDefault((x) => x.ID == _ID);
        }

    }

    [System.Serializable]
    public struct ItemDef
    {
        public int ID;
        public string str_name;
        public string str_description;
        public bool isStackable;
        public int weigth;

        public Texture img;

    }
}
