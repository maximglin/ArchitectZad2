using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Grpc.Core;

namespace GrpcClient
{
    static public class GrpcExtensions
    {
        public static async IAsyncEnumerable<T> ToAsyncEnumerable<T>(this IAsyncStreamReader<T> stream, CancellationToken cancellationToken = default)
        {
            if (stream == null)
            {
                throw new System.ArgumentNullException(nameof(stream));
            }

            while (await stream.MoveNext(cancellationToken))
            {
                yield return stream.Current;
            }
        }
    }
}
