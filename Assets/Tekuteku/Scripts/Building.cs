using System.Collections.Generic;

public class Building : FieldElement {
	List<Road> contacts = new List<Road>();

	public void AddContact(Road r) {
		contacts.Add(r);
	}


	override public IEnumerable<FieldElement> Contacts {
		get {
			return (IEnumerable<FieldElement>)contacts;
		}
	}
}
