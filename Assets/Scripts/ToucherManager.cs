using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public class ToucherManager : MonoBehaviour
{
    public GameObject[] figuras;
    public Color[] colores;
    public RectTransform parentCanvas;

    public GameObject trailImagePrefab;
    private float trailCooldown = 0.05f;
    private float lastTrailTime = 0f;

    private int colorID = 0;
    private int figuraID = 0;
    public RectTransform zonaBloqueada;
    private float lastTapTime = 0;
    private GameObject objetoSeleccionado = null;
    private bool isDragging = false;
    private Vector2 lastTouchPosition;

    private List<GameObject> objetosInstanciados = new List<GameObject>();
    private bool justDeleted = false;
    void Update()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            switch (touch.phase)
            {
                case TouchPhase.Began:
                    if (Time.time - lastTapTime < 0.3f)
                    {
                        EliminarFigura(touch.position);
                        justDeleted = true; 
                    }
                    else
                    {
                        lastTapTime = Time.time;
                        SeleccionarObjeto(touch.position);
                        justDeleted = false; 
                    }
                    break;

           

                case TouchPhase.Moved:
                    if (isDragging && objetoSeleccionado != null)
                    {
                        MoverFigura(touch.position);
                    }
                    break;

                case TouchPhase.Ended:
                    if (!isDragging && !justDeleted) 
                    {
                        CrearFigura(touch.position);
                    }
                    isDragging = false;
                    objetoSeleccionado = null;
                    justDeleted = false; 
                    break;
            }

            if (Input.touchCount == 1 && touch.phase == TouchPhase.Moved)
            {
                Vector2 delta = touch.position - lastTouchPosition;
                if (delta.magnitude > 200)
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
        if (figuras.Length == 0 || colores.Length == 0 || parentCanvas == null)
    {
        return;
    }

 
    if (zonaBloqueada != null && RectTransformUtility.RectangleContainsScreenPoint(zonaBloqueada, screenPosition))
    {
        return; 
    }

    Vector2 canvasPos;
    RectTransformUtility.ScreenPointToLocalPointInRectangle(parentCanvas, screenPosition, null, out canvasPos);

    GameObject nuevaFigura = Instantiate(figuras[figuraID], parentCanvas);
    RectTransform rectTransform = nuevaFigura.GetComponent<RectTransform>();
    rectTransform.anchoredPosition = canvasPos;

    Image img = nuevaFigura.GetComponent<Image>();
    if (img != null)
    {
        img.color = colores[colorID];
    }

    objetosInstanciados.Add(nuevaFigura);;
    }

    private void EliminarFigura(Vector2 screenPosition)
    {
        for (int i = 0; i < objetosInstanciados.Count; i++)
        {
            RectTransform rectTransform = objetosInstanciados[i].GetComponent<RectTransform>();
            if (RectTransformUtility.RectangleContainsScreenPoint(rectTransform, screenPosition))
            {
                Destroy(objetosInstanciados[i]);
                objetosInstanciados.RemoveAt(i);
                return;
            }
        }
    }

    private void SeleccionarObjeto(Vector2 screenPosition)
    {
        for (int i = 0; i < objetosInstanciados.Count; i++)
        {
            RectTransform rectTransform = objetosInstanciados[i].GetComponent<RectTransform>();
            if (RectTransformUtility.RectangleContainsScreenPoint(rectTransform, screenPosition))
            {
                objetoSeleccionado = objetosInstanciados[i];
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

        
        if (Time.time - lastTrailTime >= trailCooldown)
        {
            CrearTrailImagen(canvasPos);
            lastTrailTime = Time.time;
        }
    }
    private void CrearTrailImagen(Vector2 position)
    {
        GameObject trailImage = Instantiate(trailImagePrefab, parentCanvas);
        RectTransform rectTransform = trailImage.GetComponent<RectTransform>();
        rectTransform.anchoredPosition = position;

        Image img = trailImage.GetComponent<Image>();
        if (img != null)
        {
            img.color = colores[colorID];
        }

        Destroy(trailImage, 0.5f); 
    }
    private void RealizarSwipe()
    {
       
        for (int i = 0; i < objetosInstanciados.Count; i++)
        {
            Destroy(objetosInstanciados[i]);
        }
        objetosInstanciados.Clear();

        
        Vector2 canvasPos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(parentCanvas, lastTouchPosition, null, out canvasPos);

       
        for (int i = 0; i < 10; i++)
        {
            Vector2 offset = new Vector2(Random.Range(-10, 10), Random.Range(-10, 10));
            CrearTrailImagen(canvasPos + offset);
        }
    }
}