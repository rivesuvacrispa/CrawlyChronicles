using System.Collections;
using GameCycle;
using Gameplay.Player;
using UI.Elements;
using UI.Tooltips;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Menus
{
    public class PauseMenu : MonoBehaviour
    {
        [SerializeField] private GameObject uiGO;
        [SerializeField] private Text statsText;
        [SerializeField] private BasicAbilityButton buttonPrefab;
        [SerializeField] private Transform mutationsTransform;
        [SerializeField] private AbilityTooltip tooltip;
        
        private void OnEnable()
        {
            uiGO.SetActive(false);
            Time.timeScale = 0;
            statsText.text = StatRecorder.Print();
            StartCoroutine(KeyListenerRoutine());
            CreateMutationsList();
        }

        private void OnDisable()
        {
            uiGO.SetActive(true);
            ClearMutationsList();
            Time.timeScale = 1;
        }

        private void CreateMutationsList()
        {
            var mutations = AbilityController.GetMutationData().GetAll();
            foreach (var (basicMutation, lvl) in mutations)
            {
                var btn = Instantiate(buttonPrefab, mutationsTransform);
                btn.SetVisuals(basicMutation);
                btn.UpdateLevelText(lvl);
                btn.GetComponent<AbilityTooltipProvider>().SetTooltip(tooltip);
            }
        }

        private void ClearMutationsList()
        {
            foreach (Transform t in mutationsTransform) 
                Destroy(t.gameObject);
        }

        private IEnumerator KeyListenerRoutine()
        {
            while (gameObject.activeInHierarchy)
            {
                yield return new WaitForSecondsRealtime(0f);
                if(Input.GetKeyDown(KeyCode.Escape)) gameObject.SetActive(false);
            }
        }
    }
}