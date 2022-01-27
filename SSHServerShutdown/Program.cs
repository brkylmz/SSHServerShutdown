namespace SSHServerShutdown
{
    using System;
    using System.IO;
    using System.Threading;
    using System.Xml.Linq;
    using System.Xml.Serialization;
    using Renci.SshNet;

    public static class Program
    {
        private static void Main()
        {
            try
            {
                var config = InitConfiguration($"{AppDomain.CurrentDomain.BaseDirectory}Config.xml");
                Console.WriteLine("Yapılandırma ayarları okunuyor...");

                using var client = new SshClient(config.ServerName, config.ServerPort, config.User, config.Password);
                client.Connect();
                if (client.IsConnected)
                {
                    Console.WriteLine("Bağlantı kuruldu...");
                    Shutdown(client, config.Password);
                }
                else
                    Console.WriteLine("Bağlantı kurulamadı...");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Hata meydana geldi. Hata Detayı: {ex.ToString()}");
                Console.WriteLine("Hata sonu...........");
            }
        }

        private static void Shutdown(SshClient client, string password)
        {
            try
            {
                var clientResult = client.RunCommand($"sudo -S <<< {password} shutdown -h now");
                Console.WriteLine($"Synology kapatma komutu verildi.");
                //if(!clientResult.Error.Equals(""))
                //    Console.WriteLine($"Synology kapatma talimatı verildi. Ancak hata meydana geldi. Hata Detayı: {clientResult.Error}");
                //else
                //{
                //    Console.WriteLine($"Synology kapatma talimatı verildi. Gelen cevap: {clientResult.Result}");
                //    Console.WriteLine("Synology kapatılıyor...");
                //}
                client.Disconnect();
                Console.WriteLine("Synology bağlantısı kesildi...");
                Console.WriteLine("Kapatma programından çıkmak için herhangi bir tuşa basın\n");
                Console.ReadKey();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Hata meydana geldi. Hata Detayı: {ex.ToString()}");
                Console.WriteLine("Hata sonu...........");
            }

        }

        private static Config InitConfiguration(string fileName)
        {
            try
            {
                var xDocument = XDocument.Load(fileName);
                return CreateObjectsFromString<Config>(xDocument);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return null;
            }
        }

        private static T CreateObjectsFromString<T>(XDocument xDocument)
        {
            var xmlSerializer = new XmlSerializer(typeof(T));
            return (T)xmlSerializer.Deserialize(new StringReader(xDocument.ToString()));
        }
    }
}