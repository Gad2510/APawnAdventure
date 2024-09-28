using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace Betadron.Objects
{
    [CreateAssetMenu(fileName = "Food", menuName = "ScriptableObjects/FoodDefinitions", order = 1)]
    public class FoodCatalog : ScriptableObject
    {
        public List<Food> lst_items;

        public Food GetItemRecord(int _ID)
        {
            return lst_items.FirstOrDefault((x) => x.ID == _ID);
        }

    }

    [System.Serializable]
    public struct Food
    {
        public int ID;
        public Object obj_reference;
        public string str_name;

    }
}
