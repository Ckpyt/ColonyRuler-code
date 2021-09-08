using Newtonsoft.Json;
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;


namespace SendLocalization
{
    class Program
    {
        private static int _salt;
        private static string _siteName = "https://ckpyt.com";
        static async Task Main()
        {
            await GetSalt();
            await SendAllLocalization();
            Console.WriteLine("Hello World!" + _salt);

        }

        public static async Task GetSalt()
        {
            try
            {
                HttpClient client = new HttpClient();
                client.DefaultRequestHeaders.Accept.Clear();
                HttpResponseMessage response =
                    await client.GetAsync(_siteName + "/api/colonyrulerapi?id=3&name=q");
                if (response.StatusCode == HttpStatusCode.NotFound)
                {
                    await client.GetAsync(_siteName);
                    response =
                        await client.GetAsync(_siteName + "/api/colonyrulerapi?id=3&name=q");
                }

                var cont = await response.Content.ReadAsStringAsync();
                _salt = int.Parse(cont.Trim('\"'));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public static async Task SendText(string loc, int id, string text)
        {
            //var pass = "Cnhfnc,ehu(Y.if)";
            int hash = -1406264422;
            int passHash = hash ^ _salt;

            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Accept.Clear();

            HttpContent cont = new StringContent(text);

            Uri url = new Uri(_siteName + "/api/colonyrulerapi?id=" + id +
                              "&name=" + loc + "&hash=" + passHash);

            var responseMessage = await client.PostAsync(url, cont);
            if (!responseMessage.IsSuccessStatusCode)
            {
                Console.WriteLine("cannot send localization:" + loc + " id:" + id);
            }
        }

        public static async Task SendLocalization(string loc)
        {
            var jsonTextUi = File.ReadAllText(loc + "\\" + Localization.CUiFileName + ".json");
            var jsonTextItems = File.ReadAllText(loc + "\\" + Localization.CItemsFileName + ".json");
            var jsonTextHistory = File.ReadAllText(loc + "\\" + Localization.CHistoryFileName + ".json");
            await SendText(loc, 1, jsonTextUi);
            await SendText(loc, 2, jsonTextItems);
            await SendText(loc, 3, jsonTextHistory);

        }

        public static async Task SendAllLocalization()
        {
            //looking for a root directory
            string curDir = Directory.GetCurrentDirectory();
            while (!Directory.Exists(curDir + "\\ColonyRuler"))
            {
                int pos = curDir.Length - 1;
                while (curDir[pos] != '\\')
                    pos--;

                curDir = curDir.Substring(0, pos);
            }

            string gameDir = curDir + "\\ColonyRuler";

            Directory.SetCurrentDirectory(gameDir);

            var jsonText = File.ReadAllText(gameDir + "\\" + Localization.CLangListPath);

            LanguagesList languages = JsonConvert.DeserializeObject<LanguagesList>(jsonText);

            gameDir += "\\Assets\\Resources\\";
            Directory.SetCurrentDirectory(gameDir);

            foreach (var lang in languages.m_languages)
            {
                await SendLocalization(lang);
            }
        }

    }
}
