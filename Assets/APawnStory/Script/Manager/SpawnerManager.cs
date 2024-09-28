using UnityEngine;
using Betadron.Objects;

namespace Betadron.Managers
{
    public class SpawnerManager : MonoBehaviour
    {
        /*
         Clase para crear comida y el empleo de energia en la escena
         */
        private FoodCatalog scp_catalog;
        private void Awake()
        {
            scp_catalog = Resources.Load<FoodCatalog>("ScriptableObjects/Catalog");
        }


    }
}
