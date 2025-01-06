using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Projekat.Encryption;
using System.Threading;


namespace Projekat.SystemWatcher
{
    public class TargetWatcher : IDisposable
    {
        private FileSystemWatcher _watcher;
        private FileSystemWatcher _watcher_destination;
        private bool _disposed = false;
        private EncryptionService _encryptionService;
        private string _destinationPath;
        private string _encryptionMethod;
        private string _targetPath;
        private CancellationTokenSource _cancellationTokenSource;
        private Form1 _form;  // Reference to the form for UI updates

        public TargetWatcher(CancellationTokenSource cancellationTokenSource, string targetPath, string destinationPath, string encryptionMethod, string rc4Key, byte[] xteaKey, byte[] iv, Form1 form)
        {
            _encryptionMethod = encryptionMethod;
            _encryptionService = new EncryptionService(rc4Key, xteaKey, iv);
            _destinationPath = destinationPath;
            _targetPath = targetPath;
            _cancellationTokenSource = cancellationTokenSource;
            _form = form;

            _watcher = new FileSystemWatcher(targetPath)
            {
                NotifyFilter = NotifyFilters.FileName
            };

            _watcher.Created += async (s, e) => await HandleFileCreation(e.FullPath, _cancellationTokenSource.Token);
            _watcher.Renamed += async (s, e) => await HandleFileTarget(e.FullPath, _cancellationTokenSource.Token);
            _watcher.Deleted += async (s, e) => await HandleFileTarget(e.FullPath, _cancellationTokenSource.Token);

            _watcher_destination = new FileSystemWatcher(destinationPath)
            {
                NotifyFilter = NotifyFilters.FileName
            };

            _watcher_destination.Created += async (s, e) => await HandleFileDestination(e.FullPath, _cancellationTokenSource.Token);
            _watcher_destination.Renamed += async (s, e) => await HandleFileDestination(e.FullPath, _cancellationTokenSource.Token);
            _watcher_destination.Deleted += async (s, e) => await HandleFileDestination(e.FullPath, _cancellationTokenSource.Token);
        }

        private async Task HandleFileTarget(string filePath, CancellationToken cancellationToken)
        {
            try
            {
                if (cancellationToken.IsCancellationRequested)
                    return;

                var directoryPath = Path.GetDirectoryName(filePath);
                var fileNames = Directory.GetFiles(directoryPath)
                                         .Select(Path.GetFileName)
                                         .ToList();

                await _form.UpdateTargetViewAsync(fileNames);
            }
            catch (Exception ex)
            {
                _form.UpdateUI($"Error handling file: {ex.Message}");
            }
        }

        private async Task HandleFileDestination(string filePath, CancellationToken cancellationToken)
        {
            try
            {
                if (cancellationToken.IsCancellationRequested)
                    return;

                var directoryPath = Path.GetDirectoryName(filePath);
                var fileNames = Directory.GetFiles(directoryPath)
                                         .Select(Path.GetFileName)
                                         .ToList();

                await _form.UpdateXViewAsync(fileNames);
            }
            catch (Exception ex)
            {
                _form.UpdateUI($"Error handling file: {ex.Message}");
            }
        }

        private async Task HandleFileCreation(string filePath, CancellationToken cancellationToken)
        {
            try
            {
                if (cancellationToken.IsCancellationRequested)
                    return;

                string fileName = Path.GetFileName(filePath);
                string encryptedFilePath = Path.Combine(_destinationPath, fileName);

                // First update the UI to reflect the new file in the Target folder
                var directoryPath = Path.GetDirectoryName(filePath);
                var fileNames = Directory.GetFiles(directoryPath)
                                         .Select(Path.GetFileName)
                                         .ToList();
                await _form.UpdateTargetViewAsync(fileNames); // Update UI immediately

                if (File.Exists(encryptedFilePath))
                {
                    Console.WriteLine($"File {fileName} is already encrypted.");
                    return;
                }

                Console.WriteLine($"Waiting for file {fileName} to be ready...");
                while (!IsFileReady(filePath))
                {
                    if (cancellationToken.IsCancellationRequested)
                        return;

                    await Task.Delay(500, cancellationToken);
                }

                Console.WriteLine($"File {fileName} is ready. Starting encryption...");
                await Task.Run(() =>
                {
                    if (cancellationToken.IsCancellationRequested)
                        return;

                    try
                    {
                        if (_encryptionMethod == "XTEA")
                        {
                            _encryptionService.EncryptFileWithXTEA(filePath, encryptedFilePath);
                        }
                        else
                        {
                            _encryptionService.EncryptFileWithRC4(filePath, encryptedFilePath);
                        }
                        _form.UpdateUI($"File {fileName} encrypted successfully at {encryptedFilePath}");
                    }
                    catch (Exception ex)
                    {
                        _form.UpdateUI($"Error encrypting file {fileName}: {ex.Message}");
                    }
                }, cancellationToken);
            }
            catch (Exception ex) when (ex is TaskCanceledException)
            {
                _form.UpdateUI($"File creation handling canceled: {filePath}");
            }
            catch (Exception ex)
            {
                _form.UpdateUI($"Error handling file creation: {ex.Message}");
            }
        }


        private bool IsFileReady(string filePath)
        {
            try
            {
                using (var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.None))
                {
                    return true;
                }
            }
            catch (IOException)
            {
                return false;
            }
        }

        public async Task Start(CancellationToken cancellationToken)
        {
            _watcher.EnableRaisingEvents = true;
            _watcher_destination.EnableRaisingEvents = true;

            if (Directory.Exists(_targetPath))
            {
                var fileNames = Directory.GetFiles(_targetPath)
                                         .Select(Path.GetFileName)
                                         .ToList();

                await _form.UpdateTargetViewAsync(fileNames);
            }

            if (Directory.Exists(_destinationPath))
            {
                var fileNames = Directory.GetFiles(_destinationPath)
                                         .Select(Path.GetFileName)
                                         .ToList();

                await _form.UpdateXViewAsync(fileNames);
            }
        }

        public void Stop()
        {
            _cancellationTokenSource.Cancel();
            _watcher.EnableRaisingEvents = false;
            _watcher_destination.EnableRaisingEvents = false;
        }

        public void Dispose()
        {
            if (_disposed) return;

            _watcher.Dispose();
            _watcher_destination.Dispose();
            _cancellationTokenSource.Dispose();
            _disposed = true;
        }
    }
}