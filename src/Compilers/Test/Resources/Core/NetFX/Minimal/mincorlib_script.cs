// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
//
// augments mincorlib.cs to provide stubbed Task APIs required for the script compilation host

namespace System.Runtime.CompilerServices
{
    public struct TaskAwaiter
    {
        public void GetResult () { }
    }
}

namespace System.Threading.Tasks
{
    using System.Runtime.CompilerServices;

    public class Task
    {
        public TaskAwaiter GetAwaiter () => default(TaskAwaiter);
    }

    public class Task<T> { }
}
