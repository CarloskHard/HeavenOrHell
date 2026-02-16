using UnityEngine;
using UnityEngine.Tilemaps;

public class RowConfig : MonoBehaviour
{
    [Header("Configuración Visual")]
    public TileBase sueloTile; // Arrastra aquí el Tile de asfalto o pasto
    public int ancho = 10;     // Cuántos tiles de ancho tiene tu juego

    //[Header("Objetos (Opcional)")]
    // Aquí podrías poner referencias para instanciar árboles o monedas
    // si no vienen ya como hijos del prefab.

    // Esta función será llamada por el Generador
    public void PintarFila(Tilemap map, int yPos)
    {
        // Calculamos el inicio para que quede centrado (ej: de -5 a 5)
        int startX = -(ancho / 2);

        for (int x = 0; x < ancho; x++)
        {
            Vector3Int posicionCelda = new Vector3Int(startX + x, yPos, 0);
            map.SetTile(posicionCelda, sueloTile);
        }
    }
}