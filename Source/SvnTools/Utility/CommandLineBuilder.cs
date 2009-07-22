using System;
using System.Text;
using System.Text.RegularExpressions;

// $Id$

namespace SvnTools.Utility
{
    /// <summary>
    /// Comprises utility methods for constructing a command line.
    /// </summary>
    public class CommandLineBuilder
    {
        private static readonly Regex _allowedUnquoted = new Regex(@"^[a-z\\/:0-9\._+\-=]*$", RegexOptions.IgnoreCase);
        private static readonly Regex _definitelyNeedQuotes = new Regex(@"[|><\s,;]+", RegexOptions.None);
        private readonly StringBuilder _commandLine;

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandLineBuilder"/> class.
        /// </summary>
        public CommandLineBuilder()
        {
            _commandLine = new StringBuilder();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandLineBuilder"/> class.
        /// </summary>
        /// <param name="commandLine">The command line to start with.</param>
        public CommandLineBuilder(string commandLine)
        {
            _commandLine = new StringBuilder(commandLine);
        }

        /// <summary>
        /// Gets the command line buffer.
        /// </summary>
        /// <value>The command line.</value>
        protected StringBuilder CommandLine
        {
            get { return _commandLine; }
        }

        /// <summary>
        /// Appends the command line with file name represented by the parameter, inserting quotation marks if necessary.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        public void AppendFileNameIfNotNull(string fileName)
        {
            if (fileName == null)
                return;

            VerifyThrowNoEmbeddedDoubleQuotes(string.Empty, fileName);
            AppendSpaceIfNotEmpty();
            AppendFileNameWithQuoting(fileName);
        }

        /// <summary>
        /// Appends the command line with a list of file names, inserting quotation marks if necessary. 
        /// </summary>
        /// <param name="fileNames">The file names to append. If the array is null reference, then this method has no effect.</param>
        /// <param name="delimiter">The delimiter to put between file names in the command line.</param>
        public void AppendFileNamesIfNotNull(string[] fileNames, string delimiter)
        {
            VerifyThrowArgumentNull(delimiter, "delimiter");

            if ((fileNames == null) || (fileNames.Length <= 0))
                return;

            for (int i = 0; i < fileNames.Length; i++)
                VerifyThrowNoEmbeddedDoubleQuotes(string.Empty, fileNames[i]);

            AppendSpaceIfNotEmpty();
            for (int j = 0; j < fileNames.Length; j++)
            {
                if (j != 0)
                    AppendTextUnquoted(delimiter);

                AppendFileNameWithQuoting(fileNames[j]);
            }
        }

        /// <summary>
        /// Appends the command line with a file name, and surrounds the file name with quotation marks as necessary.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        protected void AppendFileNameWithQuoting(string fileName)
        {
            if (fileName == null)
                return;

            VerifyThrowNoEmbeddedDoubleQuotes(string.Empty, fileName);

            if ((fileName.Length != 0) && (fileName[0] == '-'))
                AppendTextWithQuoting(@".\" + fileName);
            else
                AppendTextWithQuoting(fileName);
        }

        /// <summary>
        /// Appends the space if command line is not empty.
        /// </summary>
        protected void AppendSpaceIfNotEmpty()
        {
            if (CommandLine.Length == 0)
                return;

            CommandLine.Append(" ");
        }

        /// <summary>
        /// Appends the command line with the specified switch. 
        /// </summary>
        /// <param name="switchName">Name of the switch.</param>
        public void AppendSwitch(string switchName)
        {
            VerifyThrowArgumentNull(switchName, "switchName");
            AppendSpaceIfNotEmpty();
            AppendTextUnquoted(switchName);
        }

        /// <summary>
        /// Appends the switch if true.
        /// </summary>
        /// <param name="switchName">Name of the switch.</param>
        /// <param name="condition">if set to <c>true</c> [condition].</param>
        public void AppendSwitchIfTrue(string switchName, bool condition)
        {
            VerifyThrowArgumentNull(switchName, "switchName");
            if (condition)
                AppendSwitch(switchName);
        }

        /// <summary>
        /// Appends the switch if not null.
        /// </summary>
        /// <param name="switchName">Name of the switch.</param>
        /// <param name="parameter">The parameter.</param>
        public void AppendSwitchIfNotNull(string switchName, string parameter)
        {
            VerifyThrowArgumentNull(switchName, "switchName");
            if (parameter == null)
                return;

            VerifyThrowNoEmbeddedDoubleQuotes(switchName, parameter);
            AppendSwitch(switchName);
            AppendTextWithQuoting(parameter);
        }

        /// <summary>
        /// Appends the switch if not default.
        /// </summary>
        /// <typeparam name="T">Type of the parameter</typeparam>
        /// <param name="switchName">Name of the switch.</param>
        /// <param name="parameter">The parameter.</param>
        public void AppendSwitchIfNotDefault<T>(string switchName, T parameter) where T : IEquatable<T>
        {
            AppendSwitchIfNotDefault(switchName, parameter, default(T));
        }

        /// <summary>
        /// Appends the switch if not default.
        /// </summary>
        /// <typeparam name="T">Type of the parameter</typeparam>
        /// <param name="switchName">Name of the switch.</param>
        /// <param name="parameter">The parameter.</param>
        /// <param name="defaultValue">The default value.</param>
        public void AppendSwitchIfNotDefault<T>(string switchName, T parameter, T defaultValue) where T : IEquatable<T>
        {
            VerifyThrowArgumentNull(switchName, "switchName");
            if (parameter.Equals(defaultValue))
                return;

            string param = parameter.ToString();
            VerifyThrowNoEmbeddedDoubleQuotes(switchName, param);
            AppendSwitch(switchName);
            AppendTextWithQuoting(param);
        }

        /// <summary>
        /// Appends the switch if not null.
        /// </summary>
        /// <param name="switchName">Name of the switch.</param>
        /// <param name="parameters">The parameters.</param>
        /// <param name="delimiter">The delimiter.</param>
        public void AppendSwitchIfNotNull(string switchName, string[] parameters, string delimiter)
        {
            VerifyThrowArgumentNull(switchName, "switchName");
            VerifyThrowArgumentNull(delimiter, "delimiter");
            if ((parameters == null) || (parameters.Length <= 0))
                return;

            foreach (string parameter in parameters)
                VerifyThrowNoEmbeddedDoubleQuotes(switchName, parameter);

            AppendSwitch(switchName);
            bool flag = true;

            foreach (string parameter in parameters)
            {
                if (!flag)
                    AppendTextUnquoted(delimiter);

                flag = false;
                AppendTextWithQuoting(parameter);
            }
        }

        /// <summary>
        /// Appends the switch unquoted if not null.
        /// </summary>
        /// <param name="switchName">Name of the switch.</param>
        /// <param name="parameter">The parameter.</param>
        public void AppendSwitchUnquotedIfNotNull(string switchName, string parameter)
        {
            VerifyThrowArgumentNull(switchName, "switchName");
            if (parameter == null)
                return;

            AppendSwitch(switchName);
            AppendTextUnquoted(parameter);
        }

        /// <summary>
        /// Appends the switch unquoted if not null.
        /// </summary>
        /// <param name="switchName">Name of the switch.</param>
        /// <param name="parameters">The parameters.</param>
        /// <param name="delimiter">The delimiter.</param>
        public void AppendSwitchUnquotedIfNotNull(string switchName, string[] parameters, string delimiter)
        {
            VerifyThrowArgumentNull(switchName, "switchName");
            VerifyThrowArgumentNull(delimiter, "delimiter");
            if ((parameters == null) || (parameters.Length <= 0))
                return;

            AppendSwitch(switchName);
            bool flag = true;
            foreach (string parameter in parameters)
            {
                if (!flag)
                    AppendTextUnquoted(delimiter);

                flag = false;
                AppendTextUnquoted(parameter);
            }
        }

        /// <summary>
        /// Appends the text unquoted.
        /// </summary>
        /// <param name="textToAppend">The text to append.</param>
        protected void AppendTextUnquoted(string textToAppend)
        {
            if (textToAppend == null)
                return;

            CommandLine.Append(textToAppend);
        }

        /// <summary>
        /// Appends the text with quoting.
        /// </summary>
        /// <param name="textToAppend">The text to append.</param>
        protected void AppendTextWithQuoting(string textToAppend)
        {
            if (textToAppend == null)
                return;

            bool isQuotingRequired = IsQuotingRequired(textToAppend);
            if (isQuotingRequired)
                CommandLine.Append('"');

            CommandLine.Append(textToAppend);

            if (isQuotingRequired && textToAppend.EndsWith(@"\", StringComparison.Ordinal))
                CommandLine.Append('\\');

            if (isQuotingRequired)
                CommandLine.Append('"');
        }

        /// <summary>
        /// Determines whether [is quoting required] [the specified parameter].
        /// </summary>
        /// <param name="parameter">The parameter.</param>
        /// <returns>
        /// 	<c>true</c> if [is quoting required] [the specified parameter]; otherwise, <c>false</c>.
        /// </returns>
        protected virtual bool IsQuotingRequired(string parameter)
        {
            bool isQuotingRequired = false;
            if (parameter != null)
            {
                bool isUnquotable = _allowedUnquoted.IsMatch(parameter);
                bool needQuotes = _definitelyNeedQuotes.IsMatch(parameter);
                isQuotingRequired = !isUnquotable || needQuotes;
            }
            return isQuotingRequired;
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return CommandLine.ToString();
        }

        /// <summary>
        /// Verifies the throw no embedded double quotes.
        /// </summary>
        /// <param name="switchName">Name of the switch.</param>
        /// <param name="parameter">The parameter.</param>
        protected virtual void VerifyThrowNoEmbeddedDoubleQuotes(string switchName, string parameter)
        {
            if (parameter == null)
                return;

            bool hasQuote = -1 != parameter.IndexOf('"');

            if (!hasQuote)
                return;

            string message;

            if (string.IsNullOrEmpty(switchName))
                message = string.Format("Illegal quote in the command line value [{0}].", parameter);
            else
                message = string.Format("Illegal quote passed to the command line switch named \"{0}\". The value was [{1}].", switchName, parameter);

            throw new ArgumentException(message, "parameter");
        }

        /// <summary>
        /// Verifies the throw argument null.
        /// </summary>
        /// <param name="parameter">The parameter.</param>
        /// <param name="parameterName">Name of the parameter.</param>
        internal static void VerifyThrowArgumentNull(object parameter, string parameterName)
        {
            if (parameter == null)
                throw new ArgumentNullException(parameterName);
        }
    }
}