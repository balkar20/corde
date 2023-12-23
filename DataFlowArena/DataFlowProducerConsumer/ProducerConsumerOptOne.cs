namespace DataFlowProducerConsumer;

public class ProducerConsumerOptOne
{
    private static byte[] Compress(byte[] bytes)
    {
        using (var resultStream = new MemoryStream())
        {
            using (var zipStream = new GZipStream(resultStream, CompressionMode.Compress))
            {
                using (var writer = new BinaryWriter(zipStream))
                {
                    writer.Write(bytes);
                    return resultStream.ToArray();
                }
            }
        }
    }
}