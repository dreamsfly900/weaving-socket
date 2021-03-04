
import weaving_client
from weaving_client import *
import json

flag_test = False

# ------------------ app define begin ------------------------------
WCMD_HEARTBEAT = 0x99
WCMD_SOMECMD = 0x02


server_addr = '192.168.0.20:5678'
reg_msg = {'cmd':'reg','id':'12345678'}

# ------------------ app define end --------------------------------


class WeavingApp():
    def __init__(self, server_addr):
        self.cmd_dict = {}
        self.lock_playing = False
        self.wc = WeavingClient(server_addr)
        self.wc.reg_wcmd(WCMD_HEARTBEAT,None)
        self.wc.reg_wcmd(WCMD_SOMECMD,self.proc_msg)
        self.wc.on_connect = self.reg_self
        self.reg_cmd('echo',self.cmd_echo)

    def send_data(self,bdata:bytes):
        self.wc.send_data(WCMD_SOMECMD,bdata)

    def send_msg(self,msg:dict):
        bdata = json.dumps(msg).encode()
        self.send_data(bdata)

    def reg_cmd(self,cmd,func): # this cmd is self cmd, the difference in weaving cmd(wcmd)
        self.cmd_dict[cmd] = func

    def start(self):
        self.wc.start()
        print('Now we can do other things...')
        print('This is just one example, if you want exit, you can:')
        print('on windows: print Ctrl + Pause Break')
        print('on linux: print Ctrl + C')

    def reg_self(self):
        self.send_msg(reg_msg)

    def proc_msg(self,bmsg:bytes):
        dmsg = json.loads(bmsg.decode())
        cmd = dmsg.get('cmd','')
        data = dmsg.get('data','')
        func = self.cmd_dict.get(cmd,None)
        if(func):
            func(data)

    def cmd_echo(self,data:str):
        wclog('data is: '+data)
        if(data):
            os.system('echo {}'.format(data))

def main():
    weaving_client.flag_use_wclog = True # 可以在这种位置开关 weaving_client 里的 wclog 函数的打印功能
    wa = WeavingApp(server_addr)
    wa.start()

if __name__ == '__main__':
    main()


