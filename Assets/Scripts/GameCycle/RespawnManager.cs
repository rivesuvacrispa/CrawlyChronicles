using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Camera;
using Gameplay;
using Genes;
using Player;
using Timeline;
using UI;
using UnityEngine;
using UnityEngine.UI;

namespace GameCycle
{
    public class RespawnManager : MonoBehaviour
    {
        private static RespawnManager instance;
        
        public delegate void EggBedCollectionEvent(List<EggBed> eggBeds);
        public static EggBedCollectionEvent OnEggCollectionRequested;

        [SerializeField] private AbilityController abilityController;
        [SerializeField] private FollowMovement followMovement;
        [SerializeField] private EggBedViewer viewer;
        [SerializeField] private GameObject respawnMenuGO;
        [SerializeField] private Text eggBedSelectionText;
        [SerializeField] private GameObject leftArrow;
        [SerializeField] private GameObject rightArrow;
        [SerializeField] private MutationMenu mutationMenu;
        [SerializeField] private GameObject navigatorGO;

        private int currentEggBedIndex;
        private List<EggBed> eggBeds = new();
        private EggBed selectedEggbed;

        private RespawnManager() => instance = this;
        
        /*private void Update()
        {
            if(Input.GetKeyDown(KeyCode.K)) StartRespawn();
        }*/

        public static void Respawn() => instance.StartRespawn();

        
        private void StartRespawn()
        {
            selectedEggbed = null;
            Time.timeScale = 0;
            navigatorGO.SetActive(eggBeds.Count > 0);
            eggBeds = eggBeds.OrderBy(bed => bed.transform.position.x).ToList();
            SelectEggBed(0);
            AbilityController.SetUIActive(false);
            respawnMenuGO.SetActive(true);
            StartCoroutine(RespawnRoutine());
        }

        public static int CollectEggBeds() => instance.CollectEggBedsNonStatic();
        
        private int CollectEggBedsNonStatic()
        {
            eggBeds.Clear();
            eggBeds = new List<EggBed>();
            OnEggCollectionRequested?.Invoke(eggBeds);
            return eggBeds.Count;
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
            if (count == 1) currentEggBedIndex = 0;
            else currentEggBedIndex = Mathf.Clamp(index, 0, count - 1);
            eggBedSelectionText.text = $"{currentEggBedIndex + 1}/{count}";
            leftArrow.SetActive(currentEggBedIndex > 0);
            rightArrow.SetActive(currentEggBedIndex < count - 1);
            selectedEggbed = eggBeds[currentEggBedIndex];
            followMovement.Target = selectedEggbed.Transform;
            viewer.ShowEggs(selectedEggbed);
        }

        public void GoLeft() => SelectEggBed(currentEggBedIndex - 1);

        public void GoRight() => SelectEggBed(currentEggBedIndex + 1);

        public void SelectEggToRespawn(Egg egg)
        {
            viewer.Disable();
            respawnMenuGO.SetActive(false);
            mutationMenu.SetEgg(egg);
            mutationMenu.gameObject.SetActive(true);
        }

        public void Respawn(Egg origin, Egg mutated)
        {
            Movement.Teleport(selectedEggbed.Transform.position);
            BreedingManager.Instance.TrioGene = mutated.Genes;
            BreedingManager.Instance.UpdateGeneText();
            BreedingManager.Instance.AddTotalEggsAmount(-1);
            selectedEggbed.RemoveParticular(origin);
            abilityController.UpdateAbilities(mutated);
            followMovement.Target = Movement.Transform;
            AbilityController.SetUIActive(true);
            TimeManager.Instance.ResetLifespan();
            Manager.Instance.OnRespawn();
            Time.timeScale = 1;
        }
    }
}