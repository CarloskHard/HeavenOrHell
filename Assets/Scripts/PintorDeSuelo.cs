
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;


public enum TipoCarril
{
    Vacio, // Un carril sin nada, solo asfalto
    LineaContinua,
    FlechaDerecha,
    FlechaIzquierda,
    Mediana
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

        // Generamos el plan lógico de la carretera.
        List<TipoCarril> miPlan = GenerarPlanDeCarretera(distanciaARecorrer);

        // 3. "CONSTRUIR": Le pasamos el plan a la función que dibuja.
        DibujarCarreteraDesdePlan(miPlan);

    }

    // --- NUEVA FUNCIÓN: EL CEREBRO (PLANIFICADOR) ---
    private List<TipoCarril> GenerarPlanDeCarretera(int alturaTotal)
    {
        List<TipoCarril> plan = new List<TipoCarril>();
        TipoCarril ultimoCarril = TipoCarril.Vacio;
        int alturaGenerada = 0;
        int alturaDesdeUltimaMediana = 0;

        // Función local para no repetir código al añadir carriles
        void RegistrarCarril(TipoCarril carril)
        {
            plan.Add(carril);
            ultimoCarril = carril;
            int altura = ObtenerAlturaDeCarril(carril);
            alturaGenerada += altura;

            if (carril == TipoCarril.Mediana)
                alturaDesdeUltimaMediana = 0;
            else
                alturaDesdeUltimaMediana += altura;
        }

        while (alturaGenerada < alturaTotal)
        {
            // Opciones: 0 = Derecha, 1 = Izquierda, 2 = Mediana (si está disponible)
            bool puedeMediana = (alturaDesdeUltimaMediana >= 3 && alturaTotal - alturaGenerada >= 2);
            int opciones = puedeMediana ? 3 : 2;
            int seleccion = Random.Range(0, opciones);

            TipoCarril elegido = seleccion == 0 ? TipoCarril.FlechaDerecha :
                                 seleccion == 1 ? TipoCarril.FlechaIzquierda :
                                                  TipoCarril.Mediana;

            // Lógica de inyección obligatoria de línea continua
            bool necesitaLineaContinua =
                (ultimoCarril == TipoCarril.FlechaDerecha && elegido == TipoCarril.FlechaIzquierda) ||
                (ultimoCarril == TipoCarril.FlechaIzquierda && elegido == TipoCarril.FlechaDerecha);

            if (necesitaLineaContinua)
            {
                // Si no hay espacio para la línea y la flecha, terminamos la generación
                if (alturaGenerada + 2 > alturaTotal) break;

                RegistrarCarril(TipoCarril.LineaContinua);
            }

            RegistrarCarril(elegido);
        }

        return plan;
    }


    // --- NUEVA FUNCIÓN: AYUDANTE DE ALTURAS ---
    private int ObtenerAlturaDeCarril(TipoCarril carril)
    {
        if (carril == TipoCarril.Mediana)
        {
            return 2;
        }
        return 1;
    }

    // --- NUEVA FUNCIÓN: EL OBRERO (DIBUJANTE) ---
    private void DibujarCarreteraDesdePlan(List<TipoCarril> plan)
    {
        int alturaActual = 0;
        foreach (TipoCarril carril in plan)
        {
            switch (carril)
            {
                case TipoCarril.LineaContinua:
                    LineaContinua(alturaActual);
                    break;
                case TipoCarril.FlechaDerecha:
                    PintarCarrilDer(alturaActual);
                    break;
                case TipoCarril.FlechaIzquierda:
                    PintarCarrilIzq(alturaActual);
                    break;
                case TipoCarril.Mediana:
                    PintarMediana(alturaActual);
                    break;
                case TipoCarril.Vacio:
                    break;
            }
            alturaActual += ObtenerAlturaDeCarril(carril);
        }
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
