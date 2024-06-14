using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public bool _doesAxeOfKratosHave = false;

    private bool _isPanelOpen = false;

    [SerializeField] private GameObject _marketPanel;
    [SerializeField] private Player _player;
    [SerializeField] private Button _axeOfKratos;

    
    private void Start()
    {
        _marketPanel.SetActive(false);
        _axeOfKratos.onClick.AddListener(AxeOfKratos);
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
    public void AxeOfKratos()
    {  
        if (_player._isInside == true)
        {
            Debug.Log("Axe of Kratos purchased");
            _doesAxeOfKratosHave = true;
        }
    }
}
