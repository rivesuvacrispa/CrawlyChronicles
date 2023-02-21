using System.Collections;
using GameCycle;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class PauseMenu : MonoBehaviour
    {
        [SerializeField] private Text statsText;
        
        private void OnEnable()
        {
            Time.timeScale = 0;
            statsText.text = StatRecorder.Print();
            StartCoroutine(KeyListenerRoutine());
        }

        private void OnDisable()
        {
            Time.timeScale = 1;
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