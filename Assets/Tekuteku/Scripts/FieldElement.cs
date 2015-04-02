using UnityEngine;
using System.Collections.Generic;

public abstract class FieldElement {

	abstract public IEnumerable<FieldElement> Contacts {
		get;
	}
}
