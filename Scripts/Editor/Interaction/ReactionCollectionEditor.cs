using System;
using UnityEngine;
using System.Collections.Generic;
using UnityEditor;

// wieksza czesc tego kodu jest podobna do tego co omawialismy wczesniej
// nie trzeba tego przerabiac ponownie
// ale sa pewne rzeczy na ktore byloby fajnie spojrzec
// kiedy mamy kolekcje reakcji i ona jest przyczepiona do jakiegos obiektu jako game component
// chcemy miec mozliwosc dodania nowych typów reakcji do wyboru
// uzyjemy do tego dwoch metod, dropdown list (wytlumacz jak to wyglada)
// druga to bedziemy udawac inspektor, kiedy chcemy dodac komponent mozemy przerzucic, przytrzymac i upuscic 
// skrypt w inspektorze zeby dodalo (wytlumacz)

[CustomEditor(typeof(ReactionCollection))]
public class ReactionCollectionEditor : EditorWithSubEditors<ReactionEditor, Reaction>
	// ten edytor potrzebuje pod edytorów którymi bed¹ edytory reakcji
{
    private ReactionCollection reactionCollection;
    private SerializedProperty reactionsProperty; // dla reakcji mamy serialized property i tego bedziemy uzywac zeby dodawac reakcje

    private Type[] reactionTypes;
    private string[] reactionTypeNames;
    private int selectedIndex;


    private const float dropAreaHeight = 50f;
    private const float controlSpacing = 5f;
    private const string reactionsPropName = "reactions";


    private readonly float verticalSpacing = EditorGUIUtility.standardVerticalSpacing;


    private void OnEnable () // fidning target, cash it, find serialized property, i create subeditors, 
    {
        reactionCollection = (ReactionCollection)target;

        reactionsProperty = serializedObject.FindProperty(reactionsPropName);

        CheckAndCreateSubEditors (reactionCollection.reactions);

        SetReactionNamesArray ();
    }


    private void OnDisable () // on disable czyscimy edytory
    {
        CleanupEditors ();
    }


    protected override void SubEditorSetup (ReactionEditor editor)
    {
        editor.reactionsProperty = reactionsProperty;
    }


    public override void OnInspectorGUI ()
    {
        serializedObject.Update ();

        CheckAndCreateSubEditors(reactionCollection.reactions);

        for (int i = 0; i < subEditors.Length; i++)
        {
            subEditors[i].OnInspectorGUI ();
        }

        if (reactionCollection.reactions.Length > 0)
        {
            EditorGUILayout.Space();
            EditorGUILayout.Space ();
        }

		// oninspectorgui mamy do czynienia z kilkoma Rect, Rects to obszar na ekranie, ktory jest reprezentowany przez 4 liczby x,y, wysokosc szerokos
		// wiec tworzymy pewne obszary w inspektorze ktore mozemy wykorzystywac

        Rect fullWidthRect = GUILayoutUtility.GetRect(GUIContent.none, GUIStyle.none, GUILayout.Height(dropAreaHeight + verticalSpacing));
		
		// jeden rect jest dla drop down i button selection
		// i jest inny rect dla obszaru gdzie drag and drop

        Rect leftAreaRect = fullWidthRect;
        leftAreaRect.y += verticalSpacing * 0.5f;
        leftAreaRect.width *= 0.5f;
        leftAreaRect.width -= controlSpacing * 0.5f;
        leftAreaRect.height = dropAreaHeight;

        Rect rightAreaRect = leftAreaRect;
        rightAreaRect.x += rightAreaRect.width + controlSpacing;

        TypeSelectionGUI (leftAreaRect);
        DragAndDropAreaGUI (rightAreaRect);

        DraggingAndDropping(rightAreaRect, this);

        serializedObject.ApplyModifiedProperties ();
    }


    private void TypeSelectionGUI (Rect containingRect) // drop down i button
    {
        Rect topHalf = containingRect;
        topHalf.height *= 0.5f;
        
        Rect bottomHalf = topHalf;
        bottomHalf.y += bottomHalf.height;

        selectedIndex = EditorGUI.Popup(topHalf, selectedIndex, reactionTypeNames);

        if (GUI.Button (bottomHalf, "Add Selected Reaction"))
        {
            Type reactionType = reactionTypes[selectedIndex];
            Reaction newReaction = ReactionEditor.CreateReaction (reactionType);
            reactionsProperty.AddToObjectArray (newReaction);
        }
    }


    private static void DragAndDropAreaGUI (Rect containingRect) // obszar, to tylko rysuje nam boxa na ekranie
    {
        GUIStyle centredStyle = GUI.skin.box;
        centredStyle.alignment = TextAnchor.MiddleCenter; // z jakims tekstem w srodku 
        centredStyle.normal.textColor = GUI.skin.button.normal.textColor;

        GUI.Box (containingRect, "Drop new Reactions here", centredStyle);
    }

	// tutaj mamy
	// ca³¹ te funkcjê mo¿na sobie wyci¹æ i u¿ywaæ osobno w innych projektach
	// bardzo przydatne do u¿ywania drag&drop w unity
    private static void DraggingAndDropping (Rect dropArea, ReactionCollectionEditor editor)
    {
        Event currentEvent = Event.current; // cashing current event in unity
		// event class records whats going on in unity 
		// current event is what the mouse and keyborad are currently doing
		// 

        if (!dropArea.Contains (currentEvent.mousePosition)) // obszar, który jest mierzony na ekranie, 
			// jezeli to nie zawiera kursora myszki to nie chcemy w tej funkcji nic robic i wychodzimy
            return;

        switch (currentEvent.type) //
        {
            case EventType.DragUpdated: // to sie odpala kiedy juz wczesniej na cos kliknalem w projekcie w trakcie dzialania
				// i aktualnie to trzymam i przesuwam, to bedzie pierwsza rzecz ktora sie stanie kiedy myszka wejdzie w nasz obszar 
				// i teraz musimy zdecydowac jezeli myszka sie znajduje nad obszarem czy to co trzymamy jest mozliwe do upuszczenia
				// w tym miejscu 

                DragAndDrop.visualMode = IsDragValid () ? DragAndDropVisualMode.Link : DragAndDropVisualMode.Rejected;
                currentEvent.Use ();
				// set the visual mode wheter or not the drag is valid 

                break;
            case EventType.DragPerform: // kiedy przycisk myszki zostaje zwolniony i chcemy upuscic to co trzymalismy
                
                DragAndDrop.AcceptDrag(); // musimy zaakceptowac ten event 
				// to sie moze stac tylko wtedy kiedy wykonal sie updateddrag i nasz visual mode is linked
				// (jeszcze raz)
				// jezeli wiemy ze mozemy to zaakceptowac to teraz chcemy przejsc przez cala petle 
				// obiektow ktore sa przeci¹gane i dla kazdego chcemy to
				// rzucic jako monoscript, monoscript to typ z assetow ktory jest utrzymywany dla zwyklych skryptow
				// bo kiedy tworzymy c# skrypt to tworzymy monobehaviour czy cos takiego, ale one wszystkie naleza do assetow
				// a assety do ktorych te skrypty naleza to wlasnie monoscript
                
                for (int i = 0; i < DragAndDrop.objectReferences.Length; i++)
                {
                    MonoScript script = DragAndDrop.objectReferences[i] as MonoScript;

                    Type reactionType = script.GetClass(); // potem znajdujemy typ, ten typ bedzie tym cokolwiek ten skrypt przechowuje

                    Reaction newReaction = ReactionEditor.CreateReaction (reactionType); // jezeli mamy juz przypisany skrypt
					// to teraz chcemy stworzyc reakcje tego typu
                    editor.reactionsProperty.AddToObjectArray (newReaction);
					// dodajemy do wyswietlania to co wlasnie stworzylismy
                }

                currentEvent.Use(); // no i to

                break;
        }
    }


    private static bool IsDragValid ()
    {
		// teraz sprawdzamy kiedy taki drag jest odpowiedni 
		// poniewaz drag moze miec w sobie kilka obiektow
		// wiec musimy do wszystkich zastosowac te same wymagania
		// to co chcemy akceptowac to skrypty dziedziczace z reakcji
		// wiec petla pyk i sprawdzamy czy wszystkie sa reakcjami
        for (int i = 0; i < DragAndDrop.objectReferences.Length; i++)
        {
            if (DragAndDrop.objectReferences[i].GetType () != typeof (MonoScript))
                return false; // nie ma znaczenia co jest w innych obiektach jezeli znajdziemy cos co nie jest obiektem monoscript
			// to nie chcemy nic robic
            

			// jezeli wiemy ze to monoscript to trzeba sprawdzic czy to reaction
            MonoScript script = DragAndDrop.objectReferences[i] as MonoScript;
			// taki sposob pozwala nam na rzut bez generowania b³êdów
            Type scriptType = script.GetClass ();

            if (!scriptType.IsSubclassOf (typeof(Reaction)))
                return false;

            if (scriptType.IsAbstract)
                return false;
        }

        return true;
    }


    private void SetReactionNamesArray ()
    {
        Type reactionType = typeof(Reaction);

        Type[] allTypes = reactionType.Assembly.GetTypes();

        List<Type> reactionSubTypeList = new List<Type>();

        for (int i = 0; i < allTypes.Length; i++)
        {
            if (allTypes[i].IsSubclassOf(reactionType) && !allTypes[i].IsAbstract)
            {
                reactionSubTypeList.Add(allTypes[i]);
            }
        }

        reactionTypes = reactionSubTypeList.ToArray();

        List<string> reactionTypeNameList = new List<string>();

        for (int i = 0; i < reactionTypes.Length; i++)
        {
            reactionTypeNameList.Add(reactionTypes[i].Name);
        }

        reactionTypeNames = reactionTypeNameList.ToArray();
    }
}
