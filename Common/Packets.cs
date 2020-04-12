using System;
using System.Runtime.InteropServices;

namespace Common
{
    public class DataPacket
    {
        public string header;
        public object data;


        public DataPacket(string header, object data)
        {
            this.header = header;
            this.data = data;
        }

        public DataPacket()
        {
        }
    }
}