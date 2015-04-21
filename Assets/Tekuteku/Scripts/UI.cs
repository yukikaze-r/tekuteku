﻿using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;

public class UI : MonoBehaviour {

	public GameObject roadPanelPrefab;
	public GameObject toolPalettePrefab;

	private Dictionary<GameObject, List<GameObject>> manager = new Dictionary<GameObject, List<GameObject>>();


	void Start () {
		Open(this.toolPalettePrefab);
	}

	void Update() {
		foreach (var p in manager) {
			int i = 0;
			for (int j = 0; j < p.Value.Count;j++) {
				if (p.Value[j] == null) {
					continue;
				}
				p.Value[i] = p.Value[j];
				i++;
			}
			if (i != p.Value.Count) {
				p.Value.RemoveRange(i, p.Value.Count - i);
			}
		}
	}

	public void Open(GameObject uiPrefab) {
		GameObject go = (GameObject)Instantiate(uiPrefab);
		go.transform.SetParent(gameObject.transform, false);

		List<GameObject> list;
		if (!manager.TryGetValue(uiPrefab, out list)) {
			list = new List<GameObject>();
			manager[uiPrefab] = list;
		}
		list.Add(go);
	}

	public GameObject GetWidget(GameObject uiPrefab) {
		List<GameObject> list;
		if (manager.TryGetValue(uiPrefab, out list)) {
			if (list.Count >= 1) {
				return list[list.Count-1];
			}
		}
		return null;
	}
}