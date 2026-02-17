
using UnityEngine;
using UnityEngine.Tilemaps;

public class PintorDeSuelo : MonoBehaviour
{

    [SerializeField] private Tilemap asfaltoTilemap;      // Para arrastrar nuestro Tilemap "Floor"
    [SerializeField] private Tilemap marcasDeSueloTilemap;      // Para arrastrar nuestro Tilemap "Floor"
    [SerializeField] private int anchoDeLaLinea = 10; // <-- Añadimos esta
    [SerializeField] private int distanciaARecorrer = 70;


    //Tomamos los tiles que vamos a usar de resources
    // 1. Solo declaramos las variables aquí
    private TileBase tileFlecha;
    private TileBase tileAsfalto;
    private TileBase tileLineaContinua;
    private TileBase tileMediana;
    private TileBase tilePasoZebra;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // 2. Cargamos los recursos dentro de Start (o Awake)
        tileFlecha = Resources.Load<TileBase>("Tiles/Flecha");
        tileAsfalto = Resources.Load<TileBase>("Tiles/Asfalto");
        tileLineaContinua = Resources.Load<TileBase>("Tiles/LineaContinua");
        tileMediana = Resources.Load<TileBase>("Tiles/Mediana");
        tilePasoZebra = Resources.Load<TileBase>("Tiles/PasoZebra");

        //PintarAsfalto();
        PintarCarretera1Carril(0);
        PintarMediana(1);
        PintarCarretera1Carril(3);
        PintarCarretera1Carril(4);

    }


    private void PintarAsfalto()
    {
        for (int x = 0; x < anchoDeLaLinea; x++)
        {
            for (int y = -7; y < distanciaARecorrer; y++)
            {
                Vector3Int vector = new Vector3Int(x - 5, y, 0);
                asfaltoTilemap.SetTile(vector, tileAsfalto);
            }
        }
    }

    private void PintarMediana(int altura)
    {

        for(int x = 0; x < anchoDeLaLinea; x++)
        {

            Vector3Int vector1 = new Vector3Int(x - 5, altura, 0);
            marcasDeSueloTilemap.SetTile(vector1, tileMediana);
            UnityEngine.Quaternion rotacion180 = UnityEngine.Quaternion.Euler(0, 0, 180);
            Matrix4x4 matrix1 = Matrix4x4.TRS(Vector3.zero, rotacion180, Vector3.one);
            marcasDeSueloTilemap.SetTransformMatrix(vector1, matrix1);

        }

        for (int x = 0; x < anchoDeLaLinea; x++)
        {

            Vector3Int vector1 = new Vector3Int(x - 5, altura + 1, 0);
            marcasDeSueloTilemap.SetTile(vector1, tileMediana);
        }

    }


    private void PintarCarretera1Carril(int altura)
    {
        //Flechas
        Vector3Int vectorFlecha1 = new Vector3Int(-5, altura, 0);
        Vector3Int vectorFlecha2 = new Vector3Int(4, altura, 0);


        marcasDeSueloTilemap.SetTile(vectorFlecha1, tileFlecha);
        marcasDeSueloTilemap.SetTile(vectorFlecha2, tileFlecha);

        //Paso de Zebra
        Vector3Int vectorPasoZebra1 = new Vector3Int(-1, altura, 0);
        Vector3Int vectorPasoZebra2 = new Vector3Int(0, altura, 0);

        marcasDeSueloTilemap.SetTile(vectorPasoZebra1, tilePasoZebra);
        marcasDeSueloTilemap.SetTile(vectorPasoZebra2, tilePasoZebra);

        UnityEngine.Quaternion rotacion90 = UnityEngine.Quaternion.Euler(0, 0, 90);
        UnityEngine.Quaternion rotacion270 = UnityEngine.Quaternion.Euler(0, 0, -90);


        Matrix4x4 matrix1 = Matrix4x4.TRS(Vector3.zero, rotacion90, Vector3.one);
        Matrix4x4 matrix2 = Matrix4x4.TRS(Vector3.zero, rotacion270, Vector3.one);

        // 4. Aplicamos la matriz a la celda donde pusimos la flecha
        marcasDeSueloTilemap.SetTransformMatrix(vectorPasoZebra1, matrix1);
        marcasDeSueloTilemap.SetTransformMatrix(vectorPasoZebra2, matrix2);

    }
    
}
