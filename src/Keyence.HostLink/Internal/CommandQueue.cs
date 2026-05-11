using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Keyence.HostLink.Models;
using Keyence.HostLink.Transports;

namespace Keyence.HostLink.Internal
{
    internal class CommandQueue
    {
        private readonly ITransport _transport;
        private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);
        private readonly Queue<QueuedCommand> _queue = new Queue<QueuedCommand>();
        private bool _isProcessing;

        public CommandQueue(ITransport transport)
        {
            _transport = transport;
        }

        public Task<HostLinkResult> EnqueueAsync(byte[] command, string address, int timeoutMilliseconds)
        {
            var tcs = new TaskCompletionSource<HostLinkResult>();
            var queuedCommand = new QueuedCommand
            {
                Command = command,
                Address = address,
                TimeoutMs = timeoutMilliseconds,
                CompletionSource = tcs
            };

            lock (_queue)
            {
                _queue.Enqueue(queuedCommand);
            }

            TriggerProcessQueue();
            return tcs.Task;
        }

        private void TriggerProcessQueue()
        {
            lock (_queue)
            {
                if (_isProcessing)
                    return;
                _isProcessing = true;
            }

            Task.Run(async () =>
            {
                try
                {
                    while (true)
                    {
                        QueuedCommand currentCommand;
                        lock (_queue)
                        {
                            if (_queue.Count == 0)
                                break;
                            currentCommand = _queue.Dequeue();
                        }

                        await ExecuteCommandAsync(currentCommand).ConfigureAwait(false);
                    }
                }
                finally
                {
                    lock (_queue)
                    {
                        _isProcessing = false;
                    }
                }
            });
        }

        private async Task ExecuteCommandAsync(QueuedCommand command)
        {
            try
            {
                await _transport.WriteAsync(command.Command, 0, command.Command.Length, command.TimeoutMs).ConfigureAwait(false);

                var receiveBuffer = new byte[4096];
                var bytesRead = 0;
                var totalBytes = 0;
                var startTime = DateTime.UtcNow;

                do
                {
                    bytesRead = await _transport.ReadAsync(receiveBuffer, totalBytes,
                        receiveBuffer.Length - totalBytes, command.TimeoutMs).ConfigureAwait(false);
                    totalBytes += bytesRead;

                    if (totalBytes > 0)
                    {
                        if (ContainsLineEnding(receiveBuffer, totalBytes))
                            break;
                    }

                    var elapsed = (DateTime.UtcNow - startTime).TotalMilliseconds;
                    if (elapsed > command.TimeoutMs)
                    {
                        command.CompletionSource.TrySetResult(
                            HostLinkResult.Fail(command.Command, command.Address,
                                new Exceptions.TimeoutException("Command timed out after " + command.TimeoutMs + "ms.")));
                        return;
                    }
                } while (bytesRead > 0);

                if (totalBytes == 0)
                {
                    command.CompletionSource.TrySetResult(
                        HostLinkResult.Fail(command.Command, command.Address,
                            new Exceptions.TimeoutException("No response received.")));
                    return;
                }

                var responseText = Encoding.ASCII.GetString(receiveBuffer, 0, totalBytes);
                var responseBuffer = new byte[totalBytes];
                Array.Copy(receiveBuffer, responseBuffer, totalBytes);

                command.CompletionSource.TrySetResult(
                    HostLinkResult.Success(command.Command, command.Address, responseText, responseBuffer));
            }
            catch (Exception ex)
            {
                command.CompletionSource.TrySetResult(
                    HostLinkResult.Fail(command.Command, command.Address, ex));
            }
        }

        private static bool ContainsLineEnding(byte[] buffer, int length)
        {
            for (int i = 0; i < length - 1; i++)
            {
                if (buffer[i] == (byte)'\r' && buffer[i + 1] == (byte)'\n')
                    return true;
                if (buffer[i] == (byte)'\n')
                    return true;
            }
            return false;
        }

        private class QueuedCommand
        {
            public byte[] Command { get; set; }
            public string Address { get; set; }
            public int TimeoutMs { get; set; }
            public TaskCompletionSource<HostLinkResult> CompletionSource { get; set; }
        }
    }
}
