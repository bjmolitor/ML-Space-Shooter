using UnityEngine;
using System.Collections;

public class Done_DestroyByContact : MonoBehaviour
{
	public GameObject explosion;
	public GameObject playerExplosion;
	public int scoreValue = 20;
    public int scoreValueOnHit = 0;
    public int life = 1;

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
        life--;

        //If colide boundary or other enemy do nothing and return to game
        if (other.tag == "Boundary" || other.tag == "Enemy")
		{
			return;
		}

        //Perform explosion 
		if (explosion != null && life <= 0)
		{
			Instantiate(explosion, transform.position, transform.rotation);  
		}

        //Colide with player is game over for player
        if (other.tag == "Player")
        {
            Instantiate(playerExplosion, other.transform.position, other.transform.rotation);
            gameController.GameOver();
        }
                
        Destroy(other.gameObject);
        gameController.AddScore(scoreValueOnHit);

        //If no remaining life destroy me, 
        if (life <= 0)
        {
            Destroy(gameObject);
            gameController.AddScore(scoreValue);
        }
	}

    private void OnDestroy()
    {
        //If you are destroyed, destroy your parent, too.
        Destroy(transform.parent);
    }
}