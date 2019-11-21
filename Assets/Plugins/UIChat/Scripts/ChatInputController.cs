using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UIChat
{
    public class ChatInputController : MonoBehaviour
    {
        public ChatController chatController;
        
        // Update is called once per frame
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.KeypadEnter) || Input.GetKeyDown(KeyCode.Return))
            {
                chatController.PressActiveButton();
            }
            else if (Input.GetKeyDown(KeyCode.Escape))
            {
                chatController.PressDeactiveButton();
            }
        }
    }

}
