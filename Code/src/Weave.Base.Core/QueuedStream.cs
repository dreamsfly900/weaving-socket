using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace Fleck
{
    /// <summary>
    /// 包装流并对多个写入操作进行排队。
    /// 对于包装 slstream 非常有用, 因为它不支持多个同时写入操作。
    /// </summary>
    public class QueuedStream : Stream
    {
        readonly Stream _stream;
        readonly Queue<WriteData> _queue = new Queue<WriteData>();
        int _pendingWrite;
        bool _disposed;

        public QueuedStream(Stream stream)
        {
            _stream = stream;
        }

        public override bool CanRead
        {
            get { return _stream.CanRead; }
        }

        public override bool CanSeek
        {
            get { return _stream.CanSeek; }
        }

        public override bool CanWrite
        {
            get { return _stream.CanWrite; }
        }

        public override long Length
        {
            get { return _stream.Length; }
        }

        public override long Position
        {
            get { return _stream.Position; }
            set { _stream.Position = value; }
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            return _stream.Read(buffer, offset, count);
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            return _stream.Seek(offset, origin);
        }

        public override void SetLength(long value)
        {
            _stream.SetLength(value);
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            throw new NotSupportedException("队列流还不支持同步写入操作。");
        }

        public override IAsyncResult BeginRead(byte[] buffer, int offset, int count, AsyncCallback callback, object state)
        {
            return _stream.BeginRead(buffer, offset, count, callback, state);
        }

        public override IAsyncResult BeginWrite(byte[] buffer, int offset, int count, AsyncCallback callback, object state)
        {
            lock (_queue)
            {
                var data = new WriteData(buffer, offset, count, callback, state);
                if (_pendingWrite > 0)
                {
                    _queue.Enqueue(data);
                    return data.AsyncResult;
                }
                return BeginWriteInternal(buffer, offset, count, callback, state, data);
            }
        }

        public override int EndRead(IAsyncResult asyncResult)
        {
            return _stream.EndRead(asyncResult);
        }

        public override void EndWrite(IAsyncResult asyncResult)
        {
            if (asyncResult is QueuedWriteResult)
            {
                var queuedResult = asyncResult as QueuedWriteResult;
                if (queuedResult.Exception != null) throw queuedResult.Exception;
                var ar = queuedResult.ActualResult;
                if (ar == null)
                {
                    throw new NotSupportedException(
                        "队列流不支持同步写入操作。在调用 endwrite 之前, 请等待调用回调。");
                }
            }
            else
            {
                throw new ArgumentException();
            }
        }

        public override void Flush()
        {
            _stream.Flush();
        }

        public override void Close()
        {
            _stream.Close();
        }

        protected override void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _stream.Dispose();
                }
                _disposed = true;
            }
            base.Dispose(disposing);
        }

        IAsyncResult BeginWriteInternal(byte[] buffer, int offset, int count, AsyncCallback callback, object state, WriteData queued)
        {
            _pendingWrite++;
            var result = _stream.BeginWrite(buffer, offset, count, ar =>
            {
                // 即使在将BeginWriteInternal的返回值设置为此属性之前，也可以执行回调
                queued.AsyncResult.ActualResult = ar;
                try
                {
                    // 这样我们就可以再次调用BeginWrite
                    _stream.EndWrite(ar);
                }
                catch (Exception exc)
                {
                    queued.AsyncResult.Exception = exc;
                }

                // 一个下来，另一个好进行
                lock (_queue)
                {
                    _pendingWrite--;
                    while (_queue.Count > 0)
                    {
                        var data = _queue.Dequeue();
                        try
                        {
                            data.AsyncResult.ActualResult = BeginWriteInternal(data.Buffer, data.Offset, data.Count, data.Callback, data.State, data);
                            break;
                        }
                        catch (Exception exc)
                        {
                            _pendingWrite--;
                            data.AsyncResult.Exception = exc;
                            data.Callback(data.AsyncResult);
                        }
                    }
                    callback(queued.AsyncResult);
                }
            }, state);

            //始终返回包装的异步结果。
            //如果底层流同步完成操作，则这一点尤其重要（因此"result.CompletedSynchronously"为真！）
            queued.AsyncResult.ActualResult = result;
            return queued.AsyncResult;
        }

        #region Nested type: WriteData

        class WriteData
        {
            public readonly byte[] Buffer;
            public readonly int Offset;
            public readonly int Count;
            public readonly AsyncCallback Callback;
            public readonly object State;
            public readonly QueuedWriteResult AsyncResult;

            public WriteData(byte[] buffer, int offset, int count, AsyncCallback callback, object state)
            {
                Buffer = buffer;
                Offset = offset;
                Count = count;
                Callback = callback;
                State = state;
                AsyncResult = new QueuedWriteResult(state);
            }
        }

        #endregion

        #region Nested type: QueuedWriteResult

        class QueuedWriteResult : IAsyncResult
        {
            public QueuedWriteResult(object state)
            {
                AsyncState = state;
            }

            public Exception Exception { get; set; }

            public IAsyncResult ActualResult { get; set; }

            public object AsyncState { get; private set; }

            public WaitHandle AsyncWaitHandle
            {
                get { throw new NotSupportedException("Queued write operations do not support wait handle."); }
            }

            public bool CompletedSynchronously
            {
                get { return false; }
            }

            public bool IsCompleted
            {
                get { return ActualResult != null && ActualResult.IsCompleted; }
            }
        }

        #endregion
    }
}
