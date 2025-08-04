using UnityEngine;

public class BoidSimulationController : MonoBehaviour
{
    public GameObject cpuSimulacao;
    public GameObject gpuSimulacao;

    private bool usandoGPU = true;

    void Start()
    {
        AlternarSimulacao(usandoGPU);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            usandoGPU = !usandoGPU;
            AlternarSimulacao(usandoGPU);
        }
    }

    void AlternarSimulacao(bool gpuAtivo)
    {
        gpuSimulacao.SetActive(gpuAtivo);
        cpuSimulacao.SetActive(!gpuAtivo);
        Debug.Log("Simulação atual: " + (gpuAtivo ? "GPU" : "CPU"));
    }
}
