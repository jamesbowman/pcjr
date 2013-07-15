import numpy as np
import random

class Dials:
    def __init__(self, bo):
        self.p = np.zeros(64)
        self.i = np.zeros(64)
        self.bo = bo

    def all(self, d):
        self.p += d

    def tick(self):
        fl = np.floor(self.p)
        d = fl - self.i
        # print self.p[0], self.i[0], d[0]
        d = np.sign(d)
        self.i += d
        #  0 -> 00
        #  1 -> 01
        # -1 -> 10
        co = d.astype(np.uint8) & 3
        co = np.where(co == 3, 2, co)
        pp = [co[i::4] for i in range(4)]
        cmd = (pp[0] << 0) | (pp[1] << 2) | (pp[2] << 4) | (pp[3] << 6)
        bo.write(cmd.astype(np.uint8).tostring())

bo = open("dsanim.bi", "w")
d = Dials(bo)

sp = -1 + 2 * np.random.random(64)
for i in range(100):
    d.p += sp
    d.tick()

"""
for i in range(16):
    for j in range(5):
        d.all(1)
        d.tick()
    d.all(-1)
    d.tick()
    for j in range(4):
        d.tick()
"""

for i in range(100):
    d.tick()
