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

            await Wait();

            var name = txtInput.Text;

            var greeting = await GetGreetings(name);

            lodingGif.Visible = false;

            MessageBox.Show(greeting);

    
        }

        async Task Wait()
        {
            await Task.Delay(0);
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        async Task<string> GetGreetings(string name)
        {
            using (var response = await _httpClient.GetAsync($"{_baseUrl}/greetings/{name}"))
            {
                var greeting = await response.Content.ReadAsStringAsync();
                return greeting;
            }
        }
    }
}

