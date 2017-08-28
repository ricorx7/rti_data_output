using Caliburn.Micro;
using ReactiveUI.Legacy;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RTI
{
    class InputTerminalViewModel : Caliburn.Micro.Screen
    {

        #region Variables

        /// <summary>
        /// ADCP Serial port.
        /// </summary>
        private AdcpSerialPort _adcpSerialPort;

        /// <summary>
        /// GPS Serial Port.
        /// </summary>
        private GpsSerialPort _gpsSerialPort;

        /// <summary>
        /// ADCP Serial port options.
        /// </summary>
        private SerialOptions _serialOptions;

        /// <summary>
        /// GPS Serial port options.
        /// </summary>
        private SerialOptions _gpsSerialOptions;

        /// <summary>
        /// Codec to decode ADCP data.
        /// </summary>
        private AdcpCodec _adcpCodec;

        /// <summary>
        /// Output ViewModel.  Used to pass the ensemble.
        /// </summary>
        private OutputDataViewModel _outputVM;

        #endregion

        #region Properties

        #region Ports

        /// <summary>
        /// List of all the comm ports on the computer.
        /// </summary>
        private List<string> _CommPortList;
        /// <summary>
        /// List of all the comm ports on the computer.
        /// </summary>
        public List<string> CommPortList
        {
            get { return _CommPortList; }
            set
            {
                _CommPortList = value;
                this.NotifyOfPropertyChange(() => this.CommPortList);
            }
        }

        /// <summary>
        /// List of all the baud rate options.
        /// </summary>
        public List<int> BaudRateList { get; set; }

        /// <summary>
        /// Selected COMM Port.
        /// </summary>
        private string _SelectedCommPort;
        /// <summary>
        /// Selected COMM Port.
        /// </summary>
        public string SelectedCommPort
        {
            get { return _SelectedCommPort; }
            set
            {
                _SelectedCommPort = value;
                this.NotifyOfPropertyChange(() => this.SelectedCommPort);

                // Set the serial options
                _serialOptions.Port = value;

                // Reconnect the ADCP
                ReconnectAdcpSerial(_serialOptions);

                // Reset check to update
                //this.NotifyOfPropertyChange(() => this.CanUpdate);
            }
        }

        /// <summary>
        /// Selected baud rate.
        /// </summary>
        private int _SelectedBaud;
        /// <summary>
        /// Selected baud rate.
        /// </summary>
        public int SelectedBaud
        {
            get { return _SelectedBaud; }
            set
            {
                _SelectedBaud = value;
                this.NotifyOfPropertyChange(() => this.SelectedBaud);

                // Set the serial options
                _serialOptions.BaudRate = value;

                // Reconnect the ADCP
                ReconnectAdcpSerial(_serialOptions);

                // Reset check to update
                //this.NotifyOfPropertyChange(() => this.CanUpdate);
            }
        }

        #endregion

        #region ADCP Display Output

        /// <summary>
        /// Serial Output.
        /// </summary>
        private string _SerialOutput;
        /// <summary>
        /// Serial Output.
        /// </summary>
        public string SerialOutput
        {
            get { return _SerialOutput; }
            set
            {
                _SerialOutput = value;
                this.NotifyOfPropertyChange(() => this.SerialOutput);
            }
        }

        /// <summary>
        /// Serial Output Tooltip.
        /// </summary>
        public string SerialOutputTooltip
        {
            get
            {
                return "Receive the ADCP data.  Decode the data and display some of the information.  This data will be used to reprocess.";
            }
        }

        /// <summary>
        /// Decoded ADCP output.
        /// </summary>
        private string _AdcpDecodedOutput;
        /// <summary>
        /// Decoded ADCP output.
        /// </summary>
        public string AdcpDecodedOutput
        {
            get { return _AdcpDecodedOutput; }
            set
            {
                _AdcpDecodedOutput = value;
                this.NotifyOfPropertyChange(() => this.AdcpDecodedOutput);
            }
        }

        

        #endregion

        #region Commands

        /// <summary>
        /// ADCP Commands Tooltip.
        /// </summary>
        public string AdcpCommandsTooltip
        {
            get
            {
                return "Commands to send to the ADCP.";
            }
        }

        /// <summary>
        /// Serial command to send.
        /// </summary>
        private string _SerialCmd;
        /// <summary>
        /// Serial command to send.
        /// </summary>
        public string SerialCmd
        {
            get { return _SerialCmd; }
            set
            {
                _SerialCmd = value;
                this.NotifyOfPropertyChange(() => this.SerialCmd);
            }
        }
        

        #endregion

        #region Status

        /// <summary>
        /// Status Output.
        /// </summary>
        private string _StatusOutput;
        /// <summary>
        /// Status Output.
        /// </summary>
        public string StatusOutput
        {
            get { return _StatusOutput; }
            set
            {
                _StatusOutput = value;
                this.NotifyOfPropertyChange(() => this.StatusOutput);
            }
        }

        #endregion

        #region GPS

        /// <summary>
        /// GPS Output.
        /// </summary>
        private string _GpsOutput;
        /// <summary>
        /// GPS Output.
        /// </summary>
        public string GpsOutput
        {
            get { return _GpsOutput; }
            set
            {
                _GpsOutput = value;
                this.NotifyOfPropertyChange(() => this.GpsOutput);
            }
        }

        /// <summary>
        /// Serial Output Tooltip.
        /// </summary>
        public string GpsOutputTooltip
        {
            get
            {
                return "Receive the GPS or Gyro NMEA data.  Display the incoming NMEA data.  This data will be used to reprocess.";
            }
        }

        /// <summary>
        /// List of all the comm ports on the computer.
        /// </summary>
        private List<string> _GpsCommPortList;
        /// <summary>
        /// List of all the comm ports on the computer.
        /// </summary>
        public List<string> GpsCommPortList
        {
            get { return _GpsCommPortList; }
            set
            {
                _GpsCommPortList = value;
                this.NotifyOfPropertyChange(() => this.GpsCommPortList);
            }
        }

        /// <summary>
        /// Selected GPS COMM Port.
        /// </summary>
        private string _SelectedGpsCommPort;
        /// <summary>
        /// Selected GPS COMM Port.
        /// </summary>
        public string SelectedGpsCommPort
        {
            get { return _SelectedGpsCommPort; }
            set
            {
                _SelectedGpsCommPort = value;
                this.NotifyOfPropertyChange(() => this.SelectedGpsCommPort);

                // Set the serial options
                _gpsSerialOptions.Port = value;

                // Reconnect the ADCP
                ReconnectGpsSerial(_gpsSerialOptions);

                // Reset check to update
                //this.NotifyOfPropertyChange(() => this.CanUpdate);
            }
        }

        /// <summary>
        /// Selected GPS baud rate.
        /// </summary>
        private int _SelectedGpsBaud;
        /// <summary>
        /// Selected GPS baud rate.
        /// </summary>
        public int SelectedGpsBaud
        {
            get { return _SelectedGpsBaud; }
            set
            {
                _SelectedGpsBaud = value;
                this.NotifyOfPropertyChange(() => this.SelectedGpsBaud);

                // Set the serial options
                _gpsSerialOptions.BaudRate = value;

                // Reconnect the ADCP
                ReconnectAdcpSerial(_gpsSerialOptions);

                // Reset check to update
                //this.NotifyOfPropertyChange(() => this.CanUpdate);
            }
        }

        #endregion

        #endregion

        #region Commands

        /// <summary>
        /// Command to scan for available ADCP.
        /// </summary>
        public ReactiveCommand<object> ScanAdcpCommand { get; protected set; }

        /// <summary>
        /// Connect the serial port.
        /// </summary>
        public ReactiveCommand<object> ConnectAdcpCommand { get; protected set; }

        /// <summary>
        /// Disconnect the serial port.
        /// </summary>
        public ReactiveCommand<object> DisconnectAdcpCommand { get; protected set; }

        /// <summary>
        /// Command to scan for available GPS.
        /// </summary>
        public ReactiveCommand<object> ScanGpsCommand { get; protected set; }

        /// <summary>
        /// Connect the GPS serial port.
        /// </summary>
        public ReactiveCommand<object> ConnectGpsCommand { get; protected set; }

        /// <summary>
        /// Disconnect the GPS serial port.
        /// </summary>
        public ReactiveCommand<object> DisconnectGpsCommand { get; protected set; }

        /// <summary>
        /// Send BREAK and start pinging.
        /// </summary>
        public ReactiveCommand<object> StartPingingCommand { get; protected set; }

        /// <summary>
        /// Send BREAK and stop pinging.
        /// </summary>
        public ReactiveCommand<object> StopPingingCommand { get; protected set; }

        /// <summary>
        /// Send BREAK and stop pinging.
        /// </summary>
        public ReactiveCommand<object> SendAdcpCmdCommand { get; protected set; }
        

        #endregion


        /// <summary>
        /// Initalize the lists.
        /// </summary>
        /// <param name="name"></param>
        public InputTerminalViewModel(string name)
        {
            base.DisplayName = name;

            _outputVM = IoC.Get<OutputDataViewModel>();

            _serialOptions = new SerialOptions();
            CommPortList = SerialOptions.PortOptions;
            BaudRateList = SerialOptions.BaudRateOptions;
            SelectedBaud = 115200;

            GpsCommPortList = SerialOptions.PortOptions;
            _gpsSerialOptions = new SerialOptions();
            SelectedGpsBaud = 9600;

            StatusOutput = "";

            _adcpCodec = new AdcpCodec();
            _adcpCodec.ProcessDataEvent += _adcpCodec_ProcessDataEvent;

            // Scan for ADCP command
            ScanAdcpCommand = ReactiveUI.Legacy.ReactiveCommand.Create();
            ScanAdcpCommand.Subscribe(_ => ScanForAdcp());

            // Disconnect Serial
            ConnectAdcpCommand = ReactiveUI.Legacy.ReactiveCommand.Create();
            ConnectAdcpCommand.Subscribe(_ => ConnectAdcpSerial());

            // Disconnect Serial
            DisconnectAdcpCommand = ReactiveUI.Legacy.ReactiveCommand.Create();
            DisconnectAdcpCommand.Subscribe(_ => DisconnectAdcpSerial());

            // Scan for GPS command
            ScanGpsCommand = ReactiveUI.Legacy.ReactiveCommand.Create();
            ScanGpsCommand.Subscribe(_ => ScanForGps());

            // Disconnect Serial
            ConnectGpsCommand = ReactiveUI.Legacy.ReactiveCommand.Create();
            ConnectGpsCommand.Subscribe(_ => ConnectGpsSerial());

            // Disconnect Serial
            DisconnectGpsCommand = ReactiveUI.Legacy.ReactiveCommand.Create();
            DisconnectGpsCommand.Subscribe(_ => DisconnectGpsSerial());

            // Start Pinging if not pinging
            StartPingingCommand = ReactiveUI.Legacy.ReactiveCommand.Create();
            StartPingingCommand.Subscribe(_ => StartPinging());

            // Stop Pinging if not pinging
            StopPingingCommand = ReactiveUI.Legacy.ReactiveCommand.Create();
            StopPingingCommand.Subscribe(_ => StopPinging());

            // Send the command to the ADCP
            SendAdcpCmdCommand = ReactiveUI.Legacy.ReactiveCommand.Create();
            SendAdcpCmdCommand.Subscribe(_ => SendAdcpCommand());
            
        }

        /// <summary>
        /// Dispose of the ViewModel.
        /// </summary>
        public void Dispose()
        {
            if (_adcpSerialPort != null)
            {
                DisconnectAdcpSerial();
            }

            if(_gpsSerialPort != null)
            {
                DisconnectGpsSerial();
            }

            _adcpCodec.ProcessDataEvent -= _adcpCodec_ProcessDataEvent;
            _adcpCodec.Dispose();
        }


        #region ADCP Serial Connection

        /// <summary>
        /// Connect the ADCP Serial port.
        /// </summary>
        public void ConnectAdcpSerial()
        {
            ConnectAdcpSerial(_serialOptions);
        }

        /// <summary>
        /// Create a connection to the ADCP serial port with
        /// the given options.  If no options are given, return null.
        /// </summary>
        /// <param name="options">Options to connect to the serial port.</param>
        /// <returns>Adcp Serial Port based off the options</returns>
        public AdcpSerialPort ConnectAdcpSerial(SerialOptions options)
        {
            // If there is a connection, disconnect
            if (_adcpSerialPort != null)
            {
                DisconnectAdcpSerial();
            }

            if (options != null)
            {
                // Set the connection
                //Status.Status = eAdcpStatus.Connected;

                // Create the connection and connect
                _adcpSerialPort = new AdcpSerialPort(options);
                _adcpSerialPort.Connect();


                // Subscribe to receive ADCP data
                _adcpSerialPort.ReceiveAdcpSerialDataEvent += new AdcpSerialPort.ReceiveAdcpSerialDataEventHandler(ReceiveAdcpSerialData);

                // Publish that the ADCP serial port is new
                //PublishAdcpSerialConnection();

                DispalyStatus(string.Format("ADCP Connect: {0}", _adcpSerialPort.ToString()));

                // Set flag
                //IsAdcpFound = true;

                return _adcpSerialPort;
            }

            return null;
        }

        /// <summary>
        /// Shutdown the ADCP serial port.
        /// This will stop all the read threads
        /// for the ADCP serial port.
        /// </summary>
        public void DisconnectAdcpSerial()
        {
            try
            {
                if (_adcpSerialPort != null)
                {
                    DispalyStatus(string.Format("ADCP Disconnect: {0}", _adcpSerialPort.ToString()));

                    // Disconnect the serial port
                    _adcpSerialPort.Disconnect();

                    // Unscribe to ADCP SerialPort events
                    _adcpSerialPort.ReceiveAdcpSerialDataEvent -= ReceiveAdcpSerialData;


                    // Publish that the ADCP serial conneciton is disconnected
                    //PublishAdcpSerialDisconnection();

                    // Shutdown the serial port
                    _adcpSerialPort.Dispose();
                }
                //Status.Status = eAdcpStatus.NotConnected;
            }
            catch (Exception e)
            {
                Debug.WriteLine("Error disconnecting the serial port.", e);
            }
        }

        /// <summary>
        /// Disconnect then connect with the new options given.
        /// </summary>
        /// <param name="options">Options to connect the ADCP serial port.</param>
        public void ReconnectAdcpSerial(SerialOptions options)
        {
            // Disconnect
            DisconnectAdcpSerial();

            // Wait for Disconnect to finish
            Thread.Sleep(RTI.AdcpSerialPort.WAIT_STATE);

            // Connect
            ConnectAdcpSerial(options);
        }

        /// <summary>
        /// Return if the Adcp Serial port is open and connected.
        /// </summary>
        /// <returns>TRUE = Is connected.</returns>
        public bool IsAdcpSerialConnected()
        {
            // See if the connection is open
            if (_adcpSerialPort != null && _adcpSerialPort.IsOpen())
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Scan for available serial ports.
        /// </summary>
        private void ScanForAdcp()
        {
            CommPortList = SerialOptions.PortOptions;
            GpsCommPortList = SerialOptions.PortOptions;
        }

        /// <summary>
        /// Send a BREAK and start pinging.
        /// </summary>
        private void StartPinging()
        {
            _adcpSerialPort.SendBreak();

            // Wait for an output
            Thread.Sleep(RTI.AdcpSerialPort.WAIT_STATE * 4);

            _adcpSerialPort.StartPinging();

            // Wait for an output
            Thread.Sleep(RTI.AdcpSerialPort.WAIT_STATE * 4);

            DispalyStatus("Start ADCP Pinging.");
        }

        /// <summary>
        /// Send a BREAK and stop pinging.
        /// </summary>
        private void StopPinging()
        {
            _adcpSerialPort.StopPinging();

            // Wait for an output
            Thread.Sleep(RTI.AdcpSerialPort.WAIT_STATE * 4);

            DispalyStatus("Stop ADCP Pinging");
        }

        /// <summary>
        /// Send the command to the serial port.
        /// </summary>
        private void SendAdcpCommand()
        {
            _adcpSerialPort.SendDataWaitReply(_SerialCmd);

            DispalyStatus("Send Command: " + _SerialCmd);
        }

        #endregion

        #region GPS Serial Connection

        /// <summary>
        /// Connect the GPS Serial port.
        /// </summary>
        public void ConnectGpsSerial()
        {
            ConnectGpsSerial(_gpsSerialOptions);
        }

        /// <summary>
        /// Create a connection to the GPS serial port with
        /// the given options.  If no options are given, return null.
        /// </summary>
        /// <param name="options">Options to connect to the serial port.</param>
        /// <returns>Adcp Serial Port based off the options</returns>
        public GpsSerialPort ConnectGpsSerial(SerialOptions options)
        {
            // If there is a connection, disconnect
            if (_gpsSerialPort != null)
            {
                DisconnectGpsSerial();
            }

            if (options != null)
            {
                // Set the connection
                //Status.Status = eAdcpStatus.Connected;

                // Create the connection and connect
                _gpsSerialPort = new GpsSerialPort(options);
                _gpsSerialPort.Connect();
                _gpsSerialPort.IsEnabled = true;

                // Subscribe to receive ADCP data
                _gpsSerialPort.ReceiveGpsSerialDataEvent += new GpsSerialPort.ReceiveGpsSerialDataEventHandler(ReceiveGpsSerialData);

                // Publish that the ADCP serial port is new
                //PublishAdcpSerialConnection();

                DispalyStatus(string.Format("GPS Connect: {0}", _gpsSerialPort.ToString()));

                // Set flag
                //IsAdcpFound = true;

                return _gpsSerialPort;
            }

            return null;
        }

        /// <summary>
        /// Shutdown the GPS serial port.
        /// This will stop all the read threads
        /// for the GPS serial port.
        /// </summary>
        public void DisconnectGpsSerial()
        {
            try
            {
                if (_gpsSerialPort != null)
                {
                    DispalyStatus(string.Format("GPS Disconnect: {0}", _gpsSerialPort.ToString()));

                    // Disconnect the serial port
                    _gpsSerialPort.Disconnect();

                    // Unscribe to ADCP SerialPort events
                    _gpsSerialPort.ReceiveGpsSerialDataEvent -= ReceiveGpsSerialData;


                    // Publish that the ADCP serial conneciton is disconnected
                    //PublishAdcpSerialDisconnection();

                    // Shutdown the serial port
                    _gpsSerialPort.Dispose();
                }
                //Status.Status = eAdcpStatus.NotConnected;
            }
            catch (Exception e)
            {
                DispalyStatus("Error disconnecting the GPS serial port. " + e.ToString());
            }
        }

        /// <summary>
        /// Disconnect then connect with the new options given.
        /// </summary>
        /// <param name="options">Options to connect the GPS serial port.</param>
        public void ReconnectGpsSerial(SerialOptions options)
        {
            // Disconnect
            DisconnectGpsSerial();

            // Wait for Disconnect to finish
            Thread.Sleep(RTI.AdcpSerialPort.WAIT_STATE);

            // Connect
            ConnectGpsSerial(options);
        }

        /// <summary>
        /// Return if the Adcp Serial port is open and connected.
        /// </summary>
        /// <returns>TRUE = Is connected.</returns>
        public bool IsGpsSerialConnected()
        {
            // See if the connection is open
            if (_gpsSerialPort != null && _gpsSerialPort.IsOpen())
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Scan for available serial ports.
        /// </summary>
        private void ScanForGps()
        {
            GpsCommPortList = SerialOptions.PortOptions;
        }

        /// <summary>
        /// Send the command to the serial port.
        /// </summary>
        private void SendGpsCommand()
        {
            _gpsSerialPort.SendData(_SerialCmd);

            DispalyStatus("Send GPS Command: " + _SerialCmd);
        }

        #endregion

        #region Status

        /// <summary>
        /// Display the status given.
        /// </summary>
        /// <param name="status">Status message.</param>
        private void DispalyStatus(string status)
        {
            StatusOutput += status + "\n";

            if(StatusOutput.Length > 500)
            {
                StatusOutput = StatusOutput.Substring(StatusOutput.Length - 500);
            }
        }

        #endregion

        #region EventHandler

        /// <summary>
        /// Receive binary data from the ADCP serial port.
        /// Then pass the binary data to the codec to decode the
        /// data into ensembles.
        /// 
        /// The data could be binary or dvl data.
        /// The data will go to both codec and
        /// if the codec can process the data it will.
        /// </summary>
        /// <param name="data">Data to decode.</param>
        public void ReceiveAdcpSerialData(byte[] data)
        {
            if (_adcpCodec != null)
            {
                // Add incoming data to codec
                _adcpCodec.AddIncomingData(data);
            }

        }

        /// <summary>
        /// Receive the GPS data and display it to the output.
        /// </summary>
        /// <param name="data"></param>
        public void ReceiveGpsSerialData(string data)
        {
            if (_adcpCodec != null)
            {
                // Add the data to the codec
                _adcpCodec.AddNmeaData(data);
            }

            // GPS Serial output
            GpsOutput += data;

            if (GpsOutput.Length > 1000)
            {
                GpsOutput = GpsOutput.Substring(GpsOutput.Length - 1000);
            }

        }

        public void _adcpCodec_ProcessDataEvent(byte[] binaryEnsemble, DataSet.Ensemble ensemble)
        {
            // Display the data
            //SerialOutput += System.Text.Encoding.Default.GetString(binaryEnsemble);
            //if (SerialOutput.Length > 500)
            //{
            //    SerialOutput = SerialOutput.Substring(500, SerialOutput.Length - 500);
            //}

            // Pass the ensemble to the Output VM
            _outputVM.ReceiveEnsemble(ensemble);

            // Display the ensemble
            if(ensemble != null && ensemble.IsEnsembleAvail)
            {
                StringBuilder sb = new StringBuilder();

                sb.AppendLine("Ensemble Number: " + ensemble.EnsembleData.EnsembleNumber);
                sb.AppendLine("Date: " + ensemble.EnsembleData.EnsDateString);
                sb.AppendLine("Time: " + ensemble.EnsembleData.EnsTimeString);

                if (ensemble.IsBottomTrackAvail)
                {
                    sb.AppendLine("Status: " + ensemble.BottomTrackData.Status);
                    sb.AppendLine("Depth: " + ensemble.BottomTrackData.GetAverageRange());
                    sb.AppendLine("Boat Speed: " + ensemble.BottomTrackData.GetVelocityMagnitude());
                    sb.AppendLine("Boat Direction: " + ensemble.BottomTrackData.GetVelocityDirection(true));
                }

                sb.AppendLine("Pings: " + ensemble.EnsembleData.ActualPingCount);
                sb.AppendLine("First Ping Time: " + ensemble.AncillaryData.FirstPingTime);
                sb.AppendLine("Last Ping Time: " + ensemble.AncillaryData.LastPingTime);
                sb.AppendLine("Ping Time: " + (ensemble.AncillaryData.LastPingTime - ensemble.AncillaryData.FirstPingTime));
                sb.AppendLine("Temperature: " + ensemble.AncillaryData.WaterTemp);

                if (ensemble.IsAncillaryAvail)
                {
                    double tbp = ensemble.AncillaryData.LastPingTime - ensemble.AncillaryData.FirstPingTime;
                    int tbp_minute = (int)Math.Round(tbp / 60);
                    int tbp_sec = (int)Math.Truncate(tbp - (tbp_minute * 60));
                    int tbp_hun = (int)Math.Round((tbp - tbp_sec) * 100);
                    sb.AppendLine("TBP: " + tbp);
                    sb.AppendLine("TBP Minutes: " + tbp_minute);
                    sb.AppendLine("TBP Sec: " + tbp_sec);
                    sb.AppendLine("TBP Hun: " + tbp_hun);
                }

                if (ensemble.IsNmeaAvail)
                {
                    if (ensemble.NmeaData.IsGpggaAvail())
                    {
                        if (ensemble.NmeaData.IsGpggaAvail())
                        {
                            sb.AppendLine("Lat/Lon: " + ensemble.NmeaData.GPGGA.ToString());
                        }
                        if (ensemble.NmeaData.IsGphdtAvail())
                        {
                            sb.AppendLine("Gyro Heading: " + ensemble.NmeaData.GPHDT.ToString());
                        }
                        if(ensemble.NmeaData.IsGpvtgAvail())
                        {
                            sb.AppendLine("GPS Speed: " + ensemble.NmeaData.GPVTG.ToString());
                        }
                    }
                }

                AdcpDecodedOutput = sb.ToString();
            }
        }

        #endregion

    }
}
