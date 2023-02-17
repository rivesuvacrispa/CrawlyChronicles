using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Camera;
using Gameplay;
using Genes;
using UI;
using UnityEngine;
using UnityEngine.UI;

namespace GameCycle
{
    public class RespawnManager : MonoBehaviour
    {
        public delegate void EggBedCollectionEvent(List<EggBed> eggBeds);
        public static EggBedCollectionEvent OnEggCollectionRequested;

        [SerializeField] private FollowMovement followMovement;
        [SerializeField] private EggBedViewer viewer;
        [SerializeField] private GameObject respawnMenuGO;
        [SerializeField] private Text eggBedSelectionText;
        [SerializeField] private GameObject leftArrow;
        [SerializeField] private GameObject rightArrow;

        private int currentEggBedIndex;
        private List<EggBed> eggBeds = new();

        private void Update()
        {
            if(Input.GetKeyDown(KeyCode.K)) StartRespawn();
        }

        public void StartRespawn()
        {
            Time.timeScale = 0;
            CollectAllEggBeds();
            eggBeds = eggBeds.OrderBy(bed => bed.transform.position.x).ToList();
            SelectEggBed(0);
            respawnMenuGO.SetActive(true);
            StartCoroutine(RespawnRoutine());
        }
        
        private void CollectAllEggBeds()
        {
            eggBeds.Clear();
            eggBeds = new List<EggBed>();
            OnEggCollectionRequested?.Invoke(eggBeds);
        }

        private IEnumerator RespawnRoutine()
        {
            while (Time.timeScale == 0)
            {
                var axis = Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow) ? -1 : 
                    Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow) ? 1 : 0;
                
                if (axis != 0) SelectEggBed(currentEggBedIndex + axis);
                
                followMovement.UpdateUnscaled();
                yield return new WaitForSecondsRealtime(0);
            }
        }

        private void SelectEggBed(int index)
        {
            int count = eggBeds.Count;
            currentEggBedIndex = Mathf.Clamp(index, 0, count - 1);
            eggBedSelectionText.text = $"{currentEggBedIndex + 1}/{count}";
            leftArrow.SetActive(currentEggBedIndex > 0);
            rightArrow.SetActive(currentEggBedIndex < count - 1);
            currentEggBedIndex = index;
            EggBed eggBed = eggBeds[currentEggBedIndex];
            followMovement.Target = eggBed.Transform;
            viewer.ShowEggs(eggBed);
        }

        public void GoLeft() => SelectEggBed(currentEggBedIndex - 1);

        public void GoRight() => SelectEggBed(currentEggBedIndex + 1);

        public void SelectEggToRespawn(TrioGene trioGene)
        {
            viewer.Disable();
            respawnMenuGO.SetActive(false);
            Time.timeScale = 1;
        }
    }
}