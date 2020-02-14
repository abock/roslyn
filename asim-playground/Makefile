TOP = ..
RUNTIME_DIR = $(TOP)/../dotnet-runtime-asim
SAMPLES = bin/Scratch.exe bin/Numerics.exe
OPENSSL_INCLUDE_DIR=/usr/local/opt/openssl/include

all: restore build samples

.PHONY: runtime
runtime:
	test -d "$(RUNTIME_DIR)" || git clone https://github.com/abock/runtime -b dev/abock/asim "$(RUNTIME_DIR)"
	"$(RUNTIME_DIR)/coreclr.sh"
	"$(RUNTIME_DIR)/mono.sh"
	"$(RUNTIME_DIR)/libraries.sh" --cmakeargs -DOPENSSL_INCLUDE_DIR="$(OPENSSL_INCLUDE_DIR)"

.PHONY: restore
restore:
	$(TOP)/restore.sh

.PHONY: build
build:
	$(TOP)/build.sh

.PHONY: test
test:
	dotnet test \
		--framework netcoreapp3.1 \
		$(TOP)/src/Compilers/CSharp/Test/Symbol/Microsoft.CodeAnalysis.CSharp.Symbol.UnitTests.csproj \
		--filter Microsoft.CodeAnalysis.CSharp.UnitTests.Symbols.AbstractStaticInterfaceMemberTests

.PHONY: samples
samples: $(SAMPLES)

bin/%.exe: %.cs
	mkdir -p $(dir $@)
	./csc $< -out:$@
	ikdasm $@ > $(@:.exe=.il)

.PHONY: clean
clean:
	rm -rf bin
