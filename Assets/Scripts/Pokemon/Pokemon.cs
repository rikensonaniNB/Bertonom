using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Pokemon
{
    [SerializeField] PokemonBase _base;
    [SerializeField] int _level;
    public PokemonBase Base { 
        get { return _base; }
    }

    public int Level { 
        get { return _level; }
    }

    public int HP { get; set; }

    public List<Move> Moves { get; set; }

    public void Init()
    {
        HP = MaxHp;

        Moves = new List<Move>();
        foreach (var move in Base.LearnableMoves)
        {
            if (move.Level <= Level)
                Moves.Add(new Move(move.MoveBase));

            if (Moves.Count >= 4)
                break;
        }
    }

    public int Attack
    {
        get { return Mathf.FloorToInt((Base.Attack * Level) / 100f) + 5; }
    }

    public int Defense
    {
        get { return Mathf.FloorToInt((Base.Defense * Level) / 100f) + 5; }
    }

    public int SpAttack
    {
        get { return Mathf.FloorToInt((Base.SpAttack * Level) / 100f) + 5; }
    }

    public int SpDefense
    {
        get { return Mathf.FloorToInt((Base.SpDefense * Level) / 100f) + 5; }
    }

    public int Speed
    {
        get { return Mathf.FloorToInt((Base.Speed * Level) / 100f) + 5; }
    }

    public int MaxHp
    {
        get { return Mathf.FloorToInt((Base.MaxHp * Level) / 100f) + 10; }
    }

    public DamageDetails TakeDamage(Move move, Pokemon attacker)
    {
        float critical = 1;
        if (Random.value * 100f <= 6.25f)
        {
            critical = 2;
        }

        float type = TypeChart.GetEffectivness(move.Base.Type, this.Base.Type1) * 
            TypeChart.GetEffectivness(move.Base.Type, this.Base.Type2);

        var damageDetails = new DamageDetails()
        {
            TypeEffectivness = type,
            Critical = critical,
            Fainted = false
        };

        float attack = move.Base.IsSpecial ? attacker.SpAttack : attacker.Attack;
        float defense = move.Base.IsSpecial ? SpDefense : Defense;

        float modifiers = Random.Range(0.85f, 1f) * type * critical;
        float attackCalc = (2 * attacker.Level + 10) / 250f;
        float defenseCalc = attackCalc * move.Base.Power * ((float)attack / defense) + 2;
        int damage = Mathf.FloorToInt(defenseCalc * modifiers);

        HP -= damage;
        if (HP <= 0)
        {
            HP = 0;
            damageDetails.Fainted = true;
        }

        return damageDetails;
    }

    public Move GetRandomMove()
    {
        int random = Random.Range(0, Moves.Count);
        return Moves[random];
    }
}

public class DamageDetails
{
    public bool Fainted { get; set; }
    public float Critical { get; set; }
    public float TypeEffectivness { get; set; }
}