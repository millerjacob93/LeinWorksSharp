using System.Text.Json;

namespace LienWorksSharp.Services;

public class JsonRepository<T> where T : class, new()
{
    private readonly string _filePath;
    private readonly JsonSerializerOptions _options;
    private readonly SemaphoreSlim _gate = new(1, 1);

    public JsonRepository(string filePath, JsonSerializerOptions? options = null)
    {
        _filePath = filePath;
        _options = options ?? new JsonSerializerOptions
        {
            WriteIndented = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
    }

    public async Task<T> ReadAsync()
    {
        if (!File.Exists(_filePath))
        {
            return new T();
        }

        await using var stream = File.OpenRead(_filePath);
        var data = await JsonSerializer.DeserializeAsync<T>(stream, _options);
        return data ?? new T();
    }

    public async Task WriteAsync(T data)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(_filePath)!);
        await _gate.WaitAsync();
        try
        {
            await using var stream = File.Create(_filePath);
            await JsonSerializer.SerializeAsync(stream, data, _options);
        }
        finally
        {
            _gate.Release();
        }
    }
}
