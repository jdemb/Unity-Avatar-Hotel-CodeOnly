using UnityEngine;

public class TextReaction : Reaction // inherit/rozszerzenie
{
    public string message; // wiadomosc do wyswietlenia
    public Color textColor = Color.white; // kolor wiadomosci
    public float delay; // opoznienie (podjac temat opoznionych klas, ale ta obsluguje tekst wiec)


    private TextManager textManager;


    protected override void SpecificInit() // specific initializition, ktory jest ovveridem 
    {
        textManager = FindObjectOfType<TextManager> ();
    }


    protected override void ImmediateReaction()
    {
        textManager.DisplayMessage (message, textColor, delay);
    }
}
// i to wszystko i jezeli teraz ktos przyjdzie z design teamu i powie ze chce miec wiadomosc na dole ekranu no to pyk i mamy to
// to wszystko co wtedy trzeba napisac, jezeli chcemy wymyslec kolejna reakcje to kolejny skrypt bedzie podobnej wielkosci
