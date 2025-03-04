using UnityEngine;
namespace BattlePhaze.SettingsManager.Demo
{
    public class SettingsManagerInstantiate : MonoBehaviour
    {
        /// <summary>
        /// Menu
        /// </summary>
        public GameObject ToInstantiate;
        private GameObject Instantation;
        /// <summary>
        /// keyCode to use
        /// </summary>
        public KeyCode KeyCode = KeyCode.Escape;
        /// <summary>
        /// Update is called once per frame
        /// </summary>
        void Update()
        {
            if (Input.GetKeyDown(KeyCode))
            {
                OpenCloseMenu();
            }
        }
        /// <summary>
        /// Open Close Menu
        /// </summary>
        public void OpenCloseMenu()
        {
            if (Instantation == null)
            {
                Instantation = Instantiate(ToInstantiate);
            }
            else
            {
                Destroy(Instantation);
            }
        }
    }
}