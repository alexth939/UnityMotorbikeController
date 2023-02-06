using UnityEngine;

public class Skidmarks: MonoBehaviour
{
    //@script RequireComponent(MeshFilter)
    //@script RequireComponent(MeshRenderer)

    // If the mesh needs to be updated, i.e. a new section has been added,
    // the current mesh is removed, and a new mesh for the skidmarks is generated.

    public bool skidmake = false;
    public int maxMarks = 1024;			// Maximum number of marks total handled by one instance of the script.
    public float markWidth = 0.275f;		// The width of the skidmarks. Should match the width of the wheel that it is used for. In meters.
    public float groundOffset = 0.02f;	// The distance the skidmarks is places above the surface it is placed upon. In meters.
    public float minDistance = 0.1f;		// The minimum distance between two marks places next to each other. 

    private int _marksCount = 0;

    private MarkSection[] _skidmarks;

    private bool _isUpdated = false;

    //check if at the origin or not and jump to it if not
    private void Start()
    {
        if(transform.position != new Vector3(0, 0, 0))
        {
            transform.position = new Vector3(0, 0, 0);
        }
    }

    // Initiallizes the array holding the skidmark sections.
    private void Awake()
    {
        _skidmarks = new MarkSection[maxMarks];

        for(var i = 0; i < maxMarks; i++)
            _skidmarks[i] = new MarkSection();

        if(GetComponent<MeshFilter>().mesh == null)
            GetComponent<MeshFilter>().mesh = new Mesh();
    }

    // Function called by the wheels that is skidding. Gathers all the information needed to
    // create the mesh later. Sets the intensity of the skidmark section b setting the alpha
    // of the vertex color.
    public int AddSkidMark(Vector3 pos, Vector3 normal, float intensity, int lastIndex)
    {
        if(intensity > 1)
            intensity = 1.0f;

        if(intensity < 0)
            return -1;

        MarkSection curr = _skidmarks[_marksCount % maxMarks];
        curr.Position = pos + normal * groundOffset;
        curr.Normal = normal;
        curr.Intensity = intensity;
        curr.LastIndex = lastIndex;

        if(lastIndex != -1)
        {
            MarkSection last = _skidmarks[lastIndex % maxMarks];
            Vector3 dir = (curr.Position - last.Position);
            Vector3 xDir = Vector3.Cross(dir, normal).normalized;

            curr.Posl = curr.Position + xDir * markWidth * 0.5f;
            curr.Posr = curr.Position - xDir * markWidth * 0.5f;
            curr.Tangent = new Vector4(xDir.x, xDir.y, xDir.z, 1);

            if(last.LastIndex == -1)
            {
                last.Tangent = curr.Tangent;
                last.Posl = curr.Position + xDir * markWidth * 0.5f;
                last.Posr = curr.Position - xDir * markWidth * 0.5f;
            }
        }

        _marksCount++;
        _isUpdated = true;

        return _marksCount - 1;
    }

    // If the mesh needs to be updated, i.e. a new section has been added,
    // the current mesh is removed, and a new mesh for the skidmarks is generated.

    void LateUpdate()
    {
        WheelCollider[] wheels = FindObjectsOfType(typeof(WheelCollider)) as WheelCollider[];

        foreach(WheelCollider wheel in wheels)
        {
            if(!skidmake)
            {
                wheel.gameObject.AddComponent<WheelSkidmarks>();
            }
        }

        skidmake = true;

        if(!_isUpdated)
        {
            return;
        }

        _isUpdated = false;

        Mesh mesh = GetComponent<MeshFilter>().mesh;
        mesh.Clear();
        int segmentCount = 0;

        for(int j = 0; j < _marksCount && j < maxMarks; j++)
            if(_skidmarks[j].LastIndex != -1 && _skidmarks[j].LastIndex > _marksCount - maxMarks)
                segmentCount++;

        Vector3[] vertices = new Vector3[segmentCount * 4];
        Vector3[] normals = new Vector3[segmentCount * 4];
        Vector4[] tangents = new Vector4[segmentCount * 4];
        Color[] colors = new Color[segmentCount * 4];
        Vector2[] uvs = new Vector2[segmentCount * 4];
        int[] triangles = new int[segmentCount * 6];
        segmentCount = 0;

        for(int i = 0; i < _marksCount && i < maxMarks; i++)
        {
            if(_skidmarks[i].LastIndex != -1 && _skidmarks[i].LastIndex > _marksCount - maxMarks)
            {
                MarkSection curr = _skidmarks[i];
                MarkSection last = _skidmarks[curr.LastIndex % maxMarks];

                vertices[segmentCount * 4 + 0] = last.Posl;
                vertices[segmentCount * 4 + 1] = last.Posr;
                vertices[segmentCount * 4 + 2] = curr.Posl;
                vertices[segmentCount * 4 + 3] = curr.Posr;

                normals[segmentCount * 4 + 0] = last.Normal;
                normals[segmentCount * 4 + 1] = last.Normal;
                normals[segmentCount * 4 + 2] = curr.Normal;
                normals[segmentCount * 4 + 3] = curr.Normal;

                tangents[segmentCount * 4 + 0] = last.Tangent;
                tangents[segmentCount * 4 + 1] = last.Tangent;
                tangents[segmentCount * 4 + 2] = curr.Tangent;
                tangents[segmentCount * 4 + 3] = curr.Tangent;

                colors[segmentCount * 4 + 0] = new Color(0, 0, 0, last.Intensity);
                colors[segmentCount * 4 + 1] = new Color(0, 0, 0, last.Intensity);
                colors[segmentCount * 4 + 2] = new Color(0, 0, 0, curr.Intensity);
                colors[segmentCount * 4 + 3] = new Color(0, 0, 0, curr.Intensity);

                uvs[segmentCount * 4 + 0] = new Vector2(0, 0);
                uvs[segmentCount * 4 + 1] = new Vector2(1, 0);
                uvs[segmentCount * 4 + 2] = new Vector2(0, 1);
                uvs[segmentCount * 4 + 3] = new Vector2(1, 1);

                triangles[segmentCount * 6 + 0] = segmentCount * 4 + 0;
                triangles[segmentCount * 6 + 2] = segmentCount * 4 + 1;
                triangles[segmentCount * 6 + 1] = segmentCount * 4 + 2;

                triangles[segmentCount * 6 + 3] = segmentCount * 4 + 2;
                triangles[segmentCount * 6 + 5] = segmentCount * 4 + 1;
                triangles[segmentCount * 6 + 4] = segmentCount * 4 + 3;
                segmentCount++;
            }
        }

        mesh.vertices = vertices;
        mesh.normals = normals;
        mesh.tangents = tangents;
        mesh.triangles = triangles;
        mesh.colors = colors;
        mesh.uv = uvs;
    }

    // Variables for each mark created. Needed to generate the correct mesh.
    private class MarkSection
    {
        public Vector3 Position = Vector3.zero;
        public Vector3 Normal = Vector3.zero;
        public Vector4 Tangent = Vector4.zero;
        public Vector3 Posl = Vector3.zero;
        public Vector3 Posr = Vector3.zero;
        public float Intensity = 0.0f;
        public int LastIndex = 0;
    }
}
