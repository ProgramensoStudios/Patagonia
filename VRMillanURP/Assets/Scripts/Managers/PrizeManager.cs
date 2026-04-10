using System.Collections.Generic;
using UnityEngine;

public class PrizeManager : MonoBehaviour
{
    [SerializeField] private List<GameObject> prizes = new List<GameObject>();
    [SerializeField] private ParticleSystem Fireworks;
    private bool _hasChosen = false;

    private void OnEnable()
    {
        Prize.OnSelectedPrize += Choose;
    }

    private void OnDisable()
    {
        Prize.OnSelectedPrize -= Choose;
    }

    private void Choose(GameObject chosenObject)
    {
        if (_hasChosen) return;

        _hasChosen = true;

        for (var i = 0; i < prizes.Count; i++)
        {
            prizes[i].GetComponent<Prize>().enabled = false;
            if (prizes[i] != chosenObject)
            {
                prizes[i].SetActive(false);
            }
        }

        //Fireworks.Play();
        
    }
}
