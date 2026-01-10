using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    static Action OnPauseChange;
    public static bool IsGamePaused
    {
        get => _isGamePaused;
        set
        {
            if (_isGamePaused != value)
            {
                _isGamePaused = value;
                OnPauseChange?.Invoke();
            }
        }
    }
    InputAction pauseAction;
    private static bool _isGamePaused = false;

    [SerializeField] GameObject escapeMenu;
    void Start()
    {
        DontDestroyOnLoad(gameObject);
        DontDestroyOnLoad(escapeMenu);

        pauseAction = InputSystem.actions.FindAction("Escape");
        OnPauseChange += () =>
        {
            escapeMenu.SetActive(IsGamePaused);
            Cursor.lockState = IsGamePaused ? CursorLockMode.None : CursorLockMode.Locked;

            if (IsGamePaused)
            {
                Time.timeScale = 0f;
            }
            else
            {
                Time.timeScale = 1f;
            }
        };
    }

    void Update()
    {
        if(pauseAction.WasPressedThisFrame())
        {
            IsGamePaused = !IsGamePaused; 
        }
    }
    public void LoadMenu()
    {
        Time.timeScale = 1f;
        IsGamePaused = false;
    }
    public void Resume()
    {
        IsGamePaused = false;
    }
    public void QuitGame(){
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
