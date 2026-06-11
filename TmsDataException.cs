using System;

namespace TmsApi;

public class TmsDataException : Exception
{
    public TmsDataException(string message) : base(message) { }
}