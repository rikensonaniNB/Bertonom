using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConditionsDB
{
    /// <summary>
    /// Setter for the <c>Id</c> parameter of the class <c>Condition</c>
    /// </summary>
    public static void Init() 
    {
        foreach (var kvp in Conditions)
        {
            var conditionId = kvp.Key;
            var condition = kvp.Value;

            condition.Id = conditionId;
        }
    }

    /// <summary>
    /// A dictionary with all the possible conditions you can have in a Pokemon game with their effects. The key is an enum <c>ConditionID</c>.
    /// </summary>
    public static Dictionary<ConditionID, Condition> Conditions { get; set; } =
        new Dictionary<ConditionID, Condition>()
        {
            {
                ConditionID.psn, 
                new Condition()
                {
                    Name = "Poison",
                    StartMessage = "has been poisoned",
                    OnAfterTurn = pokemon =>
                    {
                        pokemon.DecreaseHP(pokemon.MaxHp / 8);
                        pokemon.StatusChanges.Enqueue($"{pokemon.Base.Name} hurt itself due to poison");
                    }
                    
                }
            },
            {
                ConditionID.brn, 
                new Condition()
                {
                    Name = "Burn",
                    StartMessage = "has been burned",
                    OnAfterTurn = pokemon =>
                    {
                        pokemon.DecreaseHP(pokemon.MaxHp / 16);
                        pokemon.StatusChanges.Enqueue($"{pokemon.Base.Name} hurt itself due to burn");
                    }
                    
                }
            },
            {
                ConditionID.par, 
                new Condition()
                {
                    Name = "Paralyze",
                    StartMessage = "has been paralyzed",
                    OnBeforeMove = pokemon =>
                    {
                        if (Random.Range(1,5) == 1)
                        {
                            pokemon.StatusChanges.Enqueue($"{pokemon.Base.Name}'s paralyzed and can't move");
                            return false;
                        }
                        return true;
                    }
                    
                }
            },
            {
                ConditionID.frz, 
                new Condition()
                {
                    Name = "Freeze",
                    StartMessage = "has been frozen",
                    OnBeforeMove = pokemon =>
                    {
                        if (Random.Range(1,5) == 1)
                        {
                            pokemon.CureStatus();
                            pokemon.StatusChanges.Enqueue($"{pokemon.Base.Name} is not frozen anymore");
                            return true;
                        }
                        return false;
                    }
                    
                }
            },
            {
                ConditionID.slp, 
                new Condition()
                {
                    Name = "Sleep",
                    StartMessage = "has fallen asleep",
                    OnStart = pokemon =>
                    {
                        // sleep for 1-3 rounds
                        pokemon.StatusTime = Random.Range(1, 4);
                        Debug.Log($"Will be asleep for {pokemon.StatusTime} moves");
                    },
                    OnBeforeMove = pokemon =>
                    {
                        if (pokemon.StatusTime <= 0)
                        {
                            pokemon.CureStatus();
                            pokemon.StatusChanges.Enqueue($"{pokemon.Base.Name} woke up!");
                            return true;
                        }
                        pokemon.StatusTime--;
                        pokemon.StatusChanges.Enqueue($"{pokemon.Base.Name} is sleeping");
                        return false;
                    }
                }
            },
            {
                ConditionID.confusion,
                new Condition()
                {
                    Name = "Confusion",
                    StartMessage = "has been confused",
                    OnStart = pokemon =>
                    {
                        // confused for 1-4 rounds
                        pokemon.VolatileStatusTime = Random.Range(1, 5);
                        Debug.Log($"Will be confused for {pokemon.VolatileStatusTime} moves");
                    },
                    OnBeforeMove = pokemon =>
                    {
                        if (pokemon.VolatileStatusTime <= 0)
                        {
                            pokemon.CureVolatileStatus();
                            pokemon.StatusChanges.Enqueue($"{pokemon.Base.Name} kicked out of confusion!");
                            return true;
                        }
                        pokemon.VolatileStatusTime--;
                        pokemon.StatusChanges.Enqueue($"{pokemon.Base.Name} is confused");

                        // 50% chance to do a move
                        if (Random.Range(1, 3) == 1)
                        {
                            return true;
                        }

                        // hurt by confusion
                        pokemon.DecreaseHP(pokemon.MaxHp / 8);
                        pokemon.StatusChanges.Enqueue("It hurt itself due to confusion");
                        return false;
                    }

                }
            }
        };

    public static float GetStatusBonus(Condition condition)
    {
        if (condition == null)
        {
            return 1f;
        }
        else if (condition.Id == ConditionID.slp || condition.Id == ConditionID.frz)
        {
            return 2f;
        }
        else if (condition.Id == ConditionID.par || condition.Id == ConditionID.psn || condition.Id == ConditionID.brn)
        {
            return 1.5f;
        }
        else
        {
            return 1f;
        }
    }
}

public enum ConditionID
{
    none, psn, brn, slp, par, frz, confusion
}