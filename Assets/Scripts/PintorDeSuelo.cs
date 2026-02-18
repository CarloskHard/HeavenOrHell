
using UnityEngine;
using UnityEngine.Tilemaps;


public enum TipoCarril
{
    Vacio, // Un carril sin nada, solo asfalto
    LineaContinua,
    FlechaDerecha,
    FlechaIzquierda
    // Aquí podrías añadir más en el futuro: LineaDiscontinua, PasoDeCebra...
}
public class PintorDeSuelo : MonoBehaviour
{

    [SerializeField] private Tilemap asfaltoTilemap;      // Para arrastrar nuestro Tilemap "Floor"
    [SerializeField] private Tilemap marcasDeSueloTilemap;      // Para arrastrar nuestro Tilemap "Floor"
    [SerializeField] private int anchoDeLaLinea = 10; // <-- Añadimos esta
    [SerializeField] private int distanciaARecorrer = 70;


    //Tomamos los tiles que vamos a usar de resources
    // 1. Solo declaramos las variables aquí
    private TileBase tileFlechaIzquierda;
    private TileBase tileFlechaDerecha;
    private TileBase tileAsfalto;
    private TileBase tileLineaContinua;
    private TileBase tileMediana;
    private TileBase tilePasoZebra;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // 2. Cargamos los recursos dentro de Start (o Awake)
        tileFlechaIzquierda = Resources.Load<TileBase>("Tiles/FlechaIzquierda");
        tileFlechaDerecha = Resources.Load<TileBase>("Tiles/FlechaDerecha");
        tileAsfalto = Resources.Load<TileBase>("Tiles/Asfalto");
        tileLineaContinua = Resources.Load<TileBase>("Tiles/LineaContinua");
        tileMediana = Resources.Load<TileBase>("Tiles/Mediana");
        tilePasoZebra = Resources.Load<TileBase>("Tiles/PasoZebra");

        //PintarCarriles();
        PintarCarrilIzq(0);
        PintarMediana(1);
        PintarCarrilIzq(3);
        LineaContinua(4);
        PintarCarrilDer(5);
        LineaContinua(6);
        PintarCarrilIzq(7);
        PintarMediana(8);

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

    private void LineaContinua(int altura)
    {
        UnityEngine.Quaternion rotacion90 = UnityEngine.Quaternion.Euler(0, 0, 90);
        UnityEngine.Quaternion rotacion270 = UnityEngine.Quaternion.Euler(0, 0, -90);


        Matrix4x4 matrix1 = Matrix4x4.TRS(Vector3.zero, rotacion90, Vector3.one);
        Matrix4x4 matrix2 = Matrix4x4.TRS(Vector3.zero, rotacion270, Vector3.one);

        for (int x = 0; x < anchoDeLaLinea; x++)
        {

            Vector3Int vector1 = new Vector3Int(x - 5, altura, 0);
            marcasDeSueloTilemap.SetTile(vector1, tileLineaContinua);
            
            marcasDeSueloTilemap.SetTransformMatrix(vector1, matrix1);
        }
        //Paso de Zebra
        Vector3Int vectorPasoZebra1 = new Vector3Int(-1, altura, 0);
        Vector3Int vectorPasoZebra2 = new Vector3Int(0, altura, 0);

        marcasDeSueloTilemap.SetTile(vectorPasoZebra1, tilePasoZebra);
        marcasDeSueloTilemap.SetTile(vectorPasoZebra2, tilePasoZebra);


        // 4. Aplicamos la matriz a la celda donde pusimos el paso de zebra
        marcasDeSueloTilemap.SetTransformMatrix(vectorPasoZebra1, matrix1);
        marcasDeSueloTilemap.SetTransformMatrix(vectorPasoZebra2, matrix2);

    }

    private void PintarCarrilIzq(int altura)
    {
        PintarCarril(tileFlechaIzquierda, altura);
    }

    private void PintarCarrilDer(int altura)
    {
        PintarCarril(tileFlechaDerecha, altura);
    }


    private void PintarCarril(TileBase spriteFlecha, int altura)
    {
        //Flechas
        Vector3Int vectorFlecha1 = new Vector3Int(-5, altura, 0);
        Vector3Int vectorFlecha2 = new Vector3Int(4, altura, 0);


        marcasDeSueloTilemap.SetTile(vectorFlecha1, spriteFlecha);
        marcasDeSueloTilemap.SetTile(vectorFlecha2, spriteFlecha);

        //Paso de Zebra
        Vector3Int vectorPasoZebra1 = new Vector3Int(-1, altura, 0);
        Vector3Int vectorPasoZebra2 = new Vector3Int(0, altura, 0);

        marcasDeSueloTilemap.SetTile(vectorPasoZebra1, tilePasoZebra);
        marcasDeSueloTilemap.SetTile(vectorPasoZebra2, tilePasoZebra);

        UnityEngine.Quaternion rotacion90 = UnityEngine.Quaternion.Euler(0, 0, 90);
        UnityEngine.Quaternion rotacion270 = UnityEngine.Quaternion.Euler(0, 0, -90);


        Matrix4x4 matrix1 = Matrix4x4.TRS(Vector3.zero, rotacion90, Vector3.one);
        Matrix4x4 matrix2 = Matrix4x4.TRS(Vector3.zero, rotacion270, Vector3.one);

        // 4. Aplicamos la matriz a la celda donde pusimos el paso de zebra
        marcasDeSueloTilemap.SetTransformMatrix(vectorPasoZebra1, matrix1);
        marcasDeSueloTilemap.SetTransformMatrix(vectorPasoZebra2, matrix2);

    }
    
}
