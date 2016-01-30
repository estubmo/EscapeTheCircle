using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ClueManager : MonoBehaviour {

	private List<GameObject> clueOrder;
	private List<GameObject> clues;

	public ClueManager() {
		clueOrder = new List<GameObject> ();
		clues = new List<GameObject>(GameObject.FindGameObjectsWithTag ("Clue"));
	}

	public List<GameObject> getSceneClues() {
		return clues;
	}

	public void addPlayerClue(GameObject clue) {
		if (!clueOrder.Contains(clue))
			clueOrder.Add (clue);
	}

	public bool checkClueOrder() {
		if (clues.Count == clueOrder.Count){
			for (var i=0; i < clues.Count; i++){
				if (clues[i] != clueOrder[i])
					return false;
			}
		}
		return true;
	}
}