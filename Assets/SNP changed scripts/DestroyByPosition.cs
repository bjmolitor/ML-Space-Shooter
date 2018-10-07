using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyByPosition : MonoBehaviour {

    void FixedUpdate () {
        if (transform.position.z < -10) Destroy(gameObject); 
	}
}
