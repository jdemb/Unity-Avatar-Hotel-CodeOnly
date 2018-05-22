using UnityEngine;
using UnityEditor; // editor namespace

//condition collection is very simple but in order for us to have such a simpel script we need to have a bit of backing 
//musimy byc pewni ze kazdy ten scriptableobject ktory umieszczamy w tych tablicach warunkow sa poprawne
// i zeby to zrobic musimy miec troche bardziej skomplikowany skrypt edytora 
// musimy widziec te kolekcje, nie mozemy tego zostawic jako zwykle pola na obiekty, musimy widziec co jest w srodku 
// chcemy miec tez dropdown zeby sobie wybierac jakie chcemy miec warunki
// i te edytory moga miec jeszcze wlasne edytory, wiec chcemy miec klase ktora cala te funkcjonalnosc robi za nas
// wiec mamy interactables i one maja condition collection i te maja conditions
// czyli taka edytorocepcja


// mega skomplikowany, najbardziej skomplikowany w calym projekcie
public abstract class EditorWithSubEditors<TEditor, TTarget> : Editor // abstract, abstract nie moze byc stworzony przez sama siebie
//generics - w co celujemy i czym te podedytory maja byc			  // wiec nie ma sensu robic edytor z pod edytorami, musimy im nadać kontekst, po co są, 
																	  // ta definicja tego nie zawiera tylko jest zwykłą klase
																	  // wiec nie chcemy tworzyc jej instancji tylko chcemy zeby inne klasy dziedziczyly z niej
	where TEditor : Editor
    where TTarget : Object
{
    protected TEditor[] subEditors; 


    protected void CheckAndCreateSubEditors (TTarget[] subEditorTargets)
    {
        if (subEditors != null && subEditors.Length == subEditorTargets.Length)
            return;

        CleanupEditors ();

        subEditors = new TEditor[subEditorTargets.Length];

        for (int i = 0; i < subEditors.Length; i++)
        {
            subEditors[i] = CreateEditor (subEditorTargets[i]) as TEditor;
            SubEditorSetup (subEditors[i]);
        }
    }


    protected void CleanupEditors ()
    {
        if (subEditors == null)
            return;

        for (int i = 0; i < subEditors.Length; i++)
        {
            DestroyImmediate (subEditors[i]);
        }

        subEditors = null;
    }


    protected abstract void SubEditorSetup (TEditor editor);
}
