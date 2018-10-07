using UnityEngine;
using System.Collections;

public class Done_DestroyByContact : MonoBehaviour
{
	public GameObject explosion;
	public GameObject playerExplosion;
	public int scoreValue;

    private Done_GameController gameController;
    //private PlayerAgent playerAgent;

	void Start ()
	{
        //playerAgent = FindObjectOfType<PlayerAgent>();

        GameObject gameControllerObject = GameObject.FindGameObjectWithTag ("GameController");
		if (gameControllerObject != null)
		{
			gameController = gameControllerObject.GetComponent <Done_GameController>();
		}
		if (gameController == null)
		{
			Debug.Log ("Cannot find 'GameController' script");
		}
	}

	void OnTriggerEnter (Collider other)
	{
		if (other.tag == "Boundary" || other.tag == "Enemy")
		{
			return;
		}

		if (explosion != null)
		{
			Instantiate(explosion, transform.position, transform.rotation);
		}

        if (other.tag == "Player")
        {
            Instantiate(playerExplosion, other.transform.position, other.transform.rotation);
            gameController.GameOver();
        }
        else gameController.AddScore(scoreValue);
        
        Destroy(other.gameObject);
        Destroy (gameObject);
	}

    private void OnDestroy()
    {
        //If you are destroyed, destroy your parent, too.
        Destroy(transform.parent);
    }
}