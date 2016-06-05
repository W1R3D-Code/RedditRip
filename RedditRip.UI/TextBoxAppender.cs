using System;
using System.Windows.Forms;
using log4net.Appender;
using log4net.Core;

namespace RedditRip.UI
{
    public class TextBoxAppender : AppenderSkeleton
    {
        private TextBox textBox;
        public string FormName { get; set; }
        public string TextBoxName { get; set; }

        protected override void Append(LoggingEvent loggingEvent)
        {
            if (string.IsNullOrEmpty(FormName) ||
                    string.IsNullOrEmpty(TextBoxName))
                return;

            var form = Application.OpenForms[FormName] as Main;

            form?.AppendLog(loggingEvent.RenderedMessage + Environment.NewLine);
        }
    }
}
