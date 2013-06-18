import lz4
# d = open("TEXT.LZ4").read()[4:]
# d = lz4.compressHC("Welcome to the magic cheeseland!$")

# d = lz4.compressHC(open("TEXT.TXT").read() + "$")
# print >>open("hdr.i", "w"), "\n".join([" db %d" % ord(c) for c in d])

u = open("tiny.com").read()
d = lz4.compressHC(u)
open("tiny.lz4", "w").write(d)
print len(u), "compressed to", len(d)
# print lz4.decompress(d)
