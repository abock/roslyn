# Copyright (c)  Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

BUILD = mcs /target:library /nostdlib /noconfig /keyfile:Key.snk

.PHONY: all
all: mincorlib.dll minasync.dll mincorlib_script.dll

mincorlib.dll: mincorlib.cs
	$(BUILD) /out:$@ mincorlib.cs

minasync.dll: minasync.cs mincorlib.dll
	$(BUILD) /out:$@ /r:mincorlib.dll minasync.cs

mincorlib_script.dll: mincorlib.cs mincorlib_script.cs
	$(BUILD) /out:$@ $+
