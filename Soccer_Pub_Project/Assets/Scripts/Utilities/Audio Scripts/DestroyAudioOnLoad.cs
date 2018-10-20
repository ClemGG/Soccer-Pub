using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyAudioOnLoad : MonoBehaviour {

	// Use this for initialization
	void Awake () {
        DontDestroyOnLoad(gameObject);
	}

    private void OnLevelWasLoaded(int level)
    {
        if(level == 2)
        {
            Destroy(gameObject);
        }
    }
}
