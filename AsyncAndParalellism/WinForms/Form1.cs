using System.Diagnostics;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security.Policy;
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

            var source = Enumerable.Range(1, 20);

            var evenNumbers = source
                .AsParallel()
                .AsOrdered()
                .Where(x => x % 2 == 0).ToList();

            foreach (var item in evenNumbers)
            {
                Console.WriteLine(item);
            }

            lodingGif.Visible = false;

        }

        async Task DoMatricesTest(int maxDegreeOfParalelism)
        {
            _cts = new CancellationTokenSource();

            var columnMatrixA = 1100;
            var rows = 1000;
            var columnMatrixB = 1750;
            var matrixA = Matrices.InitializeMatrix(rows, columnMatrixA);
            var matrixB = Matrices.InitializeMatrix(columnMatrixA, columnMatrixB);
            var result = new double[rows, columnMatrixB];
            var stopwatch = new Stopwatch();
            stopwatch.Start();


            try
            {
                await Task.Run(() =>
                {
                    Matrices.MultiplyMatricesParallel(matrixA, matrixB, result, _cts.Token, maxDegreeOfParalelism);
                });
                Console.WriteLine($"With max degree of {maxDegreeOfParalelism} it took: {stopwatch.ElapsedMilliseconds /1000.0} seconds");
            }
            catch (Exception ex)
            {

                Console.WriteLine(ex.Message);
            }
            finally
            {
                _cts.Dispose();
            }


            //sequential



            var timeSeq = stopwatch.ElapsedMilliseconds / 1000.0;
            //Console.WriteLine("Sequential - duration {0} seconds", timeSeq);
            stopwatch.Restart();

            //parallerl




            var timeSim = stopwatch.ElapsedMilliseconds / 1000.0;
            //Console.WriteLine("Simultaneous - duration {0} seconds", timeSim);


            //WriteComparison(timeSeq, timeSim);
        }

        private void FlipImage(string file, string destinationDirectory)
        {
            using (var image = new Bitmap(file))
            {
                image.RotateFlip(RotateFlipType.Rotate90FlipNone);
                var fileName = Path.GetFileName(file);
                var destination = Path.Combine(destinationDirectory, fileName);
                image.Save(destination);
            }
        }

        private async Task ProcessImage(string directorio, ImageDTO imagen)
        {
            var response = await _httpClient.GetAsync(imagen.URL);
            var content = await response.Content.ReadAsByteArrayAsync();

            Bitmap bitmap;
            using (var ms = new MemoryStream(content))
            {
                bitmap = new Bitmap(ms);
            }

            bitmap.RotateFlip(RotateFlipType.Rotate90FlipNone);
            var destination = Path.Combine(directorio, imagen.Name);
            bitmap.Save(destination);
        }

        public static void WriteComparison(double time1, double time2)
        {
            var difference = time2 - time1;
            difference = Math.Round(difference, 2);
            var porcentualIncrement = ((time2 - time1) / time1) * 100;
            porcentualIncrement = Math.Round(porcentualIncrement, 2);
            Console.WriteLine($"Difference {difference} ({porcentualIncrement}%)");
        }

        public static void PrepareExecution(string destinationParallel, string destinationSequential)
        {
            if (!Directory.Exists(destinationParallel))
            {
                Directory.CreateDirectory(destinationParallel);
            }

            if (!Directory.Exists(destinationSequential))
            {
                Directory.CreateDirectory(destinationSequential);
            }

            DeleteFiles(destinationSequential);
            DeleteFiles(destinationParallel);
        }

        public static void DeleteFiles(string directory)
        {
            var files = Directory.EnumerateFiles(directory);
            foreach (var file in files)
            {
                File.Delete(file);
            }
        }

        private static List<ImageDTO> GetImages()
        {
            var images = new List<ImageDTO>();
            for (int i = 0; i < 5; i++)
            {
                {
                    images.Add(
                    new ImageDTO()
                    {
                        Name = $"Spider-Man Spider-Verse {i}.jpg",
                        URL = "https://m.media-amazon.com/images/M/MV5BMjMwNDkxMTgzOF5BMl5BanBnXkFtZTgwNTkwNTQ3NjM@._V1_UY863_.jpg"
                    });
                    images.Add(

                    new ImageDTO()
                    {
                        Name = $"Spider-Man Far From Home {i}.jpg",
                        URL = "https://m.media-amazon.com/images/M/MV5BMGZlNTY1ZWUtYTMzNC00ZjUyLWE0MjQtMTMxN2E3ODYxMWVmXkEyXkFqcGdeQXVyMDM2NDM2MQ@@._V1_UY863_.jpg"
                    });
                    images.Add(

                    new ImageDTO()
                    {
                        Name = $"Moana {i}.jpg",
                        URL = "https://lumiere-a.akamaihd.net/v1/images/r_moana_header_poststreet_mobile_bd574a31.jpeg?region=0,0,640,480"
                    });
                    images.Add(

                    new ImageDTO()
                    {
                        Name = $"Avengers Infinity War {i}.jpg",
                        URL = "https://img.redbull.com/images/c_crop,x_143,y_0,h_1080,w_1620/c_fill,w_1500,h_1000/q_auto,f_auto/redbullcom/2018/04/23/e4a3d8a5-2c44-480a-b300-1b2b03e205a5/avengers-infinity-war-poster"
                    });
                }
            }

            return images;
        }
    

    async Task<string> GetValue()
        {
            await Task.Delay(2000);
            return "Nigel";
        }

        async Task ProcessNames(IAsyncEnumerable<string> namesEnumerable)
        {
            _cts = new CancellationTokenSource();   

            await foreach(var name in namesEnumerable.WithCancellation(_cts.Token))
            {
                Console.WriteLine(name);
            }
        }

        async IAsyncEnumerable<string> GenerateNames([EnumeratorCancellation]CancellationToken token = default)
        {
            yield return "Uli";
            await Task.Delay(3000, token);
            yield return "Haylee";
            await Task.Delay(3000, token);
            yield return "Henry";
        }

        public Task EvaluateValue(string value)
        {
            var tcs = new TaskCompletionSource<object>(TaskCreationOptions.RunContinuationsAsynchronously);   

            if(value == "1")
            {
                tcs.SetResult(null);
            }else if(value == "2")
            {
                tcs.SetCanceled();
            }
            else
            {
                tcs.SetException(new ApplicationException("Invalid " + value));  
            }

            return tcs.Task;
        }


        async Task<string> GetGreetings(string name, CancellationToken token)
        {
            using (var response = await _httpClient.GetAsync($"{_baseUrl}/Greetings/GetGreetings/{name}", token))
            {
                response.EnsureSuccessStatusCode();
                var greeting = await response.Content.ReadAsStringAsync();
                return greeting;
            }
        }

        async Task<string> GetGoodBye(string name, CancellationToken token)
        {
            using (var response = await _httpClient.GetAsync($"{_baseUrl}/Greetings/GetGoodbye/{name}", token))
            {
                response.EnsureSuccessStatusCode();
                var greeting = await response.Content.ReadAsStringAsync();
                return greeting;
            }
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

        async Task<T> OnlyOne<T>(IEnumerable<Func<CancellationToken,Task<T>>> functions)
        {
            var concellationToken = new CancellationTokenSource();
            var tasks = functions.Select(function => function(concellationToken.Token));
            var task = await Task.WhenAny(tasks);
            concellationToken.Cancel();
            return await task;  
        }

        async Task<T> OnlyOne<T>(params Func<CancellationToken, Task<T>>[] functions)
        {
            var concellationToken = new CancellationTokenSource();
            var tasks = functions.Select(function => function(concellationToken.Token));
            var task = await Task.WhenAny(tasks);
            concellationToken.Cancel();
            return await task;
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

//RETRY PATTERN
//private async void btnStart_Click_1(object sender, EventArgs e)
//{

//    lodingGif.Visible = true;

//    try
//    {

//        var txt = await Retry(ProcessGreetingsRet);
//        Console.WriteLine(txt);
//    }
//    catch (Exception ex)
//    {

//        Console.WriteLine("FAILED");
//    }


//    lodingGif.Visible = false;


//}


//Only One Pattern
//private async void btnStart_Click_1(object sender, EventArgs e)
//{

//    lodingGif.Visible = true;

//    //_cts = new();
//    //var token = _cts.Token;
//    //var names = new List<string>() { "Uli","Haylee","Henry","Nigel"};

//    //var httpTasks = names.Select(x => GetGreetings(x, token));

//    //var task = await Task.WhenAny(httpTasks);    
//    //var content = await task;
//    //_cts.Cancel();  
//    //Console.WriteLine(content);

//    //var names = new List<string>() { "Uli","Haylee","Henry","Nigel"};
//    //var tasks = names.Select(name =>
//    //{
//    //    Func<CancellationToken, Task<string>> func = (ct) => GetGreetings(name, ct);
//    //    return func;
//    //});

//    //var content = await OnlyOne(tasks);
//    //Console.WriteLine(content);

//    var content = await OnlyOne(

//        (ct) => GetGreetings("Ulises", ct),
//        (ct) => GetGoodBye("Ulises", ct)

//        );

//    Console.WriteLine(content);
//    lodingGif.Visible = false;


//}


//Controlling the result of a task

//private async void btnStart_Click_1(object sender, EventArgs e)
//{

//    lodingGif.Visible = true;

//    var task = EvaluateValue(txtInput.Text);

//    Console.WriteLine($"Is Completed: {task.IsCompleted}");
//    Console.WriteLine($"Is Canceled: {task.IsCanceled}");
//    Console.WriteLine($"Is Faulted: {task.IsFaulted}");

//    try
//    {
//        await task;
//    }
//    catch (Exception ex)
//    {

//        Console.WriteLine(ex.Message);
//    }

//    lodingGif.Visible = false;


//}


/* Cancelling non cancellable tasks
private async void btnStart_Click_1(object sender, EventArgs e)
{

    lodingGif.Visible = true;

    _cts = new CancellationTokenSource();

    try
    {
        var result = await Task.Run(async () =>
        {
            await Task.Delay(5000);
            return 7;
        }).WithCancellation(_cts.Token);
        Console.WriteLine(result);
    }
    catch (Exception ex)
    {
        Console.WriteLine(ex.Message);
    }
    finally { _cts.Dispose(); }


    lodingGif.Visible = false;


}

*/

/*
 * Lesson 40
 * private async void btnStart_Click_1(object sender, EventArgs e)
        {

            lodingGif.Visible = true;

            //_cts = new CancellationTokenSource();   

            //var  names = new List<string>() { "Uli", "Haylee"};

            //try
            //{
            //    await foreach (var item in GenerateNames(_cts.Token))
            //    {
            //        Console.WriteLine(item);
            //    }

            //}
            //catch (TaskCanceledException ex)
            //{

            //    Console.WriteLine(ex.Message);
            //}finally { _cts.Cancel(); _cts = null; }


            try
            {
                var names = GenerateNames();
                await ProcessNames(names);
            }
            catch (Exception ex)
            {

                Console.WriteLine(ex.Message);
            }
            finally
            {
                _cts.Cancel(); _cts = null;
            }


  

            lodingGif.Visible = false;


        }
 * 
 * 
 */

/* anit patterns
 *    private async void btnStart_Click_1(object sender, EventArgs e)
        {

            lodingGif.Visible = true;


            //anti pattern: sync over async 
            //var value = GetValue().Result;


            //optimal solution
            //var value = await GetValue();


            lodingGif.Visible = false;

        }
 */

/*Simultaneous tasks - task.when all
 * 
 *      private async void btnStart_Click_1(object sender, EventArgs e)
        {

            lodingGif.Visible = true;

            var currentDict = AppDomain.CurrentDomain.BaseDirectory;
            var destSeq = Path.Combine(currentDict, @"images\result-seq");
            var destSem = Path.Combine(currentDict, @"images\result-sem");

            PrepareExecution(destSeq, destSem);

            Console.WriteLine("BEGIN");

            var images = GetImages();

            var stopwatch = new Stopwatch();
            stopwatch.Start();

            //sequential part 
            foreach (var image in images)
            {
                await ProcessImage(destSeq, image);
            }

            var timeSeq = stopwatch.ElapsedMilliseconds / 1000.0;
            Console.WriteLine("Sequential - duration {0} seconds", timeSeq);
            stopwatch.Restart();

            var tasks = images.Select(async image =>
            {
                await ProcessImage(destSem, image);
            });

            await Task.WhenAll(tasks);

            var timeSim = stopwatch.ElapsedMilliseconds / 1000.0;
            Console.WriteLine("Simultaneous - duration {0} seconds", timeSim);


            WriteComparison(timeSeq, timeSim);

            //simultaneous part 




            lodingGif.Visible = false;

        }
 * 
 * 
 * 
 */

/* Parallel for
 *    private async void btnStart_Click_1(object sender, EventArgs e)
        {

            lodingGif.Visible = true;

            var columnMatrixA = 1100;
            var rows = 1000;
            var columnMatrixB = 1750;

            var matrixA = Matrices.InitializeMatrix(rows, columnMatrixA);
            var matrixB = Matrices.InitializeMatrix(columnMatrixA, columnMatrixB);

            var result = new double[rows,columnMatrixB];  

            var stopwatch = Stopwatch.StartNew();   
            stopwatch.Start();

            await Task.Run(() => Matrices.MultiplyMatricesSequential(matrixA, matrixB, result));
            var sequetialTime = stopwatch.ElapsedMilliseconds / 1000;

            Console.WriteLine("Sequential  - duration in seconds: {0}", sequetialTime);

            stopwatch.Restart();

            await Task.Run(() => Matrices.MultiplyMatricesParallel(matrixA, matrixB, result));
            var parallelTime = stopwatch.ElapsedMilliseconds / 1000;

            Console.WriteLine("parallel  - duration in seconds: {0}", parallelTime);

            WriteComparison(sequetialTime, parallelTime);


            lodingGif.Visible = false;

        }
 * 
 * 
 * 
 */


/* Paralle.ForEach
 *        private async void btnStart_Click_1(object sender, EventArgs e)
        {

            lodingGif.Visible = true;

            var currentDict = AppDomain.CurrentDomain.BaseDirectory;
            var originalFolder = Path.Combine(currentDict, @"images\result-seq");
            var destSem = Path.Combine(currentDict, @"images\foreach-sequential");
            var destParl = Path.Combine(currentDict, @"images\foreach-parallel");
            PrepareExecution(destSem, destParl);

            Console.WriteLine("BEGIN");

            //var images = GetImages();
            var files = Directory.EnumerateFiles(originalFolder);

            var stopwatch = new Stopwatch();
            stopwatch.Start();

            //sequential part 
            foreach (var image in files)
            {
                 FlipImage(image, destSem);
            }

            var timeSeq = stopwatch.ElapsedMilliseconds / 1000.0;
            Console.WriteLine("Sequential - duration {0} seconds", timeSeq);
            stopwatch.Restart();

            Parallel.ForEach(files, file =>
            {
                FlipImage(file, destParl);
            });
   

  

            var timeSim = stopwatch.ElapsedMilliseconds / 1000.0;
            Console.WriteLine("Simultaneous - duration {0} seconds", timeSim);


            WriteComparison(timeSeq, timeSim);

            //simultaneous part 




            lodingGif.Visible = false;

        }
 * 
 * 
 * 
 * 
 * 
 */

/* Parallel invoke 
 *        private async void btnStart_Click_1(object sender, EventArgs e)
        {

            lodingGif.Visible = true;

            var currentDict = AppDomain.CurrentDomain.BaseDirectory;
            var originalFolder = Path.Combine(currentDict, @"images\result-seq");
            var destSem = Path.Combine(currentDict, @"images\foreach-sequential");
            var destParl = Path.Combine(currentDict, @"images\foreach-parallel");
         

            Console.WriteLine("BEGIN");

            //var images = GetImages();
            var files = Directory.EnumerateFiles(originalFolder);


            var columnMatrixA = 1100;
            var rows = 1000;
            var columnMatrixB = 1750;
            var matrixA = Matrices.InitializeMatrix(rows, columnMatrixA);
            var matrixB = Matrices.InitializeMatrix(columnMatrixA, columnMatrixB);
            var result = new double[rows, columnMatrixB];
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            Action multipleMatrices = () => Matrices.MultiplyMatricesParallel(matrixA, matrixB,result);

            Action tranformImages = () =>
            {
                foreach (var image in files)
                {
                    FlipImage(image, destSem);
                }
            };

            //sequential
            multipleMatrices();
            tranformImages();


            var timeSeq = stopwatch.ElapsedMilliseconds / 1000.0;
            Console.WriteLine("Sequential - duration {0} seconds", timeSeq);
            stopwatch.Restart();

            //parallerl
            PrepareExecution(destSem, destParl);
            Parallel.Invoke(multipleMatrices, tranformImages);  



            var timeSim = stopwatch.ElapsedMilliseconds / 1000.0;
            Console.WriteLine("Simultaneous - duration {0} seconds", timeSim);


            WriteComparison(timeSeq, timeSim);





            lodingGif.Visible = false;

        }
 * 
 * 
 * 
 * 
 */

/* INTERLOCKED
 *     private async void btnStart_Click_1(object sender, EventArgs e)
        {

            lodingGif.Visible = true;

            var valueWithoutInterLocked = 0;
            Parallel.For(0, 100000, _ => valueWithoutInterLocked++);
            Console.WriteLine("Increment without interlocked " + valueWithoutInterLocked);

            var valueWithInterlocked = 0;
            Parallel.For(0, 100000, _ => Interlocked.Increment(ref valueWithInterlocked));
            Console.WriteLine("Increment with interlocked " + valueWithInterlocked);

            lodingGif.Visible = false;

        }
 * 
 * 
 * 
 */

/*
 * Locks
 *    private async void btnStart_Click_1(object sender, EventArgs e)
        {

            lodingGif.Visible = true;

            var incrementValue = 0;
            var sumValue = 0;
            var mutext = new object();

            Parallel.For(0, 10000, num =>
            {
                lock(mutext)
                {

                    incrementValue++;
                    sumValue += incrementValue;
                }

            });



            Console.WriteLine("Increment value: " + incrementValue);
            Console.WriteLine("Sum value: " + sumValue);



            lodingGif.Visible = false;

        }
 * 
 * 
 * 
 */