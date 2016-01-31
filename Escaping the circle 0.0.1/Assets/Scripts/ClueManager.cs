using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ClueManager : MonoBehaviour {

	private string clueTag;
	private string clueContainerTag;
	private List<GameObject> playerClueOrder;
	private GameObject clueContainer;
	private GameObject[] cluesCorrectOrder;

	public ClueManager() {
		clueTag = "Clue";
		clueContainerTag = "ClueContainer";
		playerClueOrder = new List<GameObject> ();
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

	public bool isClueOrderCorrect() {
		var clueCount = clueContainer.transform.childCount;
		var isCorrect = false;
		if (clueCount == playerClueOrder.Count){
			Debug.Log ("Correct order is " + cluesCorrectOrder [0].name +
			" " + cluesCorrectOrder [1].name + " " + cluesCorrectOrder [2].name);
			isCorrect = true;
			for (var i=0; i<clueCount; i++) {
				if (cluesCorrectOrder[i] == playerClueOrder [i]) {
					Debug.Log ("Order not correct");
					return false;
				}
			}
		}
		return isCorrect;
	}
}