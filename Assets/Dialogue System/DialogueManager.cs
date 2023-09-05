using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour {

    [SerializeField] private TextMeshProUGUI m_NPCNameTextBox;
    [SerializeField] private TextMeshProUGUI m_DialogueTextBox;
    [SerializeField] private Animator m_Animator;
	[SerializeField] private List<Dialogue> m_DialogueList;

	private Dialogue m_ActualDialogue;
	private List<string> m_CurrentText;
	private int m_CurrentMonologueIndex;

	// Use this for initialization
	private void Start () {
		m_CurrentText = new List<string>();
	}

	/// <summary>
	/// Method used for starting a specified dialogue, need an index based on the dialogue list
	/// </summary>
	/// <param name="indexDialogue"></param>
	public void StartDialogue(int indexDialogue)
	{
        m_Animator.SetBool("IsOpen", true);

        m_ActualDialogue = m_DialogueList[indexDialogue];

		StartMonologue();
    }

	/// <summary>
	/// Start the monologue based on CurrentMonologueIndex and show the first sentence
	/// </summary>
	public void StartMonologue()
	{
        m_NPCNameTextBox.text = m_ActualDialogue.DialogueParts[m_CurrentMonologueIndex].Name;

		m_CurrentText.Clear();

		m_CurrentText.Add(null);

		foreach (string sentence in m_ActualDialogue.DialogueParts[m_CurrentMonologueIndex].Sentences)
		{
			m_CurrentText.Add(sentence);
        }

		DisplayNextSentence();
	}

	/// <summary>
	/// SHow the first sentence available in the list of CurrentText
	/// </summary>
	public void DisplayNextSentence ()
	{
        m_CurrentText.RemoveAt(0);
        if (m_CurrentText.Count == 0)
		{
			if (m_CurrentMonologueIndex < m_ActualDialogue.DialogueParts.Length - 1)
			{
				m_CurrentMonologueIndex++;
				StartMonologue();
            }
            else
			{
				EndDialogue();
				return;
			}
		}

		string sentence = m_CurrentText[0];
		StopAllCoroutines();
		StartCoroutine(TypeSentence(sentence));
    }

	/// <summary>
	/// Type letter by letter the entire senteces passed by param in UI
	/// </summary>
	/// <param name="sentence"></param>
	/// <returns></returns>
	private IEnumerator TypeSentence (string sentence)
	{
		m_DialogueTextBox.text = "";
		foreach (char letter in sentence.ToCharArray())
		{
			m_DialogueTextBox.text += letter;
			yield return null;
		}
	}

	/// <summary>
	/// Stop dialogue, reset all to default values and hide text box in UI
	/// </summary>
	private void EndDialogue()
	{
		m_Animator.SetBool("IsOpen", false);
		m_ActualDialogue = null;
        m_CurrentMonologueIndex = 0;
    }
}
