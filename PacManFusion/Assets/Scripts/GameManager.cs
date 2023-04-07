using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public Ghost[] ghosts;
    public Pacman pacman;
    public Pellet[] pellets;

    public int ghostMultiplier { get; private set; } = 1;
    public int Score { get; private set; }
    public int Lives { get; private set; }

    [Header("Sounds")]
    [SerializeField]
    AudioClip[] munchsfx;
    int currentMunch = 0;

    AudioSource audioPlayer;

    private void Start()
    {
        audioPlayer = GetComponent<AudioSource>();
        pellets = FindObjectsOfType<Pellet>();
        NewGame();
    }

    private void Update()
    {
        if (Lives <=0 && Input.anyKeyDown)
        {
            NewGame();
        }
    }

    private void Initialize()
    {
        if(ghosts==null || ghosts.Length < 1)
        {
            ghosts = FindObjectsOfType<Ghost>();
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
    }

    private void NewRound()
    {
        for(int i=0; i < pellets.Length; i++)
        {
            pellets[i].gameObject.SetActive(true);
        }

        ResetState();
    }

    /// <summary>
    /// Refresh state for ghost and pacman
    /// </summary>
    private void ResetState()
    {
        ResetGhostMultiplier();

        for (int i = 0; i < ghosts.Length; i++)
        {
            ghosts[i].ResetState();
        }

        pacman.ResetState();
    }

    private void GameOver()
    {
        for (int i = 0; i < ghosts.Length; i++)
        {
            ghosts[i].gameObject.SetActive(false);
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

    public void GhostEaten(Ghost ghost)
    {
        if (!ghost)
            return;

        int points = ghost.points * this.ghostMultiplier;
        SetScore(this.Score + points);
        this.ghostMultiplier++;

        ghost.frightened.Eaten();
    }

    public void PacmanEaten()
    {
        pacman.gameObject.SetActive(false);
        SetLives(this.Lives - 1);

        if (Lives > 0)
        {
            Invoke(nameof(ResetState), 3.0f);
        }
        else
        {
            GameOver();
        }
    }

    public void PelletEaten(Pellet pellet)
    {
        pellet.Collect();

        PlayOneShot(munchsfx[currentMunch]);
        currentMunch = currentMunch == 0 ? 1 : 0;

        if (pellet.isPower)
        {
            for(int i=0; i<this.ghosts.Length; i++)
            {
                var ghost = ghosts[i];
                ghost.frightened.Enable(pellet.duration);
            }
            CancelInvoke(nameof(ResetGhostMultiplier));
            Invoke(nameof(ResetGhostMultiplier), pellet.duration);
        }

        if (!HasRemainingPellets())
        {
            this.pacman.gameObject.SetActive(false); //Dont kill pacman
            Invoke(nameof(NewRound), 3.0f);
        }
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
