using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class MatchManager : MonoBehaviour
{
    public Oni[] onis;
    public Pacman pacman;
    public PacEntity[] players;
    public Pellet[] pellets;
    public Transform[] escapePoints;

    public int ghostMultiplier { get; private set; } = 1;
    public int Score { get; private set; }
    public int Lives { get; private set; }
    bool isStartingNewRound = false;
    int speedIncreaseCount = 0;
    int maxSpeedIncrease = 5;

    [Header("Sounds")]
    [SerializeField]
    AudioClip[] munchsfx;
    int currentMunch = 0;

    [SerializeField]
    AudioSource musicPlayer;
    AudioSource audioPlayer;

    Coroutine r_PowerPelletModeRoutine = null;
    private void Start()
    {
        Screen.SetResolution(1920, 1080, true);

        audioPlayer = GetComponent<AudioSource>();
        pellets = FindObjectsOfType<Pellet>();
        Invoke(nameof(NewGame), 3);
        InvokeRepeating(nameof(ResetPellets), 0,20);
        InvokeRepeating(nameof(IncreaseGameSpeed),0, 10);
        enabled = false;
    }


    private void Initialize()
    {
        enabled = true;
        players = FindObjectsOfType<PacEntity>();

        if (onis==null || onis.Length < 1)
        {
            onis = FindObjectsOfType<Oni>();
            foreach(var oni in onis)
            {
                oni.Initialize();
            }
        }

        if (!pacman)
            pacman = FindObjectOfType<Pacman>();

    }

    private void NewGame()
    {
        Initialize();
        SetScore(0);
        SetLives(999);
        NewRound();
        musicPlayer.Play();
    }

    private void NewRound()
    {
        isStartingNewRound = false;
        ResetState();
        ResetPellets();
        Pause();
        Invoke(nameof(UnPause), 3);
    }

    private void Pause()
    {
        enabled = false;
    }

    private void UnPause()
    {
        enabled = true;
    }
    
    void ResetPellets()
    {
        for (int i = 0; i < pellets.Length; i++)
        {
            pellets[i].gameObject.SetActive(true);
        }
    }

    /// <summary>
    /// Speed up the game over time
    /// </summary>
    void IncreaseGameSpeed()
    {
        if (speedIncreaseCount >= maxSpeedIncrease)
            return;

        for(int i=0; i < players.Length; i++)
        {
            players[i].pacman.SpeedIncreaseMultiplier(1.1f);
        }

        for (int i = 0; i < onis.Length; i++)
        {
            onis[i].ChangeSpeedMultiplier(1.1f);
        }

        speedIncreaseCount++;
    }

    private void OnDisable()
    {
        Update();
        FixedUpdate();
    }

    private void Update()
    {

        float delta = enabled ? Time.deltaTime : 0;
        for(int i=0; i<players.Length; i++)
        {
            players[i].pacman.EntityUpdate(delta);
        }

        for (int i = 0; i < onis.Length; i++)
        {
            onis[i].EntityUpdate(delta);
        }
    }

    private void FixedUpdate()
    {
        float delta = enabled ? Time.fixedDeltaTime : 0;
        for (int i = 0; i < players.Length; i++)
        {
            players[i].pacman.EntityFixedUpdate(delta);
        }

    }

    /// <summary>
    /// Refresh state for ghost and pacman
    /// </summary>
    private void ResetState()
    {
        ResetGhostMultiplier();

        for (int i = 0; i < players.Length; i++)
        {
            players[i].Pac.pacman.ResetState();
        }

        for (int i = 0; i < onis.Length; i++)
        {
            onis[i].ResetState();
        }
    }

    public Transform RandomPlayer(bool aliveOnly)
    {
        int r = 0;
        if (!aliveOnly)
        {
            r = Random.Range(0, players.Length);
            return players[r].GetComponent<Transform>();
        }

        var alivePlayers = System.Array.FindAll(players, p => p.IsAlive);
        if(alivePlayers!=null && alivePlayers.Length > 0)
        {
            r = Random.Range(0, alivePlayers.Length);
            return alivePlayers[r].GetComponent<Transform>();
        }

        return null;
    }

    public Transform FindFarEscapePoint(Transform t)
    {
        float dist = 0;
        var escape = escapePoints[0];
        for(int i=0; i<escapePoints.Length; i++)
        {
            float newDist = Vector3.Distance(t.position, escapePoints[i].position);
            if (newDist > dist)
            {
                escape = escapePoints[i];
                dist = newDist;
            }
        }
        return escape;
    }

    private void GameOver()
    {
        for (int i = 0; i < onis.Length; i++)
        {
            onis[i].gameObject.SetActive(false);
        }

        pacman.gameObject.SetActive(false);
    }

    private void SetScore(int score)
    {
        this.Score = score;
    }

    private void SetLives(int lives)
    {
        this.Lives = lives;
    }

    public void OniEaten(Oni oni)
    {
        if (!oni)
            return;

        int points = oni.points * this.ghostMultiplier;
        SetScore(this.Score + points);
        this.ghostMultiplier++;

        oni.Eaten();
    }

    public void PacmanEaten(Pacman pac)
    {
        pac.Pac.IsAlive = false;
        pac.gameObject.SetActive(false);

        //Start new Round
        if (!isStartingNewRound)
        {
            var alivePlayers = System.Array.FindAll(players, p => p.IsAlive);
            if (alivePlayers != null && alivePlayers.Length < 2)
            {
                Invoke(nameof(NewRound), 3);
                isStartingNewRound = true;
            }
        }

        //SetLives(this.Lives - 1);

        /*
        if (Lives > 0)
        {
            Invoke(nameof(ResetState), 3.0f);
        }
        else
        {
            GameOver();
        }*/
    }

    public void PelletEaten(Pellet pellet, Pacman pacman)
    {
        pellet.Collect();

        PlayOneShot(munchsfx[currentMunch]);
        currentMunch = currentMunch == 0 ? 1 : 0;

        if (pellet.isPower)
        {
            for(int i=0; i<this.onis.Length; i++)
            {
                var oni = this.onis[i];
                oni.Frightened(pellet.duration);
            }
            //CancelInvoke(nameof(ResetGhostMultiplier));
            //Invoke(nameof(ResetGhostMultiplier), pellet.duration);

            pacman.PowerUp();
            if (r_PowerPelletModeRoutine != null)
                StopCoroutine(r_PowerPelletModeRoutine);

            r_PowerPelletModeRoutine = StartCoroutine(PowerPelletMode(pellet.duration));
        }

        if (!HasRemainingPellets())
        {
            //this.pacman.gameObject.SetActive(false); //Dont kill pacman
            //Invoke(nameof(NewRound), 3.0f);
        }
    }

    IEnumerator PowerPelletMode(float duration)
    {
        yield return new WaitForSeconds(duration);

        for(int i=0; i< players.Length; i++)
        {
            players[i].pacman.PowerDown();
        }

        yield return null;
    }

    bool HasRemainingPellets()
    {
        foreach (var pellet in this.pellets)
        {
            //There are pellets left
            if (pellet.gameObject.activeSelf)
            {
                return true;
            }
        }
        return false;
    }

    void ResetGhostMultiplier()
    {
        ghostMultiplier = 1;
    }

    void PlayOneShot(AudioClip clip)
    {
        audioPlayer.PlayOneShot(clip);
    }

}
