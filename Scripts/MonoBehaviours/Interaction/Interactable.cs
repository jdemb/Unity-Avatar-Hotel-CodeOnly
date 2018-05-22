using UnityEngine;

public class Interactable : MonoBehaviour
{
    public Transform interactionLocation;
    public ConditionCollection[] conditionCollections = new ConditionCollection[0];
    public ReactionCollection defaultReactionCollection;

	// przejdzie przez wszystkie i jezeli nie znajdzie czegos na co ma zareagowac no to wejdzie w default

    public void Interact ()
    {
        for (int i = 0; i < conditionCollections.Length; i++)
        {
            if (conditionCollections[i].CheckAndReact ())
                return;
        }

        defaultReactionCollection.React ();
    }
}

// tu gdzie teraz jestesmy, czyli dosyc daleko w tym malym projekcie
// warto zaznaczyc ze te skrypty ktore piszemy są malutkie
// bo mamy dobrze zorganizowany edytor w inspektorze
// skrypty do edytorow sa trudne i skomplikowane ale widac ze to sie oplaca

