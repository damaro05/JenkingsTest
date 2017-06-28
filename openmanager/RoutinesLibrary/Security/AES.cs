// VBConversions Note: VB project level imports
using System.Collections.Generic;
using System;
using System.Diagnostics;
using System.Collections;
using System.Linq;
// End of VB project level imports

using System.IO;
using System.Security.Cryptography;


    namespace RoutinesLibrary.Security
    {

        public class AES
        {

            public static byte[] EncryptStringToBytes_AES(string plainText, byte[] Key, byte[] IV)
            {

                // Check arguments
                if (ReferenceEquals(plainText, null) || plainText.Length <= 0)
                {
                    throw (new ArgumentNullException("plainText"));
                }

                if (ReferenceEquals(Key, null) || Key.Length <= 0)
                {
                    throw (new ArgumentNullException("Key"));
                }

                if (ReferenceEquals(IV, null) || IV.Length <= 0)
                {
                    throw (new ArgumentNullException("Key"));
                }

                byte[] encrypted = null;

                // Create an Aes object with the specified key and IV
                using (System.Security.Cryptography.Aes aesAlg = System.Security.Cryptography.Aes.Create())
                {
                    aesAlg.Key = Key;
                    aesAlg.IV = IV;

                    // Create a decrytor to perform the stream transform.
                    ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                    // Create the streams used for encryption.
                    using (MemoryStream msEncrypt = new MemoryStream())
                    {
                        using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                        {
                            using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                            {

                                // Write all data to the stream.
                                swEncrypt.Write(plainText);
                            }


                            encrypted = msEncrypt.ToArray();
                        }

                    }

                }


                // Return the encrypted bytes from the memory stream
                return encrypted;
            }

            public static string DecryptStringFromBytes_AES(byte[] cipherText, byte[] Key, byte[] IV)
            {

                // Check arguments
                if (ReferenceEquals(cipherText, null) || cipherText.Length <= 0)
                {
                    throw (new ArgumentNullException("cipherText"));
                }

                if (ReferenceEquals(Key, null) || Key.Length <= 0)
                {
                    throw (new ArgumentNullException("Key"));
                }

                if (ReferenceEquals(IV, null) || IV.Length <= 0)
                {
                    throw (new ArgumentNullException("Key"));
                }

                // Declare the string used to hold the decrypted text
                string plaintext = null;

                // Create an Aes object with the specified key and IV
                using (System.Security.Cryptography.Aes aesAlg = System.Security.Cryptography.Aes.Create())
                {
                    aesAlg.Key = Key;
                    aesAlg.IV = IV;

                    // Create a decrytor to perform the stream transform.
                    ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                    // Create the streams used for decryption.
                    using (MemoryStream msDecrypt = new MemoryStream(cipherText))
                    {
                        using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                        {
                            using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                            {

                                // Read the decrypted bytes from the decrypting stream and place them in a string
                                plaintext = srDecrypt.ReadToEnd();
                            }

                        }

                    }

                }

                return plaintext;
            }
        }
    }

