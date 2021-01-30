using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoryTeller : MonoBehaviour
{
    [SerializeField] private DialogSO slowedDialog;

    [SerializeField] private DialogSO currentDialog;
    private Queue<DialogSO.DialogPiece> dialogPieces;
    private DialogSO.DialogPiece currentDialogPiece;
    private Queue<string> currentDialogPieceSentences;
    private Queue<string> currentSentenceWords = new Queue<string>();
    private Queue<char> currentSentenceCharacters = new Queue<char>();

    private bool isStoryPlaying = false;
    private bool isDecisionPlaying = false;
    private bool isMorganTyping = false;
    private bool needToSendChatMessage = false;
    private bool isTyping = false;

    private const float morganMinResponseTime = 8f;
    private const float morganMaxResponseTime = 14f;
    private const float morganMinStartingToTypeDelay = 3f;
    private const float morganMaxStartingToTypeDelay = 6f;

    [SerializeField] private StoryTellerUI ui;
    [SerializeField] private FadeScene sceneTransitioner;

    private List<KeyValuePair<DecisionSO, DecisionSO.DecisionOption>> decisionHistory = new List<KeyValuePair<DecisionSO, DecisionSO.DecisionOption>>();

    private void OnEnable()
    {
        ui.DecisionMadeEvent += OnDecisionMade;
        ui.FinishedTypingEvent += OnFinishedTyping;
    }

    private void OnFinishedTyping()
    {
        isTyping = false;
    }

    private void OnDisable()
    {
        ui.DecisionMadeEvent -= OnDecisionMade;
        ui.FinishedTypingEvent -= OnFinishedTyping;
    }

    private void Start()
    {
        StartStory();
    }

    private void Update()
    {
        if(!isStoryPlaying || isDecisionPlaying)
        {
            return;
        }

        if(currentDialogPiece.speakerName == "Alice")
        {
            if(Input.anyKeyDown && !isTyping)
            {
                if(currentDialog.name != slowedDialog.name)
                {
                    PlayNextWordOfSentence();
                }
                else
                {
                    PlayNextCharacterOfSentence();
                }
            }
        }
        else if(!isMorganTyping)
        {
            StartCoroutine(nameof(PlayMorgansSentence));
        }
    }

    private IEnumerator PlayMorgansSentence()
    {
        isMorganTyping = true;

        float howMuchToWaitBeforeStartingToType =
            UnityEngine.Random.Range(morganMinStartingToTypeDelay, morganMaxStartingToTypeDelay);
        yield return new WaitForSeconds(howMuchToWaitBeforeStartingToType);
        ui.EnableMorganRespondingIcon(true);

        float howMuchToWaitForSendingMessage = 
            UnityEngine.Random.Range(morganMinResponseTime, morganMaxResponseTime);
        yield return new WaitForSeconds(howMuchToWaitForSendingMessage);

        PlayNextSentence();
        ui.EnableMorganRespondingIcon(false);
        isMorganTyping = false;
    }

    private void PlayNextSentence()
    {
        ui.SendMessage(GetNextSentence(), currentDialogPiece.speakerName);

        if (currentDialogPieceSentences.Count <= 0)
        {
            SetNextDialogPieceAndEnqueueItsSentences();
        }
    }

    //TODO: Refactor PlayNextCharacterOfSentence and PlayNextWordOfSentence
    //they have a lot of common code!
    private void PlayNextCharacterOfSentence()
    {
        if (currentSentenceCharacters.Count <= 0 && !needToSendChatMessage)
        {
            currentSentenceCharacters = new Queue<char>(GetNextSentence());
        }

        if (currentSentenceCharacters.Count > 0)
        {
            isTyping = true;
            ui.TypeCharacterInChat(currentSentenceCharacters.Dequeue());

            if (currentSentenceCharacters.Count <= 0)
            {
                needToSendChatMessage = true;
            }
        }
        else
        {
            ui.SendChatMessage();
            needToSendChatMessage = false;

            if (currentDialogPieceSentences.Count <= 0)
            {
                SetNextDialogPieceAndEnqueueItsSentences();
            }
        }
    }

    private void PlayNextWordOfSentence()
    {
        EnqueueNextSentenceWordsIfNeededAndTheChatMessageHasAlreadyBeenSent();
        if (currentSentenceWords.Count > 0)
        {
            isTyping = true;
            ui.TypeWordInChat(currentSentenceWords.Dequeue());

            if(currentSentenceWords.Count <= 0)
            {
                needToSendChatMessage = true;
            }
        }
        else
        {
            ui.SendChatMessage();
            needToSendChatMessage = false;

            if (currentDialogPieceSentences.Count <= 0)
            {
                SetNextDialogPieceAndEnqueueItsSentences();
            }
        }
    }

    private void EnqueueNextSentenceWordsIfNeededAndTheChatMessageHasAlreadyBeenSent()
    {
        if (currentSentenceWords.Count <= 0 && !needToSendChatMessage)
        {
            currentSentenceWords = new Queue<string>(GetNextSentence().Split(' '));
        }
    }

    public void StartStory()
    {
        isStoryPlaying = true;
        LoadInDialog(currentDialog);
    }

    private void SetNextDialogPieceAndEnqueueItsSentences()
    {
        if(dialogPieces.Count <= 0)
        {
            PlayDecisionOrNextDialog();
        }
        else
        {
            currentDialogPiece = dialogPieces.Dequeue();
            currentDialogPieceSentences = new Queue<string>(currentDialogPiece.sentences);
        }
    }

    private string GetNextSentence()
    {
        return currentDialogPieceSentences.Dequeue();
    }

    private void PlayDecisionOrNextDialog()
    {
        if(currentDialog.decision != null)
        {
            PlayDecision(currentDialog.decision);
        }
        else if(currentDialog.nextDialog != null)
        {
            LoadInDialog(currentDialog.nextDialog);
        }
        else
        {
            EndStory();
        }
    }

    private void PlayDecision(DecisionSO decision)
    {
        isDecisionPlaying = true;
        ui.PlayDecision(decision, GetAlreadyChosenDecisionOptions(decision));
    }

    public void LoadInDialog(DialogSO dialog)
    {
        currentDialog = dialog;
        dialogPieces = new Queue<DialogSO.DialogPiece>(currentDialog.dialogPieces);
        SetNextDialogPieceAndEnqueueItsSentences();
    }

    private void EndStory()
    {
        isStoryPlaying = false;
        sceneTransitioner.LoadMenuSceneFromGameScene();
    }

    private void OnDecisionMade(DecisionSO.DecisionOption chosenOption, DecisionSO decision)
    {
        LoadInDialog(chosenOption.nextDialog);
        isDecisionPlaying = false;
        decisionHistory.Add(new KeyValuePair<DecisionSO, DecisionSO.DecisionOption>(decision, chosenOption));
    }

    private List<DecisionSO.DecisionOption> GetAlreadyChosenDecisionOptions(DecisionSO decision)
    {
        List<DecisionSO.DecisionOption> alreadyChosen = new List<DecisionSO.DecisionOption>();

        foreach (var item in decisionHistory)
        {
            if(item.Key == decision)
            {
                alreadyChosen.Add(item.Value);
            }
        }

        return alreadyChosen;
    }
}
