using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Disable : MonoBehaviour {

	// Use this for initialization
	void Start () {
        StartCoroutine(DisableObject(GetComponent<AudioSource>().clip.length));
	}



    private IEnumerator DisableObject(float v)
    {
        yield return new WaitForSeconds(v);
        gameObject.SetActive(false);
    }
}
