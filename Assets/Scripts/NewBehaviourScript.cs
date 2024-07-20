using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NewBehaviourScript : MonoBehaviour
{
    public ParticleSystem money;
    public ParticleSystem seasonal;

    public void OnSeasonalDollarClicked()
    {
        seasonal.Play();
    }
    public void OnMenuDollarClicked()
    {
        money.Play();
    }
}

