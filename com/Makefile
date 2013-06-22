%.com: %.asm
	nasm -o $*.com -l $*.lst $<

%.i: %.fs
	gforth cross.fs vm.fs $*.fs > $*.i

all: load0.com hello.com main.com unittest.com uncompress.com x.com

assets.i: Makefile *.png gen.py
	python gen.py

main.com: main.asm main.i assets.i common.asm

main.i: main.fs *.fs

unittest.com: unittest.asm unittest.i common.asm

unitest.i: unitest.fs *.fs


load0.com: load0.asm

hello.com: hello.asm

uncompress.com: uncompress.asm

main.lz4: main.com
	lz4c -y -hc -l main.com main.lz4 2> /dev/null

x.com: uncompress.com main.lz4
	cat uncompress.com main.lz4 > x.com
	ls -l x.com

clean::
	rm -rf *.com *.i *.lz4
