using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Dialog", menuName = "ScriptableObjects/DialogSO", order = 1)]
public class DialogSO : ScriptableObject
{
    [System.Serializable]
    public struct DialogPiece
    {
        public string speakerName;
        [TextArea]
        public string[] sentences;
    }

    public DialogPiece[] dialogPieces;
    public DecisionSO decision; //the decision to be made after the dialog
    public DialogSO nextDialog; //next dialog, leave it empty if this is the end of the story, or there is a decision
}
