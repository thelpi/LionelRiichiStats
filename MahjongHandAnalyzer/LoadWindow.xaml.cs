using System.ComponentModel;
using System.Windows;

namespace MahjongHandAnalyzer
{
    /// <summary>
    /// Interaction logic for this instance.
    /// </summary>
    /// <seealso cref="Window"/>
    public partial class LoadWindow : Window
    {
        private BackgroundWorker _bgw;
        private object _argument;
        public object ReturnValue { get; private set; }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="handler">The background operation delegate.</param>
        /// <param name="argument">The input argument for the background operation.</param>
        public LoadWindow(DoWorkEventHandler handler, object argument)
        {
            InitializeComponent();

            _argument = argument;
            _bgw = new BackgroundWorker
            {
                WorkerReportsProgress = true,
                WorkerSupportsCancellation = false
            };
            _bgw.DoWork += handler;
            _bgw.ProgressChanged += delegate(object sender, ProgressChangedEventArgs e)
            {
                PgbLoading.Value = e.ProgressPercentage;
            };
            _bgw.RunWorkerCompleted += delegate(object sender, RunWorkerCompletedEventArgs e)
            {
                ReturnValue = e.Result;
                Close();
            };
        }

        /// <summary>
        /// Shows the window (modal) and starts the <see cref="BackgroundWorker"/>
        /// </summary>
        /// <returns><see cref="base.ShowDialog()"/></returns>
        public new bool? ShowDialog()
        {
            _bgw.RunWorkerAsync(_argument);
            return base.ShowDialog();
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            if (_bgw.IsBusy)
            {
                e.Cancel = true;
                TxbWaitWarning.Foreground = System.Windows.Media.Brushes.Red;
            }
        }
    }
}
