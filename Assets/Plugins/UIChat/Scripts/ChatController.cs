using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Image = UnityEngine.UI.Image;

namespace UIChat
{
    public class ChatController : MonoBehaviour
    {
        //input parameters and helpers
        public int maxInputLength;  // the maximum length of the input string
        public InputField inputField;
        public Image background;
        public float backgroundActiveAlpha;
        public float backgroundDeactiveAlpha;
        public float backgroundChangeAlphaSpeed;
        private bool isChangeAlpha;  // turn on when start coroutine for changing alpha
        private bool isChangeAlphaIncrease;  // direction of the alpha changer, true - increase the alpha (for active), false - decrease (for non-active)
        private bool isInputActive;

        //display chat parameters
        public int maxMessagesCount;  // the max number of messages on the chat window
        public RectTransform contentTransform;
        private Text contentText;
        public ScrollRect chatView;
        private float emptyContentSize;
        private float contentWidth;
        private float oneLineHeigth;  // the hight of one line in the conent text, we need it for initial spacings
        private int chanContentLinesCount;
        private TextGenerator contentTextGenerator;
        private TextGenerationSettings contentTextGeneratorSettings;

        //strings inside chat window
        private List<string> chatItems;


        // Start is called before the first frame update
        void Start()
        {
            isInputActive = false;
            isChangeAlpha = false;
            inputField.characterLimit = maxInputLength;
            contentText = contentTransform.GetComponent<Text>();
            contentTextGenerator = new TextGenerator();
            contentTextGeneratorSettings = contentText.GetGenerationSettings(contentText.rectTransform.rect.size);
            oneLineHeigth = contentTextGenerator.GetPreferredHeight(" ", contentTextGeneratorSettings);

            chatView.verticalNormalizedPosition = 0.0f;
            contentWidth = contentTransform.sizeDelta.x;
            contentTransform.sizeDelta = new Vector2(contentWidth, GetComponent<RectTransform>().sizeDelta.y - inputField.GetComponent<RectTransform>().sizeDelta.y);
            emptyContentSize = contentTransform.sizeDelta.y;
            chatView.verticalScrollbar.size = 1.0f;
            chanContentLinesCount = (int)(emptyContentSize / oneLineHeigth);

            chatItems = new List<string>();
        }

        //--------------------------------------------------------------------
        //-----------------------Event callbacks------------------------------

        public void InputFieldSelect()
        {
            InputActivate();
        }

        public void InputFieldDeselect()
        {
            InputDeactivate();
        }

        public void ScrollButtonUp()
        {
            if (chatItems.Count > chanContentLinesCount)
            {
                chatView.verticalNormalizedPosition += 1.0f / (chatItems.Count - chanContentLinesCount);
            }
        }

        public void ScrollButtonDown()
        {
            if (chatItems.Count > chanContentLinesCount)
            {
                chatView.verticalNormalizedPosition -= 1.0f / (chatItems.Count - chanContentLinesCount);
            }
        }

        public void PressActiveButton()
        {//press enter, we sohuld send a message or activate the input
            if (isInputActive)
            {//input was active, send the message, clear the inputField
                string message = inputField.text;
                if (message.Length > 0)
                {
                    SendMessageString(inputField.text);
                    inputField.text = "";
                }
				inputField.DeactivateInputField();
                EventSystem.current.SetSelectedGameObject(contentText.gameObject);
            }
            else
            {//input was nonactive
                inputField.Select();
                inputField.ActivateInputField();
            }
        }

        public void PressDeactiveButton()
        {//deactivate input field
            EventSystem.current.SetSelectedGameObject(contentText.gameObject);
            inputField.DeactivateInputField();
        }

        //------------------------------------------------------------------
        //------------------Internal methods--------------------------------

        private void InputActivate()
        {
            isInputActive = true;
            isChangeAlphaIncrease = true;
            if (!isChangeAlpha)
            {
                StartCoroutine(ChangeBackgroundAlpha());
            }
        }

        private void InputDeactivate()
        {
            isInputActive = false;
            isChangeAlphaIncrease = false;
            if (!isChangeAlpha)
            {
                StartCoroutine(ChangeBackgroundAlpha());
            }
        }

        IEnumerator ChangeBackgroundAlpha()
        {
            isChangeAlpha = true;
            bool isFinish = false;
            while (!isFinish)
            {
                //get current background color
                float a = background.color.a;
                //dynamic value
                a += Time.deltaTime * backgroundChangeAlphaSpeed * (isChangeAlphaIncrease ? 1.0f : -1.0f);
                //set current alpha
                background.color = new Color(0.0f, 0.0f, 0.0f, a);
                if ((isChangeAlphaIncrease && a > backgroundActiveAlpha) || (!isChangeAlphaIncrease && a < backgroundDeactiveAlpha))
                {
                    isFinish = true;
                }
                yield return null;
            }

            isChangeAlpha = false;
        }

        private string BuildContentString()
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < chatItems.Count; i++)
            {
                if (i < chatItems.Count - 1)
                {
                    sb.AppendLine(chatItems[i]);
                }
                else
                {
                    sb.Append(chatItems[i]);
                }
            }

            return sb.ToString();
        }

        private void AddNewMessage(string message, int linesCount)
        {
            chatItems.Add(message);
            if (chatItems.Count > maxMessagesCount)
            {
                chatItems.RemoveAt(0);
            }
        }

        private void AddMessageToChat(string message)
        {
            float scrollPosition = chatView.verticalNormalizedPosition;

            float height = contentTextGenerator.GetPreferredHeight(message, contentTextGeneratorSettings);
            AddNewMessage(message, (int)(height / oneLineHeigth));
            contentText.text = BuildContentString();
            contentTransform.sizeDelta = new Vector2(contentWidth, Mathf.Max(contentText.preferredHeight, emptyContentSize));
            chatView.verticalNormalizedPosition = scrollPosition;
        }

        //-------------------------------------------------------------------------
        //-------------------Calls from/to external methods------------------------

        public void SendMessageString(string message)
        {
            ComeMessageString(message);  // for test only, send the message to ComeMessageString method
        }
        
        public void ComeMessageString(string message)
        {
            //take some preparations of the new message
            //...

            //finally add message to the chat list
            AddMessageToChat(message);
        }
    }
}
