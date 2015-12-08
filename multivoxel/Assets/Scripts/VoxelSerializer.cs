using UnityEngine;
using System;
using System.IO;
using System.Text;
using System.Runtime.Serialization.Formatters.Binary;

[Serializable]
public class SerializedVoxelData {
	public readonly byte[] bytes;
	public SerializedVoxelData(byte[] bytes) {
		this.bytes = bytes;
	}
}

public static class VoxelSerializer
{
    // TODO: Cache all the binary formatters

    public static byte[] SeralizeVoxel(Voxel voxel)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        using (MemoryStream stream = new MemoryStream())
        {
            formatter.Serialize(stream, voxel);
            return stream.ToArray();
        }
    }

    // The given data should have been produced by calling SerializeVoxel()
    public static Voxel DeserializeVoxel(byte[] data)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        using (MemoryStream stream = new MemoryStream(data))
        {
            return (Voxel) formatter.Deserialize(stream);
        }
    }

	public static SerializedVoxelData SerializeVoxelData(VoxelData voxelData)
    {
        if (voxelData == null)
        {
            Debug.LogWarning("Attempted to serialize null VoxelData");
            return null;
        }

        BinaryFormatter formatter = new BinaryFormatter();
        using (MemoryStream stream = new MemoryStream())
        {
            formatter.Serialize(stream, voxelData.Voxels);
            return new SerializedVoxelData(stream.ToArray());
        }
    }

    // The given data should have been produced by calling SerializeVoxelData()
    public static VoxelData DeserializeVoxelData(SerializedVoxelData data)
    {
        if (data == null)
        {
            Debug.LogWarning("Attempted to deserialize null binary data into VoxelData");
            return null;
        }

        BinaryFormatter formatter = new BinaryFormatter();
        using (MemoryStream stream = new MemoryStream(data.bytes))
        {
            return new VoxelData((Voxel[]) formatter.Deserialize(stream));
        }
    }

    public static VoxelData VoxelDataFromFile(string filepath)
    {
        return DeserializeVoxelData(new SerializedVoxelData(File.ReadAllBytes(filepath)));
    }

    public static void VoxelDataToFile(string filepath, VoxelData data)
    {
        File.WriteAllBytes(filepath, SerializeVoxelData(data).bytes);
    }

    public static void VoxelMeshToObjFile(string filepath, MeshFilter mf)
    {
        // based on http://wiki.unity3d.com/index.php?title=ObjExporter
        using (StreamWriter writer = new StreamWriter(filepath))
        {
            Mesh m = mf.mesh;
            Material[] mats = mf.GetComponent<Renderer>().sharedMaterials;

            writer.Write("g ");
            writer.WriteLine(mf.name);
            foreach (Vector3 v in m.vertices)
            {
                writer.WriteLine("v {0} {1} {2}", v.x, v.y, v.z);
            }
            writer.WriteLine();
            foreach (Vector3 v in m.normals)
            {
                writer.WriteLine("vn {0} {1} {2}", v.x, v.y, v.z);
            }
            writer.WriteLine();
            foreach (Vector3 v in m.uv)
            {
                writer.WriteLine("vt {0} {1}", v.x, v.y);
            }
            for (int material = 0; material < m.subMeshCount; material++)
            {
                writer.WriteLine();
                writer.Write("usemtl ");
                writer.WriteLine(mats[material].name);
                writer.Write("usemap ");
                writer.WriteLine(mats[material].name);

                int[] triangles = m.GetTriangles(material);
                for (int i = 0; i < triangles.Length; i += 3)
                {
                    writer.WriteLine("f {0}/{0}/{0} {1}/{1}/{1} {2}/{2}/{2}",
                        triangles[i] + 1, triangles[i + 1] + 1, triangles[i + 2] + 1);
                }
            }
        }
    }
}
