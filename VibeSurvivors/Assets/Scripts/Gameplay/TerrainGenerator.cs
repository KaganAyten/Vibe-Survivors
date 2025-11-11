using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer), typeof(MeshCollider))]
public class TerrainGenerator : MonoBehaviour
{
    [Header("Terrain Size")]
    [SerializeField] private int width = 100;
    [SerializeField] private int height = 100;

    [Header("Height Settings")]
    [SerializeField] private float heightScale = 10f; // Maksimum yükseklik
[SerializeField] private float noiseScale = 0.05f; // Ne kadar büyük tepeler (küçük = büyük tepeler)

    [Header("Noise Layers (Octaves)")]
    [SerializeField] private int octaves = 4;
    [SerializeField, Range(0f, 1f)] private float persistence = 0.5f;
    [SerializeField] private float lacunarity = 2f;

    [Header("Seed")]
    [SerializeField] private int seed = 0;

    [Header("Colors")]
    [SerializeField] private Color lowColor = new Color(0.4f, 0.7f, 0.3f);      // Düz - Ye?il
    [SerializeField] private Color midColor = new Color(0.5f, 0.6f, 0.35f);     // Orta - Aç?k ye?il
    [SerializeField] private Color highColor = new Color(0.55f, 0.45f, 0.3f);   // Yüksek - Kahverengi

    private MeshFilter meshFilter;
    private MeshRenderer meshRenderer;
    private MeshCollider meshCollider;
    private Mesh mesh;

    private void Awake()
    {
        meshFilter = GetComponent<MeshFilter>();
        meshRenderer = GetComponent<MeshRenderer>();
     meshCollider = GetComponent<MeshCollider>();
    }

    private void Start()
    {
      GenerateTerrain();
    }

    [ContextMenu("Generate Terrain")]
    public void GenerateTerrain()
    {
        if (meshFilter == null) meshFilter = GetComponent<MeshFilter>();
        if (meshRenderer == null) meshRenderer = GetComponent<MeshRenderer>();
        if (meshCollider == null) meshCollider = GetComponent<MeshCollider>();

        mesh = new Mesh();
      mesh.name = "Simple Terrain";
 mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;

        Vector3[] vertices = new Vector3[(width + 1) * (height + 1)];
        Vector2[] uv = new Vector2[vertices.Length];
     Color[] colors = new Color[vertices.Length];
     int[] triangles = new int[width * height * 6];

        // Random offset seed'e göre
        System.Random prng = new System.Random(seed);
Vector2[] octaveOffsets = new Vector2[octaves];
        for (int i = 0; i < octaves; i++)
        {
    octaveOffsets[i] = new Vector2(
        prng.Next(-10000, 10000),
    prng.Next(-10000, 10000)
   );
      }

  float maxNoiseHeight = float.MinValue;
        float minNoiseHeight = float.MaxValue;

        float halfWidth = width / 2f;
        float halfHeight = height / 2f;

    // Yükseklik haritas? olu?tur
      float[,] noiseMap = new float[width + 1, height + 1];

   for (int y = 0; y <= height; y++)
{
       for (int x = 0; x <= width; x++)
{
        float amplitude = 1f;
     float frequency = 1f;
      float noiseHeight = 0f;

 // Octave'leri birle?tir
          for (int i = 0; i < octaves; i++)
        {
     float sampleX = (x - halfWidth) * noiseScale * frequency + octaveOffsets[i].x;
        float sampleY = (y - halfHeight) * noiseScale * frequency + octaveOffsets[i].y;

      float perlinValue = Mathf.PerlinNoise(sampleX, sampleY) * 2 - 1;
          noiseHeight += perlinValue * amplitude;

   amplitude *= persistence;
        frequency *= lacunarity;
        }

 if (noiseHeight > maxNoiseHeight) maxNoiseHeight = noiseHeight;
         if (noiseHeight < minNoiseHeight) minNoiseHeight = noiseHeight;

    noiseMap[x, y] = noiseHeight;
            }
     }

        // Vertex'leri olu?tur ve normalize et
        int vertIndex = 0;
  for (int y = 0; y <= height; y++)
    {
            for (int x = 0; x <= width; x++)
            {
         // Normalize (0-1 aras?)
 float normalizedHeight = Mathf.InverseLerp(minNoiseHeight, maxNoiseHeight, noiseMap[x, y]);
      
                // Yükseklik uygula
         float finalHeight = normalizedHeight * heightScale;

     vertices[vertIndex] = new Vector3(x, finalHeight, y);
          uv[vertIndex] = new Vector2((float)x / width, (float)y / height);

                // Basit renklendirme - yüksekli?e göre
     colors[vertIndex] = GetColorByHeight(normalizedHeight);

    vertIndex++;
   }
        }

        // Triangle'lar? olu?tur
int vert = 0;
     int tris = 0;
        for (int y = 0; y < height; y++)
        {
   for (int x = 0; x < width; x++)
       {
    triangles[tris + 0] = vert + 0;
          triangles[tris + 1] = vert + width + 1;
          triangles[tris + 2] = vert + 1;
      triangles[tris + 3] = vert + 1;
         triangles[tris + 4] = vert + width + 1;
  triangles[tris + 5] = vert + width + 2;

       vert++;
       tris += 6;
            }
            vert++;
     }

 mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uv;
        mesh.colors = colors;
   mesh.RecalculateNormals();
        mesh.RecalculateBounds();

        meshFilter.mesh = mesh;

   if (meshCollider != null)
    {
    meshCollider.sharedMesh = null;
        meshCollider.sharedMesh = mesh;
        }

        EnsureVertexColorMaterial();

 Debug.Log($"Terrain: {width}x{height}, Height: 0-{heightScale}m");
    }

    private Color GetColorByHeight(float normalizedHeight)
    {
        // Düz = ye?il, yüksek = kahverengi
        if (normalizedHeight < 0.4f)
        {
   // Dü?ük alanlar (ye?il)
return Color.Lerp(lowColor, midColor, normalizedHeight / 0.4f);
        }
        else
  {
     // Yüksek alanlar (kahverengi)
      return Color.Lerp(midColor, highColor, (normalizedHeight - 0.4f) / 0.6f);
        }
    }

    private void EnsureVertexColorMaterial()
{
        if (meshRenderer == null)
  meshRenderer = GetComponent<MeshRenderer>();

        if (meshRenderer == null)
     {
            Debug.LogError("MeshRenderer component bulunamad?!");
      return;
        }

     Shader vertexColorShader = Shader.Find("Custom/TerrainVertexColor");

        if (vertexColorShader == null)
      {
          Debug.LogWarning("Custom/TerrainVertexColor shader bulunamad?. Universal Render Pipeline/Lit kullan?l?yor.");
            vertexColorShader = Shader.Find("Universal Render Pipeline/Lit");
     }

        if (meshRenderer.sharedMaterial == null ||
            meshRenderer.sharedMaterial.name == "Default-Material" ||
 meshRenderer.sharedMaterial.shader != vertexColorShader)
        {
            Material material = new Material(vertexColorShader);
   material.name = "Terrain Material (Auto)";
            meshRenderer.material = material;
        }
    }

    [ContextMenu("Randomize Seed")]
    public void RandomizeSeed()
    {
        seed = Random.Range(0, 100000);
        GenerateTerrain();
    }

    private void OnValidate()
    {
      if (width < 1) width = 1;
        if (height < 1) height = 1;
    }
}
