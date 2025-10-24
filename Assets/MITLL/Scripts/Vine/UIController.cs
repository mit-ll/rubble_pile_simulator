// DISTRIBUTION STATEMENT A. Approved for public release. Distribution is unlimited.
//  
// This material is based upon work supported by the Department of the Air Force under Air Force Contract No. FA8702-15-D-0001. Any opinions, findings, conclusions or recommendations expressed in this material are those of the author(s) and do not necessarily reflect the views of the Department of the Air Force.
//  
// © 2024 Massachusetts Institute of Technology.
// Subject to FAR52.227-11 Patent Rights - Ownership by the contractor (May 2014)
//  
// The software/firmware is provided to you on an As-Is basis
//  
// Delivered to the U.S. Government with Unlimited Rights, as defined in DFARS Part 252.227-7013 or 7014 (Feb 2014). Notwithstanding any copyright notice, U.S. Government rights in this work are defined by DFARS 252.227-7013 or DFARS 252.227-7014 as detailed above. Use of this work other than as specifically authorized by the U.S. Government may violate any copyrights that exist in this work.

using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = System.Random;

public class UIController : MonoBehaviour
{
    public GameObject loadingUI;
    public TextMeshPro loadingText;
    public Slider LoadingBar;

    public GameObject instructionUI;
    // Start is called before the first frame update
    void Start()
    {
        LoadingBar.value = 0f;
    }
    
    private void OnEnable()
        {
            GameManager.doReset += Reset;
            GameManager.doInit += Initialize;
        }
    
        private void OnDisable()
        {
            GameManager.doReset -= Reset;
            GameManager.doInit -= Initialize;
        }

        private void Reset()
        {
            loadingUI.SetActive(true);
            LoadingBar.value = 0f;
            StartCoroutine(AnimateBar());
        }

        private void Initialize()
        {
            LoadingBar.value = 1;
            loadingUI.SetActive(false);
        }

        private void TickBar()
        {
            if (LoadingBar.value >= 0.85f)
            {
                StopCoroutine(AnimateBar());
                return;
            }

            LoadingBar.value += RandomManager.Instance.GetDynamicFloat(0.05f, 0.2f);

        }

        IEnumerator AnimateBar()
        {
            int randIterations = RandomManager.Instance.GetDynamicInt(10, 13);
            for (int i = 0; i < randIterations; i++)
            {
                TickBar();
                yield return new WaitForSeconds(RandomManager.Instance.GetDynamicFloat(1f, 3f));
            }
        }

        public void ToggleInstructions()
        {
            instructionUI.SetActive(!instructionUI.activeSelf);
        }
}
