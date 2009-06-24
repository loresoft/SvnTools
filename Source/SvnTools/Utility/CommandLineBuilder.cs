using System;
using System.Text;
using System.Text.RegularExpressions;

// $Id$

namespace SvnTools.Utility
{
    public class CommandLineBuilder
    {
        private static readonly Regex _allowedUnquoted = new Regex(@"^[a-z\\/:0-9\._+\-=]*$", RegexOptions.IgnoreCase);
        private static readonly Regex _definitelyNeedQuotes = new Regex(@"[|><\s,;]+", RegexOptions.None);
        private readonly StringBuilder _commandLine = new StringBuilder();

        protected StringBuilder CommandLine
        {
            get { return _commandLine; }
        }

        public void AppendFileNameIfNotNull(string fileName)
        {
            if (fileName == null)
                return;

            VerifyThrowNoEmbeddedDoubleQuotes(string.Empty, fileName);
            AppendSpaceIfNotEmpty();
            AppendFileNameWithQuoting(fileName);
        }

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

        protected void AppendSpaceIfNotEmpty()
        {
            if (CommandLine.Length == 0)
                return;

            CommandLine.Append(" ");
        }

        public void AppendSwitch(string switchName)
        {
            VerifyThrowArgumentNull(switchName, "switchName");
            AppendSpaceIfNotEmpty();
            AppendTextUnquoted(switchName);
        }

        public void AppendSwitchIfTrue(string switchName, bool condition)
        {
            VerifyThrowArgumentNull(switchName, "switchName");
            if (condition)
                AppendSwitch(switchName);
        }

        public void AppendSwitchIfNotNull(string switchName, string parameter)
        {
            VerifyThrowArgumentNull(switchName, "switchName");
            if (parameter == null)
                return;

            VerifyThrowNoEmbeddedDoubleQuotes(switchName, parameter);
            AppendSwitch(switchName);
            AppendTextWithQuoting(parameter);
        }

        public void AppendSwitchIfNotDefault<T>(string switchName, T parameter) where T : IEquatable<T>
        {
            AppendSwitchIfNotDefault(switchName, parameter, default(T));
        }

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

        public void AppendSwitchUnquotedIfNotNull(string switchName, string parameter)
        {
            VerifyThrowArgumentNull(switchName, "switchName");
            if (parameter == null)
                return;

            AppendSwitch(switchName);
            AppendTextUnquoted(parameter);
        }

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

        protected void AppendTextUnquoted(string textToAppend)
        {
            if (textToAppend == null)
                return;

            CommandLine.Append(textToAppend);
        }

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

        public override string ToString()
        {
            return CommandLine.ToString();
        }

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

        internal static void VerifyThrowArgumentNull(object parameter, string parameterName)
        {
            if (parameter == null)
                throw new ArgumentNullException(parameterName);
        }
    }
}