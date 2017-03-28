using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Venom.Lib.Security
{
    public class CryptographyHelper
    {
        private static Rijndael CreateRijndaelInstance(string key, string vectorInitialization)
        {
            if (!(key != null && (key.Length == 16 || key.Length == 24 || key.Length == 32)))
            {
                throw new Exception("The cryptography must have 16, 24 ou 32 characteres.");
            }
            if (vectorInitialization == null || vectorInitialization.Length != 16)
            {
                throw new Exception("The vector initialization must have 16 characteres.");
            }
            Rijndael algorthim = Rijndael.Create();
            algorthim.Key = Encoding.ASCII.GetBytes(key);
            algorthim.IV = Encoding.ASCII.GetBytes(vectorInitialization);
            return algorthim;
        }

        public static string Encrypt(string key, string vectorInitialization, string text)
        {
            if (String.IsNullOrWhiteSpace(text))
            {
                throw new Exception("The content could be encrypted because string is empty.");
            } using (Rijndael algoritmo = CreateRijndaelInstance(key, vectorInitialization))
            {
                ICryptoTransform encryptor = algoritmo.CreateEncryptor(algoritmo.Key, algoritmo.IV);
                using (MemoryStream streamResultado = new MemoryStream())
                {
                    using (CryptoStream csStream = new CryptoStream(streamResultado, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter writer = new StreamWriter(csStream)) { writer.Write(text); }
                    }
                    return ArrayBytesToHexString(streamResultado.ToArray());
                }
            }
        }

        private static string ArrayBytesToHexString(byte[] conteudo)
        {
            string[] arrayHex = Array.ConvertAll(conteudo, b => b.ToString("X2")); return string.Concat(arrayHex);
        }

        public static string Decrypt(string key, string vectorInitialization, string text)
        {
            if (String.IsNullOrWhiteSpace(text)) { throw new Exception("The content could be decrypted because string is empty."); } if (text.Length % 2 != 0) { throw new Exception("O conteúdo a ser decriptado é inválido."); } using (Rijndael algoritmo = CreateRijndaelInstance(key, vectorInitialization))
            {
                ICryptoTransform decryptor = algoritmo.CreateDecryptor(algoritmo.Key, algoritmo.IV);
                string textDecrypted = null;
                using (MemoryStream streamTextoEncriptado = new MemoryStream(HexStringToArrayBytes(text)))
                {
                    using (CryptoStream csStream = new CryptoStream(streamTextoEncriptado, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader reader = new StreamReader(csStream))
                        {
                            textDecrypted = reader.ReadToEnd();
                        }
                    }
                }
                return textDecrypted;
            }
        }

        private static byte[] HexStringToArrayBytes(string conteudo)
        {
            int qtd = conteudo.Length / 2;
            byte[] array = new byte[qtd]; 
            for (int i = 0; i < qtd; i++)
            {
                array[i] = Convert.ToByte(conteudo.Substring(i * 2, 2), 16);
            }
            return array;
        }
    }
}
