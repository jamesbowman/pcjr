import serial
import os
import time
import array
import struct
import math
import random
from debugtools import hexdump

class Flow():
    def __init__(self, ser):
        self.ser = ser
        self.freespace = 0
        self.count = 0
        self.t0 = time.time()
        self.sum = 0

    def rate(self):
        print 'rate', self.count / (time.time() - self.t0)

    def write(self, s):
        assert (len(s) % 256) == 0
        for c in s:
            self.ser.write(c)
            self.count += 1
            self.sum += ord(c)
            if (self.count & 255) == 0:
                self.ser.flush()
                c = ord(self.ser.read(1))
                if c != (self.sum & 0xff):
                    print c, self.sum & 0xff
                # print 'acked', hex(self.count)

                self.sum = 0


def send(port, payload, reboot = False):
    ser = serial.Serial(port, 115200)
    ser.setDTR(1)
    if reboot:
        ser.setDTR(0)
        time.sleep(0.05)
        ser.setDTR(1)

        time.sleep(2)

    f = Flow(ser)
    f.write(payload)
    f.rate()

def com0(names):
    """ Format a COM file for the load0 receiver """
    (name,) = names
    d = array.array('B', open(name).read())
    d4 = []
    for c in d:
        d4.append(c)
        d4.append((c & 15) << 4)
    return array.array('B', d4).tostring().ljust(512)

def com1(names):
    """ Format a COM file for the load1 receiver """
    (name,) = names
    pgm = open(name).read()
    d = array.array('B', struct.pack(">H", len(pgm)) + pgm)

    b = [0,0] + [0x50, 0xa0, 0x90, 0x70]
    for c in d:
        b.append(c & 0xf0)
        b.append((c & 15) << 4)
    b = array.array('B', b).tostring()
    print 'len', name, hex(len(b))
    b = b.rjust((len(b) + 255) & ~255, chr(0))
    print 'len', name, hex(len(b))
    return b

def com2(names):
    """ Format a COM file for the load1 receiver """
    d = []
    for name in names:
        pgm = open(name).read()
        dosname = os.path.basename(name).upper()
        d += [dosname, chr(0)]
        for i in range(0, len(pgm), 256):
            sub = pgm[i:i+256]
            d.append(struct.pack("<H", len(sub)))
            d.append(sub)
        d.append(struct.pack("<H", 0))
    d += chr(0) # zero-length filename terminates the run
    d = array.array('B', "".join(d))

    print hexdump(d.tostring())
    b = [0,0] + [0x50, 0xa0, 0x90, 0x80]
    for c in d:
        b.append(c & 0xf0)
        b.append((c & 15) << 4)
    b = array.array('B', b).tostring()
    print 'len', name, hex(len(b))
    b = b.rjust((len(b) + 255) & ~255, chr(0))
    print 'len', name, hex(len(b))
    return b

if __name__ == '__main__':
    t0 = time.time()
    import sys, getopt
    try:
        optlist, args = getopt.getopt(sys.argv[1:], "rp:h:")
        if len(args) < 1:
            raise getopt.GetoptError("not enough args")
    except getopt.GetoptError, msg:
        print
        print 'Error:', msg
        print
        print " usage:"
        print " -r      reboot arduino"
        print " -p N    protocol 0-1 (default 1)"
        print " -h dev  protocol 0-1 (default 1)"
        sys.exit(1)
    optdict = dict(optlist)

    reboot = '-r' in optdict
    protocol = 1
    port = optdict.get('-h', "/dev/ttyUSB0")
    if '-p' in optdict:
        protocol = int(optdict['-p'])
    payload = [com0,com1,com2][protocol](args)
    send(port, payload, reboot)

print 'took', time.time() - t0
