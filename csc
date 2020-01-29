#!/bin/bash

dotnet run --project src/Compilers/CSharp/csc/csc.csproj --framework netcoreapp2.1 -- -r:/Library/Frameworks/Mono.framework/Versions/Current/lib/mono/4.7.2-api/mscorlib.dll -nostdlib+ "$@"
