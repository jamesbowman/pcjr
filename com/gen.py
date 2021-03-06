import Image
import array


pal_image= Image.new("P", (1,1))
pal_image.putpalette( (
    0,0,0,
    0,0,0xaa,
    0,0xaa,0,
    0,0xaa,0xaa,
    0xaa,0,0,
    0xaa,0,0xaa,
    0xaa,0xaa,0,
    0xaa,0xaa,0xaa,

    0x55,0x55,0x55,
    0x55,0x55,0xff,
    0x55,0xff,0x55,
    0x55,0xff,0xff,
    0xff,0x55,0x55,
    0xff,0x55,0xff,
    0xff,0xff,0x55,
    0xff,0xff,0xff,
    )
    + (0,0,0)*240)

def convert16(im):
    """ Given a 320x200 image, returns the 32K memory pic """

    im = im.convert("RGB").quantize(palette=pal_image)
    im.save("out.png")

    pd = array.array('B', im.tostring())
    print len(set(pd))

    mem = array.array('B', " " * 32768)

    for y in range(200):
        srcline = pd[320*y:320*(y+1)]
        da = 8192 * (y & 3) + (160 * (y >> 2))
        for i in range(0, len(srcline), 2):
          p0 = srcline[i + 0]
          p1 = srcline[i + 1]
          mem[da] = ((p0 << 4) | p1)
          da += 1
    return mem

assets = open("assets.i", "w")
print >>assets, "sunset:"
# for c in mem:
# print >>assets, "  db 0x%02x" % c

for b in ("beach felix lights fruit cake three eclairs kezia".split()):
    open(b + ".pic", "w").write(convert16(Image.open(b + ".png")).tostring())
