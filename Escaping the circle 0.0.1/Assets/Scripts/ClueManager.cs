using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ClueManager : MonoBehaviour {

	private string clueTag;
	private string clueContainerTag;
	public List<GameObject> playerClueOrder;
	public GameObject clueContainer;
	private GameObject[] cluesCorrectOrder;
	private List<string> corrList;

	public ClueManager() {
		clueTag = "Clue";
		clueContainerTag = "ClueContainer";
		playerClueOrder = new List<GameObject> ();
		corrList = new List<string> ();
		clueContainer = GameObject.FindGameObjectWithTag (clueContainerTag);
		cluesCorrectOrder = GameObject.FindGameObjectsWithTag (clueTag);


	}

	public bool isClue(GameObject obj) {
		return obj.tag == clueTag;
	}

	public void addPlayerClue(GameObject clue) {
		if (!playerClueOrder.Contains (clue)) {
			playerClueOrder.Add (clue);
			Debug.Log ("Clue added: " + clue.name);
		}
	}

	public GameObject getClueContainer(){
		return clueContainer;
	}

    public void ResetPlayerClues()
    {
        playerClueOrder.Clear();
    }

	public bool isClueOrderCorrect(){
		var isCorrect = true;
		for (var i = 0; i < clueContainer.transform.childCount; i++) {
            if(i >= playerClueOrder.Count)
            { isCorrect = false; break; }

			if (clueContainer.transform.GetChild (i).GetChild(0).name != playerClueOrder [i].name)
            {
				isCorrect = false;
			}
		}
		return isCorrect;
	}
}