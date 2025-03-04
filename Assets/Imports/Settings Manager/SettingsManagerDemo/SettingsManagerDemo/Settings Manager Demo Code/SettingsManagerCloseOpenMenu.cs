using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif
namespace BattlePhaze.SettingsManager.Demo
{
    /// <summary>
    /// Close
    /// Open
    /// Menu
    /// </summary>
    public class SettingsManagerCloseOpenMenu : MonoBehaviour
    {
        /// <summary>
        /// Menu
        /// </summary>
        public GameObject Menu;
        /// <summary>
        /// keyCode to use
        /// </summary>
#if ENABLE_INPUT_SYSTEM
        public Key KeyCode = Key.Escape;
#else
        public KeyCode KeyCode = KeyCode.Escape;
#endif
        /// <summary>
        /// Update is called once per frame
        /// </summary>
        void Update()
        {
#if ENABLE_INPUT_SYSTEM
            if (Keyboard.current[KeyCode].wasPressedThisFrame)
            {
                OpenCloseMenu();
            }
#else
            if (Input.GetKeyDown(KeyCode))
            {
                OpenCloseMenu();
            }
#endif
        }
        /// <summary>
        /// Open Close Menu
        /// </summary>
        public void OpenCloseMenu()
        {
            Menu.SetActive(!Menu.activeSelf);
        }
    }
}