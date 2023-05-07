using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using Microsoft.JSInterop;

namespace Site.Shared.Storage;

public class StorageBase
{
    private readonly IJSInProcessRuntime _js;

    private readonly string _storage;

    protected StorageBase(
        IJSRuntime js,
        string storage
    )
    {
        _js = (IJSInProcessRuntime) js;
        _storage = storage;
    }

    public IReadOnlyCollection<string> GetKeys()
    {
        var length = _js.Invoke<int>("eval", $"{_storage}.length");

        if (length == 0)
            return Array.Empty<string>();

        var keys = Enumerable.Range(0, length)
            .Select(i => _js.Invoke<string>($"{_storage}.key", i))
            .ToArray();

        return keys;
    }

    public bool HasKey(string key)
    {
        return _js.Invoke<bool>($"{_storage}.hasOwnProperty", key);
    }

    public bool TryGet<T>(string key, out T? value)
    {
        if (TryGetString(key, out var raw))
        {
            value = JsonSerializer.Deserialize<T>(raw!);
            return true;
        }

        value = default;

        return false;
    }

    public bool TryGetString(string key, out string? value)
    {
        value = _js.Invoke<string>($"{_storage}.getItem", key);

        return !string.IsNullOrWhiteSpace(value);
    }

    public T Get<T>(string key)
    {
        return JsonSerializer.Deserialize<T>(GetString(key))!;
    }

    public string GetString(string key)
    {
        var raw = _js.Invoke<string>($"{_storage}.getItem", key);

        if (string.IsNullOrWhiteSpace(raw))
            throw new KeyNotFoundException($"Key {key} is not found in {_storage}");

        return raw;
    }

    public bool Set<T>(string key, T value)
    {
        return SetString(key, JsonSerializer.Serialize(value));
    }

    public bool SetString(string key, string value)
    {
        var hasKey = HasKey(key);
        _js.InvokeVoid($"{_storage}.setItem", key, value);

        return !hasKey;
    }

    public bool Remove(string key)
    {
        var hasKey = HasKey(key);
        _js.InvokeVoid($"{_storage}.removeItem", key);

        return hasKey;
    }

    public void Clear()
    {
        _js.InvokeVoid($"{_storage}.clear");
    }
}