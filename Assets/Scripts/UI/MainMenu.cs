using System.Collections.Generic;
using Definitions;
using GameCycle;
using Gameplay;
using Gameplay.Abilities;
using Genes;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class MainMenu : MonoBehaviour
    {
        [SerializeField] private GameObject rootGO;
        [SerializeField] private GameObject gameUICanvas;
        [SerializeField] private TutorialMenu tutorialMenu;
        [SerializeField] private bool showOnStartup;
        [SerializeField] private GameObject gameOverGO;
        [SerializeField] private Text statsText;
        
        public delegate void MainMenuEvent();
        public static event MainMenuEvent OnResetRequested;
        
        private void Start()
        {
            Application.targetFrameRate = 60;
        
            if(showOnStartup)
            {
                Time.timeScale = 0;
                rootGO.SetActive(true);
                gameUICanvas.SetActive(false);
            }
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
            rootGO.SetActive(true);
        }

        public void ShowGameOver()
        {
            Time.timeScale = 0;
            statsText.text = StatRecorder.Print();
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
            OnResetRequested?.Invoke();
            BreedingManager.Instance.TrioGene = TrioGene.Zero;
            BreedingManager.Instance.OnResetRequested();
            rootGO.SetActive(false);
            gameUICanvas.SetActive(true);
            CreateFirstEggBed();
            Time.timeScale = 1;
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
            bed.AddEggs(eggs);
            bed.transform.position = new Vector3(15, 15, 0);
        }

        public void Exit() => Application.Quit();
    }
}