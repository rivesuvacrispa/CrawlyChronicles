using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Elements
{
    public class FPSCounter : MonoBehaviour
    {
        [SerializeField] private float updateInterval = 0.5f;
        [SerializeField] private TMP_Text fpsText;
        [SerializeField] private Gradient colorGradient;
        
        private float accum;
        private int frames;
        private float timeleft;
        private float fps;


        private void Start() => timeleft = updateInterval;

        private void Update()
        {
            timeleft -= Time.deltaTime;
            accum += Time.timeScale / Time.deltaTime;
            ++frames;

            if (timeleft <= 0.0)
            {
                fps = accum / frames;
                timeleft = updateInterval;
                accum = 0;
                frames = 0;
            }

            if (fps.Equals(float.NaN))
            {
                fpsText.text = "Refreshing...";
                return;
            }
            
            fpsText.text = $"{fps:n2} FPS";
            fpsText.color = colorGradient.Evaluate(Mathf.Clamp01(fps / 60f));
        }
    }
}