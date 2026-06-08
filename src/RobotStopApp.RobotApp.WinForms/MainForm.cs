using System;
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using RobotStopApp.RobotApp.WinForms.Services;

namespace RobotStopApp.RobotApp.WinForms
{
    public partial class MainForm : Form
    {
        private readonly IRobotApiClient _apiClient;
        private readonly System.Windows.Forms.Timer _autoRefreshTimer;
        private bool _isBusy;

        public MainForm(IRobotApiClient apiClient, RobotApiSettings settings)
        {
            _apiClient = apiClient ?? throw new ArgumentNullException(nameof(apiClient));

            InitializeComponent();

            txtBaseUrl.Text = settings?.ApiBaseUrl ?? string.Empty;
            txtApiKey.Text = settings?.ApiKey ?? string.Empty;

            _autoRefreshTimer = new System.Windows.Forms.Timer();
            _autoRefreshTimer.Interval = 1000;
            _autoRefreshTimer.Tick += AutoRefreshTimer_Tick;

            UpdateResult(new RobotApiResult(false, false, "Ready."));
        }

        protected override async void OnShown(EventArgs e)
        {
            base.OnShown(e);
            await RefreshAsync();
            _autoRefreshTimer.Start();
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            _autoRefreshTimer.Stop();
            _autoRefreshTimer.Tick -= AutoRefreshTimer_Tick;
            _autoRefreshTimer.Dispose();
            base.OnFormClosed(e);
        }

        private async void AutoRefreshTimer_Tick(object sender, EventArgs e)
        {
            await RefreshAsync();
        }

        private async void btnCheckStatus_Click(object sender, EventArgs e)
        {
            await RefreshAsync();
        }

        private async Task RefreshAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            if (_isBusy)
            {
                return;
            }

            try
            {
                SetBusy(true);
                var result = await _apiClient.CheckAsync(txtBaseUrl.Text, txtApiKey.Text, cancellationToken);
                UpdateResult(result);
            }
            catch (OperationCanceledException)
            {
                UpdateResult(new RobotApiResult(false, false, "Request canceled."));
            }
            catch (Exception ex)
            {
                UpdateResult(new RobotApiResult(false, false, "Unexpected error: " + ex.Message));
            }
            finally
            {
                SetBusy(false);
            }
        }

        private void SetBusy(bool busy)
        {
            _isBusy = busy;
            btnCheckStatus.Enabled = !busy;
        }

        private void UpdateResult(RobotApiResult result)
        {
            lblApiConnectedValue.Text = result.ApiConnected ? "Connected" : "Not Connected";
            lblApiConnectedValue.ForeColor = result.ApiConnected ? Color.FromArgb(43, 138, 62) : Color.FromArgb(182, 58, 58);

            lblRobotRunOkValue.Text = result.IsRobotRunOk ? "True" : "False";
            lblRobotRunOkValue.ForeColor = result.IsRobotRunOk ? Color.FromArgb(43, 138, 62) : Color.FromArgb(182, 58, 58);

            txtMessage.Text = result.Message;
        }
    }
}
