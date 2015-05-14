using UnityEngine;
using System.Collections;

public class Office : Building {
	public override void RegisterFieldMap(FieldMap fieldMap, VectorInt3 position) {
		base.RegisterFieldMap(fieldMap, position);
		fieldMap.Offices.Add(this);
		CalculatePath();
	}

}
