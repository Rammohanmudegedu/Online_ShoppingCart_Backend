using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Shopping.Utilities.Common
{
    #region ResourceConstants
    public class ResourceConstants
    {
        public static string DecryptionKey { get; set; } = "kxDlNHBcDDSWx0J_WrVXQrlNjw==";
        public static string? ApplicationEnvironment { get; set; }
        public static string? ApplicationName { get; set; }
        public static string? ErrorHandlerEmailToAddress { get; set; }
        public static string? ErrorHandlerEmailSubject { get; set; }
        public static bool LogToDB { get; set; }
        public static bool LogToFile { get; set; }
        public static string? SMTPDeliveryMethod { get; set; }
        public static string? SMTPFromAddress { get; set; }
        public static string? SMTPFromDisplayName { get; set; }
        public static string? SMTPToAddress { get; set; }
        public static string? SMTPHost { get; set; }
        public static int SMTPPort { get; set; }
        public static string? SMTPUserName { get; set; }
        public static string? SMTPDivertEmailToAddress { get; set; }
        public static string? SMTPEncryptedSecret { get; set; }
        public static string? SMTPPassword { get; set; } = "hhanxyowiplcfvtk";
        public static bool SMTPUseDefaultCredentials { get; set; }
        public static int SMTPTimeout { get; set; }
        public static int SMTPRetryCount { get; set; }
        public static string? RandomEncryptionKey { get; internal set; }
        public static bool SMTPDivertEmail { get; set; }
    }
    #endregion ResourceConstants Class


    #region ErrorHandler
    public sealed class ErrorHandler
    {
        public static void Handle(Exception ex, string application, string custommessage = "")
        {
            try
            {
                string mailSubject = Utility.PrepareEmailSubject(ResourceConstants.ErrorHandlerEmailSubject, ResourceConstants.ApplicationEnvironment, application);

                StringBuilder sbBody = new StringBuilder();
                sbBody.AppendLine("Application\t\t:\t" + application);
                sbBody.AppendLine("Time of Error\t\t:\t" + DateTime.Now.ToString());
                sbBody.AppendLine("Message\t\t:\t" + Convert.ToString(ex.Message));
                if (!string.IsNullOrEmpty(custommessage)) sbBody.AppendLine("Custom Message\t\t:\t" + custommessage);
                sbBody.AppendLine("BaseException\t\t:\t" + ex.GetBaseException().Message.ToString());
                sbBody.AppendLine("InnerException\t\t:\t" + Convert.ToString(ex.InnerException != null ? ex.InnerException.Message : ""));
                sbBody.AppendLine("Server Host Name\t:\t" + Dns.GetHostName());
                sbBody.AppendLine("User Name\t\t:\t" + Environment.UserName);
                sbBody.AppendLine("User Domain\t\t:\t" + Environment.UserDomainName);
                sbBody.AppendLine("Source\t\t\t:\t" + Convert.ToString(ex.Source));
                sbBody.AppendLine("TargetSite\t\t:\t" + Convert.ToString(ex.TargetSite));
                sbBody.AppendLine("StackTrace\t\t:\t" + Convert.ToString(ex.StackTrace));

                using MailMessage message = new();
                message.To.Add(ResourceConstants.SMTPDivertEmail ? ResourceConstants.SMTPDivertEmailToAddress : ResourceConstants.ErrorHandlerEmailToAddress);
                message.Subject = mailSubject;
                message.Body = sbBody.ToString();
                message.IsBodyHtml = false;
                message.Priority = MailPriority.Normal;

                SMTPClient.SendEmail(message);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
    #endregion ErrorHandler

    #region SMTPClient
    public class SMTPClient
    {
        //public static void SendEmail(MailMessage mailMessage)
        //{
        //    string decryptedSecret = Security.Decrypt(ResourceConstants.SMTPEncryptedSecret).Trim();
        //SendEmail:
        //    try
        //    {
        //        mailMessage.From = new MailAddress(ResourceConstants.SMTPFromAddress, ResourceConstants.SMTPFromDisplayName);
        //        using SmtpClient smtp = new();
        //        smtp.UseDefaultCredentials = false;
        //        smtp.Credentials = new NetworkCredential() { UserName = ResourceConstants.SMTPUserName, Password = decryptedSecret };
        //        smtp.Host = ResourceConstants.SMTPHost;
        //        smtp.Port = ResourceConstants.SMTPPort;
        //        smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
        //        smtp.EnableSsl = true;
        //        smtp.Timeout = ResourceConstants.SMTPTimeout;

        //        smtp.Send(mailMessage);
        //    }
        //    catch (Exception ex)
        //    {
        //        int counter = 0;
        //        // Retry sending email
        //        while (counter < ResourceConstants.SMTPRetryCount)
        //        {
        //            counter++;
        //            goto SendEmail;
        //        }
        //        ErrorHandler.Handle(ex, ResourceConstants.ApplicationName);
        //    }
        //}

        public static void SendEmail(MailMessage mailMessage)
        {
            int counter = 0;

        SendEmail:
            try
            {
                mailMessage.From = new MailAddress(ResourceConstants.SMTPFromAddress, ResourceConstants.SMTPFromDisplayName);

                using SmtpClient smtp = new();

                smtp.UseDefaultCredentials = false;
                smtp.Credentials = new NetworkCredential(ResourceConstants.SMTPUserName, ResourceConstants.SMTPPassword);

                smtp.Host = ResourceConstants.SMTPHost;
                smtp.Port = ResourceConstants.SMTPPort;
                smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                smtp.EnableSsl = true;
                smtp.Timeout = ResourceConstants.SMTPTimeout;

                smtp.Send(mailMessage);
            }
            catch (Exception ex)
            {
                counter++;

                if (counter < ResourceConstants.SMTPRetryCount)
                {
                    Thread.Sleep(2000); // wait before retry
                    goto SendEmail;
                }

                ErrorHandler.Handle(ex, ResourceConstants.ApplicationName);
                throw;
            }
        }

    }
    #endregion SMTPClient

    #region Security
    public class Security
    {   /// <summary>
        /// Encrypts the specified plain text using AES encryption with the provided key.
        /// </summary>
        public static string Encrypt(string plainText)
        {
            var aes = Aes.Create();
            var sha256 = SHA256.Create();
            aes.Key = sha256.ComputeHash(Encoding.UTF8.GetBytes(ResourceConstants.DecryptionKey));
            aes.GenerateIV();
            var encryptor = aes.CreateEncryptor();
            byte[] plainBytes = Encoding.UTF8.GetBytes(plainText);
            byte[] cipherBytes = encryptor.TransformFinalBlock(plainBytes, 0, plainBytes.Length);
            byte[] result = new byte[aes.IV.Length + cipherBytes.Length];
            Buffer.BlockCopy(aes.IV, 0, result, 0, aes.IV.Length);
            Buffer.BlockCopy(cipherBytes, 0, result, aes.IV.Length, cipherBytes.Length);
            return Convert.ToBase64String(result);
        }

        /// <summary>
        /// Decrypts the specified cipher text using AES decryption with the provided key.
        /// </summary>
        /// <param name="cipherText"></param>
        /// <returns></returns>
        public static string Decrypt(string cipherText)
        {
            byte[] fullCipher = Convert.FromBase64String(cipherText);
            var aes = Aes.Create();
            var sha256 = SHA256.Create();
            aes.Key = sha256.ComputeHash(Encoding.UTF8.GetBytes(ResourceConstants.DecryptionKey));
            byte[] iv = new byte[aes.BlockSize / 8];
            byte[] cipherBytes = new byte[fullCipher.Length - iv.Length];
            Buffer.BlockCopy(fullCipher, 0, iv, 0, iv.Length);
            Buffer.BlockCopy(fullCipher, iv.Length, cipherBytes, 0, cipherBytes.Length);
            aes.IV = iv;
            var decryptor = aes.CreateDecryptor();
            byte[] plainBytes = decryptor.TransformFinalBlock(cipherBytes, 0, cipherBytes.Length);
            return Encoding.UTF8.GetString(plainBytes);
        }

        public static string DeCryptConnectionString(string connectionString)
        {
            var parts = connectionString.Split(';', (char)StringSplitOptions.RemoveEmptyEntries);
            var dict = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            bool passwordFound = false;

            foreach (var part in parts)
            {
                var kv = part.Split(new[] { '=' }, 2);
                if (kv.Length == 2)
                {
                    var key = kv[0].Trim();
                    var value = kv[1].Trim();
                    if (string.Equals(key, "Password", StringComparison.OrdinalIgnoreCase) ||
                        string.Equals(key, "Pwd", StringComparison.OrdinalIgnoreCase))
                    {
                        dict[key] = Decrypt(value);
                        passwordFound = true;
                    }
                    else
                    {
                        dict[key] = value;
                    }
                }
            }

            var sb = new StringBuilder();
            foreach (var kvp in dict)
            {
                sb.Append($"{kvp.Key}={kvp.Value};");
            }
            return sb.ToString();
        }

        /// <summary>
        /// Generates a random alphanumeric key of the specified length, using the provided encryption key as a salt.
        /// </summary>
        /// <param name="length">Length of the key to generate.</param>
        /// <returns>Random alphanumeric key.</returns>
        public static string GenerateRandomKey(int length)
        {
            if (length <= 0) throw new ArgumentOutOfRangeException(nameof(length));
            if (string.IsNullOrEmpty(ResourceConstants.RandomEncryptionKey)) throw new ArgumentNullException(nameof(ResourceConstants.RandomEncryptionKey));

            const string validChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var result = new StringBuilder(length);

            // Use HMACSHA256 with the encryptionKey as the key
            using (var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(ResourceConstants.RandomEncryptionKey)))
            {
                int generated = 0;
                int counter = 0;
                while (generated < length)
                {
                    // Hash the counter to get a pseudo-random block
                    byte[] counterBytes = BitConverter.GetBytes(counter++);
                    byte[] hash = hmac.ComputeHash(counterBytes);

                    for (int i = 0; i < hash.Length && generated < length; i++)
                    {
                        // Map each byte to a valid character
                        int idx = hash[i] % validChars.Length;
                        result.Append(validChars[idx]);
                        generated++;
                    }
                }
            }
            return result.ToString();
        }
    }
    #endregion Security

    #region Utility
    public class Utility
    {
        public static bool ConvertStringToBool(string value)
        {
            return string.Equals(value, "true", StringComparison.OrdinalIgnoreCase);
        }

        public static string PrepareEmailSubject(string sub, string env, string app)
        {
            string mailSubject = string.Empty;
            string divert = ResourceConstants.SMTPDivertEmail ? "DIVERTED - " : "";
            env = string.IsNullOrWhiteSpace(env) ? ResourceConstants.ApplicationEnvironment : env;

            mailSubject += divert;
            mailSubject += (env.Contains("prod", StringComparison.CurrentCultureIgnoreCase) || env.Contains("production", StringComparison.CurrentCultureIgnoreCase) ? "" : " [" + env + "] - " + app + " : ");
            mailSubject += sub;

            return mailSubject;
        }

        //a.XML to JSON
        //b.JSON Serialize/DeSerialize
        //c. Security key generator
        //d. GetConnectionStrings
    }
    #endregion Utility

}
