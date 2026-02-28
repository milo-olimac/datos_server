using System;

[System.Serializable]
public class PaqueteDato
{
    public string id;
    public int tamañoCarga;
    public float tiempoLlegada;

    public PaqueteDato(int carga, float tiempo)
    {
        id = Guid.NewGuid().ToString();
        tamañoCarga = carga;
        tiempoLlegada = tiempo;
    }
}
