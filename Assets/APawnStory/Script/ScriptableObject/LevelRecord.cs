using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
namespace Betadron.Objects
{
    [CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/LevelRecord", order = 1)]
    public class LevelRecord : ScriptableObject
    {
        public LevelDef lv_default;
        public List<LevelDef> lst_levels;

        public LevelDef GetLevelRecord(string _ID)
        {
            LevelDef record = lst_levels.FirstOrDefault((x) => x.str_name == _ID);
            if (record.Equals(default(LevelDef)))
                record = lv_default;
            return record;
        }

    }

    public enum Modes
    {
        Game,
        Menu,
        Pause
    }

    [System.Serializable]
    public struct LevelDef
    {
        public int ID;
        public string str_name;
        public string str_description;
        public int size;

        public Modes gameMode;

    }


}

