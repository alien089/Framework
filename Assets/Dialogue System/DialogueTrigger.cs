using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueTrigger : MonoBehaviour {
	[SerializeField] private DialogueManager m_DialogueManager;
	/// <summary>
	/// method used to trigger a dialogue, need the index of the specified dialogue
	/// </summary>
	/// <param name="indexDialogue"></param>
	public void TriggerDialogue (int indexDialogue)
	{
        m_DialogueManager.StartDialogue(indexDialogue);
	}
}