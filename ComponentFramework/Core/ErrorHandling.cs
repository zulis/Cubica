using System;
using System.Diagnostics;
using System.Threading;
using System.Windows.Forms;

namespace ComponentFramework.Core
{
    public partial class Core
    {
        /// <summary>
        /// Handles any thread exceptions
        /// </summary>
        private static void UIThreadException(object sender, ThreadExceptionEventArgs t)
        {
            DialogResult result = DialogResult.Cancel;
            try
            {
                result = ShowThreadExceptionDialog("Windows Forms Error", t.Exception);
            }
            catch
            {
                try
                {
                    MessageBox.Show("Fatal Windows Forms Error",
                        "Fatal Windows Forms Error", MessageBoxButtons.AbortRetryIgnore, MessageBoxIcon.Stop);
                }
                finally
                {
                    Application.Exit();
                }
            }

            // Exits the program when the user clicks Abort.
            if (result == DialogResult.Abort)
                Application.Exit();
        }

        private static void UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            try
            {
                Exception ex = (Exception)e.ExceptionObject;
                string errorMsg = "An application error occurred:\n\n";

                // Since we can't prevent the app from terminating, log this to the event log.
                if (!EventLog.SourceExists(Application.ProductName))
                {
                    EventLog.CreateEventSource(Application.ProductName, "Application");
                }

                // Create an EventLog instance and assign its source.
                EventLog myLog = new EventLog();
                myLog.Source = Application.ProductName;
                myLog.WriteEntry(errorMsg + ex.Message + "\n\nStack Trace:\n" + ex.StackTrace);
            }
            catch (Exception exc)
            {
                try
                {
                    MessageBox.Show("Fatal Non-UI Error",
                        "Fatal Non-UI Error. Could not write the error to the event log. Reason: "
                        + exc.Message, MessageBoxButtons.OK, MessageBoxIcon.Stop);
                }
                finally
                {
                    Application.Exit();
                }
            }
        }

        private static DialogResult ShowThreadExceptionDialog(string title, Exception e)
        {
            string errorMsg = "An application error occurred:\n\n";
            errorMsg = errorMsg + e.Message + "\n\nStack Trace:\n" + e.StackTrace;
            return MessageBox.Show(errorMsg, title, MessageBoxButtons.AbortRetryIgnore,
                MessageBoxIcon.Stop);
        }
    }
}
