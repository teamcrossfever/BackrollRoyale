using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public Ghost[] ghosts;
    public Pacman pacman;
    public Pellet[] pellets;

    public int Score { get; private set; }
    public int Lives { get; private set; }

    private void Start()
    {
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

    private void NewGame()
    {
        SetScore(0);
        SetLives(3);
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
        for (int i = 0; i < ghosts.Length; i++)
        {
            ghosts[i].gameObject.SetActive(true);
        }

        pacman.gameObject.SetActive(true);
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
        SetScore(this.Score + ghost.points);
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
}
