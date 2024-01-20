using UnityEngine;
using System.Collections;

public class Letters : MonoBehaviour {
	
    public bool utilized = false;
    public bool identified = false;
	public TextMesh letter;
	public int gridX, gridY;

    void Start() {
        GetComponent<Renderer>().materials[0].color = WordSearch2.Instance.defaultTint;
    }
    
    void Update() {
        if (WordSearch2.Instance.ready) {
            if (!utilized && WordSearch2.Instance.current == gameObject) {
                WordSearch2.Instance.selected.Add(this.gameObject);
                GetComponent<Renderer>().materials[0].color = WordSearch2.Instance.mouseoverTint;
                WordSearch2.Instance.selectedString += letter.text;
                utilized = true;
            }
        }

        if (identified) {
			if (GetComponent<Renderer>().materials[0].color != WordSearch2.Instance.identifiedTint) {
				GetComponent<Renderer>().materials[0].color = WordSearch2.Instance.identifiedTint;
			} 
			return;
        }

        if (Input.GetMouseButtonUp(0)) {
            utilized = false;
			if (GetComponent<Renderer>().materials[0].color != WordSearch2.Instance.defaultTint) {
				GetComponent<Renderer>().materials[0].color = WordSearch2.Instance.defaultTint;
			}
        }
    }
}
