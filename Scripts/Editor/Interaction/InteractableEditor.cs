using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Interactable))]
public class InteractableEditor : EditorWithSubEditors<ConditionCollectionEditor, ConditionCollection>
{
    private Interactable interactable; // nasz target
    private SerializedProperty interactionLocationProperty;
    private SerializedProperty collectionsProperty;
    private SerializedProperty defaultReactionCollectionProperty;


    private const float collectionButtonWidth = 125f;
    private const string interactablePropInteractionLocationName = "interactionLocation";
    private const string interactablePropConditionCollectionsName = "conditionCollections";
    private const string interactablePropDefaultReactionCollectionName = "defaultReactionCollection";


    private void OnEnable ()
    {
        interactable = (Interactable)target; // trzeba go zcastowaæ

        collectionsProperty = serializedObject.FindProperty(interactablePropConditionCollectionsName);
        interactionLocationProperty = serializedObject.FindProperty(interactablePropInteractionLocationName);
        defaultReactionCollectionProperty = serializedObject.FindProperty(interactablePropDefaultReactionCollectionName);
        
        CheckAndCreateSubEditors(interactable.conditionCollections); // wywowalnie naszej funkcji do podedytorow
    }


    private void OnDisable () // jesli mamy wywolanie to trzeba tez czyszczenie miec
    {
        CleanupEditors ();
    }


    protected override void SubEditorSetup(ConditionCollectionEditor editor)
    {
        editor.collectionsProperty = collectionsProperty;
    }


    public override void OnInspectorGUI ()
    {
        serializedObject.Update (); //updejt i zapis na koncu, reszta kodu leci pomiedzy
        
        CheckAndCreateSubEditors(interactable.conditionCollections); // w kazdym framie chcemy wiedziec ile editorow potrzebujemy, bo byc moze zmiana, dlatego update
        
        EditorGUILayout.PropertyField (interactionLocationProperty); //interaction location, pierwsza rzecz ktora wyswietlamy

        for (int i = 0; i < subEditors.Length; i++) // condition collection maja swoje edytory, ale musimy im powiedziec zeby sie tu wyswietlily
        {
            subEditors[i].OnInspectorGUI (); // te edytory sa czescia tak jak zwykly inspektor
			// i funkcje jak onispectorgiu trzeba wywolywac recznie
            EditorGUILayout.Space ();
		// pomiedzy chcemy miec troche miejsca 
        }

		// potrzebujemy buttona do dodawania kolekcji
        EditorGUILayout.BeginHorizontal(); //jak begin to i end
        GUILayout.FlexibleSpace ();
        if (GUILayout.Button("Add Collection", GUILayout.Width(collectionButtonWidth))) // nasz button, add colection as a string, 
			//no i jakies width, mamy wczesniej ustalony constant
        {
			// w srodku to co mamy zrobic jezeli jest klikniety
            ConditionCollection newCollection = ConditionCollectionEditor.CreateConditionCollection ();
            collectionsProperty.AddToObjectArray (newCollection);
        }
        EditorGUILayout.EndHorizontal ();

        EditorGUILayout.Space (); // troche miejsca

        EditorGUILayout.PropertyField (defaultReactionCollectionProperty); // default

        serializedObject.ApplyModifiedProperties (); // zapisanie zmian
    }
}
