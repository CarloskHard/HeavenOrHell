using UnityEngine;
using UnityEngine.Tilemaps;

public class PintorDeSuelo : MonoBehaviour
{

    [SerializeField] private Tilemap miLienzo;      // Para arrastrar nuestro Tilemap "Floor"
    [SerializeField] private TileBase miPincel;     // Para arrastrar nuestro Tile de Asfalto
    [SerializeField] private int anchoDeLaLinea = 10; // <-- Añadimos esta


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

        for (int x = 0; x < anchoDeLaLinea; x++)
        {
            Vector3Int vector = new Vector3Int( x - 5, 0, 0);
            miLienzo.SetTile(vector, miPincel);
        }
    }

    
}
