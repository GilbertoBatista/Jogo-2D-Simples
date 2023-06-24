using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Player : MonoBehaviour
{
    public float moveSpeed = 5f; // Velocidade de movimento do personagem
    public float jumpForce = 5f; // Força do salto do personagem
    public float cameraFollowSpeed = 5f; // Velocidade de acompanhamento da câmera
    public Transform cameraFollowTarget; // Alvo de acompanhamento da câmera
    public TextMeshProUGUI starCountText; // Referência ao TextMesh Pro para exibir a quantidade de estrelas coletadas
    public TextMeshProUGUI finshMensage; // Referência ao TextMesh Pro para exibir o texto em caso de morte do player
    public SpriteRenderer healthSpriteRenderer; // Referência ao SpriteRenderer para exibir o sprite de vida
    public Sprite[] healthSprites; // Array de sprites de vida
    public GameObject panelFinish; // Panel de fim de jogo
    public SpriteRenderer backgroundSprite; // Background para limitar a camera
    public SpriteRenderer spriterenderFlip; // Sprite o qual deseja executar o efeito de Flip
    private Rigidbody2D rb; 
    private Animator animator; 
    private bool isJumping = false; // Flag para verificar quando esta pulando 
    private bool isFacingRight = true; // Flag para verificar o lado atual do Sprite 
    private int starCount = 0; // Quantidade de estrelas coletadas
    private int health = 3; // Vida inicial do personagem
    private bool hasCollided = false; // Flag para verificar se houve colisão
    private bool finishOn = false; // Flag para verificar se a partida terminou


    void Start()
    {
        rb = GetComponent<Rigidbody2D>(); // Obtém o componente Rigidbody2D do personagem
        animator = GetComponent<Animator>(); // Obtém o componente Animator do personagem
    }

    void Update()
    {
        if (!finishOn)
        {
            // Obtem o Input Horizontal do Player (Seta Esquerda, Seta Direita, A, D)
            var moveHorizontal = Input.GetAxis("Horizontal");

            transform.position += new Vector3(moveHorizontal, 0, 0) * Time.deltaTime * moveSpeed; // Aplica a velocidade de movimento
            animator.SetFloat("Speed", Mathf.Abs(moveHorizontal)); // Define o parâmetro "Speed" do Animator para controlar a animação de andar

            if (moveHorizontal < 0f && !isFacingRight)
            {
                Flip(); // Inverte a direção do personagem se estiver se movendo para a esquerda e estiver virado para a direita
            }
            else if (moveHorizontal > 0f && isFacingRight)
            {
                Flip(); // Inverte a direção do personagem se estiver se movendo para a direita e estiver virado para a esquerda
            }

            if (Input.GetButtonDown("Jump") && !isJumping) // Verifica se o botão de pulo (espaço) foi pressionado e o personagem está no chão e não está pulando
            {
                Jump();
            }

            AdjustCameraPosition(); // Ajusta a posição da câmera para acompanhar o personagem
        }
    }

    void Jump()
    {
        rb.AddForce(new Vector2(0, jumpForce), ForceMode2D.Impulse); // Aplica uma força vertical para simular o pulo
        isJumping = true;
        animator.SetTrigger("Jump"); // Ativa a animação de pulo no Animator
    }

    void Flip()
    {
        isFacingRight = !isFacingRight;
        spriterenderFlip.flipX = isFacingRight; // Inverte a posição X do Sprite
    }

    void AdjustCameraPosition()
    {
        // Ajusta a cameta para ficar no limite do Backgroud selecionado e acompanha o Player
        Vector3 targetPosition = cameraFollowTarget.position;
        targetPosition.z = -3f; // Define a posição Z da câmera como -3

        float cameraHalfWidth = Camera.main.orthographicSize * Screen.width / Screen.height;
        float cameraHalfHeight = Camera.main.orthographicSize;

        float minX = backgroundSprite.bounds.min.x + cameraHalfWidth;
        float maxX = backgroundSprite.bounds.max.x - cameraHalfWidth;
        float minY = backgroundSprite.bounds.min.y + cameraHalfHeight;
        float maxY = backgroundSprite.bounds.max.y - cameraHalfHeight;

        targetPosition.x = Mathf.Clamp(targetPosition.x, minX, maxX);
        targetPosition.y = Mathf.Clamp(targetPosition.y, minY, maxY);

        Vector3 newPosition = Vector3.Lerp(Camera.main.transform.position, targetPosition, Time.deltaTime * cameraFollowSpeed);
        newPosition.x = Mathf.Clamp(newPosition.x, minX, maxX);
        newPosition.y = Mathf.Clamp(newPosition.y, minY, maxY);

        Camera.main.transform.position = newPosition;
    }




    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy") && !hasCollided) // Verifica se colidiu com um inimigo
        {
            // Identifica o lado da colisão
            Vector2 playerPosition = transform.position;
            Vector2 obstaclePosition = collision.transform.position; 

            if (playerPosition.x < obstaclePosition.x && playerPosition.y < (obstaclePosition.y + 0.63f) || playerPosition.y == (obstaclePosition.y + 0.63f))
            {
                // Colisão na direita
                hasCollided = true; // Flag de controle para garantir que não haja varias colisões.
                TakeDamage(1); // Recebe 1 de dano
            }
            else if (playerPosition.x > obstaclePosition.x && playerPosition.y < (obstaclePosition.y + 0.63f) || playerPosition.y == (obstaclePosition.y + 0.63f))
            {
                //colisão na esquerda
                hasCollided = true;
                TakeDamage(1); // Recebe 1 de dano
            }
        }
        else if (collision.CompareTag("Star") && !hasCollided) // Verifica se colidiu com uma estrela
        {
            hasCollided = true;
            CollectStar(); // Coleta a estrela
        }
        else if (collision.CompareTag("Life") && !hasCollided) // Verifica se colidiu com um item de vida
        {
            hasCollided = true;
            CollectHealthItem(); // Coleta o item de vida
        }
        else if (collision.CompareTag("Trap") && !hasCollided) // Verifica se colidiu com um item de Trap
        {
            hasCollided = true;
            isJumping = false;
            TakeDamage(1); // Recebe 1 de dano
        }
        else if (collision.CompareTag("Checkpoint") && !hasCollided) // Verifica se colidiu com um item de Checkpoint
        {
            hasCollided = true;
            finishOn = true; // Desebalita movimento
        }

    }
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground") && isJumping)
        {
            Vector2 collisionNormal = collision.contacts[0].normal;

            float dotUp = Vector2.Dot(collisionNormal, Vector2.up);
            float dotDown = Vector2.Dot(collisionNormal, Vector2.down);
            float dotRight = Vector2.Dot(collisionNormal, Vector2.right);
            float dotLeft = Vector2.Dot(collisionNormal, Vector2.left);

            if (dotUp > dotDown && dotUp > dotRight && dotUp > dotLeft)
            {
                // Colisão ocorreu pelo lado superior do Ground
                isJumping = false;
            }
        }
    }


    void OnTriggerExit2D(Collider2D collision)
    {
        // Libera a flag de colisão
        if (collision.CompareTag("Enemy") && hasCollided) 
        {
            hasCollided = false;
        }
        else if (collision.CompareTag("Star") && hasCollided) 
        {
            hasCollided = false;
        }
        else if (collision.CompareTag("Life") && hasCollided) 
        {
            hasCollided = false;
        }
        else if (collision.CompareTag("Checkpoint") && hasCollided)
        {
            hasCollided = false;
        }
        else if (collision.CompareTag("Trap") && hasCollided)
        {
            hasCollided = false;
        }
    }

        void TakeDamage(int damageAmount)
    {
        health -= damageAmount; // Reduz a vida pelo valor do dano recebido
        animator.SetTrigger("hurt"); // Ativa a animação de Ferimento
        if (health <= 0)
        {
            Die(); // Chama a função para lidar com a morte do personagem
        }
        else
        {
            UpdateHealthSprite(); // Atualiza o sprite de vida
        }
    }

    void CollectStar()
    {
        starCount++; // Incrementa a quantidade de estrelas coletadas
        UpdateStarCountText(); // Atualiza o texto para exibir a nova quantidade de estrelas
    }

    void CollectHealthItem()
    {
        if (health < 3) // Verifica se a vida do personagem não está no máximo
        {
            health++; // Aumenta a vida do personagem
            UpdateHealthSprite(); // Atualiza o sprite de vida
        }
    }

    void Die()
    {
        panelFinish.SetActive(true); // Ativa o Panel de Menu Final para reiniciar a partida
        finshMensage.text = "Vecê Morreu !!! \n Tente mais uma vez."; // Texto apresentado no Panel 
        finishOn = true; // Desabilita o movimento
    }

   

    void UpdateStarCountText()
    {
        starCountText.text = starCount.ToString(); // Atualiza o texto para exibir a quantidade de estrelas coletadas
    }

    void UpdateHealthSprite()
    {
        healthSpriteRenderer.sprite = healthSprites[health]; // Atualiza o sprite de vida com base na quantidade de vida atual
    }

    public void Restart()
    {
        // Reinicia, atualiza os valores e retorna o Player a posição inicial
        health = 3;
        UpdateHealthSprite();
        starCount = 0;
        UpdateStarCountText();
        transform.position = new Vector3(3.66f, 15.34f, 0);
        finishOn = false;
        hasCollided = false;
    }
}
