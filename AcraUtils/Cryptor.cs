using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace AcraUtils
{
    public class Cryptor
    {
        private readonly string _rsaPrivateKeyString;
        private readonly string _rsaPublicKeyString;

        public Cryptor()
        {
            _rsaPrivateKeyString = $"<?xml version=\"1.0\" encoding=\"utf-16\"?>" +
                                    $"<RSAParameters xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\">" +
                                      $"<D>QLPPYWhvun2QRK1p4ETctcQTZzaUzjUzsiignSUCu83smjX3pGblKLIvXnf2nC/2ALM7X80DL0fzEUN5RXTxVwEf160xIYQLphIV8lsFHuU29T8HoTZiHYnDybinJ7IKqv/1T/VmAhCc9p4hLIWDcaVoGgPd+K+u7eP10t5JLGrP7svjyuzREZgT6d6NhBwcBoil8mcp7woTHVtmTfPtxPN3WgYIP9/dClSFi3TPJNSi9MNOHCIXP3MuAdWdMtXEBDS/bskEguytxiISaOFbLqm3L8T/K7D5PoEFNMvzY4ebeN9EszgVtRhb46PLb1Q4uPCvx9LZh/d9fSDI52C/QQ==</D>" +
                                      $"<DP>sUstun3/hKl/7anzA/IYeK7vb+j34D9E6ySkxHHBtm6qCCIz9wVH+uFSaT7p7peAha7MGWE1czKm5O+I35Ma0MsGhfotaN2a8sg4q0QqUAieI69vra/mhnrHjJ6fwl78sjhXYeuAmqhoyy0n+DcyY5yJ9yApARFbFpjk11R1ngk=</DP>" +
                                      $"<DQ>TE0QFQuTxSi1prmyMRP5M7Y3TCWbumnDD18+yetdgbbLUiZXJkUB/0zDiPpCQdFAviKaVXm5XWgstfb5k8MQ05EcHza5DpthlhoLJRCiQwqDsi0PBt8OFjXlN8lD3DR/F2iOjHZiazI0GKuV33NxaJ0ANZld1wR+poV9uM2Phb0=</DQ>" +
                                      $"<Exponent>AQAB</Exponent>" +
                                      $"<InverseQ>eCBStePFztAom+zrpQ/nkqDsduzMtzlgr+IqeZbzbKeqyTZUmTwN3CQ5o8aiSMXLredbhiADEQC3IE9E2+GNDti9GpiMeSvQ/pHzbbe0Fnxn9Ttexw8h2NHoe+jcUoFXi9xRfdibYGt2de+H5qhA5U+PBfOicEuD19/lzSLMrz4=</InverseQ>" +
                                      $"<Modulus>lDwRTPqol8DzcAFGlpXai10QXuEEpIoe4waFm+1QI199xdp3Qo0EYaXB07apiNHubgOm5LVfay94Mr8kJSQXFqCuVkd+QuD08JrkdVml6rjMkHUZSuMEPb+KwuGQER8cTn1mCadQO8Sj8SLhyx3WExZVulbg4TY17IREJXrohhB88TZNQ/JJ7ZvdcaCXMp/r9BcZFYUjGb7i8yF+YS9KD/2WzWS3dwLdefS/ufdMMxXVMi3If1VRhH3L7XjiCJWTivDqt49Ao5DUAfkMbrv1WXiBDGYFs2qT82tktFtk32SH2AogefgrcjF/1tuzKLYhJmqb1sUbLv2vMPJ10IRIfQ==</Modulus>" +
                                      $"<P>w5nk3ZzqOgln7gLsv7GELnRQr92gSkr7hi85P5bxStY/Lm+2ZfGBypNq/Kg9vIb9CSwOOOZ4MewXHVh2E+bBoKM+IF4ilZSCrzJdAaVBoYWBlcbGbMwINzPJtuBZF1tNZn1jjB6/HtbUGwDfBC5Uih/+BCNArbyS0UNMvbY7txk=</P>" +
                                      $"<Q>wgHkEm48DoudHEAXTvOyaELuu08CNw77eOgTPXio0Rf5+HzrRxsCDxq3OZMJhpCyS4l+qjeMdgxlfNVgdT61DHbTGaSlrkNciEPKAA6xXUKfN+9nzLs1tyhjYKJd1VC7KN37a+bXPpGJvLd9uI++9xM6DyHfckWuK2earPNW/QU=</Q>" +
                                    $"</RSAParameters>";

            _rsaPublicKeyString = $"<?xml version=\"1.0\" encoding=\"utf-16\"?>" +
                                    $"<RSAParameters xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\">" +
                                    $"<Exponent>AQAB</Exponent>" +
                                    $"<Modulus>lDwRTPqol8DzcAFGlpXai10QXuEEpIoe4waFm+1QI199xdp3Qo0EYaXB07apiNHubgOm5LVfay94Mr8kJSQXFqCuVkd+QuD08JrkdVml6rjMkHUZSuMEPb+KwuGQER8cTn1mCadQO8Sj8SLhyx3WExZVulbg4TY17IREJXrohhB88TZNQ/JJ7ZvdcaCXMp/r9BcZFYUjGb7i8yF+YS9KD/2WzWS3dwLdefS/ufdMMxXVMi3If1VRhH3L7XjiCJWTivDqt49Ao5DUAfkMbrv1WXiBDGYFs2qT82tktFtk32SH2AogefgrcjF/1tuzKLYhJmqb1sUbLv2vMPJ10IRIfQ==" +
                                    $"</Modulus>" +
                                    $"</RSAParameters>";
        }

        /// <summary>
        /// Use for encrypting small texts
        /// </summary>
        /// <param name="plainText"></param>
        /// <returns></returns>
        public string EncryptRSA(string plainText)
        {
            var pubKey = GetRsaParametersFromString(_rsaPublicKeyString);

            var csp = new RSACryptoServiceProvider();
            csp.ImportParameters(pubKey);

            var bytesPlainTextData = Encoding.Unicode.GetBytes(plainText);
            var bytesCypherText = csp.Encrypt(bytesPlainTextData, false);
            
            return Convert.ToBase64String(bytesCypherText);
        }

        public string DecryptRSA(string cypherText)
        {
            var privKey = GetRsaParametersFromString(_rsaPrivateKeyString);

            var csp = new RSACryptoServiceProvider();
            csp.ImportParameters(privKey);

            var bytesCypherText = Convert.FromBase64String(cypherText);
            var bytesPlainTextData = csp.Decrypt(bytesCypherText, false);

            return Encoding.Unicode.GetString(bytesPlainTextData);
        }

        private RSAParameters GetRsaParametersFromString(string keyString)
        {
            var sr = new System.IO.StringReader(keyString);
            var xs = new System.Xml.Serialization.XmlSerializer(typeof(RSAParameters));
            return (RSAParameters)xs.Deserialize(sr);
        }

        private byte[] desKey = new byte[8] { 7, 2, 9, 6, 1, 0, 7, 1 };
        private byte[] desIV = new byte[8] { 0, 2, 6, 4, 1, 6, 5, 0 };

        /// <summary>
        /// Use for encrypting large texts
        /// </summary>
        /// <param name="plainText"></param>
        /// <returns></returns>
        public string EncryptDES(string plainText)
        {
            SymmetricAlgorithm algorithm = DES.Create();
            ICryptoTransform transform = algorithm.CreateEncryptor(desKey, desIV);
            byte[] inputbuffer = Encoding.Unicode.GetBytes(plainText);
            byte[] outputBuffer = transform.TransformFinalBlock(inputbuffer, 0, inputbuffer.Length);
            return Convert.ToBase64String(outputBuffer);
        }

        public byte[] EncryptDES(byte[] bytesToEncrypt)
        {
            SymmetricAlgorithm algorithm = DES.Create();
            ICryptoTransform transform = algorithm.CreateEncryptor(desKey, desIV);
            byte[] outputBuffer = transform.TransformFinalBlock(bytesToEncrypt, 0, bytesToEncrypt.Length);
            return outputBuffer;
        }

        public byte[] DecryptDES(byte[] cypherStream)
        {
            SymmetricAlgorithm algorithm = DES.Create();
            ICryptoTransform transform = algorithm.CreateDecryptor(desKey, desIV);           
            byte[] outputBuffer = transform.TransformFinalBlock(cypherStream, 0, cypherStream.Length);
            return outputBuffer;
        }


        public string DecryptDES(string cypherText)
        {
            //System.IO.File.AppendAllText("C:/Logs/log.txt", $"{DateTime.Now} Decrypt Started" + Environment.NewLine);
            SymmetricAlgorithm algorithm = DES.Create();            
            ICryptoTransform transform = algorithm.CreateDecryptor(desKey, desIV);
            byte[] inputbuffer;
            try
            {
                inputbuffer = Convert.FromBase64String(cypherText);
                //System.IO.File.AppendAllText("C:/Logs/log.txt", $"{DateTime.Now} inptbuffer = {inputbuffer}" + Environment.NewLine);
            }
            catch { inputbuffer = DecodeUrlBase64(cypherText); }
            //System.IO.File.AppendAllText("C:/Logs/log.txt", $"{DateTime.Now} inptbuffer = ");
            foreach (var item in inputbuffer)
            {
                ///System.IO.File.AppendAllText("C:/Logs/log.txt", $"{item} ");
            }
            //System.IO.File.AppendAllText("C:/Logs/log.txt",Environment.NewLine);
            byte[] outputBuffer = new byte[] { };
            try
            {
                outputBuffer = transform.TransformFinalBlock(inputbuffer, 0, inputbuffer.Length);
            }
            catch (Exception ex)
            {

                //System.IO.File.AppendAllText("C:/Logs/log.txt", $"{DateTime.Now} Exeption = {ex}" + Environment.NewLine);
            }
            //System.IO.File.AppendAllText("C:/Logs/log.txt", $"{DateTime.Now} outputbuffer = {outputBuffer}" + Environment.NewLine);
            //System.IO.File.AppendAllText("C:/Logs/log.txt", $"{DateTime.Now} return value = {Encoding.Unicode.GetString(outputBuffer)}" + Environment.NewLine);
            return Encoding.Unicode.GetString(outputBuffer);
        }

        public byte[] DecodeUrlBase64(string s)
        {
            s = s.Replace(' ', '+').Replace('_', '/').PadRight(4 * ((s.Length + 3) / 4), '=');
            return Convert.FromBase64String(s);
        }

        public string Base64Encode(string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
        }

        public string Base64Decode(string base64EncodedData)
        {
            var base64EncodedBytes = System.Convert.FromBase64String(base64EncodedData);
            return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
        }
    }
}
