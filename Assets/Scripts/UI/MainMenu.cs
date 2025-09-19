using System;
using System.Collections.Generic;
using Definitions;
using GameCycle;
using Gameplay;
using Gameplay.Breeding;
using Gameplay.Genes;
using Gameplay.Mutations;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace UI
{
    public class MainMenu : MonoBehaviour
    {
#if UNITY_EDITOR
        [SerializeField] private bool showOnStartup;
#endif
        [SerializeField] private GameObject rootGO;
        [SerializeField] private GameObject gameUICanvas;
        [SerializeField] private GameObject gameOverGO;
        [SerializeField] private GameObject settingsGO;
        [SerializeField] private GameObject difficultyGO;
        [SerializeField] private TutorialMenu tutorialMenu;
        [SerializeField] private PauseMenu pauseMenu;
        [SerializeField] private Text gameOverStatsText;
        [SerializeField] private Text subtitleText;
        
        private bool settingsOpenedFromMainMenu;
        
        public delegate void MainMenuEvent();

        public static event MainMenuEvent OnResetRequested;
        
        private void Start()
        {
            subtitleText.text = $"{Application.version}\nby RivesUvaCrispa\naka Cpeed4";
#if UNITY_EDITOR
            if (showOnStartup) ShowMainMenu();
            else ResetGame();
#else
            ShowMainMenu();
#endif
        }



        public void ShowDifficultyMenu()
        {
            rootGO.SetActive(false);
            difficultyGO.SetActive(true);
        }

        public void ShowMainMenu()
        {
            difficultyGO.SetActive(false);
            pauseMenu.gameObject.SetActive(false);
            Time.timeScale = 0;
            rootGO.SetActive(true);
            gameUICanvas.SetActive(false);
        }

        public void ShowTutorial()
        {
            rootGO.SetActive(false);
            gameUICanvas.SetActive(true);
            tutorialMenu.Show();
        }

        public void CloseTutorial()
        {
            tutorialMenu.Close();
            gameUICanvas.SetActive(false);
            rootGO.SetActive(true);
        }

        public void ShowGameOver()
        {
            Time.timeScale = 0;
            gameOverStatsText.text = StatRecorder.Print();
            gameOverGO.SetActive(true);
            gameUICanvas.SetActive(false);
        }

        public void CloseGameOver()
        {
            rootGO.SetActive(true);
            gameOverGO.SetActive(false);
        }
        
        public void Play()
        {
            ResetGame();
            rootGO.SetActive(false);
            gameUICanvas.SetActive(true);
            Time.timeScale = 1;
        }

        public void Restart()
        {
            ResetGame();
            pauseMenu.gameObject.SetActive(false);
        }

        public void ShowSettingsMenu(bool fromMainMenu)
        {
            settingsOpenedFromMainMenu = fromMainMenu;
            if(fromMainMenu) rootGO.SetActive(false);
            else
            {
                pauseMenu.gameObject.SetActive(false);
                Time.timeScale = 0;
            }
            settingsGO.SetActive(true);
        }

        public void CloseSettingsMenu()
        {
            settingsGO.SetActive(false);
            if (settingsOpenedFromMainMenu)
                rootGO.SetActive(true);
            else
                pauseMenu.gameObject.SetActive(true);
        }
        
        public void Pause() => pauseMenu.gameObject.SetActive(true);

        public void Exit() => Application.Quit();

        
        
        private void ResetGame()
        {
            OnResetRequested?.Invoke();
            CreateFirstEggBed();
        }
        
        private void CreateFirstEggBed()
        {
            var bed = Instantiate(GlobalDefinitions.EggBedPrefab, GlobalDefinitions.GameObjectsTransform);
            int amount = Random.Range(1, 7);
            var eggs = new List<Egg>();
            while (amount > 0)
            {
                Egg egg = new Egg(TrioGene.Zero, new MutationData());
                eggs.Add(egg);
                amount--;
            }

            StatRecorder.eggsLayed += amount;
            bed.SetEggs(eggs);
            bed.transform.position = new Vector3(15, 15, 0);
        }
    }
}