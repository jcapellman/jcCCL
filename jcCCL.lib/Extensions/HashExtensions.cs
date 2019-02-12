using System;
using System.Security.Cryptography;

namespace jcCCL.lib.Extensions
{
    public static class HashExtensions
    {
        public static string ComputeMD5(this byte[] data) => BitConverter.ToString(MD5.Create().ComputeHash(data));
    }
}