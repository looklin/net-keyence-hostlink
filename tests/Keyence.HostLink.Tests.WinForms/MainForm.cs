using System;
using System.Text;
using System.Windows.Forms;
using Keyence.HostLink;
using Keyence.HostLink.Models;

namespace Keyence.HostLink.Tests.WinForms
{
    public partial class MainForm : Form
    {
        private HostLinkClient _client;
        private readonly object _logLock = new object();

        public MainForm()
        {
            InitializeComponent();
            InitializeClient();
        }

        private void InitializeClient()
        {
            var options = new HostLinkOptions();
            _client = new HostLinkClient(options);
            _client.Connected += OnClientConnected;
            _client.Disconnected += OnClientDisconnected;
        }

        private async void btnConnect_Click(object sender, EventArgs e)
        {
            try
            {
                btnConnect.Enabled = false;
                UpdateStatus("Connecting...");
                AppendLog("Connecting to " + txtHost.Text + ":" + txtPort.Text);

                int port;
                if (!int.TryParse(txtPort.Text, out port))
                {
                    MessageBox.Show("Invalid port number.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    UpdateStatus("Invalid port");
                    btnConnect.Enabled = true;
                    return;
                }

                _client.Options.Host = txtHost.Text;
                _client.Options.Port = port;

                await _client.ConnectAsync();

                AppendLog("Connected successfully.");
            }
            catch (Exception ex)
            {
                AppendLog("Connection failed: " + ex.Message);
                UpdateStatus("Connection failed");
                btnConnect.Enabled = true;
            }
        }

        private void btnDisconnect_Click(object sender, EventArgs e)
        {
            try
            {
                _client.Disconnect();
                AppendLog("Disconnected.");
                UpdateStatus("Disconnected");
                btnConnect.Enabled = true;
            }
            catch (Exception ex)
            {
                AppendLog("Disconnect error: " + ex.Message);
            }
        }

        private async void btnRead_Click(object sender, EventArgs e)
        {
            if (!_client.IsConnected)
            {
                MessageBox.Show("Please connect to PLC first.", "Not Connected", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string address = txtAddress.Text.Trim();
            if (string.IsNullOrEmpty(address))
            {
                MessageBox.Show("Please enter an address.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            btnRead.Enabled = false;
            AppendLog("Reading address: " + address);

            try
            {
                var result = await _client.ReadItemAsync(address);

                if (result.IsSuccess)
                {
                    var sb = new StringBuilder();
                    sb.AppendLine("Address: " + result.Address);
                    sb.AppendLine("Raw Response: " + result.RawResponse);
                    sb.AppendLine("Response Length: " + result.ResponseBuffer.Length + " bytes");

                    try
                    {
                        sb.AppendLine("Int16 Value: " + result.ToInt16());
                    }
                    catch { }

                    try
                    {
                        sb.AppendLine("Boolean Value: " + result.ToBoolean());
                    }
                    catch { }

                    txtResult.Text = sb.ToString();
                    AppendLog("Read success: " + result.RawResponse);
                }
                else
                {
                    txtResult.Text = "Read failed: " + (result.Error != null ? result.Error.Message : "Unknown error");
                    AppendLog("Read failed: " + result.Error.Message);
                }
            }
            catch (Exception ex)
            {
                txtResult.Text = "Exception: " + ex.Message;
                AppendLog("Read exception: " + ex.Message);
            }
            finally
            {
                btnRead.Enabled = true;
            }
        }

        private async void btnWrite_Click(object sender, EventArgs e)
        {
            if (!_client.IsConnected)
            {
                MessageBox.Show("Please connect to PLC first.", "Not Connected", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string address = txtAddress.Text.Trim();
            string value = txtWriteValue.Text.Trim();

            if (string.IsNullOrEmpty(address))
            {
                MessageBox.Show("Please enter an address.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (string.IsNullOrEmpty(value))
            {
                MessageBox.Show("Please enter a value to write.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            btnWrite.Enabled = false;
            AppendLog("Writing address: " + address + " = " + value);

            try
            {
                await _client.WriteItemAsync(address, value);
                txtResult.Text = "Write success: " + address + " = " + value;
                AppendLog("Write success.");
            }
            catch (Exception ex)
            {
                txtResult.Text = "Write failed: " + ex.Message;
                AppendLog("Write exception: " + ex.Message);
            }
            finally
            {
                btnWrite.Enabled = true;
            }
        }

        private async void btnReadContinuous_Click(object sender, EventArgs e)
        {
            if (!_client.IsConnected)
            {
                MessageBox.Show("Please connect to PLC first.", "Not Connected", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string address = txtAddress.Text.Trim();
            int count;

            if (string.IsNullOrEmpty(address))
            {
                MessageBox.Show("Please enter a start address.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!int.TryParse(txtReadCount.Text, out count) || count <= 0)
            {
                MessageBox.Show("Invalid read count.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            btnReadContinuous.Enabled = false;
            AppendLog("Reading continuous from " + address + ", count: " + count);

            try
            {
                var result = await _client.ReadContinuousAsync(address, count);

                if (result.IsSuccess)
                {
                    var sb = new StringBuilder();
                    sb.AppendLine("Start Address: " + result.Address);
                    sb.AppendLine("Count: " + count);
                    sb.AppendLine("Raw Response: " + result.RawResponse);

                    try
                    {
                        var values = result.ToIntArray();
                        sb.AppendLine("Values:");
                        for (int i = 0; i < values.Length; i++)
                        {
                            sb.AppendLine("  [" + i + "]: " + values[i]);
                        }
                    }
                    catch { }

                    txtResult.Text = sb.ToString();
                    AppendLog("Continuous read success.");
                }
                else
                {
                    txtResult.Text = "Read failed: " + (result.Error != null ? result.Error.Message : "Unknown error");
                    AppendLog("Continuous read failed.");
                }
            }
            catch (Exception ex)
            {
                txtResult.Text = "Exception: " + ex.Message;
                AppendLog("Continuous read exception: " + ex.Message);
            }
            finally
            {
                btnReadContinuous.Enabled = true;
            }
        }

        private void btnClearLog_Click(object sender, EventArgs e)
        {
            txtLog.Clear();
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                _client.Dispose();
            }
            catch
            {
            }
        }

        private void OnClientConnected(object sender, ConnectionEventArgs e)
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => OnClientConnected(sender, e)));
                return;
            }

            UpdateStatus("Connected");
            btnConnect.Enabled = false;
            btnDisconnect.Enabled = true;
        }

        private void OnClientDisconnected(object sender, ConnectionEventArgs e)
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => OnClientDisconnected(sender, e)));
                return;
            }

            UpdateStatus("Disconnected");
            btnConnect.Enabled = true;
            btnDisconnect.Enabled = false;
        }

        private void UpdateStatus(string status)
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => UpdateStatus(status)));
                return;
            }

            lblStatus.Text = "Status: " + status;
        }

        private void AppendLog(string message)
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => AppendLog(message)));
                return;
            }

            lock (_logLock)
            {
                string timestamp = DateTime.Now.ToString("HH:mm:ss.fff");
                txtLog.AppendText("[" + timestamp + "] " + message + Environment.NewLine);
            }
        }
    }
}
