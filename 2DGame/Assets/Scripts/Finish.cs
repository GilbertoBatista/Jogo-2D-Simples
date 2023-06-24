using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Finish : MonoBehaviour
{

    public TextMeshProUGUI finishMensage;
    public GameObject panelFinish;
    public GameObject[] enemy;
    public GameObject[] star;
    public GameObject[] carrot;
    public Player player;
    private bool hasCollided = false; // Vari�vel de controle para verificar se houve colis�o

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !hasCollided) // Verifica se colidiu com um item de vida
        {
            hasCollided = true; // Flag de controle para garantir que n�o haja varias colis�es.

            if (panelFinish != null)
            {
                panelFinish.SetActive(true); // Habilita o Panel
            }
            // Mensagem de vit�ria exibida no Panel
            finishMensage.text = "Parab�ns voc� venceu !!! \n Reinicie para jogar mais uma vez!";
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && hasCollided)
        {
            hasCollided = false;
        }
    }

    public void RestartButton()
    {
        // Reinicia os valores do Player e ativa todos inimigos e itens para uma nova partida
        player.Restart();
        
        foreach (GameObject gameObject in enemy)
        {
            gameObject.SetActive(true);
        }

        foreach (GameObject gameObject in star)
        {
            gameObject.SetActive(true);
        }

        foreach (GameObject gameObject in carrot)
        {
            gameObject.SetActive(true);
        }

        hasCollided = false;
        panelFinish.SetActive(false);
    }
}
