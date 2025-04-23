using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using Azure.Security.KeyVault.Secrets;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ApiOAuthCubos.Helpers
{
    //ESTA CLASE CIFRA BASANDOSE EN UNAS CARACTERISTICAS 
    public static class HelperCryptography
    {
        private static IConfiguration configuration;
        private static SecretClient secretclient;
        private static string saltconf;
        private static string bucleconf;
        private static string passwd;
        public static void Initialize(IConfiguration config, SecretClient client)
        {
            configuration = config;
            secretclient = client;
        }
        public static void GetSecrets()
        {
            KeyVaultSecret secretSalt = secretclient.GetSecret("Salt");
            saltconf = secretSalt.Value;

            KeyVaultSecret secretIterate = secretclient.GetSecret("Iterate");
            bucleconf = secretIterate.Value;

            KeyVaultSecret secretKey = secretclient.GetSecret("Key");
            passwd = secretKey.Value;
        }

        public static string EncryptString(string dato)
        {
            GetSecrets();

            byte[] saltpassword = EncriptarPasswordSalt
                (passwd, saltconf, int.Parse(bucleconf));
            string res = EncryptString(saltpassword, dato);
            return res;
        }

        public static string DecryptString(string dato)
        {
            GetSecrets();

            byte[] saltpassword = EncriptarPasswordSalt
                (passwd, saltconf, int.Parse(bucleconf));
            string res = DecryptString(saltpassword, dato);
            return res;
        }

        private static string EncryptString(byte[] key, string plainText)
        {
            byte[] iv = new byte[16];
            byte[] array;

            using (Aes aes = Aes.Create())
            {
                aes.Key = key;
                aes.IV = iv;

                ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

                using (MemoryStream memoryStream = new MemoryStream())
                {
                    using (CryptoStream cryptoStream = new CryptoStream((Stream)memoryStream, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter streamWriter = new StreamWriter((Stream)cryptoStream))
                        {
                            streamWriter.Write(plainText);
                        }

                        array = memoryStream.ToArray();
                    }
                }
            }

            return Convert.ToBase64String(array);
        }

        private static byte[] EncriptarPasswordSalt(string contenido
            , string salt, int numhash)
        {
            //REALIZAMOS LA COMBINACION DE ENCRIPTADO
            //CON SU SALT
            string textocompleto = contenido + salt;
            //DECLARAMOS EL OBJETO SHA256
            //SHA256Managed objsha = new SHA256Managed();
            SHA256 objsha = SHA256.Create();
            byte[] bytesalida = null;

            try
            {
                //CONVERTIMOS EL TEXTO A BYTES
                bytesalida =
                    Encoding.UTF8.GetBytes(textocompleto);
                //Convert.FromBase64String(textocompleto);
                //ENCRIPTAMOS EL TEXTO 1000 VECES
                for (int i = 0; i < numhash; i++)
                    bytesalida = objsha.ComputeHash(bytesalida);
            }
            finally
            {
                objsha.Clear();
            }
            //DEVOLVEMOS LOS BYTES DE SALIDA
            return bytesalida;
        }

        private static string DecryptString(byte[] key, string cipherText)
        {
            byte[] iv = new byte[16];
            byte[] buffer = Convert.FromBase64String(cipherText);

            using (Aes aes = Aes.Create())
            {
                aes.Key = key;
                aes.IV = iv;
                ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

                using (MemoryStream memoryStream = new MemoryStream(buffer))
                {
                    using (CryptoStream cryptoStream = new CryptoStream((Stream)memoryStream, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader streamReader = new StreamReader((Stream)cryptoStream))
                        {
                            return streamReader.ReadToEnd();
                        }
                    }
                }
            }
        }
    }
}
