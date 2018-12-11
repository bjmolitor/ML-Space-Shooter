using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grower : MonoBehaviour {

    public float growPerFrame;
    public float max;

   /* private GameObject parentGameObject;

    private void Start()
    {
        parentGameObject = GetComponentInParent<GameObject>();
    }*/

    // Update is called once per frame
    void Update () {
       transform.localScale = 
            new Vector3(
                Mathf.Max(transform.localScale.x + (growPerFrame * Time.deltaTime), max),
                Mathf.Max(transform.localScale.y + (growPerFrame * Time.deltaTime), max),
                Mathf.Max(transform.localScale.z + (growPerFrame * Time.deltaTime), max)
            );
	}
}
