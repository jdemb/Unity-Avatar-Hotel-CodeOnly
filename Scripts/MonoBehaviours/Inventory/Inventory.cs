using UnityEngine;
using UnityEngine.UI; // trza dodac

public class Inventory : MonoBehaviour
{
    public Image[] itemImages = new Image[numItemSlots];
    public Item[] items = new Item[numItemSlots];


    public const int numItemSlots = 4; // public bo editor chce miec access do tego, const bo tworzymy na podstawie tego tablice

	//chcemy tylko dodawac i odejmowac rzeczy z inventory bo to ma byc podstawowe

    public void AddItem(Item itemToAdd) // pierwszy jest add item, przekazujemy item jako parametr, 
    {
        for (int i = 0; i < items.Length; i++) // loop through dopoki nie znajdziemy pustego miejsca 
        {
            if (items[i] == null)
            {
                items[i] = itemToAdd;
                itemImages[i].sprite = itemToAdd.sprite; // chcemy tez zeby wygladalo poprawnie
                itemImages[i].enabled = true;
                return; // bo tylko dla pierwszego ktory znalezlismy
            }
        }
    }
	//potem mamy remove item i to wyglada podobnie tylko odwrotnie

    public void RemoveItem (Item itemToRemove)
    {
        for (int i = 0; i < items.Length; i++)
        {
            if (items[i] == itemToRemove)
            {
                items[i] = null;
                itemImages[i].sprite = null;
                itemImages[i].enabled = false;
                return;
            }
        }
    }
}
