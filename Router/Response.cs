using System;
using UnityEngine;

namespace RMServerAssist.Router;

[Serializable]
public struct Response<T>
{
    public string message;
    public long timestamp;
    public T data;
    
    public static Response<T> Create(T data = default, string message = "ok")
    {
        return new Response<T>
        {
            message = message,
            timestamp = DateTimeOffset.Now.ToUnixTimeSeconds(),
            data = data
        };
    }
}