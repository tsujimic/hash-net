using System;
using System.IO;
using System.Diagnostics;

using Framework;

namespace Framework.IO
{
    public class FileReader : IReaderCloser, IDisposable
    {
        private Stream stream;

        public FileReader(Stream stream)
        {
            this.stream = stream;
        }

        ~FileReader()
        {
            // Do not re-create Dispose clean-up code here.
            // Calling Dispose(false) is optimal in terms of
            // readability and maintainability.
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            // Take yourself off the Finalization queue 
            // to prevent finalization code for this object
            // from executing a second time.
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
        }

        public virtual Stream BaseStream
        {
            get { return stream; }
        }


        public long Length {
            set { }
            get { return stream.Length; }
        }


        public int Read(byte[] buffer, int index, int count)
        {
            return stream.Read(buffer, index, count);
        }

        public void Close()
        {
            stream.Close();
        }

    }

}
