using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

abstract public class Road : FieldElement {

	private int roadIndex;

	public Road() {
	}

	abstract public int Cost {
		get;
	}

	abstract public bool IsConnectFrom(FieldElement contacted);

	abstract public bool IsConnectTo(FieldElement contacted);

	public override void RegisterFieldMap(FieldMap fieldMap, VectorInt3 position) {
		base.RegisterFieldMap(fieldMap, position);

		roadIndex = fieldMap.Roads.Count();
		fieldMap.Roads.Add(this);

		fieldMap.MakeGridPathFinders();
	}



	public bool IsContactableTo(FieldElement fieldElement) {
		return this.ContactedPositions.Intersect(fieldElement.Positions).Count() >= 1;
	}

	public int Index {
		get {
			return roadIndex;
		}
	}


	public override IEnumerable<FieldElement> ConnectionsFrom {
		get {
			return base.ConnectionsFrom.Where(IsConnectFrom);
		}
	}

	public override IEnumerable<FieldElement> ConnectionsTo {
		get {
			return base.ConnectionsTo.Where(IsConnectTo);
		}
	}


}
