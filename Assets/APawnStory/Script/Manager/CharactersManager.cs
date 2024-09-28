using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Betadron.Interfaces;
namespace Betadron.Managers
{
    public class CharactersManager : MonoBehaviour
    {
        /*
        CharactersManageR: Clase encargada de guardar una referencia a todos elementos
        con los que se puede interactuar
         */
        private List<IPlayable> lst_playerChar;
        //Lista de todos los Caracteres
        private List<IPlayable> lst_interactable;
        //Lista de Caracteres en rango de ataque
        private List<IPlayable> lst_interacInRange;
        private void Awake()
        {
            //Inicializa las listas
            lst_interactable = new List<IPlayable>();
            lst_interacInRange = new List<IPlayable>();
            lst_playerChar = new List<IPlayable>();
        }
        //Inicia todos los carateres
        public void InitCharacters()
        {
            lst_playerChar.ForEach((x) => x.StartTurn());
            lst_interactable.ForEach((x) => x.StartTurn());
        }
        //Los npc´s hacen todas las acciones automaticas
        public IEnumerator ExecuteNPCActions()
        {
            yield return new WaitForSeconds(1f);
            foreach (IPlayable npc in lst_interactable)
            {
                npc.AutomaticActions();
                yield return new WaitForSeconds(1f);
                //Espera hasta que el caracter termine su turno
                yield return new WaitWhile(()=> { return npc.EndPhase; });
                Debug.Log("NPC finish turn");
            }
            ((GameModeGameplay)GameManager.gm_gamemode).EndTurn();
        }

        //CRUD 
        //Create
        // Acción de agregar Caracter a la lista ejecutada por la clase Pawn 
        public void AddCharacter(IPlayable _char)
        {
            if (_char.IsControllable) { lst_playerChar.Add(_char); }

            else { lst_interactable.Add(_char); }
        }
        //Delete
        //Acción de quitar Caracter a la lista 
        public void RemoveCharacter(IPlayable _char)
        {
            //Quita la referencia de los enemigos en rango
            if (lst_interacInRange.Contains(_char))
                lst_interacInRange.Remove(_char);

            if (_char.IsControllable) { lst_playerChar.Remove(_char); }

            else { lst_interactable.Remove(_char); }
        }
        //Update
        //Agrega caracteres a la lista de elementos en rango
        public void AddCharacterInRange(Vector2Int _coord)
        {
            //LINQ(First): Devuelve el primer elemento que encuentra sobre las coordenas dadas
            IPlayable ch = lst_interactable.FirstOrDefault((x) => x.GetCoordinates() == _coord);
            if (ch != null)
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
            bool charReachable2Attack = lst_interacInRange.Any((x) => x.GetCoordinates() == _coord);
            return charReachable2Attack;
        }
    }
}