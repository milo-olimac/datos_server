using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServidorManager : MonoBehaviour
{
    public static ServidorManager Instance;

    [Header("Configuración")]
    [SerializeField] private float intervaloGeneracion = 3f;
    [SerializeField] private int minCarga = 10;
    [SerializeField] private int maxCarga = 500;

    public Queue<PaqueteDato> colaProcesamiento = new Queue<PaqueteDato>();
    public Dictionary<string, PaqueteDato> historialProcesados = new Dictionary<string, PaqueteDato>();

    // Métricas
    public PaqueteDato ultimoProcesado { get; private set; }
    public float tiempoEsperaAcumulado { get; private set; }
    public float tiempoEsperaPromedio { get; private set; }
    public float ultimoTiempoEspera { get; private set; }

    // Eventos para que UIManager se suscriba
    public System.Action onColaActualizada;
    public System.Action<PaqueteDato, float> onPaqueteProcesado;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        StartCoroutine(GenerarPaquetes());
    }

    IEnumerator GenerarPaquetes()
    {
        while (true)
        {
            yield return new WaitForSeconds(intervaloGeneracion);

            int cantidad = Random.Range(1, 6);
            for (int i = 0; i < cantidad; i++)
            {
                int carga = Random.Range(minCarga, maxCarga + 1);
                PaqueteDato nuevo = new PaqueteDato(carga, Time.time);
                colaProcesamiento.Enqueue(nuevo);
            }

            onColaActualizada?.Invoke();
        }
    }

    public bool ProcesarSiguiente()
    {
        if (colaProcesamiento.Count == 0) return false;

        PaqueteDato paquete = colaProcesamiento.Dequeue();

        // Garantizar que no haya IDs duplicados
        if (!historialProcesados.ContainsKey(paquete.id))
        {
            historialProcesados.Add(paquete.id, paquete);
        }

        float tiempoEspera = Time.time - paquete.tiempoLlegada;
        tiempoEsperaAcumulado += tiempoEspera;
        tiempoEsperaPromedio = tiempoEsperaAcumulado / historialProcesados.Count;
        ultimoTiempoEspera = tiempoEspera;
        ultimoProcesado = paquete;

        onPaqueteProcesado?.Invoke(paquete, tiempoEspera);
        onColaActualizada?.Invoke();

        return true;
    }

    public bool BuscarPorID(string id, out PaqueteDato resultado)
    {
        if (historialProcesados.ContainsKey(id))
        {
            resultado = historialProcesados[id];
            return true;
        }
        resultado = null;
        return false;
    }
}
