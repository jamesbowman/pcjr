import serial
import time
import array
import struct
import math
import random

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


def send(payload, reboot = False):
    ser = serial.Serial("/dev/ttyUSB1", 115200)
    if reboot:
        ser.setDTR(0)
        time.sleep(0.05)
        ser.setDTR(1)

        time.sleep(2)

    f = Flow(ser)
    f.write(payload)
    f.rate()

def com0(name):
    """ Format a COM file for the load0 receiver """
    d = array.array('B', open(name).read())
    d4 = []
    for c in d:
        d4.append(c)
        d4.append((c & 15) << 4)
    return array.array('B', d4).tostring().ljust(512)

def com1(name):
    """ Format a COM file for the load1 receiver """
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

if __name__ == '__main__':
    t0 = time.time()
    import sys, getopt
    try:
        optlist, args = getopt.getopt(sys.argv[1:], "rp:")
        if len(args) != 1:
            raise getopt.GetoptError
    except getopt.GetoptError:
        print " usage:"
        print " -r      reboot arduino"
        print " -p N    protocol 0-1 (default 1)"
        sys.exit(1)
    optdict = dict(optlist)

    reboot = '-r' in optdict
    protocol = 1
    if '-p' in optdict:
        protocol = int(optdict['-p'])
    payload = [com0,com1][protocol](args[0])
    send(payload, reboot)

print 'took', time.time() - t0
