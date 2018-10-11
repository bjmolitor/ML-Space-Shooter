using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAgents;

public class SpaceShooterAcademy : Academy
{
    public float playerTimeScale = 0.5f;

    /*private PlayerAgent playerAgent;

    public override void InitializeAcademy()
    {
        base.InitializeAcademy();
        playerAgent = FindObjectOfType<PlayerAgent>();
    }

    public override void AcademyReset()
    {
        base.AcademyReset();

        //FindObjectOfType<PlayerAgent>();
        // Slow the game down, if the PlayerAgent in manually controlled
        if (playerAgent.brain.brainType == BrainType.Player) {
            ApplyPlayerTimeScale();
        }

    }

    public void ApplyPlayerTimeScale()
    {
        Time.timeScale = playerTimeScale;
        Time.fixedDeltaTime = 0.02F * Time.timeScale;
    }
    */
}
