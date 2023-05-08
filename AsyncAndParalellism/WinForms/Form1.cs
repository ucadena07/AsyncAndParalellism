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

            try
            {
                var greeting = await GetGreetings(name);
                MessageBox.Show(greeting);
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }


            lodingGif.Visible = false;


        }

        async Task Wait()
        {
            await Task.Delay(0);
        }


        async Task<string> GetGreetings(string name)
        {
            using (var response = await _httpClient.GetAsync($"{_baseUrl}/greetings2/{name}"))
            {
                response.EnsureSuccessStatusCode(); 
                var greeting = await response.Content.ReadAsStringAsync();
                return greeting;
            }
        }
        private void label1_Click(object sender, EventArgs e)
        {

        }
    }
}

