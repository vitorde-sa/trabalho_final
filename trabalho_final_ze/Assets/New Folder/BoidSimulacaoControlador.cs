using UnityEngine;




public enum ModoDeSimulacao
{
    CPU,
    GPU
}

public class BoidSimulacaoControlador : MonoBehaviour
{
    public ModoDeSimulacao modoAtual = ModoDeSimulacao.CPU;

    public BoidManager boidManagerCPU;
    public BoidGPUManager boidManagerGPU;

    void Start()
    {
        AtivarModo(modoAtual);
    }

    void Update()
    {
        // Tecla para alternar entre os modos
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            modoAtual = (modoAtual == ModoDeSimulacao.CPU) ? ModoDeSimulacao.GPU : ModoDeSimulacao.CPU;
            AtivarModo(modoAtual);
        }
    }

    void AtivarModo(ModoDeSimulacao modo)
    {
        if (modo == ModoDeSimulacao.CPU)
        {
            if (boidManagerGPU != null) boidManagerGPU.enabled = false;
            if (boidManagerCPU != null)
            {
                boidManagerCPU.enabled = true;
                AtivarTodosBoids(true);
            }
        }
        else
        {
            if (boidManagerCPU != null)
            {
                boidManagerCPU.enabled = false;
                AtivarTodosBoids(false);
            }
            if (boidManagerGPU != null) boidManagerGPU.enabled = true;
        }
    }

    void AtivarTodosBoids(bool ativo)
    {
        if (boidManagerCPU == null) return;

        foreach (var boid in boidManagerCPU.boids)
        {
            if (boid != null)
                boid.gameObject.SetActive(ativo);
        }
    }
}
