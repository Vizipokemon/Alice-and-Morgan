using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Decision", menuName = "ScriptableObjects/DecisionSO", order = 2)]
public class DecisionSO : ScriptableObject
{
    [System.Serializable]
    public class DecisionOption
    {
        public string text;
        public DialogSO nextDialog;
    }

    public DecisionOption[] options;
}
