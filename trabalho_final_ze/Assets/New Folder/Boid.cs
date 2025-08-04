using UnityEngine;
using System.Collections.Generic;

public class Boid : MonoBehaviour
{
    [HideInInspector]
    public BoidManager manager;
    private Vector3 velocidade;
    private Vector3 aceleracao;

    void Start()
    {
        // Define uma velocidade inicial aleatória
        float angulo = Random.Range(0, 2 * Mathf.PI);
        velocidade = new Vector3(Mathf.Cos(angulo), Mathf.Sin(angulo), Random.Range(-1f, 1f)).normalized * manager.velocidadeMin;
    }

    void Update()
    {
        AplicarRegras();
        AplicarLimites();

        // Atualiza a posição e rotação
        velocidade += aceleracao * Time.deltaTime;

        // Limita a velocidade
        float speed = velocidade.magnitude;
        if (speed > manager.velocidadeMax)
        {
            velocidade = velocidade.normalized * manager.velocidadeMax;
        }
        else if (speed < manager.velocidadeMin)
        {
            velocidade = velocidade.normalized * manager.velocidadeMin;
        }

        transform.position += velocidade * Time.deltaTime;
        transform.rotation = Quaternion.LookRotation(velocidade);

        // Reseta a aceleração para o próximo frame
        aceleracao = Vector3.zero;
    }

    void AplicarRegras()
    {
        Vector3 centroDoGrupo = Vector3.zero;
        Vector3 direcaoDoGrupo = Vector3.zero;
        Vector3 afastamento = Vector3.zero;
        int vizinhos = 0;

        foreach (Boid outroBoid in manager.boids)
        {
            if (outroBoid != this)
            {
                float distancia = Vector3.Distance(this.transform.position, outroBoid.transform.position);

                if (distancia > 0 && distancia < manager.raioDeVisao)
                {
                    // Coesão: Encontra o centro do grupo
                    centroDoGrupo += outroBoid.transform.position;

                    // Alinhamento: Encontra a direção média do grupo
                    direcaoDoGrupo += outroBoid.velocidade;

                    // Separação: Calcula o vetor de afastamento
                    Vector3 diff = this.transform.position - outroBoid.transform.position;
                    afastamento += diff.normalized / distancia;

                    vizinhos++;
                }
            }
        }

        if (vizinhos > 0)
        {
            // Coesão
            centroDoGrupo /= vizinhos;
            Vector3 direcaoCoesao = (centroDoGrupo - this.transform.position).normalized * manager.fatorCoesao;
            aceleracao += direcaoCoesao;

            // Alinhamento
            direcaoDoGrupo /= vizinhos;
            Vector3 direcaoAlinhamento = direcaoDoGrupo.normalized * manager.fatorAlinhamento;
            aceleracao += direcaoAlinhamento;

            // Separação
            Vector3 direcaoSeparacao = afastamento.normalized * manager.fatorSeparacao;
            aceleracao += direcaoSeparacao;
        }
    }
    void AplicarLimites()
    {
        Vector3 centro = manager.transform.position;
        Vector3 metadeLimites = manager.limitesDoEspaco / 2;

        Vector3 pos = transform.position;

        // Rebater no limite X
        if (pos.x > centro.x + metadeLimites.x || pos.x < centro.x - metadeLimites.x)
        {
            velocidade.x = -velocidade.x;
            pos.x = Mathf.Clamp(pos.x, centro.x - metadeLimites.x, centro.x + metadeLimites.x);
        }

        // Rebater no limite Y
        if (pos.y > centro.y + metadeLimites.y || pos.y < centro.y - metadeLimites.y)
        {
            velocidade.y = -velocidade.y;
            pos.y = Mathf.Clamp(pos.y, centro.y - metadeLimites.y, centro.y + metadeLimites.y);
        }

        // Rebater no limite Z
        if (pos.z > centro.z + metadeLimites.z || pos.z < centro.z - metadeLimites.z)
        {
            velocidade.z = -velocidade.z;
            pos.z = Mathf.Clamp(pos.z, centro.z - metadeLimites.z, centro.z + metadeLimites.z);
        }

        transform.position = pos;
    }
}