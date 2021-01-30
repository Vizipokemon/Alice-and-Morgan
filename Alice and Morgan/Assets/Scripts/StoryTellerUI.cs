using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class StoryTellerUI : MonoBehaviour
{
    public delegate void DecisionMadeDelegate(DecisionSO.DecisionOption chosenOption, DecisionSO decision);
    public event DecisionMadeDelegate DecisionMadeEvent;
    public delegate void FinishedTypingDelegate();
    public event FinishedTypingDelegate FinishedTypingEvent;

    [SerializeField] private Button[] optionButtons;
    private DecisionSO.DecisionOption[] currentOptions = new DecisionSO.DecisionOption[3];
    [SerializeField] private GameObject chatTextBlockPrefab;
    [SerializeField] private TMP_Text responseBarText;
    [SerializeField] private GameObject ChatContentContainer;

    [SerializeField] private ScrollRect chatScrollRect;
    [SerializeField] private ScrollRect responseBarScrollRect;
    [SerializeField] private TMP_Text morganIsTypingText;

    private const float caretCounterInitial = 0.5f;
    private float caretCounter;
    private bool isCaretVisible = false;
    private bool isCaretPaused = false;

    private DecisionSO currentDecision;

    private void Start()
    {
        caretCounter = caretCounterInitial;
    }

    private void Update()
    {
        if(!isCaretPaused)
        {
            BlinkCaret();
        }
    }

    private void ScrollChatToBottom()
    {
        Canvas.ForceUpdateCanvases();
        chatScrollRect.verticalNormalizedPosition = 0;
    }

    private void ScrollResponseBarToEnd()
    {
        Canvas.ForceUpdateCanvases();
        responseBarScrollRect.horizontalNormalizedPosition = 1;
    }

    public void EnableMorganRespondingIcon(bool enable)
    {
        morganIsTypingText.gameObject.SetActive(enable);
    }

    public void PrintSentence(string speakerName, string text)
    {
        Debug.Log(speakerName + ": " + text);
    }

    public void TypeCharacterInChat(char character)
    {
        isCaretPaused = true;
        RemoveCaret();
        responseBarText.text += character;
        ScrollResponseBarToEnd();
        AudioPlayer.Instance.PlayRandomKeyboardClick();
        FinishedTypingEvent?.Invoke();
        isCaretPaused = false;
    }

    public void TypeWordInChat(string word)
    {
        StartCoroutine(TypeWordInChatOneLetterAtATime(word));
    }

    private IEnumerator TypeWordInChatOneLetterAtATime(string word)
    {
        isCaretPaused = true;
        RemoveCaret();
        responseBarText.text += " ";
        AudioPlayer.Instance.PlayRandomKeyBoardMultiClick();
        foreach (char character in word)
        {
            responseBarText.text += character;
            ScrollResponseBarToEnd();
            yield return new WaitForSeconds(0.07f);
        }
        FinishedTypingEvent?.Invoke();
        isCaretPaused = false;
    }

    private void RemoveCaret()
    {
        responseBarText.text = responseBarText.text.Replace("|", string.Empty);
        isCaretVisible = false;
    }

    private void AddCaret()
    {
        responseBarText.text += '|';
        isCaretVisible = true;
    }

    private void BlinkCaret()
    {
        if (caretCounter <= 0)
        {
            if (isCaretVisible)
            {
                RemoveCaret();
            }
            else
            {
                AddCaret();
            }

            caretCounter = caretCounterInitial;
        }
        else
        {
            caretCounter -= Time.deltaTime;
        }
    }

    public void SendMessage(string message, string sender)
    {
        GameObject chatBlockGO = Instantiate(chatTextBlockPrefab, ChatContentContainer.transform);
        chatBlockGO.GetComponent<TMP_Text>().text = sender + ": " + message;
        ScrollChatToBottom();
        if(sender == "Morgan")
        {
            AudioPlayer.Instance.PlayPop();
        }
    }

    public void SendChatMessage()
    {
        RemoveCaret();
        SendMessage(responseBarText.text.TrimStart(' '), "Alice");
        responseBarText.text = "";
    }

    public void PlayDecision(DecisionSO decision, List<DecisionSO.DecisionOption> alreadyChosenOptions)
    {
        currentDecision = decision;
        UnFreezeAllButtons();

        for (int i = 0; i < decision.options.Length; i++)
        {
            DecisionSO.DecisionOption decisionOption = decision.options[i];
            Button optionButton = optionButtons[i];
            optionButton.gameObject.SetActive(true);
            Text optionButtonText = optionButton.GetComponentInChildren<Text>();
            optionButtonText.text = decisionOption.text;
            currentOptions[i] = decisionOption;

            if(alreadyChosenOptions.Contains(decisionOption))
            {
                FreezeButton(optionButton, optionButtonText);
            }
        }
    }

    private void FreezeButton(Button button, Text buttonText)
    {
        button.interactable = false;
        Color newColor = buttonText.color;
        newColor.a = 0.5f;
        buttonText.color = newColor;
    }

    private void UnFreezeAllButtons()
    {
        foreach (Button button in optionButtons)
        {
            button.interactable = true;
            Text buttonText = button.GetComponentInChildren<Text>();
            Color newColor = buttonText.color;
            newColor.a = 1f;
            buttonText.color = newColor;
        }
    }

    private void DisableAllOptionButtons()
    {
        foreach (Button button in optionButtons)
        {
            button.gameObject.SetActive(false);
        }
    }

    public void ChooseOption(int indexOfChosenOption)
    {
        DisableAllOptionButtons();
        DecisionMadeEvent?.Invoke(currentOptions[indexOfChosenOption], currentDecision);
        currentDecision = null;
    }

    private void OnEnable()
    {
        optionButtons[0].onClick.AddListener(() => ChooseOption(0));
        optionButtons[1].onClick.AddListener(() => ChooseOption(1));
        optionButtons[2].onClick.AddListener(() => ChooseOption(2));
    }

    private void OnDisable()
    {
        optionButtons[0].onClick.RemoveListener(() => ChooseOption(0));
        optionButtons[1].onClick.RemoveListener(() => ChooseOption(1));
        optionButtons[2].onClick.RemoveListener(() => ChooseOption(2));
    }
}
