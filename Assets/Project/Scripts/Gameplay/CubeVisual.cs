using TMPro;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

namespace GamePlay
{
    public class CubeVisual : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI[] valueTexts;
        [SerializeField] private Renderer cubeRenderer;

        [System.Serializable]
        private struct ValueColor
        {
            public int value;
            public Color color;
        }

        [SerializeField] private ValueColor[] valueColors;

        private Cube cube;

        private void Awake()
        {
            this.cube = GetComponent<Cube>();


            if (this.valueTexts == null || this.valueTexts.Length == 0)
            {
                this.valueTexts = this.GetComponentsInChildren<TextMeshProUGUI>();
            }
        }

        private void Start()
        {
            UpdateVisual();
        }

        public void UpdateVisual()
        {
            if (valueTexts != null)
            {
                foreach (var text in valueTexts)
                {
                    text.text = this.cube.Value.ToString();

                }
            }

            foreach (var vc in this.valueColors)
            {
                if (vc.value == this.cube.Value)
                {
                    this.cubeRenderer.material.color = vc.color;
                    break;
                }
            }
        }
    }
}
