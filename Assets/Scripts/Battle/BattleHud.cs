using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleHud : MonoBehaviour
{
    [SerializeField] Text nameText;
    [SerializeField] Text levelText;
    [SerializeField] HPbar hpBar;

    Pokemon pokemon1;
    public void SetData(Pokemon pokemon)
    {
        pokemon1 = pokemon;

        nameText.text = pokemon.Base.PkmName;
        levelText.text = "Lvl " + pokemon.Level;
        hpBar.SetHP((float)pokemon.HP / pokemon.MaxHp);       
    }

    public IEnumerator UpdateHP()
    {
        yield return hpBar.SetHPSmooth((float)pokemon1.HP / pokemon1.MaxHp);
    }
}