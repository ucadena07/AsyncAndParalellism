namespace WinForms
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private async void btnStart_Click(object sender, EventArgs e)
        {
            lodingGif.Visible= true;

            await Wait();

            MessageBox.Show("5 seconds have passsed");

            lodingGif.Visible = false;

        }

        async Task Wait()
        {
            await Task.Delay(5000);
        } 




    }
}