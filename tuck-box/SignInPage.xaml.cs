namespace tuck_box
{
    public partial class SignInPage : ContentPage
    {
        public SignInPage()
        {
            Image image = new Image
            {
                Source = ImageSource.FromFile(@"Resources\CustomImages\logo.svg")
            };

            InitializeComponent();

        }
    }
}