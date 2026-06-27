using System.Text.Json;

namespace AirportTicketBooking.Infrastructure.Repositories;

/// <summary>
/// Thread-safe base class for JSON file-backed repositories.
///
/// WHY SemaphoreSlim(1,1)?
///   File I/O is not thread-safe. If two async operations (e.g., a save and a
///   concurrent read triggered by DI-scoped services) race on the same file,
///   the second FileStream open will throw IOException on Windows because the
///   first still holds the write lock. A SemaphoreSlim with an initial count
///   of 1 acts as an async-compatible mutex — only one caller enters the
///   critical section at a time while still releasing the thread pool thread
///   while waiting (unlike a plain `lock` which blocks the thread).
/// </summary>
public abstract class JsonFileRepository<T>
{
    private readonly string _filePath;
    private readonly SemaphoreSlim _fileLock = new(1, 1);

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true,
        PropertyNameCaseInsensitive = true,
    };

    protected JsonFileRepository(string filePath)
    {
        _filePath = filePath;
        EnsureFileExists();
    }

    // Creates the data directory and an empty JSON array file on first run
    // so callers never need to check for missing files.
    private void EnsureFileExists()
    {
        var directory = Path.GetDirectoryName(_filePath)!;
        if (!Directory.Exists(directory))
            Directory.CreateDirectory(directory);

        if (!File.Exists(_filePath))
            File.WriteAllText(_filePath, "[]");
    }

    protected async Task<List<T>> ReadAllAsync()
    {
        await _fileLock.WaitAsync();
        try
        {
            await using var stream = File.OpenRead(_filePath);
            return await JsonSerializer.DeserializeAsync<List<T>>(stream, JsonOptions)
                   ?? [];
        }
        finally
        {
            _fileLock.Release();
        }
    }

    protected async Task WriteAllAsync(List<T> items)
    {
        await _fileLock.WaitAsync();
        try
        {
            // Write to a temp file first, then atomically replace the target.
            // This prevents a crash mid-write from corrupting the data file.
            var tempPath = _filePath + ".tmp";
            await using (var stream = File.Create(tempPath))
            {
                await JsonSerializer.SerializeAsync(stream, items, JsonOptions);
            }
            File.Move(tempPath, _filePath, overwrite: true);
        }
        finally
        {
            _fileLock.Release();
        }
    }
}
