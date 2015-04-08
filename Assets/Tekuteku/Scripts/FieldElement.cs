using UnityEngine;
using System.Collections.Generic;

public abstract class FieldElement {

	public VectorInt2 Position { get; set; }

	abstract public IEnumerable<FieldElement> Connections {
		get;
	}
}
