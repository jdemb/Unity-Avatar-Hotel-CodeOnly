using UnityEngine;

// model behaviour wiec attached to gameobjects
// we have a reference from our condition collection to our reaction collection
// to jest wszystko czego potrzebujemy
// 

public class ReactionCollection : MonoBehaviour
{
    public Reaction[] reactions = new Reaction[0];
	// nie bedziemy przechowywac tablic zlozonych z animacji, dzwieku czy tekstu
	// ale prosta tablice reakcji i osiagamy to dzieki polimorfizmowi
	// zakladam ze nie musze tlumaczyc jak dziala dziedziczenie i polimorfizm


    private void Start () // start function gonna loop through all of the reactions
		// i ona wyzwali ich initialization function
    {
        for (int i = 0; i < reactions.Length; i++)
        {
            DelayedReaction delayedReaction = reactions[i] as DelayedReaction;

            if (delayedReaction)
                delayedReaction.Init ();
            else
                reactions[i].Init ();
        }
    }


    public void React () // again looping through all of reactions and calling their react
    {
        for (int i = 0; i < reactions.Length; i++)
        {
            DelayedReaction delayedReaction = reactions[i] as DelayedReaction;

            if(delayedReaction)
                delayedReaction.React (this);
            else
                reactions[i].React (this);
        }
    }
}
// dlaczego to wazne zeby przechowywac wszystkie reakcje w tym samym miejscu
// przede wszystkim bardzo przydatne dla inspektora
// tu slajd