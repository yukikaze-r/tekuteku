using UnityEngine;
using System.Collections.Generic;

public class FieldElementComponent : MonoBehaviour {

	public int name;
	public List<int> connectionsFrom = new List<int>();
	public List<int> connectionsTo = new List<int>();

	private FieldElement fieldElement;

	public FieldElement FieldElement {
		get {
			return fieldElement;
		}
	}

	protected void Start() {
		gameObject.renderer.material.SetFloat("_Shininess", 1f);
	}

	protected void Update() {
		if (fieldElement != null) {
			name = fieldElement.GetHashCode();
			connectionsFrom.Clear();
			foreach (var c in fieldElement.ConnectionsFrom) {
				connectionsFrom.Add(c.GetHashCode());
			}
			connectionsTo.Clear();
			foreach (var c in fieldElement.ConnectionsTo) {
				connectionsTo.Add(c.GetHashCode());
			}
		}

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
