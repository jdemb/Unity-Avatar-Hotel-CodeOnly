using UnityEngine;

// reprezentuje global state, czyli czy gracz ma monete czy nie, czy ma kawe, czy podniosl juz monete


public class AllConditions : ResettableScriptableObject //dziedziczenie
{
    public Condition[] conditions; // warunki ktore sa w stanie gry omawiane wczesniej


    private static AllConditions instance; // kiedy mamy dostep do typu AllConditions, ktory bedziemy mieli ciagle, mozemy uzyc AllConditions kropka instance to
	// find a type, to jest prywatne ale mamy public static instance


    private const string loadPath = "AllConditions";
    

    public static AllConditions Instance // property to find AllConditions type
    {
        get
        {
            if (!instance) // jezeli nie mamy tej instancji to sprobuj ja znalezc w pamieci
                instance = FindObjectOfType<AllConditions> ();
            if (!instance) // jezeli ciagle nie mamy to sprobuj ja zaladowac z resources folder
                instance = Resources.Load<AllConditions> (loadPath);
            if (!instance) // jezeli dalej nie istnieje to wysylamy error
                Debug.LogError ("AllConditions has not been created yet.  Go to Assets > Create > AllConditions.");
            return instance;
        }
        set { instance = value; }
    }


    public override void Reset () // musimy byc pewni ze stan globalny sie resetuje przy kazdym uruchomieniu gry
    {
        if (conditions == null)
            return;

        for (int i = 0; i < conditions.Length; i++)
        {
            conditions[i].satisfied = false;
        }
	}// jezeli nie jest nic ustawione no to nie trzeba nic robic, jezeli jest to loop through i set satisfied to false


	public static bool CheckCondition (Condition requiredCondition) //sprawdzanie danego warunku
    {
        Condition[] allConditions = Instance.conditions; //³adujemy wszystkie warunki
        Condition globalCondition = null; //potem tworzymy warunek globalny czyli ten z ktorym chcemy porownywac
        
        if (allConditions != null && allConditions[0] != null) // jezeli jest conajmniej jeden warunek to loop through
        {
            for (int i = 0; i < allConditions.Length; i++)
            {
                if (allConditions[i].hash == requiredCondition.hash) // jezeli warunek ma taki sam hash jak wymagany warunek ktorego szukamy, 
					// jezeli tak to znaczy ze to ten warunek chcemy sprawdzic
                    globalCondition = allConditions[i];
            }
        }

        if (!globalCondition)
            return false; // jezeli nic nie znalazlo, nie ma warunku do ktorego mozemy sie odniesc wiec taki warunek nie moze byc spelniony

		//porownujemy i jezeli sa takie same to true jezeli nie to false
        return globalCondition.satisfied == requiredCondition.satisfied;
    }
}
