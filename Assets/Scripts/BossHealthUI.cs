using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BossHealthUI : MonoBehaviour
{
    [SerializeField] private Slider _healthSlider;

    private WaveSpawner _waveSpawner;
    private Boss _boss;

    private bool _isFilled;

    private void Update()
    {
        if (!_waveSpawner && !_isFilled)
        {
            _waveSpawner = FindObjectOfType<WaveSpawner>();
            if (_waveSpawner)
            {
                _isFilled = true;
            }
            return;
        }

        if (_waveSpawner)
        {
            return;
        }

        if (!_boss)
        {
            _boss = FindObjectOfType<Boss>();

            if (!_boss)
            {
                return;
            }

            _healthSlider.gameObject.SetActive(true);
            _healthSlider.maxValue = _boss.HalfHealth * 2;
        }

        _healthSlider.value = _boss.Health;
    }
}
