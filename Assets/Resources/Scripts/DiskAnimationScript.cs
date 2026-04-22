using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class DiskAnimationScript : MonoBehaviour
{
    [Header("Configuració del Moviment")]
    [Range(0f, 5f)] public float velocitatLevitacio = 2f; // Velocitat de pujar i baixar
    [Range(0f, 2f)] public float amplitudLevitacio = 0.5f; // Quanta distància es mou

    [Header("Configuració de la Rotació (Efecte Mirall)")]
    [Range(0f, 10f)] public float velocitatRotacio = 5f; // Velocitat del gir visual

    private Vector3 posicioInicial;
    private Vector3 escalaOriginal;

    void Start()
    {
        // Guardem la posició i escala per tenir-les com a referència
        posicioInicial = transform.position;
        escalaOriginal = transform.localScale;
    }

    void Update()
    {
        // 1. MOVIMENT AMUNT I AVALL (Levitació)
        // Calculem la nova Y fent servir el Sinus del temps
        float novaY = posicioInicial.y + Mathf.Sin(Time.time * velocitatLevitacio) * amplitudLevitacio;
        transform.position = new Vector3(posicioInicial.x, novaY, posicioInicial.z);

        // 2. EFECTE DE ROTACIÓ (Canvi d'escala X)
        // Fem que l'escala X oscil·li entre l'original i la seva inversa
        float factorEscalaX = Mathf.Sin(Time.time * velocitatRotacio);
        transform.localScale = new Vector3(escalaOriginal.x * factorEscalaX, escalaOriginal.y, escalaOriginal.z);
    }
    
    // Aquesta funció s'executa quan el RoomManager torna a activar l'objecte
    private void OnEnable()
    {
        // Resetegem la posició inicial per si l'objecte s'ha mogut per error
        posicioInicial = transform.position;
    }
}