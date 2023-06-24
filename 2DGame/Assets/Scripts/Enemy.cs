using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float moveSpeed = 2f; // Velocidade de movimento dos inimigos
    public float moveDistance = 5f; // Dist�ncia total que os inimigos percorrem
    public Animator animatorEnemy; // Refer�ncia ao componente Animator dos inimigos
    public SpriteRenderer spriterenderFlip; // Sprite o qual deseja executar o efeito de Flip
    private Vector3 startPosition; // Posi��o inicial dos inimigos
    private float moveDirection = 1f; // Dire��o de movimento dos inimigos (-1 para esquerda, 1 para direita)
    private bool isDead = false; // Flag para verificar se os inimigos est�o mortos
    private bool hasCollided = false; // Flag para verificar se houve colis�o
    private bool isFacingRight = true; // Flag para verificar o lado atual do Sprite

    private void Start()
    {
        startPosition = transform.position; // Armazena a posi��o inicial dos inimigos
    }

    private void Update()
    {
        if (!isDead)
        {
            // Move os inimigos na dire��o especificada
            transform.position += new Vector3(moveDirection, 0, 0) * Time.deltaTime * moveSpeed;

            // Verifica se os inimigos atingiram a dist�ncia m�xima e inverte a dire��o de movimento
            if (Mathf.Abs(transform.position.x - startPosition.x) >= moveDistance / 2f && (transform.position.x - startPosition.x) > 0 && isFacingRight)
            {
                Flip();
            }
            if (Mathf.Abs(transform.position.x - startPosition.x) >= moveDistance / 2f && (transform.position.x - startPosition.x) < 0 && !isFacingRight)
            {
                Flip();
            }
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !hasCollided) // Verifica se colidiu com um inimigo
        {
            Vector2 playerPosition = collision.transform.position; 
            Vector2 obstaclePosition = transform.position;

            // Verifica se a colis�o ocerreu na parte superior (o player pulou em cima do inimigo)
            if (playerPosition.y > (obstaclePosition.y + 0.63f) && playerPosition.y != (obstaclePosition.y + 0.63f))
            {
                hasCollided = true; // Flag de controle para garantir que n�o haja varias colis�es.
                StartCoroutine(Die());
            }
        }

    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && hasCollided)
        {
            hasCollided = false; // teg de controle para garantir que n�o haja varias colis�es.
        }
    }

    void Flip()
    {
        // Inverte a dire��o de movimento e espelha o sprite horizontalmente
        moveDirection *= -1;
        isFacingRight = !isFacingRight;
        spriterenderFlip.flipX = isFacingRight;
    }

     IEnumerator Die()
    {
        // Ativa a anima��o de morte e define os inimigos como mortos
        isDead = true; // o inimigo atingido para de se mover
        animatorEnemy.SetTrigger("Die"); // chama a anima��o para morte do inimigo
        yield return new WaitForSeconds(0.5f); // espera 0.5 segundos para ocorrer a anima��o antes de desativar
        gameObject.SetActive(false);
        isDead = false;
    }
}
