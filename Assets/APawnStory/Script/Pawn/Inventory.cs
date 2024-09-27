using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Betadron.Objects;
using Betadron.Managers;
namespace Betadron.Pawn
{
    public class Inventory : MonoBehaviour
    {
        public enum Equipment
        {
            body,
            waepon
        }

        private Dictionary<Equipment, ItemDef> dic_equipment;
        public List<ItemDef> lst_items;
        private Dictionary<int, int> dic_consumables;

        private int int_weigth;

        GameModeGameplay gm_gamemode;

        private void Start()
        {
            gm_gamemode = ((GameModeGameplay)GameManager.gm_gamemode);
            dic_equipment = new Dictionary<Equipment, ItemDef>();
            lst_items = new List<ItemDef>();
            dic_consumables = new Dictionary<int, int>();
        }

        public void AddItem(int _item)
        {
            ItemDef def = gm_gamemode.ItemsRecords.GetItemRecord(_item);
            if (def.isStackable)
            {
                if (!dic_consumables.ContainsKey(_item))
                {
                    dic_consumables.Add(_item, 1);
                }
                else
                {
                    dic_consumables[_item]++;
                }
            }
            else
            {

            }
        }
    }
}