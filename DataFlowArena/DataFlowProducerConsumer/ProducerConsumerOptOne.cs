// using System.Diagnostics;
// using System.IO.Compression;
// using System.Threading.Tasks.Dataflow;
//
// namespace DataFlowProducerConsumer;
//
// public class ProducerConsumerOptOne
// {
//     public ProducerConsumerOptOne()
//     {
//         BufferSize = 16384;
//     }
//     public void MainExecute()
//     {
//         var stopwatch = Stopwatch.StartNew();
//         using (var inputStream = File.OpenRead(@"C:\file.bak"))
//         {
//             using (var outputStream = File.Create(@"E:\file.gz"))
//             {
//                 Compress(inputStream, outputStream);
//             }
//         }
//         stopwatch.Stop();
//
//         Console.WriteLine();
//         Console.WriteLine(string.Format("Time elapsed: {0}s", stopwatch.Elapsed.TotalSeconds));
//         Console.ReadKey();
//     }
//     // public void Execute()
//     // {
//     //     var buffer = new BufferBlock<byte[]>();
//     //     
//     //     var compressor = new TransformBlock<byte[], byte[]>(bytes => Compress(bytes));
//     //     var writer = new ActionBlock<byte[]>(bytes => outputStream.Write(bytes, 0, bytes.Length));
//     //
//     //     buffer.LinkTo(compressor);
//     //     compressor.LinkTo(writer);
//     //         
//     //     while (!buffer.Post(bytes))
//     //     {
//     //     }
//     // }
//     
//     public static void Compress(Stream inputStream, Stream outputStream)
//     {
//         var buffer = new BufferBlock<byte[]>(new DataflowBlockOptions {BoundedCapacity = 100});
//         var compressorOptions = new ExecutionDataflowBlockOptions
//         {
//             MaxDegreeOfParallelism = 4,
//             BoundedCapacity = 100
//         };
//         var compressor = new TransformBlock<byte[], byte[]>(bytes => Compress(bytes), compressorOptions);
//         var writerOptions = new ExecutionDataflowBlockOptions
//         {
//             BoundedCapacity = 100,
//             SingleProducerConstrained = true
//         };
//         var writer = new ActionBlock<byte[]>(bytes => outputStream.Write(bytes, 0, bytes.Length), writerOptions);
//
//         buffer.LinkTo(compressor);
//         buffer.Completion.ContinueWith(task => compressor.Complete());
//         compressor.LinkTo(writer);
//         compressor.Completion.ContinueWith(task => writer.Complete());
//
//         var readBuffer = new byte[BufferSize];
//         while (true)
//         {
//             int readCount = inputStream.Read(readBuffer, 0, BufferSize);
//             if (readCount > 0)
//             {
//                 var postData = new byte[readCount];
//                 Buffer.BlockCopy(readBuffer, 0, postData, 0, readCount);
//                 while (!buffer.Post(postData))
//                 {
//                 }
//             }
//             if (readCount != BufferSize)
//             {
//                 buffer.Complete();
//                 break;
//             }
//         }
//
//         writer.Completion.Wait();
//     }
//
//     public static int BufferSize { get; set; }
// }