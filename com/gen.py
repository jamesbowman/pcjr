import Image
import array

# im = Image.open("baby.png")
im = Image.new("RGB", (320,200))

i0 = 0
i1 = 0x65
i2 = 0x9a
i3 = 0xff
pal = (
    ( i0,i0,i0,   ),
    ( i0,i0,i2,   ),
    ( i0,i2,i0,   ),
    ( i0,i2,i2,   ),
    ( i2,i0,i0,   ),
    ( i2,i0,i2,   ),
    ( i2,i1,i0,   ),
    ( i1,i1,i1,   ),
    ( i2,i2,i2,   ),
    ( i1,i1,i3,   ),
    ( i1,i3,i1,   ),
    ( i1,i3,i3,   ),
    ( i3,i1,i1,   ),
    ( i3,i1,i3,   ),
    ( i3,i3,i1,   ),
    ( i3,i3,i3,   ),
    )
pal_image= Image.new("P", (1,1))
if 1:
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
else:
    pal_image.putpalette(sum(pal, ()) + (0,0,0)*240)
im = im.convert("RGB").quantize(palette=pal_image)
im.save("out.png")

pd = array.array('B', im.tostring())
print len(set(pd))
assets = open("assets.i", "w")

mem = array.array('B', " " * 32768)

for y in range(200):
    srcline = pd[320*y:320*(y+1)]
    da = 8192 * (y & 3) + (160 * (y >> 2))
    for i in range(0, len(srcline), 2):
      p0 = srcline[i + 0]
      p1 = srcline[i + 1]
      mem[da] = ((p0 << 4) | p1)
      da += 1

print >>assets, "sunset:"
for c in mem:
  print >>assets, "  db 0x%02x" % c
