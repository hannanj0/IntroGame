using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// The MainMenu script controls the main menu, allowing you to play the game, view the game instructions or quit the game.
/// </summary>
public class MainMenu : MonoBehaviour
{
    public GameObject mainMenuScreen; // Main menu screen object to hide and set visible.
    public GameObject instructionsPage1; // Instructions page 1 object to hide and set visible.
    public GameObject instructionsPage2; // Instructions page 2 object to hide and set visible.

    public GameObject newGameDialog;
    public GameObject loadGameDialog;
    public GameObject noSavesFoundDialog;
    public GameObject quitGameDialog;

    public void NewGame()
    {
        mainMenuScreen.SetActive(false);
        newGameDialog.SetActive(true);
    }

    /// <summary>
    /// Load the game and set time to flow at the normal rate.
    /// </summary>
    public void NewGame_Yes()
    {
        Time.timeScale = 1;
        SceneManager.LoadSceneAsync(1);
    }

    public void NewGame_No()
    {
        newGameDialog.SetActive(false);
        mainMenuScreen.SetActive(true);
    }

    public void LoadGame()
    {
        mainMenuScreen.SetActive(false);
        loadGameDialog.SetActive(true);
    }

    public void LoadGame_Yes()
    {
        loadGameDialog.SetActive(false);
        //if save files not found
        noSavesFoundDialog.SetActive(true);
    }

    public void LoadGame_No()
    {
        loadGameDialog.SetActive(false);
        //if save files not found
        noSavesFoundDialog.SetActive(true);
    }

    public void ConfirmNoSavesFound()
    {
        noSavesFoundDialog.SetActive(false);
        mainMenuScreen.SetActive(true);
    }

    /// <summary>
    /// Load the main menu screen and hide the instructions pages.
    /// </summary>
    public void LoadMainMenu()
    {
        mainMenuScreen.SetActive(true);

        if (instructionsPage1.activeSelf)
        {
            instructionsPage1.SetActive(false);
        }

        if (instructionsPage2.activeSelf)
        {
            instructionsPage2.SetActive(false);
        }
    }

    /// <summary>
    /// Load the first instructions page and check to hide the main menu and second instructions page.
    /// </summary>
    public void LoadInstructionsPage1()
    {
        instructionsPage1.SetActive(true);

        if (mainMenuScreen.activeSelf)
        {
            mainMenuScreen.SetActive(false);
        }

        if (instructionsPage2.activeSelf)
        {
            instructionsPage2.SetActive(false);
        }
    }

    /// <summary>
    /// Load the second instructions page and check to hide the main menu and first instructions page.
    /// </summary>
    public void LoadInstructionsPage2()
    {
        instructionsPage2.SetActive(true);

        if (mainMenuScreen.activeSelf)
        {
            mainMenuScreen.SetActive(false);
        }

        if (instructionsPage1.activeSelf)
        {
            instructionsPage1.SetActive(false);
        }
    }

    public void QuitGame()
    {
        mainMenuScreen.SetActive(false);
        quitGameDialog.SetActive(true);
    }

    public void QuitGame_Yes()
    {
        Application.Quit();
    }

    public void QuitGame_No()
    {
        quitGameDialog.SetActive(false);
        mainMenuScreen.SetActive(true);
    }
}

