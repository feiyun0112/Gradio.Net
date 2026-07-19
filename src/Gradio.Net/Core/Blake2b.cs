using System.Collections.Concurrent;
using System.Net.Http.Headers;
using System.Reflection;
using System.Security.Cryptography;
using System.Globalization;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Primitives;

namespace Gradio.Net.Core;


internal class Blake2b : HashAlgorithm
{
    private readonly Org.BouncyCastle.Crypto.Digests.Blake2bDigest _digest;

    public Blake2b(int hashSizeBits)
    {
        _digest = new Org.BouncyCastle.Crypto.Digests.Blake2bDigest(hashSizeBits);
        HashSizeValue = hashSizeBits;
    }

    public override void Initialize()
    {
        _digest.Reset();
    }

    protected override void HashCore(byte[] array, int ibStart, int cbSize)
    {
        _digest.BlockUpdate(array, ibStart, cbSize);
    }

    protected override byte[] HashFinal()
    {
        var result = new byte[HashSizeValue / 8];
        _digest.DoFinal(result, 0);
        return result;
    }
}
