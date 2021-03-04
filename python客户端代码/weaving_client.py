import socket
import threading
import time
import traceback
import os

# ------------------ function for weaving port begin ---------------------

flag_use_wclog = False

def get_time():
    a = time.time() # such like 1579141285.7733445
    b = int(a) # such like 1579141285
    e = time.localtime(b)
    f = time.strftime("%y%m%d-%H:%M:%S",e) # such like '200116102125'
    return f

def wclog(*x):
    if(flag_use_wclog):
        s = ''.join(x)
        print(get_time(),s,flush=True)

# crc16_xmodem
# 0x11 0x22 0x33 0x44  crc is  0xdd 0x33
# 0x12 0x34 0x56 0x78  crc is  0xb4 0x2c
def crc16_xmodem(bdata:bytes)->bytes:
    wcrc = 0
    for b in bdata:
        for j in range(8):
            treat = b & 0x80
            b <<= 1
            bcrc = (wcrc >> 8) & 0x80
            wcrc <<= 1
            wcrc = wcrc & 0xffff
            if (treat != bcrc):
                wcrc ^= 0x1021
    return wcrc.to_bytes(2,'big')

def con_crc(bdata:bytes)->bytes:
    return bdata + crc16_xmodem(bdata)

def chk_crc(bdata:bytes)->bool:
    if(len(bdata)<3):
        return False
    if(bdata[-2:] == crc16_xmodem(bdata[:-2])):
        return True
    else:
        return False

def weaving_pack(icmd:int,bdata:bytes)->bytes:
    len_bdata = len(bdata)
    blen = len_bdata.to_bytes(4,'little').rstrip(b'\x00')
    len_blen = len(blen)
    ret = bytes([icmd,len_blen]) + blen
    ret = con_crc(ret) + bdata
    return ret

def weaving_unpack(bdata:bytes)->(bool,int,bytes,bytes):
    wclog('into weaving_unpack')
    flag_result = False
    icmd = 0x00
    bmsg = b''

    while(True):
        if(len(bdata) < 4):
            return flag_result,icmd,bmsg,bdata
        len_blen = bdata[1]
        if(len(bdata) < len_blen+4):
            return flag_result,icmd,bmsg,bdata
        blen = bdata[2:2+len_blen]
        len_bdata = int.from_bytes(blen,'little')
        if(len(bdata) < len_blen+4+len_bdata):
            return flag_result,icmd,bmsg,bdata
        if(not chk_crc(bdata[:4+len_blen])):
            bdata = bdata[1:]
            continue
        else:
            flag_result,icmd,bmsg,bdata = True, bdata[0], bdata[4+len_blen:4+len_blen+len_bdata], bdata[4+len_blen+len_bdata:]
            return flag_result,icmd,bmsg,bdata

# ------------------ function for weaving port end ------------------------


class WeavingClient(threading.Thread):

    def __init__(self, addr, recv_timeout=10, reconnection_delay=10, client_id='1'):
        threading.Thread.__init__(self, name="WeavingClient" + client_id)
        self.addr = addr
        self.recv_timeout = recv_timeout
        self.reconnection_delay = reconnection_delay
        self.host, self.port = addr.split(':')
        self.port = int(self.port)
        self.all_bdata = b''
        self.wcmd_dict = {} # wcmd is weaving cmd, the difference in app cmd
        self.on_connect = None

    def reg_wcmd(self,cmd,func):
        self.wcmd_dict[cmd] = func

    def run(self):
        self.do_connect()
        while True:
            try:
                wclog('recving data...')
                self.recv_data()
            except OSError:
                traceback.print_exc()
                wclog('{} connect error, reconnection  after {}s'.format(self.addr,self.reconnection_delay))
                time.sleep(self.reconnection_delay)
                self.sck = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
                self.do_connect()
            except Exception as e:
                traceback.print_exc()
                wclog('other error, reconnection  after {}s'.format(self.reconnection_delay))
                time.sleep(self.reconnection_delay)
                self.do_connect()

    def do_connect(self):
        while True:
            try:
                self.sck = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
                self.sck.setsockopt(socket.SOL_SOCKET, socket.SO_REUSEADDR, 1)
                self.sck.settimeout(self.recv_timeout)
                wclog('try to connect {}'.format(self.addr))
                self.sck.connect((self.host, self.port))
                wclog('client start connect to {}'.format(self.addr))
                if(callable(self.on_connect)):
                    self.on_connect()
                break
            except ConnectionRefusedError:
                wclog('{} refused or not started, reconnection  after {}s'.format(self.reconnection_delay, self.addr))
                time.sleep(self.reconnection_delay)
            except Exception as e:
                traceback.print_exc()
                wclog('do connect error: {}'.format(str(e)))
                wclog('reconnection  after {}s'.format(self.reconnection_delay))
                time.sleep(self.reconnection_delay)

    def send_data(self,cmd,bdata):
        bdata = weaving_pack(cmd,bdata)
        self.sck.send(bdata)

    def recv_data(self):
        try:
            data = self.sck.recv(1024)
            if data:
                wclog('recv data:{}'.format(data))
                self.all_bdata += data
                flag_need_proc = True
                while(flag_need_proc):
                    flag_need_proc = self.proc_data()
            else:
                wclog('the socket may be closed, reconnection  after {}s'.format(self.reconnection_delay))
                time.sleep(self.reconnection_delay)
                self.do_connect()
        except socket.timeout:
            wclog('recv timeout, reconnection  after {}s'.format(self.reconnection_delay))
            time.sleep(self.reconnection_delay)
            self.do_connect()
        except Exception as e:
            traceback.print_exc()
            self.sck = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
            self.do_connect()
    
    def proc_data(self):
        wclog('into proc_data')
        wclog('all_bdata is: {}'.format(self.all_bdata))
        flag_result, icmd, bmsg, self.all_bdata = weaving_unpack(self.all_bdata)
        if(flag_result):
            wclog('proc_data, flag_result is True')
            func = self.wcmd_dict.get(icmd,None)
            if(callable(func)):
                threading.Thread(target=func,args=(bmsg,)).start()
            return True
        else:
            return False
    
