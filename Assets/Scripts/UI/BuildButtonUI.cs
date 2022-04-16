using Buildings;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class BuildButtonUI : MonoBehaviour
    {
        [SerializeField] private Button buildButton;
        [SerializeField] private TMP_Text buildCost;
        [SerializeField] private Building building;

        private GameObject buildingPreviewInstance;

        private void Start()
        {
            buildButton.image.sprite = building.Icon;
            buildCost.text = building.Price.ToString();
            buildButton.onClick.AddListener(OnBuildClick);
        }

        private void OnDestroy()
        {
            buildButton.onClick.RemoveAllListeners();
        }

        private void OnBuildClick()
        {
            Debug.Log("build!");
        }
    }
}