using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ClueManager : MonoBehaviour {

	private string clueTag;
	private List<GameObject> clueOrder;
	private List<GameObject> clues;

	public ClueManager() {
		clueTag = "Clue";
		clueOrder = new List<GameObject> ();
		clues = new List<GameObject>(GameObject.FindGameObjectsWithTag (clueTag));
	}

	public bool isClue(GameObject obj) {
		return obj.tag == clueTag;
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