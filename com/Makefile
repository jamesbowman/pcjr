%.com: %.asm
	nasm -o $*.com -l $*.lst $<

%.i: %.fs
	gforth cross.fs vm.fs $*.fs > $*.i

all: load0.com load1.com hello.com colors.com main.com unittest.com bench.com fwrite.com  uncompress.com x.com

assets.i: Makefile *.png gen.py
	python gen.py

main.com: main.asm main.i assets.i common.asm

fwrite.com: fwrite.asm fwrite.i assets.i common.asm

main.i: main.fs *.fs

colors.com: colors.asm colors.i assets.i common.asm

colors.i: colors.fs *.fs

unittest.com: unittest.asm unittest.i common.asm

unittest.i: unittest.fs *.fs


bench.com: bench.asm bench.i common.asm

bench.i: bench.fs *.fs


load0.com: load0.asm

load1.com: load1.asm

hello.com: hello.asm

uncompress.com: uncompress.asm

main.lz4: main.com
	lz4c -y -hc -l main.com main.lz4 2> /dev/null

x.com: uncompress.com main.lz4
	cat uncompress.com main.lz4 > x.com
	ls -l x.com

clean::
	rm -rf *.com *.i *.lz4

.DELETE_ON_ERROR:
