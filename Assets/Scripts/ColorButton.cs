using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class ColorButton : MonoBehaviour
{
    public int colorID;
    private ToucherManager touchManager;

    void Start()
    {
        touchManager = FindObjectOfType<ToucherManager>();
        GetComponent<Button>().onClick.AddListener(() => touchManager.SeleccionarColor(colorID));

        // Asignar color del botón según el colorID
        GetComponent<Image>().color = touchManager.colores[colorID];

 

    }
}
