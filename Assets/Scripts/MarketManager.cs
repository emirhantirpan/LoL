using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;
using UnityEngine.UI;

public class MarketManager : MonoBehaviour
{
    [Header("Item Bools")]
    public bool doesLevithanAxeHave = false;
    public bool doesAssassinBladeHave = false;
    public bool itemSelled = false;


    private bool _isPanelOpen = false;

    [SerializeField] private GameObject _marketPanel;
    [SerializeField] private Player _player;
    [SerializeField] private Button _sellButton;

    [Header("Items")]
    [SerializeField] private Button _levithanAxe;
    [SerializeField] private Button _assassinBlade;

    private void Start()
    {
        _marketPanel.SetActive(false);
        _levithanAxe.onClick.AddListener(LevithanAxe);
        _assassinBlade.onClick.AddListener(AssassinBlade);
        _sellButton.onClick.AddListener(Sell);
    }
    public void MarketPanel()
    {
        _isPanelOpen = !_isPanelOpen;
        if (_marketPanel.activeInHierarchy)
        {
            _marketPanel.SetActive(false);
        }
        else
        {
            _marketPanel.SetActive(true);
        }
    }
    public void LevithanAxe()
    {  
        if (_player._isInside == true)
        {
            Debug.Log("Levithan Axe purchased");
            doesLevithanAxeHave = true;
            _levithanAxe.interactable = false;
        }
    }
    public void AssassinBlade()
    {
        if (_player._isInside == true)
        {
            Debug.Log("Assassin Blade purchased");
            doesAssassinBladeHave = true;
            _assassinBlade.interactable = false;
        }
    }
    public void Sell()//Böyle olmaz çok yanlış revize edilmeli
    {
        if (doesLevithanAxeHave == true)
        {
            Debug.Log("Levithan Axe sold");
            doesLevithanAxeHave = false;
            _levithanAxe.interactable = true;
            itemSelled = true;
            _player.valueChangedAxe = true;
        }
    }
}

