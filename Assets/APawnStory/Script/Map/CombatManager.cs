using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
public class CombatManager : MonoBehaviour
{
    private List<IInteractable> lst_characters;
    private List<IInteractable> lst_charInRange;
    private void Awake()
    {
        lst_characters = new List<IInteractable>();
        lst_charInRange = new List<IInteractable>();
    }

    public void AddCharacter(IInteractable _char)
    {
        lst_characters.Add(_char);
    }

    public void RemoveCharacter(IInteractable _char)
    {
        if (lst_charInRange.Contains(_char))
            lst_charInRange.Remove(_char);

        lst_characters.Remove(_char);
    }

    public void AddCharacterInRange(Vector2Int _coord)
    {
        IInteractable ch= lst_characters.FirstOrDefault((x) => x.Coordinates == _coord);
        if (ch!= null)
            lst_charInRange.Add(ch);

    }

    public void CleanCharacterInRange()
    {
        lst_charInRange.Clear();
    }

    public bool LookForCharacter(Vector2Int _coord)
    {
        if (lst_charInRange.Count == 0)
            return false;

        bool charReachable2Attack= lst_charInRange.Any((x) => x.Coordinates == _coord);
        return charReachable2Attack;
    }
}
