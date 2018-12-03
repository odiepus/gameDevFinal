using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalController : MonoBehaviour {

	// Use this for initialization
	void Awake () {
        QualitySettings.vSyncCount = 1;
        Application.targetFrameRate = 60;
	}	
	// Update is called once per frame
	void Update () {
	}
}
