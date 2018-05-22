using UnityEngine;
using UnityEditor; // odpowiedni namespace do zmiany klas unity

[CustomEditor(typeof(Inventory))] // bo to customtype, bez tego nic sie nie stargetuje i nie bedzie dzialac
public class InventoryEditor : Editor // musimy dziedziczyc z edytora
{
    private bool[] showItemSlots = new bool[Inventory.numItemSlots]; // zeby wiedziec czy box ma byc zamkniety, mozemy uzyc public constant co stworzylismy wczesniej
    private SerializedProperty itemImagesProperty; //represents arrey images array
    private SerializedProperty itemsProperty; // represents items array


    private const string inventoryPropItemImagesName = "itemImages"; // zeby serialiazed property wiedzialo czego szukac musimy podac nazwe 
    private const string inventoryPropItemsName = "items";


    private void OnEnable () // to jak wiemy czego szukamy to trzeba to znalezc
    {
        itemImagesProperty = serializedObject.FindProperty (inventoryPropItemImagesName);
        itemsProperty = serializedObject.FindProperty (inventoryPropItemsName);
    }
	//musimy sobie stworzyc boxa ktory bedzie przechowywal dwa componenty i zdecydowac czy je pokazuje wiec bool array

    public override void OnInspectorGUI () // musimy zmienic jak sie zachowuje cos co juz istnieje
    {
        serializedObject.Update (); // musimy byc pewni ze informacje ktore mamy sa aktualne

        for (int i = 0; i < Inventory.numItemSlots; i++) // wywolujemy dla kazdego slotu
        {
            ItemSlotGUI (i);
        }

        serializedObject.ApplyModifiedProperties (); // bo zmieniamy tylko reprezentacje obiektu, wiec musimy zapisac zmiany
    }


    private void ItemSlotGUI (int index) // index czyli ktory itemslot
    {
        EditorGUILayout.BeginVertical (GUI.skin.box); // zeby sformowac boxa do wyswietlania, box jako parametr jest stylem w jaki bedziemy wyswietlac dane
        EditorGUI.indentLevel++; // indent level increase very slightly
        
        showItemSlots[index] = EditorGUILayout.Foldout (showItemSlots[index], "Item slot " + index); // nag³ówki, bool value tutaj wskazuje czy foldout jest otwarty czy zamkniety, 
		// sprawdza tez czy user na to kliknal w tym framie i zwraca te wartosc

        if (showItemSlots[index])
        {
            EditorGUILayout.PropertyField (itemImagesProperty.GetArrayElementAtIndex (index)); // bierzemy tablice itemimagespropety i getatindex nam zwroci poprawny element z wielu
            EditorGUILayout.PropertyField (itemsProperty.GetArrayElementAtIndex (index));
        }

        EditorGUI.indentLevel--; // powrot do poprzedniego indent lvl 
        EditorGUILayout.EndVertical (); // musimy gdzies zakonczyc nasze wyswietlanie 
    }
}
