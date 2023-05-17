using System.Diagnostics;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using WinForms;

namespace WinForms
{
    public partial class Form1 : Form
    {
        public string _baseUrl { get; set; }

        private readonly HttpClient _httpClient;
        private CancellationTokenSource _cts;

        public Form1()
        {
            InitializeComponent();
            _baseUrl = "https://localhost:7014/api";
            _httpClient = new HttpClient();


        }

        private async void btnStart_Click_1(object sender, EventArgs e)
        {

            lodingGif.Visible = true;

            try
            {

                var txt = await Retry(ProcessGreetingsRet);
                Console.WriteLine(txt);
            }
            catch (Exception ex)
            {

                Console.WriteLine("FAILED");
            }


            lodingGif.Visible = false;


        }

        async Task ProcessGreetings()
        {
            using (var response = await _httpClient.GetAsync($"{_baseUrl}/greetings/Ulises"))
            {
                response.EnsureSuccessStatusCode();
                var greeting = await response.Content.ReadAsStringAsync();
                Console.WriteLine(greeting);
            }
        }

        async Task<string> ProcessGreetingsRet()
        {
            using (var response = await _httpClient.GetAsync($"{_baseUrl}/greetings/Ulises"))
            {
                response.EnsureSuccessStatusCode();
                var greeting = await response.Content.ReadAsStringAsync();
                //Console.WriteLine(greeting);
                return greeting;    
            }
        }

        async Task Retry(Func<Task> f, int retryTimes = 3, int waitTime = 500)
        {
            for (int i = 0; i < retryTimes - 1; i++)
            {
                try
                {
                    await f();
                    break;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    await Task.Delay(waitTime);
                }
      
            }
            await f();
        }


        async Task<T> Retry<T>(Func<Task<T>> f, int retryTimes = 3, int waitTime = 500)
        {
            for (int i = 0; i < retryTimes - 1; i++)
            {
                try
                {
                    return await f();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    await Task.Delay(waitTime);
                }

            }
            return await f();
        }

        void ReportCardProcessingProgress(int percentage)
        {
            pgBar.Value = percentage;
        }

        async Task<List<string>> GetCards(int amount, CancellationToken token = default)
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
                    if (token.IsCancellationRequested)
                    {
                        throw new TaskCanceledException();
                    }
                }
                return cards;
            });
        }
        Task ProcessCardsMock(List<string> cards, IProgress<int> progress = null, CancellationToken token = default)
        {
            //...
            return Task.CompletedTask;
        }
        Task<List<string>> GetCardsMock(int amount, CancellationToken token = default)
        {
            var cards = new List<string>();
            cards.Add("001");
            return Task.FromResult(cards);
        }

        Task CreateTaskWithException()
        {
            return Task.FromException(new ApplicationException());
        }

        Task CreateTaskCanceled()
        {
            var ct2 = new CancellationTokenSource();
            return Task.FromCanceled(ct2.Token);
        }

        async Task ProcessCards(List<string> cards, IProgress<int> progress = null, CancellationToken token = default)
        {
            using var semaphore = new SemaphoreSlim(25);
            var tasks = new List<Task<HttpResponseMessage>>();
            pgBar.Visible = true;


            tasks = cards.Select(async card =>
            {
                var json = JsonSerializer.Serialize(card);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                token.ThrowIfCancellationRequested();
                await semaphore.WaitAsync();

                try
                {
                    token.ThrowIfCancellationRequested();
                    if (!token.IsCancellationRequested)
                    {
                        var internalTask = await _httpClient.PostAsync($"{_baseUrl}/cards", content, token);
                        return internalTask;
                    }
                    else
                    {
                        token.ThrowIfCancellationRequested();
                        return null;
                    }

                }
                finally
                {

                    semaphore.Release();
                }

            }).ToList();
            token.ThrowIfCancellationRequested();



            var responsesTasks = Task.WhenAll(tasks);

            if (progress is not null)
            {
                while (await Task.WhenAny(responsesTasks, Task.Delay(1000)) != responsesTasks)
                {

                    if (!token.IsCancellationRequested)
                    {
                        var completedTasks = tasks.Where(x => x.IsCompleted).Count();
                        var percentage = (double)completedTasks / tasks.Count();
                        percentage = percentage * 100;
                        var percentageInt = (int)Math.Round(percentage, 0);
                        progress.Report(percentageInt);
                    }
                    else
                    {
                        token.ThrowIfCancellationRequested();
                    }

                }
            }
            //Will not run twice -- it will take a look at the completed property
            var responses = await responsesTasks;

            var rejectedCards = new List<string>();

            foreach (var res in responses)
            {

                var content = await res.Content.ReadAsStringAsync();
                var responseCard = JsonSerializer.Deserialize<CardResponse>(content);
                if (!responseCard.approved)
                {
                    rejectedCards.Add(responseCard.card);
                }
            }

            foreach (var card in rejectedCards)
            {
                Console.WriteLine($"Card {card} was rejected");
            }
        }

        private void btnCancel_Click_1(object sender, EventArgs e)
        {
            _cts?.Cancel();

        }

        private void lodingGif_Click(object sender, EventArgs e)
        {

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

//REPORTING WITH IPROGRESS AND WHEN ALL
//async Task ProcessCards(List<string> cards, IProgress<int> progress = null)
//{
//    using var semaphore = new SemaphoreSlim(250);
//    var tasks = new List<Task<HttpResponseMessage>>();
//    pgBar.Visible = true;
//    var taskResolved = 0;

//    tasks = cards.Select(async card =>
//    {
//        var json = JsonSerializer.Serialize(card);
//        var content = new StringContent(json, Encoding.UTF8, "application/json");

//        await semaphore.WaitAsync();

//        try
//        {
//            var internalTask = await _httpClient.PostAsync($"{_baseUrl}/cards", content);

//            if (progress is not null)
//            {
//                taskResolved++;
//                var percentage = (double)taskResolved / cards.Count;
//                percentage = percentage * 100;
//                var percentageInt = (int)Math.Round(percentage);
//                progress.Report(percentageInt);
//            }

//            return internalTask;
//        }
//        finally
//        {

//            semaphore.Release();
//        }

//    }).ToList();




//    var responses = await Task.WhenAll(tasks);

//    var rejectedCards = new List<string>();

//    foreach (var res in responses)
//    {

//        var content = await res.Content.ReadAsStringAsync();
//        var responseCard = JsonSerializer.Deserialize<CardResponse>(content);
//        if (!responseCard.approved)
//        {
//            rejectedCards.Add(responseCard.card);
//        }
//    }

//    foreach (var card in rejectedCards)
//    {
//        Console.WriteLine($"Card {card} was rejected");
//    }
//}

//CANCELING TASKS
//private async void btnStart_Click_1(object sender, EventArgs e)
//{
//    _cts = new CancellationTokenSource();
//    lodingGif.Visible = true;
//    var progressReport = new Progress<int>(ReportCardProcessingProgress);


//    var stopwatch = new Stopwatch();

//    try
//    {
//        var cards = await GetCards(100, _cts.Token);
//        stopwatch.Start();
//        await ProcessCards(cards, progressReport, _cts.Token);

//    }
//    catch (TaskCanceledException cex)
//    {
//        MessageBox.Show("The operation was canceled");
//    }
//    catch (Exception ex)
//    {

//        MessageBox.Show(ex.Message);
//    }
//    finally
//    {
//        _cts.Dispose();
//    }

//    MessageBox.Show($"Operation done in {stopwatch.ElapsedMilliseconds / 1000} seconds");

//    lodingGif.Visible = false;
//    pgBar.Visible = false;
//    pgBar.Value = 0;
//    _cts = null;

//}