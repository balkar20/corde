using System.Threading.Tasks.Dataflow;

namespace DataFlowProducerConsumer;

public class ProducerConsumerArena
{
    public static void Produce(ITargetBlock<byte[]> target)
    {
        var rand = new Random();

        for (int i = 0; i < 100; ++ i)
        {
            var buffer = new byte[1024];
            rand.NextBytes(buffer);
            target.Post(buffer);
        }

        target.Complete();
    }

    public static async Task<int> ConsumeAsync(ISourceBlock<byte[]> source)
    {
        int bytesProcessed = 0;

        while (await source.OutputAvailableAsync())
        {
            byte[] data = await source.ReceiveAsync();
            bytesProcessed += data.Length;
        }

        return bytesProcessed;
    }

}