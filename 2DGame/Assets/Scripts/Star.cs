using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Star : MonoBehaviour
{

    private bool hasCollided = false; // Vari�vel de controle para verificar se houve colis�o

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !hasCollided) // Verifica se colidiu com o Player
        {
            hasCollided = true; // Flag de controle para garantir que n�o haja varias colis�es.
            gameObject.SetActive(false); // Desabilita o item
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && hasCollided)
        {
            hasCollided = false;
        }
    }
}
