using UnityEngine;
using System.Collections;

public class Done_DestroyByBoundary : MonoBehaviour
{
    Done_GameController gController;

    private void Start()
    {
        gController = FindObjectOfType<Done_GameController>();
    }

    void OnTriggerExit (Collider other) 
	{
        gController.AddScore(TrainingConstants.DieMalus);
        Destroy(other.gameObject);
    }
}