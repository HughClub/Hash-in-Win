using System;
using System.IO;
using System.Security.Cryptography;

namespace Hash
{
    public class Encrypto {
        public static FileStream GetFileStream(string file) => new FileStream(file, FileMode.Open, FileAccess.Read);
        #region Byte Base
        /// <summary>
        /// you should keep it smaller than 127 and can be a visible ASCII char
        /// </summary>
        public static char ByteToChar(byte b) => (char)(b >= 10 ? b - 10 + 'a' : b + '0');
        public static string ByteToString(byte b) =>
            new string(new char[2] { ByteToChar((byte)((b & 0xf0) >> 4)), ByteToChar((byte)(b & 0x0f)) });
        public static string ByteToHex(ref byte[] bytes) {
            System.Text.StringBuilder builder = new System.Text.StringBuilder();
            foreach (byte B in bytes) {
                builder.Append(ByteToString(B));
            }
            return builder.ToString();
        }
        #endregion
        #region Hash
        public static bool HashBase(ref string[] files, HashAlgorithmName algo) {
            byte[] content;
            HashAlgorithm hash = HashAlgorithm.Create(algo.Name);
            foreach (string file in files) {
                if (!File.Exists(file)) {
                    Console.Error.WriteLine($"file : {file} doesn't exist");
                    return false;
                }
                content = hash.ComputeHash(GetFileStream(file));
                Console.WriteLine(ByteToHex(ref content));
            }
            return true;
        }
        public static bool HashBase(ref string[] files, HashAlgorithmName algo,ref string[]results,bool print) {
            byte[] content;
            HashAlgorithm hash = HashAlgorithm.Create(algo.Name);
            results = new string[files.Length];
            int idx = 0;
            foreach(string file in files) {
                if (!File.Exists(file)) {
                    Console.Error.WriteLine($"file : {file} doesn't exist");
                    return false;
                }
                content = hash.ComputeHash(GetFileStream(file));
                results[idx++]=ByteToHex(ref content);
                if (print) {
                    Console.WriteLine(results[idx - 1]);
                }
            }return true;
        }
        public static bool md5(string[] files) {
            return HashBase(ref files,HashAlgorithmName.MD5);
        }
        public static bool sha1(string[] files) {
            return HashBase(ref files, HashAlgorithmName.SHA1);
        }
        public static bool sha256(string[] files) {
            return HashBase(ref files, HashAlgorithmName.SHA256);
        }
        public static bool sha384(string[] files) {
            return HashBase(ref files, HashAlgorithmName.SHA384);
        }
        public static bool sha512(string[] files) {
            return HashBase(ref files, HashAlgorithmName.SHA512);
        }
        #endregion
        #region Symmetric

        #endregion
        #region Usage
        public const string hash_diff_usage ="hash diff file1 file2";
        public const string hash_crypto_usage = "hash __hash_alg files";
        public static readonly string Usage =
            $"Usage : hash COMMAND Params\n\t{hash_crypto_usage}\t\n{hash_diff_usage}\n\t{SupportedAlgorithms}";
        public static readonly string[] Algorithms = { "md5","sha1", "sha256","sha384", "sha512"};
        public static readonly string SupportedAlgorithms = "Supported hash Algs:" + String.Join(',', Algorithms);
        #endregion
        [Obsolete]
        enum Algo {
            NotSup,MD5,SHA,SHA1,SHA256,SHA384,SHA512
        }
        public static bool ArgsCheck(string[] args) {
            if (args.Length < 2) {
                Console.Error.WriteLine(Usage);
                return false;
            } else {
                string cmd = args[0].ToLower();
                if (cmd == "diff") {
                    if (args.Length != 3) {
                        Console.Error.WriteLine(Usage);
                        return false;
                    }
                } else {
                    foreach (string alg in Algorithms) {
                        if (cmd == alg) {
                            return true;
                        }
                    }
                    Console.Error.WriteLine($"{cmd} is no supported\n {SupportedAlgorithms}");
                    return false;
                }
            }
            return true;
        }
        #region Output to File
        public static bool WriteToFile(string file,byte[] content) {
            try{
                File.WriteAllBytes(file, content);
            }catch (IOException){ return false; }
            return true;
        }
        public static bool WriteToFile(string file,string content) {
            try {
                File.WriteAllText(file, content);
            } catch (IOException) { return false; }
            return true;
        }
        #endregion
        public void Parse(string[] args,bool show=true) {
            if (!ArgsCheck(args)) {return;}
            string[] files = new string[args.Length-1];
            Array.Copy(args, 1, files, 0, files.Length);
            Hash(ref files, args[0].ToLower(),show);
        }
        public void Hash(ref string[] files,string type,bool show) {
            switch (type) {
                case "md5":
                    HashBase(ref files, HashAlgorithmName.MD5,ref results,show); break;
                case "sha":
                case "sha1":
                    HashBase(ref files, HashAlgorithmName.SHA1, ref results, show); break;
                case "sha256":
                    HashBase(ref files, HashAlgorithmName.SHA256, ref results, show); break;
                case "sha384":
                    HashBase(ref files, HashAlgorithmName.SHA384, ref results, show); break;
                case "sha512":
                    HashBase(ref files, HashAlgorithmName.SHA512, ref results, show); break;
                default:
                    Console.Error.WriteLine(Usage);
                    break;
            }
        }
        public Encrypto() {}
        string[] results=null;
        public string[] Results { get { return results; } }
    }
    public class Program
    {
        [Obsolete]
        public static string FileGen(string rawfile, bool encrypo) => rawfile + (encrypo ? ".ecy":".dcy");
        public static String Ascii(byte[] bytes) => System.Text.Encoding.ASCII.GetString(bytes);
        public static String Utf8(byte[] bytes) => System.Text.Encoding.UTF8.GetString(bytes);

        static void Main(string[] args){
            new Encrypto().Parse(args);
        }
    }
}