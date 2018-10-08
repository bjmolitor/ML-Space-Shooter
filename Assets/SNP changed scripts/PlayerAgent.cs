// The player agent receives input from the brain, conducts actions and returns observations and rewards

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAgents;
using System;

[System.Serializable]
public class Boundary
{
    public float xMin, xMax, zMin, zMax;
}

public class PlayerAgent : Agent
{
    public float speed;
    public float tilt;
    public Boundary boundary;
    public Brain manualBrain;
    public Brain mlBrain;

    public GameObject shot;
    public float shotOffset;
    //public Transform shotSpawn;
    public float fireRate;
    /* For each hazard observer two observation vectors have to be added to the brain config */
    public int numHazardsObserved;

    private float nextFire;
    private Rigidbody rBody;
    private AudioSource aSource;
    private Done_GameController gController;

    // This is an "external" player agent, surviving player ship destruction. So ships needs to be assigned.
    private GameObject pShip;

    public void ManualBrain()
    {
        GiveBrain(manualBrain);
    }

    public void MLBrain()
    {
        GiveBrain(mlBrain);
    }

    public void Initialize(GameObject playerShip, Done_GameController gameController)
    {
        pShip = playerShip;
        rBody = pShip.GetComponent<Rigidbody>();
        aSource = pShip.GetComponent<AudioSource>();
        gController = gameController;
        //shotSpawn = GameObject.FindGameObjectWithTag("Respawn").transform;
    }

    public override void AgentAction(float[] vectorAction, string textAction)
    {
        // Actions - size 3
        if (brain.brainType == BrainType.Player) MoveShip(vectorAction[0], vectorAction[1]);
        // AI brains deliver 0 to 2 instead of -1 to 1.  
        else MoveShip(vectorAction[0] - 1, vectorAction[1] - 1);
        Fire(vectorAction[2]);
    }

    public override void CollectObservations()
    {
        GameObject onCollisionCourse;

        List<GameObject> currentHazards;
        List<GameObject> observedHazards = new List<GameObject>();

        //First we need to factor in our position relative to the boundaries (normalize to between -1 and 1)
        AddVectorObs(CalcRelPos(this.transform.position.x, boundary.xMin, boundary.xMax));
        AddVectorObs(CalcRelPos(this.transform.position.z, boundary.zMin, boundary.zMax));

        //The game controller has been modified to keep a list of current hazards - but needs to clear destroyed ones
        gController.TidyUpHazardList();
        currentHazards = gController.CurrentHazards;

        //The number of objects and their relative position we observe dictates the number of vectors (which is limited)
        //The observed hazards should be those closest to the agent.
        observedHazards = SelectClosest(currentHazards, numHazardsObserved);
        observedHazards.ForEach(AddHazardVectors);

        //Fill up unused observation vectors. Using 1 instead of 0, to reflect the hazard being far away (instead of close)
        for (int i = 0; i < numHazardsObserved - observedHazards.Count; i++)
        {
            AddVectorObs(1);
            AddVectorObs(1);
        }

        //since the AI can only see a certain number of hazards it's fair to give a collision warning & distance
        if (rBody == null)
            AddVectorObs(0);
        else
        { 
            // First sort hazards by their z position (descending because smaller means closer)
            currentHazards.Sort((a, b) => {
                if (a.transform.position.z > b.transform.position.z) return 1;
                else return -1;
            });

            //Find the object which is in front of the ship and on a similar x coordinate
            onCollisionCourse = currentHazards.Find(h => h.transform.position.z > rBody.transform.position.z
                && h.transform.position.x > rBody.transform.position.x - 0.5f
                && h.transform.position.x < rBody.transform.position.x + 0.5f);

            //Add it's distance as an observation vector
            if (onCollisionCourse == null) AddVectorObs(-1);
            else
            {
                AddVectorObs(Vector3.Distance(rBody.transform.position, onCollisionCourse.transform.position) / (Math.Abs(boundary.zMin) + boundary.zMax));
                //Here's a litte danger zone bonus, to make the AI less defencive and boring
                AddReward(0.01f);
            }
        }
    }

    private void AddHazardVectors(GameObject gameObject)
    {
        //Add relative x and y positions of hazards
        AddVectorObs(CalcRelPos(gameObject.transform.position.x, boundary.xMin, boundary.xMax));
        //z-Position takes into account, that objects sometimes spawn or fly outside the play field
        if (gameObject.transform.position.z > boundary.zMax) AddVectorObs(1);
        else if (gameObject.transform.position.z < boundary.zMin) AddVectorObs(-1);
        else AddVectorObs(CalcRelPos(gameObject.transform.position.z, boundary.zMin, boundary.zMax));
    }

    // returns 0..1 relative to the position between 0 and max and -1..0 for position between min and 0
    private float CalcRelPos(float pos, float min, float max)
    {
        if (pos < 0) return -1 + ((min - pos) / min);
        else return 1 - ((max - pos) / max);   
    }

    // Returns the closest N hazards
    private List<GameObject> SelectClosest(List<GameObject> list, int number)
    {
        int returnHazards = numHazardsObserved;

        if (list.Count < returnHazards) returnHazards = list.Count;
        //If there are no hazards or ship is destroyed return empty list.
        if (list.Count == 0 || rBody == null) return new List<GameObject>();

        //Sort list by smallest distance
        list.Sort(SmallestDistance);
        //Return a number of objects with the smallest distance
        return list.GetRange(0, returnHazards);
    }

    //This is used as a comparator, returning 1 if X is closer then Y.
    private int SmallestDistance(GameObject x, GameObject y)
    {
        float distanceFromX;
        float distanceFromY;

        distanceFromX = Vector3.Distance(
            x.transform.position,
            rBody.transform.position
            );
        distanceFromY = Vector3.Distance(
            y.transform.position,
            rBody.transform.position
            );

        // Actual comparison
        if (distanceFromX < distanceFromY) return 1;
        else if (distanceFromX == distanceFromY) return 0;
            else return -1; 
    }

    // Shoot a bolt
    private void Fire(float fire)
    {
        if (rBody == null) return;

        //Debug.Log("Fire:" + fire);

        if (fire > 0 && Time.time > nextFire)
        {
            nextFire = Time.time + fireRate;
            Instantiate(shot, new Vector3(rBody.position.x, rBody.position.y, rBody.position.z + shotOffset), rBody.rotation);
            aSource.Play();
        }
    }

    // This actually moves the ship
    private void MoveShip(float moveHorizontal, float moveVertical)
    {
        if (rBody == null) return;

        Vector3 movement = new Vector3(moveHorizontal, 0.0f, moveVertical);
        rBody.velocity = movement * speed;
        //Debug.Log("Horizontal: " + moveHorizontal + " Vertical: " + moveVertical);

        // Prevent ship from leaving the screen
        rBody.position = new Vector3
        (
            Mathf.Clamp(rBody.position.x, boundary.xMin, boundary.xMax),
            0.0f,
            Mathf.Clamp(rBody.position.z, boundary.zMin, boundary.zMax)
        );

        rBody.rotation = Quaternion.Euler(0.0f, 0.0f, rBody.velocity.x * -tilt);
    }

}
