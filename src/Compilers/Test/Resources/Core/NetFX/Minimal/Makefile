BUILD = mcs /target:library /nostdlib /noconfig /keyfile:Key.snk

.PHONY: all
all: mincorlib.dll mincorlib_script.dll

mincorlib.dll: mincorlib.cs
	$(BUILD) /out:$@ $+

mincorlib_script.dll: mincorlib.cs mincorlib_script.cs
	$(BUILD) /out:$@ $+
