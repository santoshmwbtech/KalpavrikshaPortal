using Microsoft.Practices.EnterpriseLibrary.Logging;

namespace JBNWebAPI.Logger
{
    public class LoggerBlock
    {
        protected LogWriter logWriter;

        public LoggerBlock()
        {
            InitLogging();
        }

        private void InitLogging()
        {
            logWriter = new LogWriterFactory().Create();
            //Logger.SetLogWriter(logWriter, false);
        }

        public LogWriter LogWriter
        {
            get
            {
                return logWriter;
            }
        }
    }
}