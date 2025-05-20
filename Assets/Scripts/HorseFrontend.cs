using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;
using UnityEngine.EventSystems;

public class HorseFrontend : MonoBehaviour
{
    [Header("References")]
    [SerializeField] HorseSelector horseSelector;
    [SerializeField] Transform startTransform;
    [SerializeField] Transform endTransform;
    [SerializeField] Transform horseTransform;
    [SerializeField] Animator horseAnimator;
    [SerializeField] Transform pivotTransform;
    [SerializeField] AnimationCurve speedCurve;
    [SerializeField] ParticleSystem dustTrail;

    [Header("UI Settings")]
    [SerializeField] TMP_Text horseName;
    [SerializeField] Color defaultNameColor;
    [SerializeField] Color selectedNameColor;
    [SerializeField] TMP_Text horsePosition;
    [SerializeField] Color[] positionColors;

    [Header("Audio")]
    [SerializeField] AudioSource gallopSource;
    [SerializeField] AudioSource boostSource;

    RaceFrontend raceVisuals;

    float actualLinearSpeed;

    float timerProgress;
    float distProgress;

    public bool raceInProgess;

    public float DistProgress { get => distProgress; }

    Coroutine raceCoroutine;

    public void InitailizeHorse(RaceFrontend raceVisuals, HorseAttributes attributes)
    {
        horseName.text = attributes.name;
        horsePosition.text = "";
        horseName.color = defaultNameColor;
        this.raceVisuals = raceVisuals;
    }

    public void StartRace(float timeToFinish, HorseAttributes attributes, int endPosition)
    {
        raceCoroutine = StartCoroutine(RaceTask(timeToFinish, attributes, endPosition));
    }

    public void SetHorsePosition(int position)
    {
        switch(position)
        {
            case 0:
                horsePosition.text = "1st";
                horsePosition.color = positionColors[0];
                break;
            case 1:
                horsePosition.text = "2nd";
                horsePosition.color = positionColors[1];
                break;
            case 2:
                horsePosition.text = "3rd";
                horsePosition.color = positionColors[2];
                break;
            default:
                horsePosition.text = $"{position + 1}th";
                horsePosition.color = positionColors[3];
                break;
        }
    }

    public IEnumerator RaceTask(float timeToFinish, HorseAttributes attributes, int endPosition)
    {
        //Horse animator
        horseAnimator.SetBool("Run", true);

        gallopSource.Play();

        float timer = 0;
        timerProgress = 0; //Goes from [0,1]
        distProgress = 0; //Goes from [0,1]

        float startX = startTransform.position.x;
        float endX = endTransform.position.x;
        float currentX = startX;

        horseTransform.position = startTransform.position;
        float defaultSpeed = (endX - startX) / timeToFinish;

        actualLinearSpeed = Random.Range(defaultSpeed * 0.25f, defaultSpeed * 1.0f);
        horseAnimator.SetFloat("RunSpeed", actualLinearSpeed / defaultSpeed); //Set Horse animation speed

        //Time before the current speed is randomly changed
        float speedChangeTimer = Random.Range(3f, 6f);

        //Time before a boost is applied
        float boostStartTimer = Random.Range(6f, 30f / attributes.HorseStamina);
        float boostEndTimer = 1000f;
        bool boostStarted = false;

        while (timer < timeToFinish)
        {
            //If the timer gets to 85% or more complete or
            //the distance progress gets to 80% or more complete then
            //DOTween to the end based on remaining distance and remaining time
            //to ensure predetermined outcome
            if (timerProgress >= 0.85f || distProgress >= 0.8f)
            {
                horseTransform.DOMoveX(endX, timeToFinish * (1 - timerProgress));
                actualLinearSpeed = (endX - currentX) / (timeToFinish * (1 - timerProgress));
                horseAnimator.SetFloat("RunSpeed", actualLinearSpeed / defaultSpeed); //Set Horse animation speed

                if (actualLinearSpeed < 1.5f)
                {
                    dustTrail.Stop();
                    boostStarted = false;

                    //raceVisuals.ZoomOut();
                }
                else
                {
                    dustTrail.Play();
                    boostStarted = true;
                    boostSource.Play();
                    raceVisuals.ZoomIn(timeToFinish * (1 - timerProgress) / 2);
                }

                while(timer < timeToFinish)
                {
                    yield return null;
                    timer += Time.deltaTime;
                    timerProgress = timer / timeToFinish;
                    distProgress = (horseTransform.position.x - startX) / (endX - startX);
                }

                timerProgress = distProgress = 1;
                break;
            }
            else
            {
                //Initiate boost with dust trails and a zoom effect
                if (timer > boostStartTimer)
                {
                    if(timer > boostEndTimer)
                    {
                        boostStartTimer = timer + Random.Range(5f, 20f);
                        boostStarted = false;
                        dustTrail.Stop();
                    }
                    else if (!boostStarted)
                    {
                        actualLinearSpeed = Random.Range(defaultSpeed * 1.75f, defaultSpeed * 2.25f);
                        horseAnimator.SetFloat("RunSpeed", actualLinearSpeed / defaultSpeed); //Set Horse animation speed
                        boostEndTimer = timer + Random.Range(2f, 3f * attributes.HorseStamina);
                        boostStarted = true;
                        dustTrail.Play();
                        raceVisuals.ZoomIn(boostEndTimer - timer);
                        boostSource.Play();
                    }
                }
                //Initiate speed change if not in boost
                //speed can get gradually bigger as race progresses
                else if (timer > speedChangeTimer)
                {
                    actualLinearSpeed = Random.Range(defaultSpeed * (0.25f + (timerProgress) / 2), defaultSpeed * (0.75f + (timerProgress) / 2));
                    horseAnimator.SetFloat("RunSpeed", actualLinearSpeed / defaultSpeed); //Set Horse animation speed
                    speedChangeTimer = timer + Random.Range(2f, 5f);
                }
            }

            //Move horse forward based on current speed
            horseTransform.Translate(Vector3.right * actualLinearSpeed * Time.deltaTime);

            //Track current position relative to end
            currentX += (Vector3.right * actualLinearSpeed * Time.deltaTime).x;

            yield return null;

            timer += Time.deltaTime;
            timerProgress = timer / timeToFinish;
            distProgress = (currentX - startX) / (endX - startX);
        }
        actualLinearSpeed = 0;
        dustTrail.Stop();
        boostStarted = false;

        horseTransform.position = endTransform.position;

        //Horse animator
        horseAnimator.SetBool("Run", false);

        gallopSource.Stop();
    }

    public void CancelRace()
    {
        if (raceCoroutine != null)
        {
            StopCoroutine(raceCoroutine);
            horseTransform.DOKill();
            horseTransform.position = startTransform.position;
            horseAnimator.SetBool("Run", false);
            dustTrail.Stop();
            gallopSource.Stop();
            boostSource.Stop();
        }
    }

    public void SkipRace()
    {
        if(raceCoroutine != null)
        {
            StopCoroutine(raceCoroutine);
            horseTransform.DOKill();
            horseTransform.position = endTransform.position;
            horseAnimator.SetBool("Run", false);
            dustTrail.Stop();
            gallopSource.Stop();
            boostSource.Stop();
        }
    }

    public void SelectThisHorse()
    {
        horseName.color = selectedNameColor;
        raceVisuals.SelectHorse(transform.GetSiblingIndex());
    }

    public void ToggleSelector(bool active)
    {
        horseSelector.enabled = active;
    }
}
