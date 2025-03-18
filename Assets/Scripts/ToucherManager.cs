using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public class ToucherManager : MonoBehaviour
{
    public GameObject[] figuras; // Prefabs de figuras UI
    public Color[] colores; // Lista de colores
    public RectTransform parentCanvas; // Canvas donde se instanciarán las figuras
    public GameObject trailPrefab; // Prefab del Trail Renderer para Swipe

    private int colorID = 0; // ID del color seleccionado
    private int figuraID = 0; // ID de la figura seleccionada

    private float lastTapTime = 0;
    private GameObject objetoSeleccionado = null;
    private bool isDragging = false;
    private Vector2 lastTouchPosition;

    private List<GameObject> objetosInstanciados = new List<GameObject>(); // Lista de figuras creadas

    void Update()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            switch (touch.phase)
            {
                case TouchPhase.Began:
                    if (Time.time - lastTapTime < 0.3f) // Doble Tap
                    {
                        EliminarFigura(touch.position);
                    }
                    else
                    {
                        lastTapTime = Time.time;
                        SeleccionarObjeto(touch.position);
                    }
                    break;

                case TouchPhase.Moved:
                    if (isDragging && objetoSeleccionado != null)
                    {
                        MoverFigura(touch.position);
                    }
                    break;

                case TouchPhase.Ended:
                    if (!isDragging)
                    {
                        CrearFigura(touch.position);
                    }
                    isDragging = false;
                    objetoSeleccionado = null;
                    break;
            }

            // Detectar Swipe
            if (Input.touchCount == 1 && touch.phase == TouchPhase.Moved)
            {
                Vector2 delta = touch.position - lastTouchPosition;
                if (delta.magnitude > 200) // Longitud mínima del swipe
                {
                    RealizarSwipe();
                }
            }
            lastTouchPosition = touch.position;
        }
    }

    public void SeleccionarColor(int id)
    {
        colorID = id;
    }

    public void SeleccionarFigura(int id)
    {
        figuraID = id;
    }

    private void CrearFigura(Vector2 screenPosition)
    {
        if (figuras.Length == 0 || colores.Length == 0 || parentCanvas == null) return;

        Vector2 canvasPos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            parentCanvas, screenPosition, null, out canvasPos);

        GameObject nuevaFigura = Instantiate(figuras[figuraID], parentCanvas);
        RectTransform rectTransform = nuevaFigura.GetComponent<RectTransform>();
        rectTransform.anchoredPosition = canvasPos;

        Image img = nuevaFigura.GetComponent<Image>();
        if (img != null)
        {
            img.color = colores[colorID];
        }

        objetosInstanciados.Add(nuevaFigura);
    }

    private void EliminarFigura(Vector2 screenPosition)
    {
        foreach (GameObject obj in objetosInstanciados)
        {
            RectTransform rectTransform = obj.GetComponent<RectTransform>();
            if (RectTransformUtility.RectangleContainsScreenPoint(rectTransform, screenPosition))
            {
                objetosInstanciados.Remove(obj);
                Destroy(obj);
                return;
            }
        }
    }

    private void SeleccionarObjeto(Vector2 screenPosition)
    {
        foreach (GameObject obj in objetosInstanciados)
        {
            RectTransform rectTransform = obj.GetComponent<RectTransform>();
            if (RectTransformUtility.RectangleContainsScreenPoint(rectTransform, screenPosition))
            {
                objetoSeleccionado = obj;
                isDragging = true;
                return;
            }
        }
    }

    private void MoverFigura(Vector2 screenPosition)
    {
        if (objetoSeleccionado == null) return;

        Vector2 canvasPos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            parentCanvas, screenPosition, null, out canvasPos);

        objetoSeleccionado.GetComponent<RectTransform>().anchoredPosition = canvasPos;
    }

    private void RealizarSwipe()
    {
        foreach (GameObject obj in objetosInstanciados)
        {
            Destroy(obj);
        }
        objetosInstanciados.Clear();

        // Instanciar un Trail Renderer con el color actual
        if (trailPrefab != null)
        {
            GameObject trail = Instantiate(trailPrefab);
            trail.GetComponent<TrailRenderer>().startColor = colores[colorID];
            trail.GetComponent<TrailRenderer>().endColor = new Color(colores[colorID].r, colores[colorID].g, colores[colorID].b, 0);
        }
    }
}