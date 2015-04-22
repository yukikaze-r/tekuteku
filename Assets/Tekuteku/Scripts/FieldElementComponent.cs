using UnityEngine;
using System.Collections;

public class FieldElementComponent : MonoBehaviour {

	private FieldElement fieldElement;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void AcceptModel(FieldElement fieldElement) {
		this.fieldElement = fieldElement;
		fieldElement.FieldMap.SelectChangeListener += OnSelectChange;
	}

	private void OnSelectChange(FieldElement fieldElement, bool isSelect) {
		if (fieldElement == this.fieldElement) {
			gameObject.renderer.material.SetFloat("_Shininess", isSelect ? 0f : 1f);
		}
	}
}
