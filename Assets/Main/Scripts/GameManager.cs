using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public static event Action<GameState> OnGameStateChanged;

    public GameState State;

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        UpdateGameState(GameState.DEFAULT);
    }

    public void UpdateGameState(GameState newState)
    {
        State = newState;

        switch (newState)
        {
            case GameState.DEFAULT:
                break;
            case GameState.WAITING:
                break;
            case GameState.PLAYING:
                break;
            case GameState.PAUSED:
                break;
            case GameState.BUYING:
                break;
            case GameState.GAMESTART:
                break;
            case GameState.GAMEOVER:
                break;
            case GameState.LOBBY:
                break;
            case GameState.MENU:
                break;
            case GameState.OPTIONS:
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(newState), newState, null);
        }
        OnGameStateChanged?.Invoke(newState);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public enum GameState
    {
        DEFAULT,      //Fall-back state, should never happen
        WAITING,      //waiting for other player to finish his turn
        PLAYING,      //My turn
        PAUSED,        //Game paused
        BUYING,       //Buying something new
        GAMEOVER,
        GAMESTART,
        LOBBY,        //Player is in the lobby
        MENU,         //Player is viewing in-game menu
        OPTIONS       //player is adjusting game options
    };
}
