using Projekat;
using Projekat.Encryption;
using System.Text;

namespace Projekat
{

    internal static class Program
    {
        public static CustomApplicationContext AppContext { get; private set; }
        [STAThread]

        static void Main()
        {
            string XPath = Properties.Settings.Default.XTargetPath;
            string TargetPath = Properties.Settings.Default.TargetPath;

            var cts = new CancellationTokenSource();

            //ApplicationConfiguration.Initialize();
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // Initialize the static AppContext property
            AppContext = new CustomApplicationContext(cts);

            // Run the application with the custom application context
            System.Windows.Forms.Application.Run(AppContext);
        }
    }

    // Custom ApplicationContext to manage forms
    public class CustomApplicationContext : ApplicationContext
    {
        private CancellationTokenSource _cts;  //readonly
        private Type _nextFormType;
        private bool _isFormClosed = false; // Track if the form has been closed

        public CustomApplicationContext(CancellationTokenSource cts)
        {
            _cts = cts;

            // Start with Form1
            var form1 = new Form1(_cts);
            form1.FormClosed += OnFormClosed;
            MainForm = form1;
            form1.Show();
        }

        private void OnFormClosed(object sender, FormClosedEventArgs e)
        {
            // If the app has already decided to exit, don't reopen forms
            if (_isFormClosed)
            {
                //it doesnt even go beyond this
               _cts.Cancel();
               ExitThread();
               return;
            }

            // Mark that the form has been closed
            _isFormClosed = true;

            // Check if we have a valid next form type to switch to
            if (_nextFormType == typeof(NetworkTCP))
            {
                // Switch to NetworkTCP form
                _cts.Cancel();
                _cts=new CancellationTokenSource(); //no
                var networkTcpForm = new NetworkTCP(_cts);
                networkTcpForm.FormClosed += OnFormClosed;
                MainForm = networkTcpForm;
                networkTcpForm.Show();
            }
            else if (_nextFormType == typeof(Form1))
            {
                // Switch back to Form1
                _cts.Cancel();
                _cts = new CancellationTokenSource(); //no
                var form1 = new Form1(_cts);
                form1.FormClosed += OnFormClosed;
                MainForm = form1;
                form1.Show();
            }
          
        }

        public void EnsureExit()
        {
            _nextFormType = null;
            _isFormClosed = true;
        }

        public void SwitchToForm(Type formType)
        {
            //if (formType == null)
            //{
            //    throw new ArgumentNullException(nameof(formType));
            //    _isFormClosed = false;
            //}

            // Set the next form type to switch to
            _nextFormType = formType;
            _isFormClosed = false;

            // Close the current form to trigger the OnFormClosed event
            MainForm?.Close();
        }
    }
}