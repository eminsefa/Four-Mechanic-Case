using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class UIManager : MonoBehaviour
{
    /// <summary>
    /// All UI elements are being controlled by this script.
    /// Both level start and end events are called here with buttons.
    /// Didn't have time to do good looking ui elements.
    /// </summary>
    public static UIManager Instance;
    public event Action<bool> LevelPlaying;
    private bool _inventoryOpen;
    [SerializeField] private GameObject inLevelUIParent;
    [SerializeField] private TextMeshProUGUI currentLevelText;
    [SerializeField] private TextMeshProUGUI nextLevelText;
    [SerializeField] private Image fillImage;
    [SerializeField] private GameObject startButton;
    [SerializeField] private GameObject tapToContinueButton;
    
    //--Inventory--//
    [SerializeField] private GameObject renderCam;
    [SerializeField] private GameObject inventoryButton;
    [SerializeField] private List<TextMeshProUGUI> inventoryCountTexts;
    private void Awake()
    {
        Instance = this;
        var playerLevel = LevelManager.GetPrefIndex();
        currentLevelText.text = playerLevel.ToString();
        nextLevelText.text = (playerLevel + 1).ToString();
        SetInventory();
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.A)) LevelCompleted();
    }

    private void SetInventory()
    {
        for (int i = 0; i < inventoryCountTexts.Count; i++)
        {
            inventoryCountTexts[i].text = "x"+PlayerPrefs.GetInt(i.ToString(), 0);
        }
    }
    public void LevelCompleted()
    {
        LevelPlaying?.Invoke(false);
        tapToContinueButton.SetActive(true);
        inLevelUIParent.SetActive(false);
        LevelManager.LevelCompleted();
    }
    public void StartButtonTapped()
    {
        LevelPlaying?.Invoke(true);
        startButton.SetActive(false);
        inLevelUIParent.SetActive(true);
        inventoryButton.gameObject.SetActive(false);
        renderCam.SetActive(false);
    }
    public void SetFillImage(float amount)
    {
        fillImage.fillAmount = amount;
    }
    public void RestartButtonTapped()
    {
        LevelManager.RestartLevel();
    }
    public void TapToContinueButtonTapped()
    {
        LevelManager.ActivateScene();
    }
    public void InventoryButtonTapped()
    {
        var invText = inventoryButton.transform.GetChild(0).gameObject;
        var invImage= inventoryButton.transform.GetChild(1).gameObject;
        if(!_inventoryOpen)
        {
            renderCam.SetActive(true);
            var scale = inventoryButton.transform.localScale;
            scale.x = 4;
            scale.y = 4;
            inventoryButton.transform.localScale = scale;
            _inventoryOpen = true;
            invText.SetActive(false);
            invImage.SetActive(true);
        }
        else
        {
            renderCam.SetActive(false);
            inventoryButton.transform.localScale = Vector3.one;
            _inventoryOpen = false;
            invText.SetActive(true);
            invImage.SetActive(false);
        }
    }
}
