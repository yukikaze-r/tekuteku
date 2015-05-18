using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class FieldElementComponent : MonoBehaviour {

	public GameObject mainObject;

	public int name;
	public List<int> connectionsFrom = new List<int>();
	public List<int> connectionsTo = new List<int>();
	public Direction4 direction;
	public int vehicles;

	private FieldElement fieldElement;

	public FieldElement FieldElement {
		get {
			return fieldElement;
		}
	}

	protected void Start() {
		if (mainObject == null) {
			mainObject = gameObject;
		}
		mainObject.renderer.material.SetFloat("_Shininess", 1f);
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
			if (fieldElement is Slope) {
				direction = ((Slope)fieldElement).Direction;
			}
			vehicles = fieldElement.Vehicles.Count();

		}

	}
	
	public void AcceptModel(FieldElement fieldElement) {
		this.fieldElement = fieldElement;
		fieldElement.FieldMap.SelectChangeListener += OnSelectChange;
	}

	protected void OnDestroy() {
		if (fieldElement != null) {
			fieldElement.FieldMap.SelectChangeListener -= OnSelectChange;
		}
	}

	private void OnSelectChange(FieldElement fieldElement, bool isSelect) {
		if (fieldElement == this.fieldElement) {
			gameObject.renderer.material.SetFloat("_Shininess", isSelect ? 0f : 1f);
		}
	}

	public void MakeCursor() {
		
		Material material = new Material(mainObject.renderer.material);
		material.shader = Shader.Find("Transparent/Diffuse");
		var c = material.color;
		c.a = 0.5f;
		material.color = c;
		mainObject.renderer.material = material;
	}
}
