using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class DestroyByPosition : MonoBehaviour {

    public float zDestroyPosition = -20;

    void FixedUpdate () {
        if (transform.position.z < zDestroyPosition) Destroy(gameObject); 
	}
}
