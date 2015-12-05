using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

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

    public static byte[] SerializeVoxelData(VoxelData voxelData)
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
            return stream.ToArray();
        }
    }

    // The given data should have been produced by calling SerializeVoxelData()
    public static VoxelData DeserializeVoxelData(byte[] data)
    {
        if (data == null)
        {
            Debug.LogWarning("Attempted to deserialize null binary data into VoxelData");
            return null;
        }

        BinaryFormatter formatter = new BinaryFormatter();
        using (MemoryStream stream = new MemoryStream(data))
        {
            return new VoxelData((Voxel[]) formatter.Deserialize(stream));
        }
    }

    public static VoxelData VoxelDataFromFile(string filepath)
    {
        return DeserializeVoxelData(File.ReadAllBytes(filepath));
    }

    public static void VoxelDataToFile(string filepath, VoxelData data)
    {
        File.WriteAllBytes(filepath, SerializeVoxelData(data));
    }
}
