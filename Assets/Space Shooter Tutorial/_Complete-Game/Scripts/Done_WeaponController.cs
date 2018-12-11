using UnityEngine;
using System.Collections;

public class Done_WeaponController : MonoBehaviour
{
    private Done_GameController gController;
	public GameObject shot;
	public Transform shotSpawn;
	public float fireRate;
	public float delay;

	void Start ()
	{
        gController = FindObjectOfType<Done_GameController>();
        InvokeRepeating ("Fire", delay, fireRate);
    }

	void Fire ()
	{
		gController.RegisterHazard(Instantiate(shot, shotSpawn.position, shotSpawn.rotation));
		GetComponent<AudioSource>().Play();
	}
}
