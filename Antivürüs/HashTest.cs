using System;
using System.IO;
using System.Security.Cryptography;

class Program
{
    static void Main()
    {
        var filePath = @"data\custom_test_virus.txt";
        
        if (!File.Exists(filePath))
        {
            Console.WriteLine($"File not found: {filePath}");
            return;
        }
        
        using var md5 = MD5.Create();
        using var sha256 = SHA256.Create();
        
        using var stream1 = File.OpenRead(filePath);
        var md5Hash = Convert.ToHexString(md5.ComputeHash(stream1)).ToLowerInvariant();
        
        using var stream2 = File.OpenRead(filePath);
        var sha256Hash = Convert.ToHexString(sha256.ComputeHash(stream2)).ToLowerInvariant();
        
        Console.WriteLine($"File: {filePath}");
        Console.WriteLine($"MD5: {md5Hash}");
        Console.WriteLine($"SHA256: {sha256Hash}");
        Console.WriteLine($"MD5 Length: {md5Hash.Length}");
    }
}
