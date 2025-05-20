using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;

public class RaceFrontend : MonoBehaviour
{
    [Header("References")]
    [SerializeField] GameState gameState;
    [SerializeField] ScoreboardVisuals scoreboardVisuals;
    [SerializeField] HorseAttributes[] horseAttributes;
    [SerializeField] HorseFrontend[] horseVisuals;
    [SerializeField] Animator cameraAnimator;
    [SerializeField] GameObject speedLines;

    [Header("UI Settings")]
    [SerializeField] TMP_Text startCountdownText;
    [SerializeField] int startCountdownTime = 3;
    [SerializeField] TMP_Text raceCountdownText;
    [SerializeField] TMP_Text currentPointsText;
    [SerializeField] GameObject selectHorsePrompt;

    [Header("Audio")]
    [SerializeField] AudioSource countdownSource;
    [SerializeField] AudioClip[] countdownClips;
    [SerializeField] AudioClip countdownEnd;
    [SerializeField] AudioSource winSource;

    RaceBackend raceManager;

    int[] liveScoreboard;

    bool horseSelected;

    bool gameLoaded;

    Coroutine raceCoroutine;

    public bool raceStart { get; private set; }

    // Start is called before the first frame update
    void Start()
    {
        gameLoaded = false;

        DOTween.Init();
        RaceInitialization();
    }

    public void RaceInitialization(bool reset = false)
    {
        scoreboardVisuals.HideScoreboard();

        if (reset && gameState != null) gameState.raceFinished = true;

        raceManager = new RaceBackend(horseAttributes, 30f, Random.Range(int.MinValue, int.MaxValue), gameState);

        if (!gameLoaded)
        {
            raceManager.LoadGame(gameState);
            gameLoaded = true;
        }

        for (int i = 0; i < horseVisuals.Length; i++)
        {
            horseVisuals[i].InitailizeHorse(this, horseAttributes[i]);
        }

        if (gameState.raceFinished || gameState.selectedHorse == -1)
        {
            gameState.selectedHorse = -1;
            horseSelected = false;
        }
        else
        {
            horseSelected = true;
            horseVisuals[gameState.selectedHorse].SelectThisHorse();
        }

        for (int i = 0; i < horseVisuals.Length; i++)
        {
            horseVisuals[i].raceInProgess = false;
        }

        
        raceStart = false;

        currentPointsText.text = $"Points: {gameState.points}";

        gameState.raceFinished = false;

        liveScoreboard = new int[horseVisuals.Length];
        for (int i = 0; i < liveScoreboard.Length; i++)
        {
            liveScoreboard[i] = i;
        }

        raceCoroutine = StartCoroutine(StartRace());
    }

    IEnumerator StartRace()
    {
        selectHorsePrompt.gameObject.SetActive(true);

        raceCountdownText.gameObject.SetActive(false);
        startCountdownText.gameObject.SetActive(false);

        yield return new WaitUntil(() => horseSelected);

        for (int i = 0; i < horseVisuals.Length; i++)
        {
            horseVisuals[i].raceInProgess = true;
        }

        selectHorsePrompt.gameObject.SetActive(false);

        //Start Countdown
        startCountdownText.gameObject.SetActive(true);
        for (int i = 0; i < startCountdownTime; i++)
        {
            countdownSource.PlayOneShot(countdownClips[i]);
            startCountdownText.text = (startCountdownTime - i).ToString(); //Set countdown text
            yield return new WaitForSeconds(1);
        }

        raceStart = true;
        for (int i = 0; i < horseVisuals.Length; i++)
        {
            horseVisuals[i].StartRace(horseAttributes[i].horseWeight, horseAttributes[i], raceManager.scoreboardInverse[i]);
        }

        countdownSource.PlayOneShot(countdownEnd);
        startCountdownText.text = "GO!";
        yield return new WaitForSeconds(0.75f);
        startCountdownText.gameObject.SetActive(false);

        raceCountdownText.gameObject.SetActive(true);
        float timer = 0;
        while(timer < raceManager.raceLength)
        {
            raceCountdownText.text = ((int)(raceManager.raceLength - timer)).ToString();
            yield return new WaitForSeconds(1f);
            timer += 1;
        }
        raceCountdownText.gameObject.SetActive(false);

        EndRace();
    }

    private void EndRace()
    {
        raceStart = false;
        gameState.raceFinished = true;

        raceCountdownText.gameObject.SetActive(false);

        for (int i = 0; i < raceManager.scoreboard.Length; i++)
        {
            horseVisuals[raceManager.scoreboard[i]].SetHorsePosition(i);
        }

        scoreboardVisuals.SetScoreboard(raceManager.scoreboard, raceManager.scoreboardInverse, horseAttributes, gameState.points, gameState.selectedHorse, raceManager.scores);

        gameState.points += raceManager.scores[raceManager.scoreboardInverse[gameState.selectedHorse]];

        if(raceManager.scoreboardInverse[gameState.selectedHorse] <= 2)
        {
            winSource.Play();
        }

        SaveCurrentGame();
    }

    // Update is called once per frame
    void Update()
    {
        if(raceStart)
        {
            SortLiveScoreboard(liveScoreboard, horseVisuals);

            for (int i = 0; i < liveScoreboard.Length; i++)
            {
                horseVisuals[liveScoreboard[i]].SetHorsePosition(i);
            }
        }
    }

    Coroutine zoomCoroutine;
    float startZoomTime;
    float currentZoomLength;
    public void ZoomIn(float length)
    {
        if(zoomCoroutine != null)
        {
            if (currentZoomLength - (Time.time - startZoomTime) < length && length > 1f)
            {
                StopCoroutine(zoomCoroutine);

                zoomCoroutine = StartCoroutine(Zoom(length));
            }
        }
        else if (length > 1f)
        {
            zoomCoroutine = StartCoroutine(Zoom(length));
        }
    }
    public IEnumerator Zoom(float length)
    {
        startZoomTime = Time.time;
        currentZoomLength = length;

        cameraAnimator.SetBool("Zoom", true);
        speedLines.SetActive(true);

        yield return new WaitForSeconds(length);

        cameraAnimator.SetBool("Zoom", false);
        speedLines.SetActive(false);
    }
    public void ZoomOut()
    {
        if (zoomCoroutine != null)
        {
            StopCoroutine(zoomCoroutine);

            cameraAnimator.SetBool("Zoom", false);
            speedLines.SetActive(false);
        }
    }

    public void OnCancelAction()
    {
        print("Cancel Race");
        for (int i = 0; i < horseVisuals.Length; i++)
        {
            horseVisuals[i].CancelRace();
        }

        if(raceCoroutine != null) 
        {
            StopCoroutine(raceCoroutine);
        }

        //gameState.selectedHorse = -1;

        ZoomOut();
        RaceInitialization(true);
    }
    public void OnSkipAction()
    {
        print("Skip Race");
        if(gameState.selectedHorse == -1)
        {
            gameState.selectedHorse = Random.Range(0, 8);
            horseVisuals[gameState.selectedHorse].SelectThisHorse();
        }

        for (int i = 0; i < horseVisuals.Length; i++)
        {
            horseVisuals[i].SkipRace();
        }
        if (raceCoroutine != null)
        {
            StopCoroutine(raceCoroutine);
        }

        ZoomOut();
        EndRace();
    }
    public void ResetRace()
    {
        print("Reset Race");
        for (int i = 0; i < horseVisuals.Length; i++)
        {
            horseVisuals[i].CancelRace();
        }
        if (raceCoroutine != null)
        {
            StopCoroutine(raceCoroutine);
        }

        ZoomOut();
        RaceInitialization(true);
    }

    public void SortLiveScoreboard(int[] scoreboard, HorseFrontend[] horses)
    {
        int i, j, temp;
        bool swapped;
        for (i = 0; i < scoreboard.Length - 1; i++)
        {
            swapped = false;
            for (j = 0; j < scoreboard.Length - i - 1; j++)
            {
                if (horses[scoreboard[j]].DistProgress < horses[scoreboard[j + 1]].DistProgress)
                {

                    // Swap scoreboard[j] and scoreboard[j+1]
                    temp = scoreboard[j];
                    scoreboard[j] = scoreboard[j + 1];
                    scoreboard[j + 1] = temp;
                    swapped = true;
                }
            }

            // If no two elements werent
            // swapped by inner loop, then break
            if (swapped == false)
                break;
        }
    }

    public void SelectHorse(int index)
    {
        if(gameState.selectedHorse == -1)
        {
            gameState.selectedHorse = index;
            horseSelected = true;
        }
    }

    public void SaveCurrentGame()
    {
        if(raceManager != null)
            raceManager.SaveGame(gameState);
    }
}
