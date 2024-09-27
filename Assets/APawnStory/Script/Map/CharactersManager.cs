using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
public class CharactersManager : MonoBehaviour
{
    /*
    CharactersManageR: Clase encargada de guardar una referencia a todos elementos
    con los que se puede interactuar
     */
    //Lista de todos los Caracteres
    private List<IInteractable> lst_interactable;
    //Lista de Caracteres en rango de ataque
    private List<IInteractable> lst_interacInRange;
    private void Awake()
    {
        //Inicializa las listas
        lst_interactable = new List<IInteractable>();
        lst_interacInRange = new List<IInteractable>();
    }
    //CRUD 
    //Create
    // Acción de agregar Caracter a la lista ejecutada por la clase Pawn 
    public void AddCharacter(IInteractable _char)
    {
        lst_interactable.Add(_char);
    }
    //Delete
    //Acción de quitar Caracter a la lista 
    public void RemoveCharacter(IInteractable _char)
    {
        //Quita la referencia de los enemigos en rango
        if (lst_interacInRange.Contains(_char))
            lst_interacInRange.Remove(_char);

        lst_interactable.Remove(_char);
    }
    //Update
    //Agrega caracteres a la lista de elementos en rango
    public void AddCharacterInRange(Vector2Int _coord)
    {
        //LINQ(First): Devuelve el primer elemento que encuentra sobre las coordenas dadas
        IInteractable ch= lst_interactable.FirstOrDefault((x) => x.Coordinates == _coord);
        if (ch!= null)
            lst_interacInRange.Add(ch);

    }
    //Delete
    //Limpia toda las referencias
    public void CleanCharacterInRange()
    {
        lst_interacInRange.Clear();
    }
    //Read
    //Devuelve si un Caracter se encunetra un una posicion del mapa
    //Lamado por el control
    public bool LookForCharacter(Vector2Int _coord)
    {
        //Vrifica que halla enemigos en rango
        if (lst_interacInRange.Count == 0)
            return false;
        //LINQ(Any): Compara las coordenadas del caracter con las que estan en el rango
        bool charReachable2Attack= lst_interacInRange.Any((x) => x.Coordinates == _coord);
        return charReachable2Attack;
    }
}
