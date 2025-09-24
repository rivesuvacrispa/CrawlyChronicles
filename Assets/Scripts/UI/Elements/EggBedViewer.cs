using System.Collections.Generic;
using Gameplay.Breeding;
using UnityEngine;

namespace UI.Elements
{
    public class EggBedViewer : MonoBehaviour
    {
        [SerializeField] private float radius;
        
        private int maxCount;
        private List<EggBedViewerElement> elements;

        private void Awake()
        {
            elements = new List<EggBedViewerElement>();
            foreach (Transform t in transform) 
                elements.Add(t.GetComponent<EggBedViewerElement>());
            maxCount = elements.Count;
        }
        
        public void ShowEggs(EggBed eggBed)
        {
            transform.localPosition = eggBed.transform.position;
            int eggsAmount = eggBed.EggsAmount;
            float sliceSize = Mathf.PI * 2 / eggsAmount;
            for (int i = 0; i < maxCount; i++)
            {
                EggBedViewerElement display = elements[i];
                if (i < eggsAmount)
                {
                    float slice = sliceSize * i;
                    Vector2 pos = new Vector2(Mathf.Sin(slice), Mathf.Cos(slice)) * radius;
                    display.transform.localPosition = pos;
                    Egg egg = eggBed.GetEgg(i);
                    display.SetEgg(egg);
                    display.gameObject.SetActive(true);
                }
                else
                {
                    display.gameObject.SetActive(false);
                }
            }
        }

        public void Disable()
        {
            foreach (EggBedViewerElement element in elements) element.gameObject.SetActive(false);
        }
    }
}