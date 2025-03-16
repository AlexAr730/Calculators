using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ButtonsBehavior : MonoBehaviour
{
    public TMP_Text canvasText;


    public void nueve() {
        canvasText.text += "9";
    }
    public void ocho() {
        canvasText.text += "8";
    }
    public void siete() {
        canvasText.text += "7";
    }
    public void seis() {
        canvasText.text += "6";
    }
    public void cinco() {
        canvasText.text += "5";
    }
    public void cuatro() {
        canvasText.text += "4";
    }
    public void tres() {
        canvasText.text += "3";
    }
    public void dos() {
        canvasText.text += "2";
    }
    public void uno() {
        canvasText.text += "1";
    }
    public void cero() {
        canvasText.text += "0";
    }
    public void mas() {
        canvasText.text += "+";
    }
}
