using UnityEngine;

public class BoidController : MonoBehaviour
{
    [Header("Boid Settings")]
    public int boidCount = 100;
    public float viewRadius = 5f;
    public float avoidRadius = 1f;
    public float alignmentWeight = 1f;
    public float cohesionWeight = 1f;
    public float separationWeight = 1f;
    public float speed = 5f;
    public Vector3 bounds = new Vector3(20f, 20f, 20f); // Limites do espaço

    [Header("Boid Prefab")]
    public GameObject boidPrefab;

    [Header("Compute Shader")]
    public ComputeShader boidCompute;

    private ComputeBuffer boidBuffer;
    private Boid[] boids;

    struct Boid
    {
        public Vector3 position;
        public Vector3 velocity;
    }

    void Start()
    {
        boids = new Boid[boidCount];

        for (int i = 0; i < boidCount; i++)
        {
            Vector3 pos = new Vector3(
                Random.Range(-bounds.x, bounds.x),
                Random.Range(-bounds.y, bounds.y),
                Random.Range(-bounds.z, bounds.z)
            );

            Vector3 vel = Random.onUnitSphere;
            boids[i] = new Boid { position = pos, velocity = vel };

            // Instanciar prefab (opcional — só para visual)
            if (boidPrefab != null)
            {
                Instantiate(boidPrefab, pos, Quaternion.identity, transform);
            }
        }

        boidBuffer = new ComputeBuffer(boidCount, sizeof(float) * 6);
        boidBuffer.SetData(boids);
    }

    void Update()
    {
        if (boidCompute == null) return;

        int kernel = boidCompute.FindKernel("CSMain");

        // Enviar parâmetros para o compute shader
        boidCompute.SetBuffer(kernel, "boidsBuffer", boidBuffer);
        boidCompute.SetFloat("deltaTime", Time.deltaTime);
        boidCompute.SetFloat("viewRadius", viewRadius);
        boidCompute.SetFloat("avoidRadius", avoidRadius);
        boidCompute.SetFloat("alignmentWeight", alignmentWeight);
        boidCompute.SetFloat("cohesionWeight", cohesionWeight);
        boidCompute.SetFloat("separationWeight", separationWeight);
        boidCompute.SetFloat("speed", speed);
        boidCompute.SetInt("boidCount", boidCount);
        boidCompute.SetVector("bounds", bounds); // Envia os limites para o shader

        int threadGroups = Mathf.CeilToInt(boidCount / 256f);
        boidCompute.Dispatch(kernel, threadGroups, 1, 1);

        // Ler de volta os dados para atualizar a posição dos boids na cena
        boidBuffer.GetData(boids);

        // Atualizar objetos visuais (se houver)
        for (int i = 0; i < transform.childCount && i < boids.Length; i++)
        {
            Transform boidObj = transform.GetChild(i);
            boidObj.position = boids[i].position;
            if (boids[i].velocity != Vector3.zero)
                boidObj.rotation = Quaternion.LookRotation(boids[i].velocity);
        }
    }

    void OnDestroy()
    {
        if (boidBuffer != null)
            boidBuffer.Release();
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(Vector3.zero, bounds * 2); // Visualizar caixa de limites
    }
}
