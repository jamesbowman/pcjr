%.com: %.asm
	nasm -o $*.com -l $*.lst $<

all: hello.com main.com uncompress.com x.com

main.i: main.fs *.fs
	gforth cross.fs vm.fs main.fs > main.i

assets.i: Makefile *.png gen.py
	python gen.py

main.com: main.asm main.i assets.i common.asm

hello.com: hello.asm

uncompress.com: uncompress.asm

main.lz4: main.com
	lz4c -y -hc -l main.com main.lz4 2> /dev/null

x.com: uncompress.com main.lz4
	cat uncompress.com main.lz4 > x.com
	ls -l x.com
