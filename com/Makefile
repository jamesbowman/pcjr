%.com: %.asm
	nasm -o $*.com -l $*.lst $<

all: tiny.com hello.com uncompress.com x.com

bytecode.i: cross.fs vm.fs main.fs
	gforth cross.fs vm.fs main.fs > bytecode.i

assets.i: Makefile *.png gen.py
	python gen.py

tiny.com: tiny.asm bytecode.i assets.i

hello.com: hello.asm

uncompress.com: uncompress.asm

tiny.lz4: tiny.com
	lz4c -y -hc -l tiny.com tiny.lz4 2> /dev/null

x.com: uncompress.com tiny.lz4
	cat uncompress.com tiny.lz4 > x.com
	ls -l x.com
