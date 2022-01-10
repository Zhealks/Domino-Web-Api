using System;
using Domino;
using Serilog;

namespace DominoApi
{
    class Domino
    {
        public string SendNotes(string receivers, string topic, string body, string copies, string name,string nsf, string passwd)
        {
            try
            {
                Log.Information("[USER] receivers: "+receivers);
                Log.Information("[USER] topic: " + topic);
                Log.Information("[USER] body: " + body);
                Log.Information("[USER] copies: " + copies);
                Log.Information("[USER] name: " + name);
                Log.Information("[USER] nsf: " + nsf);
                //Log.Information("[USER] passwdRSA: " + passwdRSA);
                
                //string passwd = Domino.RSADecrypt(passwdRSA);
                //Log.Information("[USER] passwd: " + passwd);

                NotesSession NSession = new NotesSession();
                NSession.Initialize(passwd);
                NotesDatabase NDataBase = NSession.GetDatabase(name, nsf, false);
                Log.Information("[USER] NotesDataBase Is Open: " + NDataBase.IsOpen);

                NotesDocument NDocument = NDataBase.CreateDocument();
                NDocument.ReplaceItemValue("Form", "Memo");
                string[] recvArr = receivers.Split(',');
                NDocument.ReplaceItemValue("SendTo", recvArr);
                NDocument.ReplaceItemValue("Subject", topic);
                
                if (!string.IsNullOrEmpty(copies))
                {
                    string[] copyArr = copies.Split(',');
                    NDocument.ReplaceItemValue("CopyTo", copyArr);
                }

                NDocument.SaveMessageOnSend = true;
                NotesRichTextItem rt = NDocument.CreateRichTextItem("Body");
                rt.AppendText(body);

                object obj = NDocument.GetItemValue("SendTo");
                NDocument.Save(true, false);
                NDocument.Send(false, ref obj);
                NDocument = null;
                return "Successfully";
            }
            catch (Exception ex)
            {
                Log.Error("[USER] Exception: " + ex.ToString());
                return ex.Message;

            }
        }

        //RSA解密数据
        public static string RSADecrypt(string ciphertext, string KeyContainerName = null)
        {
            System.Security.Cryptography.CspParameters param = new System.Security.Cryptography.CspParameters();
            param.KeyContainerName = "zhealks"; //密匙容器的名称，保持加密解密一致才能解密成功
            using (System.Security.Cryptography.RSACryptoServiceProvider rsa = new System.Security.Cryptography.RSACryptoServiceProvider(param))
            {
                byte[] encryptdata = Convert.FromBase64String(ciphertext);
                byte[] decryptdata = rsa.Decrypt(encryptdata, false);
                return System.Text.Encoding.Default.GetString(decryptdata);
            }
        }
    }
}
