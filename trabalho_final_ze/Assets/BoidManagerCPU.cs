using UnityEngine;
using System.Collections.Generic;

public class BoidManagerCPU : MonoBehaviour
{
    public GameObject boidPrefab;
    public int numeroDeBoids = 100;
    public Vector3 limites = new Vector3(20, 20, 20);
    public float velocidade = 5f;
    public float raioVizinhanca = 5f;
    public float alinhamentoPeso = 1f;
    public float coesaoPeso = 1f;
    public float separacaoPeso = 1.5f;

    private List<BoidCPU> boids = new List<BoidCPU>();

    void Start()
    {
        for (int i = 0; i < numeroDeBoids; i++)
        {
            GameObject b = Instantiate(
                boidPrefab,
                Random.insideUnitSphere * 10,
                Quaternion.identity,
                this.transform // Define o objeto com o script como pai
            );
            boids.Add(new BoidCPU { obj = b, direcao = Random.onUnitSphere });
        }
    }

    void Update()
    {
        foreach (var boid in boids)
        {
            Vector3 pos = boid.obj.transform.position;
            Vector3 alinhamento = Vector3.zero;
            Vector3 coesao = Vector3.zero;
            Vector3 separacao = Vector3.zero;
            int vizinhos = 0;

            foreach (var outro in boids)
            {
                if (outro == boid) continue;

                float dist = Vector3.Distance(boid.obj.transform.position, outro.obj.transform.position);
                if (dist < raioVizinhanca)
                {
                    alinhamento += outro.direcao;
                    coesao += outro.obj.transform.position;
                    separacao += (boid.obj.transform.position - outro.obj.transform.position) / (dist * dist);
                    vizinhos++;
                }
            }

            if (vizinhos > 0)
            {
                alinhamento = (alinhamento / vizinhos).normalized * alinhamentoPeso;
                coesao = ((coesao / vizinhos) - boid.obj.transform.position).normalized * coesaoPeso;
                separacao = separacao.normalized * separacaoPeso;

                boid.direcao += alinhamento + coesao + separacao;
            }

            boid.direcao = boid.direcao.normalized;

            // Rebater nos limites
            Vector3 novaPos = boid.obj.transform.position + boid.direcao * velocidade * Time.deltaTime;

            if (Mathf.Abs(novaPos.x) > limites.x) boid.direcao.x *= -1;
            if (Mathf.Abs(novaPos.y) > limites.y) boid.direcao.y *= -1;
            if (Mathf.Abs(novaPos.z) > limites.z) boid.direcao.z *= -1;

            boid.obj.transform.position += boid.direcao * velocidade * Time.deltaTime;
            boid.obj.transform.rotation = Quaternion.LookRotation(boid.direcao);
        }
    }

    public class BoidCPU
    {
        public GameObject obj;
        public Vector3 direcao;
    }
}
