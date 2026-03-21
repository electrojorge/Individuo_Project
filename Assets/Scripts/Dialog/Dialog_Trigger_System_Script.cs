using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;

public class Dialog_Trigger_System_Trigger : MonoBehaviour
{
    public GameObject panelDialogo;
    public TextMeshProUGUI textoVisual;
    public Image Retrato;

    [TextArea(3, 10)]
    public string[] lineasDialogo;
    public Sprite fotoPersonaje;

    public float velocidadEscritura = 0.03f;

    private Coroutine escribiendo;
    private int indiceLinea = 0;
    private bool jugadorDentro = false;
    private bool escribiendoTexto = false;

    public GameObject imagenFinal; //Imagen de misión que aparece al desaparecer el cuadro de texto


    private void Start()
    {
        // Si la imagen existe en la escena, la desactiva
        if (imagenFinal != null)
            imagenFinal.SetActive(false); 
    }

    private void OnTriggerEnter(Collider other)
    {
        //Si el jugador entra en el trigger, inicia el diálogo
        if (other.CompareTag("Player"))
        {
            jugadorDentro = true;
            //Mostrar el panel de diálogo al entraren el trigger
            panelDialogo.SetActive(true); 

            // Ocultar imagen final al entrar
            if (imagenFinal != null)
                imagenFinal.SetActive(false);

            indiceLinea = 0;
            SiguienteLinea();

            if (fotoPersonaje != null)
                Retrato.sprite = fotoPersonaje;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        //Si el jugador sale del exit, se cierra todo
        if (other.CompareTag("Player"))
        {
            jugadorDentro = false;
            panelDialogo.SetActive(false);

            // Ocultar imagen final al salir
            if (imagenFinal != null)
                imagenFinal.SetActive(false);

            textoVisual.text = "";
            indiceLinea = 0;
        }
    }

    //Función que llama al botón para avanzar el diálogo
    public void SiguienteLinea()
    {
        if (indiceLinea < lineasDialogo.Length)
        {
            if (escribiendo != null)
                StopCoroutine(escribiendo);

            escribiendo = StartCoroutine(EfectoMaquinaDeEscribir(lineasDialogo[indiceLinea]));
            indiceLinea++;
        }
        else
        {
            // Si ya no quedan lineas, se cierra el diálogo
            panelDialogo.SetActive(false);

            // Para mostrar la imagen de misión 
            if (imagenFinal != null)
                imagenFinal.SetActive(true);

            indiceLinea = 0;
        }
    }

    //Corrutina que escribe el texto letra por letra
    IEnumerator EfectoMaquinaDeEscribir(string texto)
    {
        escribiendoTexto = true;
        textoVisual.text = "";

        foreach (char letra in texto)
        {
            textoVisual.text += letra;
            yield return new WaitForSeconds(velocidadEscritura);
        }

        escribiendoTexto = false;
    }
}
