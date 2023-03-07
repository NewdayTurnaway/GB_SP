using UnityEngine;

[ExecuteInEditMode, RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class Star : MonoBehaviour 
{
    public ColorPoint center;
    /*[NonReorderable]*/ public ColorPoint[] points;
    public int frequency = 1;
    
    private Mesh _mesh;
    private Vector3[] _vertices;
    private Color[] _colors;
    private int[] _triangles;

    private void OnEnable () 
    {
        UpdateMesh();
    }
    
    public void UpdateMesh () 
    {
        if (_mesh == null) 
        {
            GetComponent<MeshFilter>().mesh = _mesh = new Mesh();
            _mesh.name = "Star Mesh";
            //_mesh.hideFlags = HideFlags.HideAndDontSave;
        }
        
        if (frequency < 1) 
        {
            frequency = 1;
        }
        
        points ??= new ColorPoint[0];

        var numberOfPoints = frequency * points.Length;

        if (_vertices == null || _vertices.Length != numberOfPoints + 1) 
        {
            _vertices = new Vector3[numberOfPoints + 1];
            _colors = new Color[numberOfPoints + 1];
            _triangles = new int[numberOfPoints * 3];
            _mesh.Clear();
        }
        
        if (numberOfPoints >= 3) 
        {
            _vertices[0] = center.position;
            _colors[0] = center.color;

            var angle = -360f / numberOfPoints;

            for(int repetitions = 0, v = 1, t = 1; repetitions < frequency; repetitions++)
            {
                for(int p = 0; p < points.Length; p += 1, v += 1, t += 3)
                {
                    _vertices[v] = Quaternion.Euler(0f, 0f, angle * (v - 1)) * points[p].position;
                    _colors[v] = points[p].color;
                    _triangles[t] = v;
                    _triangles[t + 1] = v + 1;
                }
            }

            _triangles[^1] = 1;
        }

        _mesh.vertices = _vertices;
        _mesh.colors = _colors;
        _mesh.triangles = _triangles;
    }

    private void Reset () 
    {
        UpdateMesh();
    }
}