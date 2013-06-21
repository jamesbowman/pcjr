import Image
import array

im = Image.open("sunset.png")

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
im = im.convert("RGB").quantize(palette=pal_image)
# im.save("out.png")

pd = array.array('B', im.tostring())
# print set(pd)
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
