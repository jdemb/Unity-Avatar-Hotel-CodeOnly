using UnityEngine;

public class ConditionCollection : ScriptableObject
{
    public string description; // cos o czym jest ta kolekcja warunkow, pojawi sie w inspektorze
    public Condition[] requiredConditions = new Condition[0]; // default bo jezeli cos ma tablice i nie jest ustawione automatycznie nasze edytory rozne
	// moga sprawdzac nulle i dzieki temu jedynym momentem w ktorym to bedzie null bedzie kiedy tak ustawimy, wiec nie musimy sie o to martwic
    public ReactionCollection reactionCollection; // reakcje do zrealizowania


    public bool CheckAndReact() // musimy teraz sprawdzic i jesli konieczne to zareagowac, defaultowo bedzie zwracac true 
    {
        for (int i = 0; i < requiredConditions.Length; i++) // musimy sprawdzic wszystkie nasze kondycje w for loop, 
        {
            if (!AllConditions.CheckCondition (requiredConditions[i]))
                return false; //chcemy sprawdzac tylko czy jakis warunek NIE jest spełniony
        }

        if(reactionCollection) // jezeli wszystkie warunki zostaly spelnione to trzeba wyzwolic funkcje React
            reactionCollection.React();

        return true;
    }
}