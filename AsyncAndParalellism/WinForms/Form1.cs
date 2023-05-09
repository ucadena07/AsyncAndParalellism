using System.Diagnostics;
using System.Net.Http;
using System.Text;
using System.Text.Json;

namespace WinForms
{
    public partial class Form1 : Form
    {
        public string _baseUrl { get; set; }

        private readonly HttpClient _httpClient;

        public Form1()
        {
            InitializeComponent();
            _baseUrl = "https://localhost:7014/api";
            _httpClient = new HttpClient();
        }

        private async void btnStart_Click(object sender, EventArgs e)
        {
            lodingGif.Visible= true;

            var stopwatch = new Stopwatch();

            try
            {
                var cards = await GetCards(1000);
                stopwatch.Start();  
                await ProcessCards(cards);

            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }

            MessageBox.Show($"Operation done in {stopwatch.ElapsedMilliseconds / 1000} seconds");

            lodingGif.Visible = false;


        }

        async Task<List<string>> GetCards(int amount)
        {
            //var cards = new List<string>();
            //for (int i = 0; i < amount; i++)
            //{
            //    cards.Add(i.ToString().PadLeft(16,'0')) ;
            //}
            //return cards;

            return await Task.Run(() =>
            {
                var cards = new List<string>();
                for (int i = 0; i < amount; i++)
                {
                    cards.Add(i.ToString().PadLeft(16, '0'));
                }
                return cards;
            });
        }

        async Task ProcessCards(List<string> cards)
        {
            using var semaphore = new SemaphoreSlim(250);
            var tasks = new List<Task<HttpResponseMessage>>();

            tasks = cards.Select(async card =>
            {
                var json = JsonSerializer.Serialize(card);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                await semaphore.WaitAsync();

                try
                {
                    return await _httpClient.PostAsync($"{_baseUrl}/cards", content);
                }
                finally
                {

                    semaphore.Release();
                }
  
            }).ToList();




            await Task.WhenAll(tasks);
        }

    }
}

// LESON 11 - 17
//private async void btnStart_Click(object sender, EventArgs e)
//{
//    lodingGif.Visible = true;

//    await Wait();

//    var name = txtInput.Text;

//    try
//    {
//        var greeting = await GetGreetings(name);
//        MessageBox.Show(greeting);
//    }
//    catch (Exception ex)
//    {

//        MessageBox.Show(ex.Message);
//    }


//    lodingGif.Visible = false;


//}

//async Task Wait()
//{
//    await Task.Delay(0);
//}


//async Task<string> GetGreetings(string name)
//{
//    using (var response = await _httpClient.GetAsync($"{_baseUrl}/greetings2/{name}"))
//    {
//        response.EnsureSuccessStatusCode();
//        var greeting = await response.Content.ReadAsStringAsync();
//        return greeting;
//    }
//}


// LESSON 18 - 19 
//async Task ProcessCards(List<string> cards)
//{
//    var tasks = new List<Task<HttpResponseMessage>>();

//    //foreach (var card in cards)
//    //{
//    //    var json = JsonSerializer.Serialize(card);
//    //    var content = new StringContent(json, Encoding.UTF8, "application/json");
//    //    var responseTask = _httpClient.PostAsync($"{_baseUrl}/cards",content);

//    //    tasks.Add(responseTask);    
//    //}

//    await Task.Run(() =>
//    {
//        foreach (var card in cards)
//        {
//            var json = JsonSerializer.Serialize(card);
//            var content = new StringContent(json, Encoding.UTF8, "application/json");
//            var responseTask = _httpClient.PostAsync($"{_baseUrl}/cards", content);

//            tasks.Add(responseTask);
//        }
//    });

//    await Task.WhenAll(tasks);
//}