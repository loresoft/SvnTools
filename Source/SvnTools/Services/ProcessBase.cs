using System;
using System.Collections;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;

// $Id$

namespace SvnTools.Services
{
    /// <summary>
    /// A base class to run a process.
    /// </summary>
    public abstract class ProcessBase : IDisposable
    {
        private int _exitCode;

        private readonly StringBuilder _standardErrorData;
        private readonly StringBuilder _standardOutputData;

        private ProcessStatus _status = ProcessStatus.Ready;
        private int _timeout;

        private string _toolPath;

        private ManualResetEvent _toolExited;
        private ManualResetEvent _toolTimeoutExpired;
        private Timer _toolTimer;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProcessBase"/> class.
        /// </summary>
        protected ProcessBase()
        {
            _timeout = -1;
            _standardErrorData = new StringBuilder();
            _standardOutputData = new StringBuilder();
        }

        /// <summary>
        /// Gets the override value of the PATH environment variable. 
        /// </summary>
        /// <value>The override value of the PATH environment variable.</value>
        protected virtual StringDictionary EnvironmentOverride
        {
            get { return null; }
        }

        /// <summary>
        /// Gets the returned exit code of the executable file. 
        /// </summary>
        /// <value>The returned exit code of the executable file. </value>
        public int ExitCode
        {
            get { return _exitCode; }
        }

        /// <summary>
        /// Gets the status of the process.
        /// </summary>
        /// <value>The status of the process.</value>
        public ProcessStatus Status
        {
            get { return _status; }
        }

        /// <summary>
        /// Gets the Encoding of the standard error stream of the task.
        /// </summary>
        /// <value>The Encoding of the standard error stream of the task.</value>
        protected virtual Encoding StandardErrorEncoding
        {
            get { return Encoding.Default; }
        }

        /// <summary>
        /// Gets the Encoding of the standard output stream of the task.
        /// </summary>
        /// <value>The Encoding of the standard output stream of the task.</value>
        protected virtual Encoding StandardOutputEncoding
        {
            get { return Encoding.Default; }
        }

        /// <summary>
        /// Gets or sets the amount of time after which the task executable is terminated. 
        /// </summary>
        /// <value>The amount of time after which the task executable is terminated.</value>
        public virtual int Timeout
        {
            get { return _timeout; }
            set { _timeout = value; }
        }

        /// <summary>
        /// Gets the name of the executable file to run. 
        /// </summary>
        /// <value>The name of the executable file to run.</value>
        protected abstract string ToolName { get; }

        /// <summary>
        /// Gets or sets the path of the executable file to run.
        /// </summary>
        /// <value>The path of the executable file to run.</value>
        public string ToolPath
        {
            get { return _toolPath; }
            set { _toolPath = value; }
        }

        /// <summary>
        /// Gets the standard output stream.
        /// </summary>
        /// <value>The standard output.</value>
        public string StandardOutput
        {
            get { return _standardOutputData.ToString(); }
        }

        /// <summary>
        /// Gets the standard error stream.
        /// </summary>
        /// <value>The standard error.</value>
        public string StandardError
        {
            get { return _standardErrorData.ToString(); }
        }

        /// <summary>
        /// Gets or sets the standard error writer.
        /// </summary>
        /// <value>The standard error writer.</value>
        protected TextWriter StandardErrorWriter { get; set; }

        /// <summary>
        /// Gets or sets the standard output writer.
        /// </summary>
        /// <value>The standard output writer.</value>
        protected TextWriter StandardOutputWriter { get; set; }

        #region IDisposable Members

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion

        private string ComputePathToTool()
        {
            return string.IsNullOrEmpty(ToolPath)
                ? GenerateFullPathToTool()
                : Path.Combine(ToolPath, ToolName);
        }

        private void ConfirmProcessExit(Process proc)
        {
            while (!proc.HasExited)
                Thread.Sleep(50);
        }

        /// <summary>
        /// Deletes the specified temporary file. 
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        protected void DeleteTempFile(string fileName)
        {
            try
            {
                if (File.Exists(fileName))
                    File.Delete(fileName);
            }
            catch (IOException)
            { }
        }

        /// <summary>
        /// Runs the exectuable file with the specified task parameters.
        /// </summary>
        /// <returns><c>true</c> if the task runs successfully; otherwise, <c>false</c>.</returns>
        public bool Execute()
        {
            try
            {
                if (!ValidateParameters())
                    return false;

                if (SkipTaskExecution())
                    return true;

                string commandLineCommands = GenerateCommandLineCommands() ?? string.Empty;

                if (!string.IsNullOrEmpty(commandLineCommands))
                    commandLineCommands = " " + commandLineCommands;

                string pathToTool = ComputePathToTool();
                if (pathToTool == null)
                    return false;

                _exitCode = 0;
                _exitCode = ExecuteTool(pathToTool, commandLineCommands);
            }
            catch
            {
                _status = ProcessStatus.Error;
                _exitCode = -1;
                throw;
            }

            return (_exitCode == 0);
        }

        /// <summary>
        /// Runs the executable file. 
        /// </summary>
        /// <param name="pathToTool">The path to tool.</param>
        /// <param name="commandLineCommands">The command line arguments.</param>
        /// <returns>The returned exit code of the executable file.</returns>
        protected virtual int ExecuteTool(string pathToTool, string commandLineCommands)
        {
            Process proc = null;

            _standardErrorData.Length = 0;
            _standardOutputData.Length = 0;

            if (StandardErrorWriter == null)
                StandardErrorWriter = TextWriter.Synchronized(new StringWriter(_standardErrorData));

            if (StandardOutputWriter == null)
                StandardOutputWriter = TextWriter.Synchronized(new StringWriter(_standardOutputData));

            _toolExited = new ManualResetEvent(false);
            _toolTimeoutExpired = new ManualResetEvent(false);
            _status = ProcessStatus.Running;

            try
            {
                proc = new Process();
                proc.StartInfo = GetProcessStartInfo(pathToTool, commandLineCommands);
                proc.EnableRaisingEvents = true;
                proc.Exited += ReceiveExitNotification;
                proc.ErrorDataReceived += ReceiveStandardErrorData;
                proc.OutputDataReceived += ReceiveStandardOutputData;

                _exitCode = -1;

                proc.Start();
                proc.StandardInput.Close();
                proc.BeginErrorReadLine();
                proc.BeginOutputReadLine();

                _toolTimer = new Timer(ReceiveTimeoutNotification);
                _toolTimer.Change(Timeout, -1);
                HandleToolNotifications(proc);
            }
            finally
            {
                if (proc != null)
                {
                    try
                    {
                        _exitCode = proc.ExitCode;
                    }
                    catch (InvalidOperationException)
                    { }

                    proc.Close();
                    proc = null;
                }

                _toolExited.Close();
                _toolTimeoutExpired.Close();

                StandardErrorWriter.Dispose();
                StandardOutputWriter.Dispose();
                if (_toolTimer != null)
                    _toolTimer.Dispose();
            }
            return _exitCode;
        }

        /// <summary>
        /// Generates the command line arguments.
        /// </summary>
        /// <returns>Returns a string value containing the command line arguments to pass directly to the executable file.</returns>
        protected virtual string GenerateCommandLineCommands()
        {
            return string.Empty;
        }

        /// <summary>
        /// Generates the full path to tool.
        /// </summary>
        /// <returns>Returns the fully qualified path to the executable file.</returns>
        protected virtual string GenerateFullPathToTool()
        {
            return string.IsNullOrEmpty(ToolPath)
                ? ToolName
                : Path.Combine(ToolPath, ToolName);
        }

        private ProcessStartInfo GetProcessStartInfo(string pathToTool, string commandLineCommands)
        {
            string arguments = commandLineCommands;

            var info = new ProcessStartInfo(pathToTool, arguments);
            info.CreateNoWindow = true;
            info.UseShellExecute = false;
            info.RedirectStandardError = true;
            info.RedirectStandardOutput = true;
            info.StandardErrorEncoding = StandardErrorEncoding;
            info.StandardOutputEncoding = StandardOutputEncoding;
            info.RedirectStandardInput = true;

            string workingDirectory = GetWorkingDirectory();
            if (workingDirectory != null)
                info.WorkingDirectory = workingDirectory;

            StringDictionary environmentOverride = EnvironmentOverride;
            if (environmentOverride != null)
            {
                foreach (DictionaryEntry entry in environmentOverride)
                {
                    info.EnvironmentVariables.Remove((string)entry.Key);
                    info.EnvironmentVariables.Add((string)entry.Key, (string)entry.Value);
                }
            }
            return info;
        }

        /// <summary>
        /// Gets the working directory.
        /// </summary>
        /// <returns>Returns the directory in which to run the executable file.</returns>
        protected virtual string GetWorkingDirectory()
        {
            return null;
        }

        private void HandleToolNotifications(Process proc)
        {
            var waitHandles = new WaitHandle[] { _toolTimeoutExpired, _toolExited };

            int handle = WaitHandle.WaitAny(waitHandles);

            if (handle == 0)
            {
                _status = ProcessStatus.TimedOut;
                KillToolProcessOnTimeout(proc);
            }
            else if (handle == 1)
            {
                proc.WaitForExit();
                ConfirmProcessExit(proc);
                _status = ProcessStatus.Complete;
            }
        }

        private void KillToolProcessOnTimeout(Process proc)
        {
            if (proc.HasExited)
                return;

            try
            {
                proc.Kill();
            }
            catch (InvalidOperationException)
            {
            }

            ConfirmProcessExit(proc);
        }

        private void ReceiveExitNotification(object sender, EventArgs e)
        {
            if (_toolExited == null)
                throw new InvalidOperationException("The signaling event for tool exit must be available.");

            _toolExited.Set();
        }

        private void ReceiveStandardErrorData(object sender, DataReceivedEventArgs e)
        {
            if (e.Data == null)
                return;

            StandardErrorWriter.WriteLine(e.Data);
        }

        private void ReceiveStandardOutputData(object sender, DataReceivedEventArgs e)
        {
            if (e.Data == null)
                return;

            StandardOutputWriter.WriteLine(e.Data);
        }

        private void ReceiveTimeoutNotification(object unused)
        {
            if (_toolTimeoutExpired == null)
                throw new InvalidOperationException("The signaling event for tool time-out must be available.");

            _toolTimeoutExpired.Set();
        }

        /// <summary>
        /// Indicates whether task execution should be skipped. 
        /// </summary>
        /// <returns><c>true</c> to skip task execution; otherwise, <c>false</c>.</returns>
        protected virtual bool SkipTaskExecution()
        {
            return false;
        }

        /// <summary>
        /// Indicates whether all task parameters are valid. 
        /// </summary>
        /// <returns><c>true</c> if all task parameters are valid; otherwise, <c>false</c>c>.</returns>
        protected internal virtual bool ValidateParameters()
        {
            return true;
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (StandardErrorWriter != null)
                    StandardErrorWriter.Dispose();

                if (StandardOutputWriter != null)
                    StandardOutputWriter.Dispose();

                if (_toolTimer != null)
                    _toolTimer.Dispose();
            }
        }

        /// <summary>
        /// Appends to the standard error writer.
        /// </summary>
        /// <param name="message">The message to write.</param>
        protected virtual void AppendStandardError(string message)
        {
            StandardErrorWriter.WriteLine(message);
        }

        /// <summary>
        /// Appends to the standard error writer.
        /// </summary>
        /// <param name="format">A composite format string.</param>
        /// <param name="args">An System.Object array containing zero or more objects to format.</param>
        protected virtual void AppendStandardError(string format, params object[] args)
        {
            AppendStandardError(string.Format(format, args));
        }
    }
}