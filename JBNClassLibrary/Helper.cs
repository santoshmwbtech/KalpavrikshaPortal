using JBNClassLibrary;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Web.Script.Serialization;

namespace JBNWebAPI.Logger
{
    public class Helper
    {
        static AppSettingsReader mConfigReader = new AppSettingsReader();
        static string ErrorMailBody = string.Empty;
        static System.Timers.Timer mSetTimer = new System.Timers.Timer();
        public static System.Configuration.AppSettingsReader ConfigReader
        {
            get { return mConfigReader; }
            set { mConfigReader = value; }
        }

        public static string GetSystemFilePath()
        {
            return System.IO.Path.Combine(System.Reflection.Assembly.GetExecutingAssembly().Location, System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory));
        }
        public static void LogError(string Message, string Source, Exception InnerException, string StackTrace, bool IsLog = false)
        {
            try
            {
                if (IsLog == false)
                {
                    CreateFolder(System.IO.Path.Combine(GetSystemFilePath() + @"ErrorLog"));
                    string excelFile = System.IO.Path.Combine(GetSystemFilePath() + @"ErrorLog" + @"\" + "ErrorLog" + "_" + DateTime.Now.ToString("dd_MM_yyyy") + ".txt");

                    string fileName = excelFile;
                    StringBuilder sb = new StringBuilder();
                    //if (System.IO.File.Exists(fileName))
                    //{
                    //    sb.Append(System.IO.File.ReadAllText(fileName));
                    //}
                    sb.AppendLine("*******************Start**************");
                    sb.AppendLine("Date & Time : " + DateTime.Now.ToString());
                    sb.AppendLine("Message : " + Message);
                    sb.AppendLine("Source : " + Source);
                    if (InnerException != null)
                        sb.AppendLine("InnerException : " + InnerException.Message);
                    sb.AppendLine("StackTrace : " + StackTrace);
                    sb.AppendLine("*******************End**************");
                    //System.IO.File.WriteAllText(fileName, sb.ToString());
                    //FileWriter fileWriter = new FileWriter();
                    //fileWriter.WriteData(sb.ToString(), excelFile);

                    using (FileStream fs = new FileStream(excelFile, FileMode.OpenOrCreate, FileAccess.Write, FileShare.Read))
                    {
                        BinaryWriter br = new BinaryWriter(fs);
                        br.Write(sb.ToString());
                    }

                    ErrorMailBody = ErrorMailBody + "/n" + sb.ToString();
                }
                else
                {
                    if (ConfigReader.GetValue("IsLogGenerate", typeof(string)).ToString().Trim().Equals("true"))
                    {
                        string logFile = System.IO.Path.Combine(GetSystemFilePath() + @"ErrorLog" + @"\" + "ErrorLog" + "_" + DateTime.Now.ToString("dd_MM_yyyy") + ".txt");

                        StringBuilder sb = new StringBuilder();
                        if (System.IO.File.Exists(logFile))
                        {
                            sb.Append(System.IO.File.ReadAllText(logFile));
                        }
                        sb.AppendLine("Date & Time : " + DateTime.Now.ToString() + " [ " + Message + "]");
                        //System.IO.File.WriteAllText(logFile, sb.ToString());
                        FileWriter fileWriter = new FileWriter();
                        fileWriter.WriteData(sb.ToString(), logFile);
                    }
                }
            }
            catch (OutOfMemoryException outofMemory)
            {
                string message = outofMemory.Message;
            }
            catch (Exception ex)
            {
                string message = ex.Message;
            }
        }

        public static void TransactionLog(string Message, int TransactionType, bool IsTransactionLog = true)
        {
            if (IsTransactionLog)
            {
                try
                {
                    CreateFolder(System.IO.Path.Combine(GetSystemFilePath() + @"TransactionLog"));
                    string logFile = System.IO.Path.Combine(GetSystemFilePath() + @"TransactionLog" + @"\" + "TransactionLog" + "_" + DateTime.Now.ToString("dd_MM_yyyy") + ".txt");
                    StringBuilder sb = new StringBuilder();

                    using (FileStream fs = new FileStream(logFile, FileMode.OpenOrCreate, FileAccess.Read, FileShare.Read))
                    {
                        StreamReader sr = new StreamReader(fs);
                        sb.Append(sr.ReadToEnd());
                        sr.Close();
                        //BinaryWriter br = new BinaryWriter(fs);
                        //br.Write(sb.ToString());
                    }

                    if (TransactionType == 1)
                        sb.AppendLine("Date & Time : " + DateTime.Now.ToString() + "[Request]" + " [ " + Message + "]");
                    else
                        sb.AppendLine("Date & Time : " + DateTime.Now.ToString() + "[Response]" + " [ " + Message + "]");

                    //if (System.IO.File.Exists(logFile))
                    //{
                    //    sb.Append(System.IO.File.ReadAllText(logFile));
                    //}

                    FileWriter fileWriter = new FileWriter();
                    fileWriter.WriteData(sb.ToString(), logFile);
                }
                catch (Exception ex)
                {
                    LogError(ex.Message, null, ex.InnerException, null);
                }


                //System.IO.File.WriteAllText(logFile, sb.ToString());
            }
        }

        public class FileWriter
        {
            private static ReaderWriterLockSlim lock_ = new ReaderWriterLockSlim();
            public void WriteData(string dataWh, string filePath)
            {
                lock_.EnterWriteLock();
                try
                {
                    using (var fs = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.ReadWrite))
                    {
                        byte[] dataAsByteArray = new UTF8Encoding(true).GetBytes(dataWh);
                        fs.Write(dataAsByteArray, 0, dataWh.Length);
                    }
                }
                finally
                {
                    lock_.ExitWriteLock();
                }
            }
        }
        public static void CreateFolder(string DirPath)
        {
            if (!System.IO.Directory.Exists(DirPath))
            {
                System.IO.Directory.CreateDirectory(DirPath);
            }
        }
        public static void SendMail(string ToEmailID, string CCEmailID, string FromMailID, string BodyMessage, string MailSubject, string MailServerHOST, string SendingPort, string MailPassword, AttachmentCollection MailAttachment)
        {

            MailMessage mail = new MailMessage();
            mail.To.Add(ToEmailID);

            if (!string.IsNullOrEmpty(CCEmailID))
            {
                mail.CC.Add(CCEmailID);
            }

            mail.From = new MailAddress(FromMailID, "Kalpavriksha Group");
            mail.Subject = MailSubject;
            mail.Body = BodyMessage;
            mail.IsBodyHtml = true;

            if (MailAttachment != null)
            {
                if (MailAttachment.Count > 0)
                {
                    if (MailAttachment != null)
                    {
                        foreach (Attachment attach in MailAttachment)
                        {
                            mail.Attachments.Add(attach);
                        }
                    }
                }
            }
            mail.Priority = MailPriority.Normal;
            SmtpClient smtp = new SmtpClient();
            smtp.Host = MailServerHOST;
            smtp.Port = Convert.ToInt16(SendingPort);
            smtp.EnableSsl = true;
            smtp.Credentials = new System.Net.NetworkCredential(FromMailID, MailPassword);
            smtp.Send(mail);
            smtp.Dispose();
            mail.Dispose();
        }

        public static void SendMail(string ToEmailID, string CCEmailID, string FromMailID, string BodyMessage, string MailSubject, string MailServerHOST, string SendingPort, string MailPassword, List<Attachment> MailAttachment)
        {

            MailMessage mail = new MailMessage();
            mail.To.Add(ToEmailID);

            if (!string.IsNullOrEmpty(CCEmailID))
            {
                mail.CC.Add(CCEmailID);
            }

            mail.From = new MailAddress(FromMailID, "Kalpavriksha Group");
            mail.Subject = MailSubject;
            mail.Body = BodyMessage;
            mail.IsBodyHtml = true;

            //if (MailAttachment != null)
            //    mail.Attachments.Add(MailAttachment);

            if (MailAttachment != null)
            {
                if (MailAttachment.Count > 0)
                {
                    if (MailAttachment != null)
                    {
                        foreach (Attachment attach in MailAttachment)
                        {
                            mail.Attachments.Add(attach);
                        }
                    }
                }
            }
            mail.Priority = MailPriority.Normal;
            SmtpClient smtp = new SmtpClient();
            smtp.Host = MailServerHOST;
            smtp.Port = Convert.ToInt16(SendingPort);
            smtp.EnableSsl = true;
            smtp.Credentials = new System.Net.NetworkCredential(FromMailID, MailPassword);
            smtp.Send(mail);
            smtp.Dispose();
            mail.Dispose();
        }

        public static void SendMail(string ToEmailID, string FromMailID, string BodyMessage, string MailSubject, string MailServerHOST, string MailPassword, string SendingPort, List<CustomerDetails> bccList = null, List<Attachment> MailAttachment = null)
        {
            try
            {
                System.Net.Mail.MailMessage mail = new System.Net.Mail.MailMessage();
                System.Net.Mail.SmtpClient SmtpServer = new System.Net.Mail.SmtpClient(MailServerHOST);

                if (bccList != null)
                {
                    foreach (var item in bccList)
                    {
                        mail.Bcc.Add(item.EmailID);
                    }
                }

                if (MailAttachment != null)
                {
                    if (MailAttachment.Count > 0)
                    {
                        if (MailAttachment != null)
                        {
                            foreach (Attachment attach in MailAttachment)
                            {
                                mail.Attachments.Add(attach);
                            }
                        }
                    }
                }

                mail.From = new System.Net.Mail.MailAddress(FromMailID, "Kalpavriksha Group");
                mail.To.Add(ToEmailID);
                mail.Subject = MailSubject;
                mail.Body = BodyMessage;
                mail.IsBodyHtml = true;

                SmtpServer.Port = Convert.ToInt16(SendingPort);
                SmtpServer.Credentials = new System.Net.NetworkCredential(FromMailID, MailPassword);
                SmtpServer.EnableSsl = false;
                SmtpServer.Send(mail);
                SmtpServer.Dispose();
                mail.Dispose();

            }
            catch (System.Exception ex)
            {
                throw ex;
                //LogError(ex.Message, ex.Source, ex.StackTrace);
            }
        }

        public static string GenerateOTP()
        {
#pragma warning disable CS0219 // The variable 'alphabets' is assigned but its value is never used
            string alphabets = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
#pragma warning restore CS0219 // The variable 'alphabets' is assigned but its value is never used
#pragma warning disable CS0219 // The variable 'small_alphabets' is assigned but its value is never used
            string small_alphabets = "abcdefghijklmnopqrstuvwxyz";
#pragma warning restore CS0219 // The variable 'small_alphabets' is assigned but its value is never used
            string numbers = "1234567890";
            string characters = numbers;
            //if (string.IsNullOrEmpty(UserId))
            //{
            //    characters += alphabets + small_alphabets + numbers;
            //}

            int length = 6;// int.Parse(UserId);
            string otp = string.Empty;
            for (int i = 0; i < length; i++)
            {
                string character = string.Empty;
                do
                {
                    int index = new Random().Next(0, characters.Length);
                    character = characters.ToCharArray()[index].ToString();
                } while (otp.IndexOf(character) != -1);
                otp += character;
            }
            return otp;
        }
        public static string Encrypt(string input, string key)
        {
            byte[] inputArray = UTF8Encoding.UTF8.GetBytes(input);
            TripleDESCryptoServiceProvider tripleDES = new TripleDESCryptoServiceProvider();
            tripleDES.Key = UTF8Encoding.UTF8.GetBytes(key);
            tripleDES.Mode = CipherMode.ECB;
            tripleDES.Padding = PaddingMode.PKCS7;
            ICryptoTransform cTransform = tripleDES.CreateEncryptor();
            byte[] resultArray = cTransform.TransformFinalBlock(inputArray, 0, inputArray.Length);
            tripleDES.Clear();
            return Convert.ToBase64String(resultArray, 0, resultArray.Length);
        }


        /// <summary>
        /// Decrypts the given string using TripleDES algorithm
        /// </summary>
        /// <param name="input"></param>
        /// <param name="key"></param>
        /// <returns>returns the the Decrypted text</returns>
        public static string Decrypt(string input, string key)
        {
            //input = "hrYETT0+6PxhiiQ6wsj64A==";
            byte[] inputArray = Convert.FromBase64String(input);
            TripleDESCryptoServiceProvider tripleDES = new TripleDESCryptoServiceProvider();
            tripleDES.Key = UTF8Encoding.UTF8.GetBytes(key);
            tripleDES.Mode = CipherMode.ECB;
            tripleDES.Padding = PaddingMode.PKCS7;
            ICryptoTransform cTransform = tripleDES.CreateDecryptor();
            byte[] resultArray = cTransform.TransformFinalBlock(inputArray, 0, inputArray.Length);
            tripleDES.Clear();
            return UTF8Encoding.UTF8.GetString(resultArray);
        }
        public static string SendSMS(string User, string password, string Mobile_Number, string Message,
            [System.Runtime.InteropServices.OptionalAttribute, System.Runtime.InteropServices.DefaultParameterValueAttribute("N")]  // ERROR: Optional parameters aren't supported in C#
            string MType)
        {
            string stringpost = "User=" + User + "&passwd=" + password + "&mobilenumber=" + Mobile_Number + "&message=" + Message + "&MTYPE=" + MType;
            // LogError(stringpost, "", "");
            System.Net.HttpWebRequest objWebRequest = null;
            System.Net.HttpWebResponse objWebResponse = null;
            System.IO.StreamWriter objStreamWriter = null;
            System.IO.StreamReader objStreamReader = null;
            try
            {
                string stringResult = null;
                objWebRequest = (System.Net.HttpWebRequest)System.Net.WebRequest.Create("http://info.bulksms-service.com/WebserviceSMS.aspx");

                objWebRequest.Method = "POST";
                objWebRequest.ContentType = "application/x-www-form-urlencoded";
                objStreamWriter = new System.IO.StreamWriter(objWebRequest.GetRequestStream());
                objStreamWriter.Write(stringpost);
                objStreamWriter.Flush();
                objStreamWriter.Close();
                objWebResponse = (System.Net.HttpWebResponse)objWebRequest.GetResponse();
                objWebResponse = (System.Net.HttpWebResponse)objWebRequest.GetResponse();
                objStreamReader = new System.IO.StreamReader(objWebResponse.GetResponseStream());
                stringResult = objStreamReader.ReadToEnd();
                objStreamReader.Close();
                return (stringResult);
            }
            catch (Exception ex)
            {
                LogError(ex.Message, ex.Source, ex.InnerException, ex.StackTrace);
                //logger(ex.Message);
                return (ex.ToString());
            }
            finally
            {

                if ((objStreamWriter != null))
                {
                    objStreamWriter.Close();
                }
                if ((objStreamReader != null))
                {
                    objStreamReader.Close();
                }
                objWebRequest = null;
                objWebResponse = null;
            }
        }

        public static string SendMessage(string BaseURL, string APIKey, string Mobile_Number, string Message)
        {
            //string stringpost = "User=" + User + "&passwd=" + password + "&mobilenumber=" + Mobile_Number + "&message=" + Message + "&MTYPE=" + MType;
            // LogError(stringpost, "", "");
            System.Net.HttpWebRequest objWebRequest = null;
            System.Net.HttpWebResponse objWebResponse = null;
            System.IO.StreamWriter objStreamWriter = null;
            System.IO.StreamReader objStreamReader = null;
            string URL = BaseURL + APIKey + "&method=sms&message=" + Message + "&to=" + Mobile_Number + "&sender=MWBTEC";
            try
            {
                string stringResult = null;
                objWebRequest = (System.Net.HttpWebRequest)System.Net.WebRequest.Create(URL);

                objWebRequest.Method = "POST";
                objWebRequest.ContentType = "application/x-www-form-urlencoded";
                objStreamWriter = new System.IO.StreamWriter(objWebRequest.GetRequestStream());
                objStreamWriter.Write(Message);
                objStreamWriter.Flush();
                objStreamWriter.Close();
                objWebResponse = (System.Net.HttpWebResponse)objWebRequest.GetResponse();
                objWebResponse = (System.Net.HttpWebResponse)objWebRequest.GetResponse();
                objStreamReader = new System.IO.StreamReader(objWebResponse.GetResponseStream());
                stringResult = objStreamReader.ReadToEnd();
                objStreamReader.Close();
                return (stringResult);
            }
            catch (Exception ex)
            {
                LogError(ex.Message, ex.Source, ex.InnerException, ex.StackTrace);
                //logger(ex.Message);
                return (ex.ToString());
            }
            finally
            {

                if ((objStreamWriter != null))
                {
                    objStreamWriter.Close();
                }
                if ((objStreamReader != null))
                {
                    objStreamReader.Close();
                }
                objWebRequest = null;
                objWebResponse = null;
            }
        }
        public static string SendPromoMessage(string BaseURL, string APIKey, string Mobile_Number, string Message, string SenderID)
        {
            //string stringpost = "User=" + User + "&passwd=" + password + "&mobilenumber=" + Mobile_Number + "&message=" + Message + "&MTYPE=" + MType;
            // LogError(stringpost, "", "");
            System.Net.HttpWebRequest objWebRequest = null;
            System.Net.HttpWebResponse objWebResponse = null;
            System.IO.StreamWriter objStreamWriter = null;
            System.IO.StreamReader objStreamReader = null;
            string URL = BaseURL + APIKey + "&method=sms&message=" + Message + "&to=" + Mobile_Number + "&sender=" + SenderID + "";
            try
            {
                string stringResult = null;
                objWebRequest = (System.Net.HttpWebRequest)System.Net.WebRequest.Create(URL);

                objWebRequest.Method = "POST";
                objWebRequest.ContentType = "application/x-www-form-urlencoded";
                objStreamWriter = new System.IO.StreamWriter(objWebRequest.GetRequestStream());
                objStreamWriter.Write(Message);
                objStreamWriter.Flush();
                objStreamWriter.Close();
                objWebResponse = (System.Net.HttpWebResponse)objWebRequest.GetResponse();
                objWebResponse = (System.Net.HttpWebResponse)objWebRequest.GetResponse();
                objStreamReader = new System.IO.StreamReader(objWebResponse.GetResponseStream());
                stringResult = objStreamReader.ReadToEnd();
                objStreamReader.Close();
                return (stringResult);
            }
            catch (Exception ex)
            {
                LogError(ex.Message, ex.Source, ex.InnerException, ex.StackTrace);
                //logger(ex.Message);
                return (ex.ToString());
            }
            finally
            {

                if ((objStreamWriter != null))
                {
                    objStreamWriter.Close();
                }
                if ((objStreamReader != null))
                {
                    objStreamReader.Close();
                }
                objWebRequest = null;
                objWebResponse = null;
            }
        }
        public static string Encrypt(string encryptString)
        {
            return GetEncrypt(encryptString, string.Empty);
        }
        public static string Decrypt(string cipherText)
        {
            return SetDecrypt(cipherText, string.Empty);
        }
        private static string GetEncrypt(string encryptString, string EncryptionKey)
        {
            if (EncryptionKey == string.Empty)
                EncryptionKey = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";

            byte[] clearBytes = Encoding.Unicode.GetBytes(encryptString);
            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] {
            0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76
        });
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);

                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(clearBytes, 0, clearBytes.Length);
                        cs.Close();
                    }
                    encryptString = Convert.ToBase64String(ms.ToArray());
                }
            }
            return encryptString;
        }
        private static string SetDecrypt(string cipherText, string EncryptionKey)
        {
            if (EncryptionKey == string.Empty)
                EncryptionKey = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";

            cipherText = cipherText.Replace(" ", "+");
            byte[] cipherBytes = Convert.FromBase64String(cipherText);
            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] {
            0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76
        });
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(cipherBytes, 0, cipherBytes.Length);
                        cs.Close();
                    }
                    cipherText = Encoding.Unicode.GetString(ms.ToArray());
                }
            }
            return cipherText;
        }

        public static void EventLogWriteEntry(string ErrorMessage, string Sourcename)
        {
            if (System.Diagnostics.EventLog.SourceExists(Sourcename))
            {
                System.Diagnostics.EventLog.WriteEntry(Sourcename, ErrorMessage);
            }
        }
        #region AddeForWindowsService
        public void WriteEventLog(string ErrorSource)
        {
            EventLog lWriteEventLog = new EventLog();
            //Helper.TransactionLog(ErrorSource, 0);
            if (!System.Diagnostics.EventLog.SourceExists(ErrorSource))
            {
                Helper.TransactionLog("Service Initialized 66", 0);
                System.Diagnostics.EventLog.CreateEventSource(ErrorSource, System.Environment.MachineName);
                Helper.TransactionLog("Service Initialized 77", 0);
            }
            lWriteEventLog.Source = ErrorSource;
            lWriteEventLog.Log = "Error Log";
        }

        //public string CurrentTime
        //{
        //    get { return DateTime.Now.ToString("hh:mm tt", CultureInfo.InvariantCulture); }
        //}
        //public string StartTime
        //{
        //    get
        //    {
        //        return WBTApplicationLevelSettings.StartTime;
        //        //  mConfigReader.GetValue("StartTime", typeof(string)).ToString().Trim();
        //    }
        //}
        //public string EndTime
        //{
        //    get
        //    {
        //        return WBTApplicationLevelSettings.EndTime;
        //        //mConfigReader.GetValue("EndTime", typeof(string)).ToString().Trim();
        //    }
        //}
        public static void LogError(Exception exception, bool IsLog = false)
        {
            LogError(exception.Message, exception.Source, exception.InnerException, exception.StackTrace, IsLog);
        }
        public void EventLogWriteEntry(string ErrorMessage)
        {
            if (System.Diagnostics.EventLog.SourceExists("SalesOrderCreate"))
            {
                System.Diagnostics.EventLog.WriteEntry("SalesOrderCreate", ErrorMessage);
            }
        }
        #endregion

        #region Notification
        //Notification 

        public static void SendNotification(string DeviceId, Notification Datacontext)
        {
            try
            {
                var applicationID = ConfigurationManager.AppSettings["APIKey"];
                var senderId = ConfigurationManager.AppSettings["SenderId"]; //"424344187672";
                //var senderId = ConfigurationManager.AppSettings["424344187672"]; //"424344187672";
                WebRequest tRequest = WebRequest.Create("https://fcm.googleapis.com/fcm/send");
                tRequest.Method = "post";
                tRequest.ContentType = "application/json";
                string respsone = string.Empty;

                #region notification

                var msgdata = new
                {
                    //to = DeviceId,
                    //priority = "high",
                    //Text_available = true,
                    //notification = new
                    //{
                    //    title = Datacontext.Title,
                    //    body = Datacontext.Body,
                    //    badge = 1,
                    //    sound = "default"
                    //}
                    to = DeviceId,
                    priority = "high",
                    Text_available = true,
                    notification = new
                    {
                        title = Datacontext.Title,
                        body = Datacontext.Body,
                        image = Datacontext.Image,
                        categoryname = Datacontext.CategoryName
                    }
                };
                Serialization(applicationID, senderId, msgdata);
                #endregion
            }
            catch (Exception ex)
            {
                Helper.LogError(ex.Message, ex.Source, ex.InnerException, ex.StackTrace, false);
            }
        }
        private static void Serialization(string applicationID, string senderId, object msgdata)
        {
            try
            {
                WebRequest tRequest = WebRequest.Create("https://fcm.googleapis.com/fcm/send");
                tRequest.Method = "post";
                tRequest.ContentType = "application/json";

                var serializer = new JavaScriptSerializer();
                var json = serializer.Serialize(msgdata);
                Byte[] byteArray = Encoding.UTF8.GetBytes(json);
                tRequest.Headers.Add(string.Format("Authorization: key={0}", applicationID));
                tRequest.Headers.Add(string.Format("Sender: id={0}", senderId));
                tRequest.ContentLength = byteArray.Length;
                using (Stream dataStream = tRequest.GetRequestStream())
                {
                    dataStream.Write(byteArray, 0, byteArray.Length);
                    using (WebResponse tResponse = tRequest.GetResponse())
                    {
                        using (Stream dataStreamResponse = tResponse.GetResponseStream())
                        {
                            using (StreamReader tReader = new StreamReader(dataStreamResponse))
                            {
                                String sResponseFromServer = tReader.ReadToEnd();
                                string str = sResponseFromServer;
                                Helper.TransactionLog(str, 1);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Helper.LogError(ex.Message, ex.Source, ex.InnerException, ex.StackTrace, false);
            }
        }

        public static void SendNotificationMultiple(string[] Registation_Ids, Notification Datacontext)
        {
            try
            {
                var applicationID = ConfigurationManager.AppSettings["APIKey"];
                var senderId = ConfigurationManager.AppSettings["SenderId"]; //"424344187672";
                //var senderId = ConfigurationManager.AppSettings["424344187672"]; //"424344187672";
                WebRequest tRequest = WebRequest.Create("https://fcm.googleapis.com/fcm/send");
                tRequest.Method = "post";
                tRequest.ContentType = "application/json";
                string respsone = string.Empty;

                #region notification

                var msgdata = new
                {
                    //to = DeviceId,
                    //priority = "high",
                    //Text_available = true,
                    //notification = new
                    //{
                    //    title = Datacontext.Title,
                    //    body = Datacontext.Body,
                    //    badge = 1,
                    //    sound = "default"
                    //}
                    registration_ids = Registation_Ids,
                    priority = "high",
                    Text_available = true,
                    notification = new
                    {
                        title = Datacontext.Title,
                        body = Datacontext.Body,
                        image = Datacontext.Image,
                        categoryname = Datacontext.CategoryName
                    }
                };
                Helper.TransactionLog(msgdata.ToString(), 1);
                Serialization(applicationID, senderId, msgdata);
                #endregion
            }
            catch (Exception ex)
            {
                Helper.LogError(ex.Message, ex.Source, ex.InnerException, ex.StackTrace, false);
            }
        }

        #endregion

        public static string UnicodeToUTF8(string UnicodeStr)
        {
            var bytes = Encoding.UTF8.GetBytes(UnicodeStr);
            //return new string(bytes.Select(b => (char)b).ToArray());
            return System.Text.Encoding.UTF8.GetString(bytes);
        }
        public static bool ContainsUnicodeCharacter(string input)
        {
            const int MaxAnsiCode = 127;

            return input.Any(c => c <= MaxAnsiCode);
        }

        
    }
}