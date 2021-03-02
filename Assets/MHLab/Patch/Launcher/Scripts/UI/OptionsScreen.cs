using UnityEngine;

namespace MHLab.Patch.Launcher.Scripts.UI
{
    public sealed class OptionsScreen : MonoBehaviour
    {
        public void OpenOptions()
        {
            gameObject.SetActive(true);
            Debug.Log("Opened");
        }

        public void CloseOptions()
        {
            gameObject.SetActive(false);
            Debug.Log("Closed");
        }
    }
}