using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HorseSelector : MonoBehaviour
{
    [SerializeField] HorseFrontend horseVisuals;
    [SerializeField] ParticleSystem hoverParticles;
    [SerializeField] ParticleSystem selectParticles;
    [SerializeField] AudioSource selectSource;

    private void OnMouseUp()
    {
        if (!horseVisuals.raceInProgess)
        {
            horseVisuals.SelectThisHorse();
            selectParticles.Play();

            selectSource.Play();
            hoverParticles.Stop();
        }
    }

    private void OnMouseEnter()
    {
        if (!horseVisuals.raceInProgess)
        {
            hoverParticles.Play();
        }
        else
        {
            hoverParticles.Stop();
        }
    }
    private void OnMouseExit()
    {
        if (!horseVisuals.raceInProgess)
        {
            hoverParticles.Stop();
        }
        else
        {
            hoverParticles.Stop();
        }
    }
}
