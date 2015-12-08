using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public static class Encoding {

	public static byte[] Serialize(object obj)
	{
		BinaryFormatter formatter = new BinaryFormatter();
		using (MemoryStream stream = new MemoryStream())
		{
			formatter.Serialize(stream, obj);
			return stream.ToArray();
		}
	}

	public static object Deserialize(byte[] bytes)
	{
		BinaryFormatter formatter = new BinaryFormatter();
		using (MemoryStream stream = new MemoryStream(bytes))
		{
			return formatter.Deserialize(stream);
		}
	}

}
