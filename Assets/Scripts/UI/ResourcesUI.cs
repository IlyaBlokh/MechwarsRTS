using Resources;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace UI
{
    public class ResourcesUI : MonoBehaviour
    {
        [SerializeField] private TMP_Text creditsAmountText;

        private void Start()
        {
            PlayerResources.OnClientCreditsUpdated += DisplayCreditsAmount;
        }

        private void OnDestroy()
        {
            PlayerResources.OnClientCreditsUpdated -= DisplayCreditsAmount;
        }

        private void DisplayCreditsAmount(int newAmount)
        {
            creditsAmountText.text = newAmount.ToString();
        }
    }
}