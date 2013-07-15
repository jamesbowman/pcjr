import Image
import ImageDraw
import math
import numpy as np
import sys

np.set_printoptions(linewidth = 140)

def circle(draw, x, y, r):
    draw.pieslice((x-r,y-r,x+r,y+r), 0, 360, fill=255)

def encode(depth, im):
    p8 = np.array(im).flatten()
    if depth == 16:
        hi4 = p8[::2] >> 4
        lo4 = p8[1::2] >> 4
        return (hi4 << 4) | lo4
    else:
        pp = [p8[i::4] >> 6 for i in range(4)]
        return (pp[0] << 6) | (pp[1] << 4) | (pp[2] << 2) | (pp[3] << 0)

memimages = {}
NFRAMES=64
DIM=24
depth = 4
if depth == 16:
    stride = DIM / 2
else:
    stride = DIM / 4

memimages[-1] = encode(depth, Image.new("1", (DIM,DIM)))

for y in range(64):
    im = Image.new("L", (20*5,20*5))
    draw = ImageDraw.Draw(im)

    if 1:
        # circle(draw, 50, 50, 30)
        # draw.rectangle((12,50-10,100-12,50+5), fill=255)
        # im = im.rotate(y * 360. / NFRAMES).resize((DIM,DIM), Image.BILINEAR)
        phi0 = math.pi * y / NFRAMES
        phi1 = phi0 + math.pi
        draw.line((47.5 + 40 * math.sin(phi0), 47.5 + 40 * math.cos(phi0),
                   47.5 + 40 * math.sin(phi1), 47.5 + 40 * math.cos(phi1)), fill = 255, width = 9.5)
        im = im.resize((DIM,DIM), Image.BILINEAR)
        pix = im.load()
        if 1:
            pi = [".XX.",
                  "X--X",
                  "X--X",
                  ".XX."]
            for xx in range(4):
                for yy in range(4):
                    c = pi[xx][yy]
                    if c == "X":
                        pix[10+xx,10+yy] = 255
                    if c == "-":
                        pix[10+xx,10+yy] = 0
        if 0:
            pi = ["..XXXX..",
                  ".X----X.",
                  "X------X",
                  "X------X",
                  "X------X",
                  "X------X",
                  ".X----X.",
                  "..XXXX.."]
            for xx in range(8):
                for yy in range(8):
                    c = pi[xx][yy]
                    if c == "X":
                        pix[8+xx,8+yy] = 255
                        print 'white', 8+xx,8+yy
                    if c == "-":
                        pix[8+xx,8+yy] = 0
        im = im.convert("1").convert("L")
    else:
        im = Image.open("arrow.png").rotate(y * 360. / NFRAMES).resize((24,24))
        im = im.convert("1").convert("L")
    im.save("out%02x.png" % y)
    memimages[y] = encode(depth, im)
    assert len(memimages[y]) == (DIM * stride)

def diff(hh, i0, i1):
    """ Emit difference code to change i0 to i1 """

    newdata = []
    print >>hh, "    mov si,.newdata"
    di_ = 0     # DI in emitted code, which may lag their DI
    di = 0      # Current DI
    for y in range(DIM):
        l0 = i0[stride*y : stride*(y+1)]
        l1 = i1[stride*y : stride*(y+1)]
        d = (l0 != l1).tolist()
        # print "%2d (%04x)" % (y, di), d
        i = 0
        while i < stride:
            if not d[i]:
                di += 1
                i += 1
            else:
                if di != di_:
                    if (di - di_) in (1,2):
                        print >>hh, "    times %d inc di ; now %04x" % (di - di_, di)
                    else:
                        print >>hh, "    add di,%d ; now %04x" % (di - di_, di)
                    di_ = di
                if (i <= (stride - 2)) and d[i] and d[i+1]:
                    if l1[i] == 0x00 and l1[i+1] == 0x00:
                        print >>hh, "    stosw ; %02x" % l1[i]
                    else:
                        print >>hh, "    movsw ; %02x %02x" % (l1[i], l1[i+1])
                        newdata.append(l1[i])
                        newdata.append(l1[i+1])
                    di_ += 2
                    di += 2
                    i += 2
                else:
                    if l1[i] == 0x00:
                        print >>hh, "    stosb ; %02x" % l1[i]
                    else:
                        print >>hh, "    movsb ; %02x" % l1[i]
                        newdata.append(l1[i])
                    di_ += 1
                    di += 1
                    i += 1
        di -= stride
        if depth == 16:
            if di < 0x6000:
                di += 0x2000
            else:
                di -= 0x6000
                di += 160
        else:
            if di < 0x2000:
                di += 0x2000
            else:
                di -= 0x2000
                di += 80
    di = stride
    if di != di_:
        print >>hh, "    add di,%d ; now %04x" % (di - di_, di)
        di_ = di
    print >>hh, "    ret"
    print >>hh, ".newdata: db", ",".join(["%03xh" % x for x in newdata])

hh = open("dsprite.i", "w")
for i0 in range(NFRAMES):
    for d in (-1, 1):
        i1 = (i0 + d) % NFRAMES
        print >>hh, "dsprite_%d_%d:" % (i0,i1)
        diff(hh, memimages[i0], memimages[i1])
print >>hh, "dsprite_0:"
diff(hh, memimages[-1], memimages[0])
print >>hh, r"""

# For when a sprite does not change for this redraw.
# Just bump DI.

park:
        add     di,%d
        ret

nextline:
        add     di,-(8 * %d)+((%d / 4)*160)
        ret

        dw  dsprite_0
dsprite_table:
""" % (stride, stride, DIM)
for i0 in range(NFRAMES):
    print >>hh, "    dw park"
for i0 in range(NFRAMES):
    i1 = (i0 + 1) % NFRAMES
    print >>hh, "    dw dsprite_%d_%d" % (i0, i1)
for i0 in range(NFRAMES):
    i1 = (i0 - 1) % NFRAMES
    print >>hh, "    dw dsprite_%d_%d" % (i0, i1)

print >>hh, "dsprite_delta:"
for d in (0, 1, -1):
    for i0 in range(NFRAMES):
        print >>hh, "    db %d" % (4 * ((i0 + d) & 63))

"""
2 bits per sprite
16 bytes per frame
960 bytes per second
"""
