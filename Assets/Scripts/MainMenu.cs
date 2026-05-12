using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private Button PlayButton;
    [SerializeField] private Button QuitButton;
    [SerializeField] private Button SettingsButton;
    [SerializeField] private Button GoBackButton;
    
    public GameObject settings;
    public GameObject MainMenu;
    public GameObject BackGroundImage;
    public GameObject BackGroundImageSettings;
    
    public AudioSource _audioSource;
    /*
    private void LoadGame()
    {
        Loader.Load(Loader.SceneNames.ChooseGamemode);
    }
    */
    IEnumerable<WaitUntil> IsSoundplaying()
    {
        _audioSource.Play();
        yield return new WaitUntil(() => _audioSource.isPlaying);
    }
    

    private void Awake()
    {
        PlayButton.onClick.AddListener(() =>
        {
            SceneManager.LoadScene(Loader.SceneNames.SceneFinale.ToString());
        });
        
        QuitButton.onClick.AddListener(() =>
        {
            Application.Quit();
        });
    }
    
    public void LoadSettingsMenu()
    {
        MainMenu.SetActive(false);
        BackGroundImage.SetActive(false);
        BackGroundImageSettings.SetActive(true);
        settings.SetActive(true);
    }
}
