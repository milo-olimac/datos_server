using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    [Header("Cola")]
    [SerializeField] private TextMeshProUGUI txtContadorCola;
    [SerializeField] private Image panelEstado;
    [SerializeField] private TextMeshProUGUI txtEstadoServidor;

    [Header("Último Procesado")]
    [SerializeField] private TextMeshProUGUI txtUltimoID;
    [SerializeField] private TextMeshProUGUI txtUltimoTamaño;
    [SerializeField] private TextMeshProUGUI txtUltimoEspera;

    [Header("Histórico")]
    [SerializeField] private TextMeshProUGUI txtTotalProcesados;
    [SerializeField] private TextMeshProUGUI txtPromedioEspera;

    [Header("Buscador (Extra)")]
    [SerializeField] private TMP_InputField inputBusquedaID;
    [SerializeField] private TextMeshProUGUI txtResultadoBusqueda;

    [Header("Colores")]
    [SerializeField] private Color colorNormal = Color.green;
    [SerializeField] private Color colorSaturado = Color.red;

    private const int LIMITE_SATURACION = 20;

    void Start()
    {
        ServidorManager.Instance.onColaActualizada += ActualizarUI;
        ServidorManager.Instance.onPaqueteProcesado += MostrarUltimoProcesado;

        ActualizarUI();
    }

    void OnDestroy()
    {
        if (ServidorManager.Instance != null)
        {
            ServidorManager.Instance.onColaActualizada -= ActualizarUI;
            ServidorManager.Instance.onPaqueteProcesado -= MostrarUltimoProcesado;
        }
    }

    void ActualizarUI()
    {
        int enCola = ServidorManager.Instance.colaProcesamiento.Count;

        // Contador de cola
        if (txtContadorCola != null)
            txtContadorCola.text = $"Paquetes en cola: {enCola}";

        // Estado del servidor
        bool saturado = enCola > LIMITE_SATURACION;
        if (panelEstado != null)
            panelEstado.color = saturado ? colorSaturado : colorNormal;

        if (txtEstadoServidor != null)
            txtEstadoServidor.text = saturado ? "⚠ SERVIDOR SATURADO" : "Servidor Normal";

        // Total procesados
        if (txtTotalProcesados != null)
            txtTotalProcesados.text = $"Total procesados: {ServidorManager.Instance.historialProcesados.Count}";

        // Promedio de espera
        if (txtPromedioEspera != null)
            txtPromedioEspera.text = $"Espera promedio: {ServidorManager.Instance.tiempoEsperaPromedio:F2}s";
    }

    void MostrarUltimoProcesado(PaqueteDato paquete, float tiempoEspera)
    {
        if (txtUltimoID != null)
            txtUltimoID.text = $"ID: {paquete.id}";

        if (txtUltimoTamaño != null)
            txtUltimoTamaño.text = $"Tamaño: {paquete.tamañoCarga} KB";

        if (txtUltimoEspera != null)
            txtUltimoEspera.text = $"Tiempo de espera: {tiempoEspera:F2}s";
    }

    // Asignar al botón "Procesar Siguiente" desde el Inspector
    public void OnBotonProcesar()
    {
        bool procesado = ServidorManager.Instance.ProcesarSiguiente();
        if (!procesado)
        {
            if (txtEstadoServidor != null)
                txtEstadoServidor.text = "Cola vacía — nada que procesar";
        }
    }

    // Asignar al botón "Buscar" desde el Inspector (DESAFÍO EXTRA)
    public void OnBotonBuscar()
    {
        if (inputBusquedaID == null || txtResultadoBusqueda == null) return;

        string idBuscado = inputBusquedaID.text.Trim();

        if (string.IsNullOrEmpty(idBuscado))
        {
            txtResultadoBusqueda.text = "Ingrese un ID para buscar.";
            return;
        }

        if (ServidorManager.Instance.BuscarPorID(idBuscado, out PaqueteDato encontrado))
        {
            txtResultadoBusqueda.text = $"✔ Encontrado\nTamaño: {encontrado.tamañoCarga} KB\nLlegada: {encontrado.tiempoLlegada:F2}s";
        }
        else
        {
            txtResultadoBusqueda.text = "✘ ID no encontrado en el historial.";
        }
    }
}
