using UnityEngine;

public class BoidGPUManager : MonoBehaviour
{
    public ComputeShader boidCompute;
    public int numeroDeBoids = 1000;
    public Vector3 limites = new Vector3(20, 20, 20);
    public Mesh boidMesh;
    public Material boidMaterial;

    [Header("Parâmetros de Boid")]
    public float velocidadeMin = 2f;
    public float velocidadeMax = 5f;
    public float raioDeVisao = 5f;
    public float fatorCoesao = 1f;
    public float fatorAlinhamento = 1f;
    public float fatorSeparacao = 3f;

    struct BoidData
    {
        public Vector3 position;
        public Vector3 velocity;
    }

    ComputeBuffer boidBuffer;

    void Start()
    {
        BoidData[] dados = new BoidData[numeroDeBoids];
        for (int i = 0; i < numeroDeBoids; i++)
        {
            dados[i].position = Random.insideUnitSphere * 10f;
            dados[i].velocity = Random.onUnitSphere * velocidadeMin;
        }

        boidBuffer = new ComputeBuffer(numeroDeBoids, sizeof(float) * 6);
        boidBuffer.SetData(dados);
    }

    void Update()
    {
        int kernel = boidCompute.FindKernel("CSMain");

        boidCompute.SetInt("boidCount", numeroDeBoids);
        boidCompute.SetFloat("deltaTime", Time.deltaTime);
        boidCompute.SetVector("bounds", limites / 2f);
        boidCompute.SetFloat("velocidadeMin", velocidadeMin);
        boidCompute.SetFloat("velocidadeMax", velocidadeMax);
        boidCompute.SetFloat("raioDeVisao", raioDeVisao);
        boidCompute.SetFloat("fatorCoesao", fatorCoesao);
        boidCompute.SetFloat("fatorAlinhamento", fatorAlinhamento);
        boidCompute.SetFloat("fatorSeparacao", fatorSeparacao);

        boidCompute.SetBuffer(kernel, "boids", boidBuffer);
        boidCompute.Dispatch(kernel, Mathf.CeilToInt(numeroDeBoids / 64f), 1, 1);

        boidMaterial.SetBuffer("boids", boidBuffer);
        Graphics.DrawMeshInstancedProcedural(boidMesh, 0, boidMaterial, new Bounds(Vector3.zero, limites * 2), numeroDeBoids);
    }

    void OnDestroy()
    {
        if (boidBuffer != null)
            boidBuffer.Release();
    }
}
