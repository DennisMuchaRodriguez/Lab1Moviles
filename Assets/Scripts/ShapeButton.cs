using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShapeButton : MonoBehaviour
{
    public int figuraID;
    private ToucherManager touchManager;

    void Start()
    {
        touchManager = FindObjectOfType<ToucherManager>();
        GetComponent<Button>().onClick.AddListener(() => touchManager.SeleccionarFigura(figuraID));

        // Asignar sprite del botón según la figura seleccionada
        GetComponent<Image>().sprite = touchManager.figuras[figuraID].GetComponent<Image>().sprite;

     



     
    }
}
