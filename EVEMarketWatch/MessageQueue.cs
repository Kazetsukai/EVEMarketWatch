using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVEMarketWatch
{
    class MessageQueue<T>
    {
        private Queue<string> _messages = new Queue<string>();
        private object _syncObject = new object();

        public List<string> Messages
        {
            get
            {
                List<string> retMessages = new List<string>();
                lock (_syncObject)
                {
                    if (_messages.Count > 0)
                        retMessages.AddRange(_messages);

                    _messages.Clear();
                }

                return retMessages;
            }
        }

        public void SetMessage(string message)
        {
            lock (_syncObject)
                _messages.Enqueue(message);

        }
    }
}
