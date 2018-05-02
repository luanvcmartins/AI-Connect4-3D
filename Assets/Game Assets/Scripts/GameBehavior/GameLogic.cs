using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using TMPro;
using UnityEngine.PostProcessing;

public class GameLogic : MonoBehaviour
{
    // Public references of objects in the scene, we will need those to interact with them:
    public GameObject boardPieceHolder;
    public GameObject boardPiece;
    public Material machineMaterial;
    public Material playerMaterial;
    public bool controlEnabled = false;
    public GameObject mainMenuBody;

    public Camera mainCamera;

    // We will be using this audiosource to play sound effects:
    public AudioSource audioSourceEffects;
    public AudioClip moveEffect; // This is a reference to the "movement" sound effect.
    public AudioClip newGameEffect;
    public AudioClip errorEffect;
    public AudioClip gameOverEffect;

    public CameraBehavior cameraBehavior;
    public TextMeshProUGUI text;

    public bool isPlaying = false;
    private int difficult = 6;

    public PostProcessingProfile highPP;

    // We need to store the local positions of where the pieces should go:
    private static float[] rows = new float[]{
        -0.1698f,
        -0.1148f,
        -0.057f,
        0.0013f,
        0.0587f,
        0.117f
    };
    private static float[] columns = new float[]{
        -0.0888f,
        -0.0289f,
        0.0298f,
        0.0867f,
        0.1442f,
        0.2017f,
        0.2586f
    };

    // Below are the two instances of the IA: the current board and the Minimax algorithm:
    private Board currentGameBoard;
    private Minimax<Board> gameAI;
    private Connect4GameRules gameRules;
    private bool autoPlay = false;

    void Start()
    {
        currentGameBoard = new Board();
        gameRules = new Connect4GameRules();
        gameAI = new Minimax<Board>(gameRules);
    }

    /**
     * Clears the board.
     */
    public void ClearBoard()
    {
        foreach (Transform piece in boardPieceHolder.transform)
            GameObject.Destroy(piece.gameObject);
    }

    /**
     * Setup the game, board, camera and logic.
     */
    public void StartGame(int depth)
    {
        currentGameBoard = new Board();
        ClearBoard();
        text.text = "";
        controlEnabled = true;
        this.difficult = depth;
        audioSourceEffects.PlayOneShot(newGameEffect, 1f);
        ResumeGame();
    }

    /**
     * Plays a match by itself.
     */
    public void AutoPlay(int depth)
    {
        // Setup the game view:
        StartGame(depth);
        autoPlay = true;
        // Configure a random player to start:
        System.Random r = new System.Random();
        int player = r.Next(2);
        if (player == 0) player = -1;
        Debug.Log("Choose: " + (player == -1 ? "Yellow" : "Red") + " to start.");

        // Register a random movement:
        if (player == -1) { currentGameBoard.registerPlayer2Movement(r.Next(6)); }
        else { currentGameBoard.registerPlayer1Movement(r.Next(6)); }

        Debug.Log(currentGameBoard);

        // Start the playing process:
        StartCoroutine(startAutoPlay(player * -1));
    }

    /**
     * Returns to the main menu.
     */
    public void MainMenu()
    {
        cameraBehavior.disableControl();
        cameraBehavior.moveCameraTo(cameraBehavior.MenuPosition);
        isPlaying = false;
    }

    /**
     * Deals with the necessary chances to the game's state to 
     * display the game over screen.
     */
    public void GameOver(int winner)
    {
        controlEnabled = false;
        cameraBehavior.disableControl();
        cameraBehavior.moveCameraTo(cameraBehavior.MenuPosition);
        mainMenuBody.SetActive(true);

        // Display who won:
        if (winner == 0)
            text.text = "The game ended with a tie!";
        else
        {
            if (autoPlay)
                text.text = (winner == -1 ? "Yellow" : "Red") + " player won!";
            else
            {
                if (winner == 1) text.text = "YOU WON!";
                else if (winner == -1) text.text = "YOU LOSE.";
            }
        }
        isPlaying = false;
        autoPlay = false;
    }

    /**
     * Returns to the current game.
     */
    public void ResumeGame()
    {
        cameraBehavior.enableControl();
        cameraBehavior.moveCameraTo(cameraBehavior.GamePosition);
        isPlaying = true;
    }

    /**
     * This function registers the human player movement.
     */
    public void RegisterPlayerMovement(int column)
    {
        // If the controls are enabled, simple register the player movement on the board and start the
        if (!controlEnabled) { audioSourceEffects.PlayOneShot(errorEffect, 1); return; }
        if (currentGameBoard.registerPlayer1Movement(column))
        {
            // We now will render the board again and start the Minimax execution:
            renderBoard(currentGameBoard, false);
            int winner = 0;

            // Checking if the game is over:
            if (gameRules.IsGameOver(currentGameBoard, ref winner))
                StartCoroutine(executeWonScene(winner));
            else
                // The game is not over, let the AI play:
                StartCoroutine(executeMachinePlay());
        }
        // This is a illegal move, play a error effect:
        else audioSourceEffects.PlayOneShot(errorEffect, 1);
    }

    /**
     * Renders the pieces of the board, should be called everytime the board has changed.
     */
    private void renderBoard(Board board, bool fastUpdate)
    {
        //Clear the board when we are doing a full update:
        if (!fastUpdate) ClearBoard();

        // We always call this function when a new move is made, so let's play a sound effect:
        audioSourceEffects.PlayOneShot(moveEffect, 1);


        for (int x = 0; x < board.Get.Length; x++)
            for (int y = 0; y < board.Get[x].Length; y++)
                if (board.Get[x][y] != 0)
                    // For each position in the board, if it's not empty and we are doing a full update:
                    if (!fastUpdate || (currentGameBoard.Get[x][y] != board.Get[x][y]))
                    {
                        // Add this piece to the board:
                        GameObject piece = Instantiate(boardPiece);
                        piece.transform.parent = boardPieceHolder.transform;
                        piece.transform.localPosition = new Vector3(columns[y], rows[x], 0.1031f);
                        piece.GetComponent<Renderer>().material = board.Get[x][y] == -1 ? machineMaterial : playerMaterial;
                    }

        // Usefull when we are not doing a full update to sync the current state of the game:
        currentGameBoard = board;
    }

    /**
     * Executes a full game automatically.
     */
    IEnumerator startAutoPlay(int player)
    {
        bool isWorking = true;
        int gameWinner = 0;
        controlEnabled = false;

        // While the game is not over:
        while (!gameRules.IsGameOver(currentGameBoard, ref gameWinner))
        {
            isWorking = true;
            new Thread(() =>
            {
                // Play the next movement for the current player:
                currentGameBoard = gameAI.PlayNextMove(currentGameBoard, player, difficult + (player == 1 ? 0 : 0));
                player = player * -1;
                isWorking = false;
            }).Start();

            while (isWorking)
                yield return null;

            // The player has decided the movement, let's re render the board:
            renderBoard(currentGameBoard, false);
        }

        // The game has ended, let's proceed to the end screen with the winner:
        StartCoroutine(executeWonScene(gameWinner));
    }

    /**
     * Start he minimax algorithm execution on a new thread.
     * We need to call it inside a Coroutine because we need to know when it ends
     * to render the board but we can't just append a callback to the function, 
     * as Unity requires all manipulations of the engine to be on the Main Thread.
     */
    IEnumerator executeMachinePlay()
    {
        controlEnabled = false;
        bool isWorking = true;
        Thread worker = new Thread(() =>
        {
            currentGameBoard = gameAI.PlayNextMove(currentGameBoard, difficult);
            isWorking = false;
        });
        worker.Start();

        while (isWorking)
            yield return null;

        // The algorithm has finished executing, let us update the board:
        renderBoard(currentGameBoard, false);
        controlEnabled = true;

        int winner = 0;
        if (gameRules.IsGameOver(currentGameBoard, ref winner))
            StartCoroutine(executeWonScene(winner));
    }

    /**
     * Waits 2 seconds and proceeds to the game over screen.
     */
    IEnumerator executeWonScene(int winner)
    {
        audioSourceEffects.PlayOneShot(gameOverEffect, 1);
        controlEnabled = false;

        // We will wait a little so the player have time to see the board.
        bool isWorking = true;
        new Thread(() =>
        {
            Thread.Sleep(2000);
            isWorking = false;
        }).Start();

        while (isWorking)
            yield return null;

        // Now we go to the game over screen:
        GameOver(winner);
    }

    /**
     * Disable all post processing effects if it enabled or enables them if currently disabled.
     */
    public void SwitchPPQuality()
    {
        PostProcessingBehaviour cPP = mainCamera.GetComponent<PostProcessingBehaviour>();
        cPP.enabled = !cPP.enabled;
    }
}
