using UnityEngine;
using UnityEngine.SceneManagement;

namespace Betadron.Managers
{
    public class LoadSceneManager : MonoBehaviour
    {
        public string GetSceneName()
        {
            return SceneManager.GetActiveScene().name;
        }
    }
}
