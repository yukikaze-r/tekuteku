﻿using System.Collections.Generic;

public class Building : FieldElement {
	private List<FieldElement> contacts = new List<FieldElement>();

	public GridPathFinder PathFinder {
		get;
		set;
	}

	public void AddContact(Road r) {
		contacts.Add(r);
	}


	override public IEnumerable<FieldElement> Connections {
		get {
			return contacts;
		}
	}

}
