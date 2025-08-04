using UnityEngine;
using System.Collections.Generic;

public class BoidManager : MonoBehaviour
{
    [Header("Configurações Gerais")]
    public GameObject boidPrefab;
    public int numeroDeBoids = 50;
    public Vector3 limitesDoEspaco = new Vector3(20, 20, 20); // Tamanho da caixa limite

    [Header("Configurações dos Boids")]
    [Range(0f, 1000f)]
    public float velocidadeMin = 2f;
    [Range(0f, 1000f)]
    public float velocidadeMax = 5f;
    [Range(1f, 1000f)]
    public float raioDeVisao = 5f;
    [Range(0f, 500f)]
    public float fatorCoesao = 1f;
    [Range(0f, 500f)]
    public float fatorAlinhamento = 1f;
    [Range(0f, 5000f)]
    public float fatorSeparacao = 3f;
    [Range(1f, 1000f)]
    public float fatorLimite = 2f; // Força para evitar os limites

    // Lista de todos os boids
    [HideInInspector]
    public List<Boid> boids;

    void Start()
    {
        boids = new List<Boid>();
        for (int i = 0; i < numeroDeBoids; i++)
        {
            Vector3 pos = this.transform.position + new Vector3(
                Random.Range(-limitesDoEspaco.x / 2, limitesDoEspaco.x / 2),
                Random.Range(-limitesDoEspaco.y / 2, limitesDoEspaco.y / 2),
                Random.Range(-limitesDoEspaco.z / 2, limitesDoEspaco.z / 2)
            );
            GameObject boidGO = Instantiate(boidPrefab, pos, Quaternion.identity);
            Boid boid = boidGO.GetComponent<Boid>();
            boid.manager = this;
            boids.Add(boid);
        }
    }

    // Desenha os limites na cena para visualização
    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(this.transform.position, limitesDoEspaco);
    }
}