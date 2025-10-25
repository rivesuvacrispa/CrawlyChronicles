using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Camera;
using Gameplay.Breeding;
using Gameplay.Player;
using SoundEffects;
using Timeline;
using TMPro;
using UI.Menus;
using UnityEngine;

namespace GameCycle
{
    public class RespawnManager : MonoBehaviour
    {
        private static RespawnManager instance;
        
        public delegate void EggBedCollectionEvent(List<EggBed> eggBeds);
        public static EggBedCollectionEvent OnEggCollectionRequested;

        [SerializeField] private GameObject respawnMenuGO;
        [SerializeField] private TMP_Text eggBedSelectionText;
        [SerializeField] private GameObject leftArrow;
        [SerializeField] private GameObject rightArrow;
        [SerializeField] private GameObject navigatorGO;

        private int currentEggBedIndex;
        private List<EggBed> eggBeds = new();
        private EggBed selectedEggbed;

        public delegate void RespawnManagerEvent(EggBed eggBed);
        public static RespawnManagerEvent OnEggbedSelected;
        

        
        private RespawnManager() => instance = this;
        
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
                
                MainCamera.FollowMovement.UpdateUnscaled();
                yield return new WaitForSecondsRealtime(0);
            }
        }

        private void SelectEggBed(int index)
        {
            int count = eggBeds.Count;
            int step = count == 1 ? 0 : Mathf.Clamp(index, 0, count - 1);
            UIAudioController.Instance.PlaySelect();
            currentEggBedIndex = step;
            eggBedSelectionText.text = $"{currentEggBedIndex + 1}/{count}";
            leftArrow.SetActive(currentEggBedIndex > 0);
            rightArrow.SetActive(currentEggBedIndex < count - 1);
            selectedEggbed = eggBeds[currentEggBedIndex];
            OnEggbedSelected?.Invoke(selectedEggbed);
            MainCamera.FollowMovement.Target = selectedEggbed.Transform;
        }

        public void GoLeft() => SelectEggBed(currentEggBedIndex - 1);

        public void GoRight() => SelectEggBed(currentEggBedIndex + 1);

        public void SelectEggToRespawn(Egg egg)
        {
            StopAllCoroutines();
            OnEggbedSelected?.Invoke(null);
            respawnMenuGO.SetActive(false);
            MutationMenu.Show(MutationTarget.Egg, egg);
        }

        public void Respawn(Egg origin, Egg mutated)
        {
            PlayerMovement.Teleport(selectedEggbed.Transform.position);
            BreedingManager.Instance.SetTrioGene(mutated.Genes);
            BreedingManager.Instance.SetCurrentFoodAmount(0);
            selectedEggbed.RemoveParticular(origin);
            AbilityController.UpdateAbilities(mutated);
            MainCamera.FollowMovement.Target = PlayerManager.Instance.Transform;
            AbilityController.SetUIActive(true);
            TimeManager.Instance.ResetLifespan();
            PlayerManager.Instance.OnRespawn();
            Time.timeScale = 1;
        }
    }
}